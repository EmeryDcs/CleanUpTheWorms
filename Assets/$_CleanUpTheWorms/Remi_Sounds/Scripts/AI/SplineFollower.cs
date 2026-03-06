using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    [Header("Spline Settings")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float completionTime = 10f;

    [Header("Rotation Settings")]
    [SerializeField] private bool alignToSpline = true;

    [Header("Movement Control")]
    [SerializeField] bool canBeginMovement = true;

    private float splineProgress = 0f;

    public void SetSplineContainer(SplineContainer container)
    {
        splineContainer = container;
    }

    void Update()
    {
        if (splineContainer == null || !canBeginMovement) return;

        splineProgress += Time.deltaTime / completionTime;

        if (splineProgress > 1f)
        {
            splineProgress -= 1f;
        }

        Vector3 position = splineContainer.EvaluatePosition(splineProgress);
        transform.position = position;

        if (alignToSpline)
        {
            Vector3 tangent = splineContainer.EvaluateTangent(splineProgress);
            Vector3 up = splineContainer.EvaluateUpVector(splineProgress);

            Vector3 right = tangent;
            Vector3 forward = Vector3.Cross(up, right).normalized;

            forward = -forward;

            if (forward != Vector3.zero && up != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(forward, up);
                transform.rotation = targetRotation;
            }
        }
    }

    public void SetProgress(float progress)
    {
        splineProgress = Mathf.Clamp01(progress);
    }

    public void SetCompletionTime(float time)
    {
        completionTime = Mathf.Max(0.1f, time);
    }
}