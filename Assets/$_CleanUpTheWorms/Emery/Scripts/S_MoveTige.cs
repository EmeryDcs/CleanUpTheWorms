using UnityEngine;

public class S_MoveTige : MonoBehaviour
{
    public Transform target;
    public bool maintainOffset = true;
    public bool avoidMeshToggle = false;
    public float minLocalX = -0.407f;

    private Vector3 positionOffset;
    private Quaternion rotationOffset;
    private Renderer objRenderer;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();

        if (target != null && maintainOffset)
        {
            positionOffset = target.InverseTransformPoint(transform.position);
            rotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            if (maintainOffset)
            {
                transform.position = target.TransformPoint(positionOffset);
                transform.rotation = target.rotation * rotationOffset;
            }
            else
            {
                transform.position = target.position;
                transform.rotation = target.rotation;
            }
        }

        if (!avoidMeshToggle && objRenderer != null)
        {
            objRenderer.enabled = transform.localPosition.x < minLocalX;
        }
    }
}