using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class S_TriggerEventButtonLift : MonoBehaviour
{
    private int m_WaitToEnd = 6;
    public UnityEvent OnButtonPushed;

    [SerializeField] private Vector3 pushOffset = new Vector3(0, -0.05f, 0);
    [SerializeField] private float smoothTime = 0.05f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private Animator animatorLift;

    [SerializeField] private GameObject canvas;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Image logo;
    [SerializeField] private Image logoXR;
    [SerializeField] private Image logoAM;
    [SerializeField] private Image logoHolo;
    [SerializeField] private TextMeshProUGUI textCredits;
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
        if (isElevator)
            StartCoroutine(FadeBeginGame());
    }
    
    private IEnumerator FadeBeginGame()
    {
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;
        canvas.SetActive(true);
        
        
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        canvas.SetActive(false);
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
        m_WaitToEnd = 8;
        
        if (value)
        {
            winBubble.Invoke();
            StateMachineGame.Instance.hasWin = true;
            S_BlendLight.instance.SetScenario("3.3", 20);

            fadeDuration = 1;

            foreach (var manager in FindObjectsByType<SplineCharacterManager>(FindObjectsSortMode.None))
            {
                manager.BeginDrainMode();
                manager.gameObject.SetActive(false);
            }
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
        canvas.SetActive(true);
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
        canvas.SetActive(false);

        elevatorFade = null;
    }

    public void SetCanActivateHasNotLose(bool value)
    {
        canActivateHasNotLose = value;
    }
    private IEnumerator FadeEnding()
    {
        yield return new WaitForSeconds(m_WaitToEnd);
        canvas.SetActive(true);
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        Color colorLogo = logo.color;
        Color colorLogoXR = logoXR.color;
        Color colorLogoAM = logoAM.color;
        Color colorLogoHolo = logoHolo.color;
        Color colorText = textCredits.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            colorLogo.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            colorLogoXR.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            colorLogoAM.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            colorLogoHolo.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            colorText.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            logo.color = colorLogo;
            logoXR.color = colorLogoXR;
            logoAM.color = colorLogoAM;
            logoHolo.color = colorLogoHolo;
            textCredits.color = colorText;
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