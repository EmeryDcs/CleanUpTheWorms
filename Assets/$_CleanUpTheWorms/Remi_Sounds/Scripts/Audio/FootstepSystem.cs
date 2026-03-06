using UnityEngine;
using System.Collections.Generic;

public class FootstepSystem : MonoBehaviour
{
    [Header("References")]
    //[SerializeField] private FirstPersonController _fpController;
    [SerializeField] private CharacterController _characterController;

    [Header("Audio Settings")]
    [SerializeField] private List<AudioClip> _footstepClips;
    [SerializeField] private UnityEngine.Audio.AudioMixerGroup _mixerGroup;

    [Range(0f, 5f)]
    [SerializeField] private float _volume = 0.5f;

    [Header("Pitch Randomization")]
    [Tooltip("The minimum pitch variation")]
    [Range(0.1f, 3f)][SerializeField] private float _minPitch = 0.8f;
    [Tooltip("The maximum pitch variation")]
    [Range(0.1f, 3f)][SerializeField] private float _maxPitch = 1.2f;

    [Header("Step Configuration")]
    [Tooltip("Distance traveled to trigger a footstep")]
    [SerializeField] private float _stepDistance = 1.8f;
    [Tooltip("Multiplier for step distance when sprinting (takes longer steps)")]
    [SerializeField] private float _sprintStepMultiplier = 1.5f;

    private float _distanceTraveled;

    private void Awake()
    {
        // Automatically get references if they are missing
        //if (_fpController == null) _fpController = GetComponent<FirstPersonController>();
        if (_characterController == null) _characterController = GetComponent<CharacterController>();
    }

    //private void Update()
    //{
    //    HandleFootsteps();
    //}

    //private void HandleFootsteps()
    //{
    //    //if (_fpController == null || _characterController == null) return;

    //    //// 1. Only play sounds if grounded and moving
    //    //if (!_fpController.Grounded || _characterController.velocity.magnitude < 0.1f) return;

    //    //// 2. Determine current step interval (Sprint steps are longer than Walk steps)
    //    //float currentStepInterval = _stepDistance;

    //    //// We check if current speed exceeds walking speed to determine if sprinting
    //    //if (_characterController.velocity.magnitude > _fpController.MoveSpeed + 0.5f)
    //    //{
    //    //    currentStepInterval *= _sprintStepMultiplier;
    //    //}

    //    // 3. Accumulate distance
    //    _distanceTraveled += _characterController.velocity.magnitude * Time.deltaTime;

    //    // 4. Trigger sound if distance threshold reached
    //    if (_distanceTraveled >= currentStepInterval)
    //    {
    //        PlayRandomFootstep();
    //        _distanceTraveled = 0f;
    //    }
    //}

    private void PlayRandomFootstep()
    {
        if (_footstepClips.Count == 0) return;
        if (AudioManager.Instance == null) return;

        // Select a random clip
        AudioClip clip = _footstepClips[Random.Range(0, _footstepClips.Count)];

        // --- USING YOUR AUDIOMANAGER METHODS ---

        // 1. Prepare the sound (Create the AudioSource object)
        // We leave parent 'null' so the footstep stays where it landed in world space
        AudioSource source = AudioManager.Instance.PrepareSound(
            clip,
            transform.position,
            _mixerGroup,
            null,
            _volume,
            1f // Spatial Blend (1 = 3D Sound)
        );

        if (source != null)
        {
            // 2. Randomize Pitch
            source.pitch = Random.Range(_minPitch, _maxPitch);

            // 3. Play the prepared sound and destroy it after it finishes
            AudioManager.Instance.PlayPreparedSound(source, transform.position, true);
        }
    }
}