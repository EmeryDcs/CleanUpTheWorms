using UnityEngine;
using TMPro;
using System.Collections;

public class S_DialogueBubbleAudio : MonoBehaviour
{
    [SerializeField] bool isFirstBubble;
    TextMeshProUGUI textComponent;
    [SerializeField] float typingSpeed = 0.02f;

    private Coroutine typingCoroutine;

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (isFirstBubble && RobotAudio.Instance != null)
        {
            RobotAudio.Instance.PlayRandomSpeech();
            PrepareRevealText();
        }
    }

    private void OnEnable()
    {
        if (RobotAudio.Instance != null)
        {
            RobotAudio.Instance.PlayRandomSpeech();
        }

        PrepareRevealText();
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
}