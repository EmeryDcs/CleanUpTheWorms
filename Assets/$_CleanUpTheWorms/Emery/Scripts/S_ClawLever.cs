using UnityEngine;

public class S_ClawLever : MonoBehaviour
{
    public Transform leverTarget;
    public float minLocalX = -0.5f;
    public float maxLocalX = 0.5f;
    public float offsetX = 0f;
    public bool isFollowing = true;

    public Transform ikTarget;
    public float minLocalXIK = -0.5f;
    public float maxLocalXIK = 0.5f;
    public float ikOffsetX = 0f;

    private Transform leftController;

    void Start()
    {
        OVRCameraRig rig = FindAnyObjectByType<OVRCameraRig>();
        if (rig != null)
        {
            leftController = rig.leftControllerAnchor;
        }

        if (leverTarget == null)
        {
            leverTarget = transform;
        }
    }

    void Update()
    {
        if (isFollowing && leftController != null)
        {
            if (leverTarget != null && leverTarget.parent != null)
            {
                Vector3 controllerLocalPos = leverTarget.parent.InverseTransformPoint(leftController.position);
                Vector3 newPos = leverTarget.localPosition;

                newPos.x = Mathf.Clamp(controllerLocalPos.x + offsetX, minLocalX, maxLocalX);
                leverTarget.localPosition = newPos;
            }

            if (ikTarget != null && ikTarget.parent != null)
            {
                Vector3 ikControllerLocalPos = ikTarget.parent.InverseTransformPoint(leftController.position);
                Vector3 ikNewPos = ikTarget.localPosition;

                ikNewPos.x = Mathf.Clamp(ikControllerLocalPos.x + ikOffsetX, minLocalXIK, maxLocalXIK);
                ikTarget.localPosition = ikNewPos;
            }
        }
    }
}