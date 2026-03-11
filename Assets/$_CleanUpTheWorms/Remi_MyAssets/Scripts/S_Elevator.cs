using UnityEngine;
using System.Collections;

public class S_Elevator : MonoBehaviour
{
    public static S_Elevator Instance { get; private set; }

    [SerializeField] private float targetY = 10f;
    [SerializeField] private float ySmoothTime = 1.5f;
    [SerializeField] private float maxYSpeed = 5f;

    [SerializeField] private float targetX = -2f;
    [SerializeField] private float xSmoothTime = 1.0f;
    [SerializeField] private float maxXSpeed = 5f;

    private Vector3 initialLocalPosition;
    private float currentVelocityY = 0f;
    private float currentVelocityX = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        initialLocalPosition = transform.localPosition;

        transform.localPosition = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);

        StartCoroutine(MoveElevatorYCoroutine());
    }

    private IEnumerator MoveElevatorYCoroutine()
    {
        while (Mathf.Abs(transform.localPosition.y - initialLocalPosition.y) > 0.01f)
        {
            float newY = Mathf.SmoothDamp(transform.localPosition.y, initialLocalPosition.y, ref currentVelocityY, ySmoothTime, maxYSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);

            yield return null;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, initialLocalPosition.y, transform.localPosition.z);
    }

    public void OpenElevator()
    {
        StartCoroutine(OpenElevatorCoroutine());
    }

    private IEnumerator OpenElevatorCoroutine()
    {
        while (Mathf.Abs(transform.localPosition.x - targetX) > 0.01f)
        {
            float newX = Mathf.SmoothDamp(transform.localPosition.x, targetX, ref currentVelocityX, xSmoothTime, maxXSpeed);
            transform.localPosition = new Vector3(newX, transform.localPosition.y, transform.localPosition.z);

            yield return null;
        }

        transform.localPosition = new Vector3(targetX, transform.localPosition.y, transform.localPosition.z);
    }
}