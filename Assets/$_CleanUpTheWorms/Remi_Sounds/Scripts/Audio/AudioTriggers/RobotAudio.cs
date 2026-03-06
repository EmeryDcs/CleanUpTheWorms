using UnityEngine;

public class RobotAudio : MonoBehaviour
{
    [Header("Upgrade Settings")]
    [SerializeField] private Sound upgradeAudio;
    [SerializeField] private string upgradeVestName;

    private void Awake()
    {
        InitializeSound(upgradeAudio, "UpgradeAudioSource");
    }

    private void InitializeSound(Sound s, string childName)
    {
        if (s == null || s.clip == null) return;

        GameObject childObj = new GameObject(childName);
        childObj.transform.SetParent(transform);
        childObj.transform.localPosition = Vector3.zero;

        s.source = childObj.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.spatialBlend = s.spatialBlend;
        s.source.outputAudioMixerGroup = s.mixerGroup;
        s.source.loop = s.loop;
        s.source.playOnAwake = s.playOnAwake;

        if (s.playOnAwake)
        {
            s.source.Play();
        }
    }

    [ContextMenu("Play Upgrade Event")]
    public void PlayUpgrade()
    {
        if (upgradeAudio != null && upgradeAudio.source != null)
        {
            if (upgradeAudio.preventOverlay || upgradeAudio.loop)
            {
                upgradeAudio.source.Play();
            }
            else
            {
                upgradeAudio.source.PlayOneShot(upgradeAudio.clip);
            }
        }

        if (AudioManagerVest.Instance != null && !string.IsNullOrEmpty(upgradeVestName))
        {
            AudioManagerVest.Instance.PlayGlobalVestSound(upgradeVestName);
        }
    }
}