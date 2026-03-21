using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class S_TriggerEventButtonLift : MonoBehaviour
{
    public UnityEvent OnButtonPushed;

    [SerializeField] private Vector3 pushOffset = new Vector3(0, -0.05f, 0);
    [SerializeField] private float smoothTime = 0.05f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private Animator animatorLift;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    bool canBePushed = true;
    bool canTPElevator = false;

    private Vector3 initialLocalPosition;
    private Vector3 currentVelocity = Vector3.zero;
    private Coroutine pushCoroutine;
    private Coroutine elevatorFade;

    private void Awake()
    {
        initialLocalPosition = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ButtonLift"))
        {
            if (elevatorFade != null) return;

            if (canBePushed)
            {
                OnButtonPushed.Invoke();

                canTPElevator = true;

                if (pushCoroutine != null)
                {
                    StopCoroutine(pushCoroutine);
                }
                pushCoroutine = StartCoroutine(PushAndReturnCoroutine());
            }

            if (canTPElevator)
            {
                elevatorFade = StartCoroutine(FadeTeleportFadeRoutine());
            }
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

    public void OpenDoors()
    {
        animatorLift.SetBool("isTutoOver", true);
    }

    private IEnumerator FadeTeleportFadeRoutine()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        FindFirstObjectByType<S_RecenterPosition>()?.TeleportDesk();

        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        elevatorFade = null;
    }
}