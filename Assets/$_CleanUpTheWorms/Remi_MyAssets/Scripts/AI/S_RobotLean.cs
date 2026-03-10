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
    [SerializeField] private float lateralThreshold = 10f;

    [Header("Configuration")]
    [SerializeField] private bool swapAxes = true;

    private Transform rootTransform;
    private Vector3 lastPosition;
    private float lastYRotation;
    private Vector3 targetLean;
    private float initialModelRotationY;
    private Vector3 smoothedVelocity;

    void Start()
    {
        rootTransform = transform.parent;

        if (rootTransform == null)
        {
            rootTransform = transform;
        }

        initialModelRotationY = transform.localEulerAngles.y;
        lastPosition = rootTransform.position;
        lastYRotation = rootTransform.eulerAngles.y;
    }

    void Update()
    {
        CalculateLean();
        ApplyLean();
    }

    void CalculateLean()
    {
        Vector3 currentPosition = rootTransform.position;
        Vector3 displacement = currentPosition - lastPosition;
        Vector3 currentVelocity = displacement / Time.deltaTime;

        smoothedVelocity = Vector3.Lerp(smoothedVelocity, currentVelocity, Time.deltaTime * 10f);
        lastPosition = currentPosition;

        Vector3 localVelocity = rootTransform.InverseTransformDirection(smoothedVelocity);

        float currentYRotation = rootTransform.eulerAngles.y;
        float deltaRotation = Mathf.DeltaAngle(lastYRotation, currentYRotation);
        float turnSpeed = deltaRotation / Time.deltaTime;
        lastYRotation = currentYRotation;

        float rawForwardLean = Mathf.Clamp(localVelocity.z * forwardLeanIntensity, -maxLeanAngle, maxLeanAngle);
        float rawLateralLean = 0f;

        if (Mathf.Abs(turnSpeed) > lateralThreshold && localVelocity.z > 0.1f)
        {
            rawLateralLean = Mathf.Clamp(-turnSpeed * turnLeanIntensity, -maxLeanAngle, maxLeanAngle);
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

        targetLean = new Vector3(finalX, initialModelRotationY, finalZ);
    }

    void ApplyLean()
    {
        Quaternion currentRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(targetLean);

        transform.localRotation = Quaternion.Lerp(
            currentRotation,
            targetRotation,
            Time.deltaTime * leanSpeed
        );
    }
}