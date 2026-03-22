using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class S_AgrandirTige : MonoBehaviour
{
    [Header("Taille de la tige min et max")]
    public float minTaille = 0.5f;
    [Tooltip("maxTaille peut évoluer dans le temps pour agrandir la tige au fur et à mesure que le joueur progresse dans le jeu")]
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

    void Update()
    {
        if (StateMachineGame.Instance.state == GameState.LEVEL2 || StateMachineGame.Instance.state == GameState.END || StateMachineGame.Instance.state == GameState.ENDING)
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