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

    private PlayerInputSystem inputActions;
    private float triggerValue;
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

            return;
        }

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