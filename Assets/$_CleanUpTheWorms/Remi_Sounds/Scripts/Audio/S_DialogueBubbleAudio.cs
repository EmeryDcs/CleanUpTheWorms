using UnityEngine;

public class S_DialogueBubbleAudio : MonoBehaviour
{

    [SerializeField] bool isFirstBubble;


    private void Start()
    {
        if (isFirstBubble && RobotAudio.Instance != null)
        {
            RobotAudio.Instance.PlayRandomSpeech();
        }
    }
    private void OnEnable()
    {
        if (RobotAudio.Instance != null)
        {
            RobotAudio.Instance.PlayRandomSpeech();
        }
    }
}