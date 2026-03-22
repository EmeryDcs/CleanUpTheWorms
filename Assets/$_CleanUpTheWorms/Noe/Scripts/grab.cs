using Meta.XR.InputActions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class grab : MonoBehaviour
{
    public static grab Instance { get; private set; }

    public Transform snapPoint;
    private GameObject grabbedElm;
    public bool canGrab;

    public GameObject bras;

    public XRNode controllerNode = XRNode.RightHand;

    public Transform leftHandle;
    public Transform rightHandle;
    public float maxLeftHandleAngle = 45f;
    public float maxRightHandleAngle = -45f;
    public float grabbedLeftAngle = 30f;
    public float grabbedRightAngle = -30f;

    private PlayerInputSystem inputActions;
    [SerializeField] private float triggerValue;
    private SphereCollider sphereCollider;

    private void Awake()
    {
        inputActions = new PlayerInputSystem();
        sphereCollider = GetComponent<SphereCollider>();

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void Update()
    {
        triggerValue = inputActions.Player.Grab.ReadValue<float>();

        bras.transform.localRotation = new Quaternion(0, 0, triggerValue, 1f);

        if (grabbedElm == null)
        {
            if (leftHandle != null) leftHandle.localRotation = Quaternion.Euler(0, triggerValue * maxLeftHandleAngle, 0);
            if (rightHandle != null) rightHandle.localRotation = Quaternion.Euler(0, triggerValue * maxRightHandleAngle, 0);
        }
        else
        {
            if (leftHandle != null) leftHandle.localRotation = Quaternion.Euler(0, grabbedLeftAngle, 0);
            if (rightHandle != null) rightHandle.localRotation = Quaternion.Euler(0, grabbedRightAngle, 0);
        }

        canGrab = (triggerValue <= 0.6f && triggerValue != 0f);

        sphereCollider.enabled = canGrab;

        if (triggerValue > 0.05f && grabbedElm == null)
        {
            InputDevices.GetDeviceAtXRNode(controllerNode).SendHapticImpulse(0u, 0.5f, Time.deltaTime);
        }

        if (grabbedElm != null && triggerValue == 0)
        {
            DetachElm(grabbedElm);
        }

        if (grabbedElm != null && !canGrab)
        {
            grabbedElm.transform.position = snapPoint.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canGrab) return;

        if (other.gameObject.tag == "GrabbableElm" && grabbedElm == null)
        {
            GrabElm(other.gameObject);
        }
    }

    private void GrabElm(GameObject elm)
    {
        InputDevices.GetDeviceAtXRNode(controllerNode).SendHapticImpulse(0u, 1f, 0.4f);

        if (elm.GetComponent<AILarva>())
        {
            grabbedElm = elm;
            grabbedElm.transform.SetParent(snapPoint);
            elm.GetComponent<AILarva>().StopBehaviorAndMakeKinematic();

            elm.GetComponent<S_GrabbableState>().SetCanBeTrashed(true);

            return;
        }

        elm.GetComponent<S_GrabbableState>().SetCanBeTrashed(true);

        grabbedElm = elm;
        Rigidbody rb = grabbedElm.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        grabbedElm.transform.SetParent(snapPoint);
    }

    private void DetachElm(GameObject elm)
    {
        if (elm.GetComponent<AILarva>())
        {
            elm.transform.SetParent(null);
            grabbedElm = null;

            elm.GetComponent<AILarva>().RestartBehaviorAndMakeDynamic();

            return;
        }

        Rigidbody rb = grabbedElm.GetComponent<Rigidbody>();

        elm.GetComponent<Collider>().isTrigger = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        elm.transform.SetParent(null);
        grabbedElm = null;
    }

    public GameObject GetGrabbedElm()
    {
        return grabbedElm;
    }
}