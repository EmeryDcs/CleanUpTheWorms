using UnityEngine;

public class S_RobotLean : MonoBehaviour
{
    [Header("Lean Settings")]
    [SerializeField] private float maxLeanAngle = 10f;
    [SerializeField] private float leanSpeed = 8f;

    [Header("Forward Intensity")]
    [SerializeField] private float forwardLeanIntensity = 5f;

    [Header("Lateral Intensity (Banking)")]
    [SerializeField] private float turnLeanIntensity = 0.5f;

    [Header("Configuration")]
    [SerializeField] private bool swapAxes = true;

    private Transform rootTransform;
    private Vector3 lastPosition;
    private float lastYRotation;
    private Vector3 currentLean;
    private Vector3 smoothedVelocity;
    private float smoothedTurnSpeed;

    void Start()
    {
        rootTransform = transform.parent;

        if (rootTransform == null)
        {
            rootTransform = transform;
        }

        lastPosition = rootTransform.position;
        lastYRotation = rootTransform.eulerAngles.y;
    }

    void LateUpdate()
    {
        CalculateLean();
        ApplyLean();
    }

    void CalculateLean()
    {
        if (Time.deltaTime <= 0f) return;

        Vector3 currentPosition = rootTransform.position;
        Vector3 displacement = currentPosition - lastPosition;
        Vector3 currentVelocity = displacement / Time.deltaTime;

        smoothedVelocity = Vector3.Lerp(smoothedVelocity, currentVelocity, Time.deltaTime * 10f);
        lastPosition = currentPosition;

        Vector3 localVelocity = rootTransform.InverseTransformDirection(smoothedVelocity);

        float currentYRotation = rootTransform.eulerAngles.y;
        float deltaRotation = Mathf.DeltaAngle(lastYRotation, currentYRotation);
        float currentTurnSpeed = deltaRotation / Time.deltaTime;
        lastYRotation = currentYRotation;

        smoothedTurnSpeed = Mathf.Lerp(smoothedTurnSpeed, currentTurnSpeed, Time.deltaTime * 10f);

        float rawForwardLean = Mathf.Clamp(localVelocity.z * forwardLeanIntensity, -maxLeanAngle, maxLeanAngle);
        float rawLateralLean = 0f;

        if (localVelocity.z > 0.1f)
        {
            rawLateralLean = Mathf.Clamp(smoothedTurnSpeed * turnLeanIntensity, -maxLeanAngle, maxLeanAngle);
        }

        float finalX = 0f;
        float finalZ = 0f;

        if (swapAxes)
        {
            finalZ = rawForwardLean;
            finalX = rawLateralLean;
        }
        else
        {
            finalX = rawForwardLean;
            finalZ = rawLateralLean;
        }

        Vector3 targetLean = new Vector3(finalX, 0f, finalZ);
        currentLean = Vector3.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);
    }

    void ApplyLean()
    {
        transform.localRotation = transform.localRotation * Quaternion.Euler(currentLean);
    }
}