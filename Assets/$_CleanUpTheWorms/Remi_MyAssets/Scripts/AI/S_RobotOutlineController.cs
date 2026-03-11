using UnityEngine;

public class RobotOutlineController : MonoBehaviour
{
    [Header("Settings")]
    public float activationDistance = 2.0f; // Distance in meters
    public Color alertColor = Color.red;
    public float outlineThickness = 5.0f;

    private Outline robotOutline;

    void Start()
    {
        // Get the Outline component on the Robot
        robotOutline = GetComponent<Outline>();

        // Ensure outline is off at start
        if (robotOutline != null)
            robotOutline.enabled = false;
    }

    void Update()
    {
        // Check for robotOutline and the grab singleton instance
        if (robotOutline == null || grab.Instance == null) return;

        // 1. Calculate distance between the Grab Instance (the hand/tool) and this Robot
        float distance = Vector3.Distance(grab.Instance.transform.position, transform.position);

        // 2. Check if the robot is currently holding an object
        bool isHoldingObject = grab.Instance.GetGrabbedElm() != null;

        // 3. Logic: Near the grabber AND holding something
        if (distance <= activationDistance && isHoldingObject)
        {
            if (!robotOutline.enabled)
            {
                FindFirstObjectByType<S_RobotAnim>().SetAnimOpening(true);
                robotOutline.enabled = true;
                robotOutline.OutlineColor = alertColor;
                robotOutline.OutlineWidth = outlineThickness;
            }
        }
        else
        {
            if (robotOutline.enabled)
            {
                robotOutline.enabled = false;
                FindFirstObjectByType<S_RobotAnim>().SetAnimOpening(false);
            }
                
        }
    }
}