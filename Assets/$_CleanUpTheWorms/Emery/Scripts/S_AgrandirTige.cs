using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using System;
using System.Globalization;

public class S_AgrandirTige : MonoBehaviour
{
    [Header("Taille de la tige min et max")]
    public float minTaille = 0.5f;
    [Tooltip("maxTaille peut evoluer dans le temps pour agrandir la tige au fur et a mesure que le joueur progresse dans le jeu")]
    public float maxTaille = 5f;
    [Header("GameObject tige")]
    public GameObject tige;

    [Header("Distance entre les deux manettes min et max")]
    [SerializeField]
    float distanceMaxGun = 0.3f;
    [SerializeField]
    float distanceMinGun = 0f;

    [Header("Controllers")]
    public GameObject controllerLeft;
    public GameObject controllerRight;

    [Header("Sans device (manettes seules) : anti-tremblements")]
    [Tooltip("Transform qui définit l'axe de la tige. Si vide, on utilise la tige elle-même.")]
    [SerializeField] Transform axisReference;
    [Tooltip("Axe LOCAL de la référence le long duquel on mesure l'écartement (Z = avant par défaut).")]
    [SerializeField] Vector3 localAxis = Vector3.forward;
    [Tooltip("Zone morte en mètres : les variations plus petites que cette valeur sont ignorées (tremblements).")]
    [SerializeField] float deadZone = 0.003f;
    [Tooltip("Plus la valeur est haute, plus la tige réagit vite. Plus elle est basse, plus c'est lissé.")]
    [SerializeField] float smoothing = 12f;

    [Header("Unity Event Size Changed")]
    public UnityEvent onSizeChanged;

    [Header("Debug")]
    public bool isInTestingScene = false;
    [Tooltip("Pour tester le mode 'avec device' directement dans l'éditeur, sans argument de lancement.")]
    [SerializeField] bool forceWithDevice = false;

    private float lastTaille = -1f;

    private bool isFastVersion = false; // Fast version flag
    private bool isWithDevice = false;  // Argument -withdevice

    // Filtrage (mode sans device uniquement)
    private bool filterInitialized = false;
    private float filteredDistance;
    private float smoothedDistance;

    void Start()
    {
        isWithDevice = forceWithDevice;

        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-fastversion")
            {
                isFastVersion = true;
            }
            else if (args[i] == "-withdevice")
            {
                isWithDevice = true;
            }
            else if (args[i] == "-mintaille" && i + 1 < args.Length)
            {
                float.TryParse(args[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out distanceMinGun);
            }
            else if (args[i] == "-maxtaille" && i + 1 < args.Length)
            {
                float.TryParse(args[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out distanceMaxGun);
            }
        }
    }

    void Update()
    {
        if (isFastVersion || StateMachineGame.Instance.state == GameState.LEVEL2 || StateMachineGame.Instance.state == GameState.END || StateMachineGame.Instance.state == GameState.ENDING)
        {
            ResizedStick();
        }
    }

    // ---------- AVEC DEVICE : système d'origine, inchangé ----------

    private float DistanceBetweenControllers()
    {
        return Vector3.Distance(controllerRight.transform.position, controllerLeft.transform.position);
    }

    private float NormalizedDistanceBetweenControllers()
    {
        float distance = DistanceBetweenControllers();
        return Mathf.InverseLerp(distanceMinGun, distanceMaxGun, distance);
    }

    // ---------- SANS DEVICE : mesure sur un seul axe + filtrage ----------

    // Écartement des manettes projeté sur l'axe de la tige uniquement.
    // Tout mouvement perpendiculaire à cet axe (tremblements latéraux, haut/bas) est ignoré.
    private float AxisDistanceBetweenControllers()
    {
        Transform reference = axisReference != null ? axisReference : tige.transform;
        Vector3 axis = reference.TransformDirection(localAxis).normalized; // ignore l'échelle de la tige
        Vector3 offset = controllerRight.transform.position - controllerLeft.transform.position;
        return Mathf.Abs(Vector3.Dot(offset, axis));
    }

    private float NormalizedDistanceNoDevice()
    {
        float raw = AxisDistanceBetweenControllers();

        if (!filterInitialized)
        {
            filteredDistance = raw;
            smoothedDistance = raw;
            filterInitialized = true;
        }

        // 1) Zone morte : la valeur ne bouge que si le changement dépasse deadZone
        float delta = raw - filteredDistance;
        if (Mathf.Abs(delta) > deadZone)
        {
            filteredDistance = raw - Mathf.Sign(delta) * deadZone;
        }

        // 2) Lissage indépendant du framerate
        smoothedDistance = Mathf.Lerp(smoothedDistance, filteredDistance, 1f - Mathf.Exp(-smoothing * Time.deltaTime));

        return Mathf.InverseLerp(distanceMinGun, distanceMaxGun, smoothedDistance);
    }

    // ---------- Commun ----------

    private void ResizedStick()
    {
        float opening = isWithDevice ? NormalizedDistanceBetweenControllers() : NormalizedDistanceNoDevice();
        float newTaille = Mathf.Lerp(minTaille, maxTaille, 1 - opening);

        if (lastTaille != -1f && Mathf.Abs(newTaille - lastTaille) > 0.01f)
        {
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).SendHapticImpulse(0u, 0.15f, 0.02f);
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0u, 0.15f, 0.02f);
            lastTaille = newTaille;
        }
        else if (lastTaille == -1f)
        {
            lastTaille = newTaille;
        }

        tige.transform.localScale = new Vector3(tige.transform.localScale.x, tige.transform.localScale.y, newTaille);

        onSizeChanged.Invoke();
    }
}