using UnityEngine;

public class RobotOutlineController : MonoBehaviour
{
    [Header("Settings")]
    public float activationDistance = 2.0f; // Distance in meters
    public Color alertColor = Color.red;
    public float outlineThickness = 5.0f;

    private Outline robotOutline;
    private Transform vrCamera;

    void Start()
    {
        // Get the Outline component on the Robot
        robotOutline = GetComponent<Outline>();

        // Ensure outline is off at start
        if (robotOutline != null)
            robotOutline.enabled = false;

        // Find the VR Camera
        if (Camera.main != null)
            vrCamera = Camera.main.transform;
    }

    void Update()
    {
        if (vrCamera == null || robotOutline == null || grab.Instance == null) return;

        // 1. Calculate distance between VR Headset and this Robot
        float distance = Vector3.Distance(vrCamera.position, transform.position);

        // 2. Check if the robot is currently holding an object
        bool isHoldingObject = grab.Instance.GetGrabbedElm() != null;

        // 3. Logic: Near camera AND holding something
        if (distance <= activationDistance && isHoldingObject)
        {
            if (!robotOutline.enabled)
            {
                robotOutline.enabled = true;
                robotOutline.OutlineColor = alertColor;
                robotOutline.OutlineWidth = outlineThickness;
            }
        }
        else
        {
            if (robotOutline.enabled)
                robotOutline.enabled = false;
        }
    }
}