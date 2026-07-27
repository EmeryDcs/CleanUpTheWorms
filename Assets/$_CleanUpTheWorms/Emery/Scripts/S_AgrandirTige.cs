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

    [Header("Unity Event Size Changed")]
    public UnityEvent onSizeChanged;

    [Header("Debug")]
    public bool isInTestingScene = false;

    private float lastTaille = -1f;
    
    private bool isFastVersion = false; // NEW: Fast version flag

    // NEW: Check for the argument when the script starts
    void Start()
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-fastversion")
            {
                isFastVersion = true;
            }
            else if (args[i] == "-mintaille" && i + 1 < args.Length)
            {
                float.TryParse(args[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out minTaille);
            }
            else if (args[i] == "-maxtaille" && i + 1 < args.Length)
            {
                float.TryParse(args[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out maxTaille);
            }
        }
    }

    void Update()
    {
        // NEW: Added "isFastVersion ||" to bypass state checks if true
        if (isFastVersion || StateMachineGame.Instance.state == GameState.LEVEL2 || StateMachineGame.Instance.state == GameState.END || StateMachineGame.Instance.state == GameState.ENDING)
        {
            ResizedStick();
        }
    }

    private float DistanceBetweenControllers()
    {
        return Vector3.Distance(controllerRight.transform.position, controllerLeft.transform.position);
    }

    private float NormalizedDistanceBetweenControllers()
    {
        float distance = DistanceBetweenControllers();
        return Mathf.InverseLerp(distanceMinGun, distanceMaxGun, distance);
    }

    private void ResizedStick()
    {
        float opening = NormalizedDistanceBetweenControllers();
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