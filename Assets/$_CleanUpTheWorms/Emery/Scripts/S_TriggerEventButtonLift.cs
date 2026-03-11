using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class S_TriggerEventButtonLift : MonoBehaviour
{
    public UnityEvent OnButtonPushed;

    [SerializeField] private Vector3 pushOffset = new Vector3(0, -0.05f, 0);
    [SerializeField] private float smoothTime = 0.05f;
    [SerializeField] private float maxSpeed = 5f;

    bool canBePushed = true;

    private Vector3 initialLocalPosition;
    private Vector3 currentVelocity = Vector3.zero;
    private Coroutine pushCoroutine;

    private void Awake()
    {
        initialLocalPosition = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ButtonLift") && canBePushed)
        {
            OnButtonPushed.Invoke();

            if (pushCoroutine != null)
            {
                StopCoroutine(pushCoroutine);
            }
            pushCoroutine = StartCoroutine(PushAndReturnCoroutine());
        }
    }

    public void SetCanBePushed(bool can)
    {
        canBePushed = can;
    }

    private IEnumerator PushAndReturnCoroutine()
    {
        Vector3 targetPosition = initialLocalPosition + pushOffset;

        while (Vector3.Distance(transform.localPosition, targetPosition) > 0.001f)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref currentVelocity, smoothTime, maxSpeed);
            yield return null;
        }

        while (Vector3.Distance(transform.localPosition, initialLocalPosition) > 0.001f)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, initialLocalPosition, ref currentVelocity, smoothTime, maxSpeed);
            yield return null;
        }

        transform.localPosition = initialLocalPosition;
        currentVelocity = Vector3.zero;
    }
}