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

    [Header("Distance entre les deux manettes min et max (mode -withdevice)")]
    [SerializeField]
    float distanceMaxGun = 0.3f;
    [SerializeField]
    float distanceMinGun = 0f;

    [Header("Controllers")]
    public GameObject controllerLeft;
    public GameObject controllerRight;

    [Header("Claw Lever (mode sans device)")]
    public S_ClawLever clawLever;

    [Header("Unity Event Size Changed")]
    public UnityEvent onSizeChanged;

    [Header("Debug")]
    public bool isInTestingScene = false;

    private float lastTaille = -1f;

    private bool isFastVersion = true;
    private bool withDevice = false; // true si l'argument -withdevice est present

    void Start()
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-fastversion")
            {
                isFastVersion = true;
            }
            else if (args[i] == "-nofastversion")
            {
                isFastVersion = false;
            }
            else if (args[i] == "-withdevice")
            {
                withDevice = true;
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

        if (!withDevice && clawLever == null)
        {
            clawLever = FindAnyObjectByType<S_ClawLever>();
            if (clawLever == null)
            {
                Debug.LogWarning("S_AgrandirTige : mode sans device mais aucun S_ClawLever trouve.");
            }
        }
    }

    // LateUpdate : le claw lever a deja mis a jour sa position dans son Update
    void LateUpdate()
    {
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

    private float ComputeNewTaille()
    {
        if (withDevice)
        {
            // Systeme d'origine : distance entre les manettes (inchange)
            float opening = NormalizedDistanceBetweenControllers();
            return Mathf.Lerp(minTaille, maxTaille, 1 - opening);
        }

        // Sans device : position du claw lever entre son min local et son max local
        // min local -> minTaille, max local -> maxTaille
        if (clawLever == null) return tige.transform.localScale.z;

        float t = clawLever.NormalizedPosition;
        return Mathf.Lerp(minTaille, maxTaille, t);
    }

    private void ResizedStick()
    {
        float newTaille = ComputeNewTaille();

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