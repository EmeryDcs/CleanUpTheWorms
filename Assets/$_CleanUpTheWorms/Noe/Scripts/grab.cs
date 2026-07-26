using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class grab : MonoBehaviour
{
    public static grab Instance { get; private set; }

    public Transform snapPoint;
    private GameObject grabbedElm;
    public bool m_CanGrab;

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
    
    private Vector3 controllerVelocity;

    private void Awake()
    {
        inputActions = new PlayerInputSystem();
        sphereCollider = GetComponent<SphereCollider>();

        sphereCollider.enabled = true;

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
        
        InputDevices.GetDeviceAtXRNode(controllerNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceVelocity, out controllerVelocity);
        Debug.Log(controllerVelocity);
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

        m_CanGrab = (triggerValue <= 0.6f && triggerValue != 0f);

        if (triggerValue > 0.05f && grabbedElm == null)
        {
            InputDevices.GetDeviceAtXRNode(controllerNode).SendHapticImpulse(0u, 0.5f, Time.deltaTime);
        }

        if (grabbedElm != null && triggerValue == 0)
        {
            DetachElm(grabbedElm);
        }

        if (grabbedElm != null && !m_CanGrab)
        {
            grabbedElm.transform.position = snapPoint.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "GrabbableElm" && grabbedElm == null)
        {
            other.gameObject.GetComponent<S_GrabbableState>().SetOutline(true);
            if (!m_CanGrab) return;
            GrabElm(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "GrabbableElm" && grabbedElm == null)
            other.gameObject.GetComponent<S_GrabbableState>().SetOutline(false);
    }

    private void GrabElm(GameObject elm)
    {
        InputDevices.GetDeviceAtXRNode(controllerNode).SendHapticImpulse(0u, 1f, 0.4f);

        if (elm.GetComponent<AILarva>())
        {
            grabbedElm = elm;
            grabbedElm.transform.SetParent(snapPoint);
            elm.GetComponent<AILarva>().StopBehaviorAndMakeKinematic();
            
            elm.GetComponent<S_GrabbableState>().SetIsGrabbed(true);

            elm.GetComponent<S_GrabbableState>().SetCanBeTrashed(true);
            


            return;
        }

        elm.GetComponent<S_GrabbableState>().SetIsGrabbed(true);

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
            elm.GetComponent<S_GrabbableState>().SetIsGrabbed(false);
            
            Rigidbody larvaRb = elm.GetComponent<Rigidbody>();
            if (larvaRb != null)
            {
                larvaRb.linearVelocity = controllerVelocity * 2;
            }

            return;
        }

        Rigidbody rb = grabbedElm.GetComponent<Rigidbody>();

        if (elm != null && elm.GetComponent<Collider>())
        {
            elm.GetComponent<Collider>().isTrigger = false;
            elm.GetComponent<S_GrabbableState>().SetIsGrabbed(false);
        }

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.linearVelocity = controllerVelocity * 2;
        elm.transform.SetParent(null);
        grabbedElm = null;
    }

    public GameObject GetGrabbedElm()
    {
        return grabbedElm;
    }
}