using Meta.XR.InputActions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class grab : MonoBehaviour
{
	public static grab Instance { get; private set; }

	public Transform snapPoint;
    //Transform localPos;
    private GameObject grabbedElm;
    public bool canGrab;

    public GameObject bras;

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

        //inputActions.Player.Grab.performed += OnGrabStarted;
        //inputActions.Player.Grab.performed += OnGrabCanceled;
    }

    // Update is called once per frame
    void Update()
    {
        triggerValue = inputActions.Player.Grab.ReadValue<float>();

        bras.transform.localRotation = new Quaternion(0,0,triggerValue, 1f);

        canGrab = (triggerValue <= 0.6f && triggerValue != 0f);

        sphereCollider.enabled = canGrab;

        if (grabbedElm != null && triggerValue == 0)
        {
            DetachElm(grabbedElm);
        }

        if(grabbedElm != null && !canGrab)
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
        grabbedElm = elm; 
        Rigidbody rb = grabbedElm.GetComponent<Rigidbody>(); 
        rb.useGravity = false;
        rb.isKinematic = true;
        
        // Snap propre
        grabbedElm.transform.SetParent(snapPoint); 
        Debug.Log("truc");
        //grabbedElm.transform.localPosition = Vector3.zero;
    
    }

    private void DetachElm(GameObject elm)
    {
        Rigidbody rb = grabbedElm.GetComponent<Rigidbody>();
        rb.useGravity = true; 
        rb.isKinematic = false;
        elm.transform.SetParent(null); 
        grabbedElm = null;
    }

	public GameObject GetGrabbedElm()
	{
		return grabbedElm;
	}

    //private void OnGrabStarted(InputAction.CallbackContext context)
    //{
    //    isOpen = true;
    //    GetComponent<SphereCollider>().enabled = true;
    //}

    //private void OnGrabCanceled(InputAction.CallbackContext context)
    //{
    //    isOpen = false;
    //    GetComponent<SphereCollider>().enabled = false;
    //}
}
