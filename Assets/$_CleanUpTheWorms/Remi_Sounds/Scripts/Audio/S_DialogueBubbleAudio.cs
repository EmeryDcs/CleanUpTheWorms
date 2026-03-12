using UnityEngine;
using TMPro;
using System.Collections;

public class S_DialogueBubbleAudio : MonoBehaviour
{
    [SerializeField] bool isFirstBubble;
    TextMeshProUGUI textComponent;
    [SerializeField] float typingSpeed = 0.02f;

    [SerializeField] bool isPlayingAnim;
    [SerializeField] bool isOpening;

    [SerializeField] int voiceOverIndex;

    [Header("Special Upgrade: Blinking Light")]
    [SerializeField] bool isSpecialUpgrade;
    [SerializeField] Light targetLight;
    [SerializeField] float blinkInterval = 0.15f;

    private Coroutine typingCoroutine;
    private Coroutine blinkingCoroutine;

    private static S_DialogueBubbleAudio currentActiveInstance;
    private bool originalLightState;

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (isFirstBubble && RobotAudio.Instance != null)
        {
            RobotAudio.Instance.PlayRandomSpeech();
        }

        PrepareRevealText();
        PlayingAnim();
        HandleBlinkingState();

        S_VoiceOver.Instance.PlayAudio(voiceOverIndex);
    }

    private void OnEnable()
    {
        if (RobotAudio.Instance != null)
        {
            RobotAudio.Instance.PlayRandomSpeech();
        }
    }

    private void OnDisable()
    {
        if (currentActiveInstance == this)
        {
            StopCurrentBlinking();
        }
    }

    private void PrepareRevealText()
    {
        if (textComponent != null)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(RevealText());
        }
    }

    private IEnumerator RevealText()
    {
        textComponent.ForceMeshUpdate();
        int totalCharacters = textComponent.textInfo.characterCount;
        textComponent.maxVisibleCharacters = 0;

        for (int i = 0; i <= totalCharacters; i++)
        {
            textComponent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void PlayingAnim()
    {
        if (isPlayingAnim)
        {
            FindFirstObjectByType<S_RobotAnim>().SetAnimOpening(isOpening);
        }
    }

    private void HandleBlinkingState()
    {
        if (currentActiveInstance != null && currentActiveInstance != this)
        {
            currentActiveInstance.StopCurrentBlinking();
        }

        currentActiveInstance = this;

        if (isSpecialUpgrade && targetLight != null)
        {
            originalLightState = targetLight.enabled;
            if (blinkingCoroutine != null)
            {
                StopCoroutine(blinkingCoroutine);
            }
            blinkingCoroutine = StartCoroutine(BlinkLightCoroutine());
        }
    }

    private void StopCurrentBlinking()
    {
        if (blinkingCoroutine != null)
        {
            StopCoroutine(blinkingCoroutine);
            blinkingCoroutine = null;
        }

        if (targetLight != null)
        {
            targetLight.enabled = originalLightState;
        }
    }

    private IEnumerator BlinkLightCoroutine()
    {
        while (true)
        {
            targetLight.enabled = !targetLight.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}