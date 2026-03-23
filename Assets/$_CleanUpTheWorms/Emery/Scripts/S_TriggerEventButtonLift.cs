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
    [SerializeField] private Image logo;
    [SerializeField] private float fadeDuration = 1f;

    bool canBePushed = true;
    bool canTPElevator = false;
    [SerializeField] bool isElevator = false;

    private Vector3 initialLocalPosition;
    private Vector3 currentVelocity = Vector3.zero;
    private Coroutine pushCoroutine;
    private Coroutine elevatorFade;
    private Coroutine blinking;

    [SerializeField] UnityEvent winBubble;
    [SerializeField] UnityEvent looseBubble;

    [SerializeField] private Light targetLight;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 5f;
    [SerializeField] private float pulseSpeed = 2f;
    
    private bool canTriggerEnd = false;



    bool canActivateHasNotLose = true;

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
                if (isElevator)
                {
                    OnButtonPushed.Invoke();
                    canTPElevator = true;
                    SetCanBePushed(false);
                }
                else if (StateMachineGame.Instance.state == GameState.LEVEL1)
                {
                    OnButtonPushed.Invoke();
                    SetCanBePushed(false);

                }




                if (pushCoroutine != null)
                {
                    StopCoroutine(pushCoroutine);
                }
                pushCoroutine = StartCoroutine(PushAndReturnCoroutine());

                
            }

            if (canTPElevator && StateMachineGame.Instance.state != GameState.ENDING)
            {
                elevatorFade = StartCoroutine(FadeTeleportFadeRoutine());

            }

            

            if (StateMachineGame.Instance.state == GameState.END && !isElevator && canActivateHasNotLose && canTriggerEnd)
            {
                Debug.Log("Button Lift Triggered");

                canTriggerEnd = false;

                if (pushCoroutine != null)
                {
                    StopCoroutine(pushCoroutine);
                }
                pushCoroutine = StartCoroutine(PushAndReturnCoroutine());

                TriggerBlinking(false);
                Ending(true);
            }

            else
            {
               Debug.Log("Button Lift Triggered but conditions not met : " + StateMachineGame.Instance.state + ", " + isElevator + ", " + canActivateHasNotLose);
            }
        }
    }

    public void Ending(bool value)
    {
        StateMachineGame.Instance.state = GameState.ENDING;

        if (value)
        {
            winBubble.Invoke();
            StateMachineGame.Instance.hasWin = true;
        }
        else 
        {
            looseBubble.Invoke();
        }

        TriggerBlinking(false);


        StartCoroutine(FadeEnding());
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

    public void SetCanActivateHasNotLose(bool value)
    {
        canActivateHasNotLose = value;
    }
    private IEnumerator FadeEnding()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        Color colorLogo = logo.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            colorLogo.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            logo.color = colorLogo;
            yield return null;
        }
    }


    public void TriggerBlinking(bool value)
    {
        if (value)
        {
            blinking = StartCoroutine(BlinkingLight());
        }
        else
        {
            if (blinking != null)
            {
                StopCoroutine(blinking);
                blinking = null;
            }
            targetLight.intensity = minIntensity;
        }
    }

    private IEnumerator BlinkingLight()
    {
        while (true)
        {
            while (true)
            {
                float pingPong = Mathf.PingPong(Time.time * pulseSpeed, 1f);
                targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, pingPong);
                yield return null;
            }
        }
    }
    
    public void SetCanTriggerEnd(bool value)
    {
        canTriggerEnd = value;
    }
}