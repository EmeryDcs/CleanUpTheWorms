using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class WalkieTalkieLocal : MonoBehaviour
{
    public InputActionReference boutonParler;
    private AudioSource sourceAudio;
    private string nomMicrophone;

    void Awake()
    {
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        sourceAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            nomMicrophone = Microphone.devices[0];
        }
    }

    void OnEnable()
    {
        if (boutonParler != null)
        {
            boutonParler.action.started += DemarrerMicro;
            boutonParler.action.canceled += ArreterMicro;
            boutonParler.action.Enable();
        }
    }

    void OnDisable()
    {
        if (boutonParler != null)
        {
            boutonParler.action.started -= DemarrerMicro;
            boutonParler.action.canceled -= ArreterMicro;
            boutonParler.action.Disable();
        }
    }

    private void DemarrerMicro(InputAction.CallbackContext context)
    {
        Debug.Log("test");
        if (string.IsNullOrEmpty(nomMicrophone)) return;
        sourceAudio.clip = Microphone.Start(nomMicrophone, true, 1, 44100);
        sourceAudio.loop = true;
        while (!(Microphone.GetPosition(nomMicrophone) > 0)) { }
        sourceAudio.Play();
    }

    private void ArreterMicro(InputAction.CallbackContext context)
    {
        if (string.IsNullOrEmpty(nomMicrophone)) return;
        Microphone.End(nomMicrophone);
        sourceAudio.Stop();
        sourceAudio.clip = null;
    }
}