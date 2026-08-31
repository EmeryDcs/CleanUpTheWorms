using System;
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
    
    private Vector3[] velocityHistory = new Vector3[10];
    private int velocityHistoryIndex = 0;

    private Rigidbody recentlyDetachedRb;
    private float throwWindowTimer = 0f;
    private float throwWindowDuration = 0.15f;

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
        Vector3 currentVelocity;
        InputDevices.GetDeviceAtXRNode(controllerNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceVelocity, out currentVelocity);

        velocityHistory[velocityHistoryIndex] = currentVelocity;
        velocityHistoryIndex = (velocityHistoryIndex + 1) % velocityHistory.Length;

        Vector3 bestVelocity = Vector3.zero;
        float maxSqrMag = 0f;
        foreach (Vector3 v in velocityHistory)
        {
            if (v.sqrMagnitude > maxSqrMag)
            {
                maxSqrMag = v.sqrMagnitude;
                bestVelocity = v;
            }
        }
        controllerVelocity = bestVelocity;
        
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

        if (throwWindowTimer > 0f && recentlyDetachedRb != null)
        {
            throwWindowTimer -= Time.deltaTime;
            
            if (currentVelocity.sqrMagnitude > (recentlyDetachedRb.linearVelocity.sqrMagnitude / 4f))
            {
                recentlyDetachedRb.linearVelocity = currentVelocity * 2;
            }
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
        
        recentlyDetachedRb = null;
        throwWindowTimer = 0f;

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
        Rigidbody targetRb = null;

        if (elm.GetComponent<AILarva>())
        {
            elm.transform.SetParent(null);
            grabbedElm = null;

            elm.GetComponent<AILarva>().RestartBehaviorAndMakeDynamic();
            elm.GetComponent<S_GrabbableState>().SetIsGrabbed(false);
            
            targetRb = elm.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                targetRb.linearVelocity = controllerVelocity * 2;
            }
        }
        else
        {
            targetRb = elm.GetComponent<Rigidbody>();

            if (elm != null && elm.GetComponent<Collider>())
            {
                elm.GetComponent<Collider>().isTrigger = false;
                elm.GetComponent<S_GrabbableState>().SetIsGrabbed(false);
            }

            targetRb.useGravity = true;
            targetRb.isKinematic = false;
            targetRb.linearVelocity = controllerVelocity * 2;
            
            elm.transform.SetParent(null);
            grabbedElm = null;
        }

        if (targetRb != null)
        {
            recentlyDetachedRb = targetRb;
            throwWindowTimer = throwWindowDuration;
        }
    }

    public GameObject GetGrabbedElm()
    {
        return grabbedElm;
    }
}