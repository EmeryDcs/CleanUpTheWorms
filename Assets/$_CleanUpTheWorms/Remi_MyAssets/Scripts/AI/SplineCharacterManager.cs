using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class SplineCharacterManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private int numberOfCharacters = 5;

    [Header("Movement Settings")]
    [SerializeField] private float completionTime = 10f;

    [Header("Offset Settings")]
    [SerializeField] private float startingOffset = 0f;

    [Header("Randomization Settings")]
    [SerializeField] private float minRandomDelay = 0f;
    [SerializeField] private float maxRandomDelay = 1f;
    [SerializeField][Range(0f, 1f)] private float sizeRandomness = 0.2f;
    [SerializeField][Range(0f, 1f)] private float speedRandomness = 0.2f;

    private SplineContainer splineContainer;
    private GameObject[] spawnedCharacters;
    private Coroutine spawnCoroutine;
    private bool isDraining = false;

    void Start()
    {
        splineContainer = GetComponent<SplineContainer>();
        SpawnCharacters();
    }

    public void SpawnCharacters()
    {
        if (splineContainer == null)
        {
            splineContainer = GetComponent<SplineContainer>();
            if (splineContainer == null)
            {
                Debug.LogError("No SplineContainer found on this GameObject!");
                return;
            }
        }

        if (characterPrefab == null)
        {
            Debug.LogError("CharacterPrefab is not assigned!");
            return;
        }

        ClearCharacters();

        isDraining = false;
        spawnedCharacters = new GameObject[numberOfCharacters];
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        float baseSpawnDelay = completionTime / numberOfCharacters;

        for (int i = 0; i < numberOfCharacters; i++)
        {
            // Stop spawning new characters if drain mode was activated
            if (isDraining) yield break;

            GameObject character = Instantiate(characterPrefab, transform);
            spawnedCharacters[i] = character;

            // --- ADDED: Disable mesh immediately to avoid spawning flicker ---
            Renderer charRenderer = character.GetComponentInChildren<Renderer>();
            if (charRenderer != null) charRenderer.enabled = false;
            // -----------------------------------------------------------------

            float randomSizeFactor = Random.Range(1f - sizeRandomness, 1f + sizeRandomness);
            character.transform.localScale = characterPrefab.transform.localScale * randomSizeFactor;

            SplineFollower follower = character.GetComponent<SplineFollower>();

            if (follower != null)
            {
                follower.SetSplineContainer(splineContainer);

                float randomTimeFactor = Random.Range(1f - speedRandomness, 1f + speedRandomness);
                follower.SetCompletionTime(completionTime * randomTimeFactor);

                follower.SetProgress(startingOffset);

                // Wire up the drain callback
                follower.OnSplineCompleted += OnFollowerReachedEnd;

                // --- ADDED: Start the delay to show the mesh after 2 frames ---
                if (charRenderer != null) StartCoroutine(ShowMeshAfterFrames(charRenderer, 2));
                // --------------------------------------------------------------
            }
            else
            {
                Debug.LogWarning($"Character {i} doesn't have a SplineFollower component!");
                // Safety: show mesh anyway if no follower exists
                if (charRenderer != null) charRenderer.enabled = true;
            }

            float finalDelay = baseSpawnDelay + Random.Range(minRandomDelay, maxRandomDelay);
            yield return new WaitForSeconds(finalDelay);
        }
    }

    // New helper method to wait for specific frames
    private IEnumerator ShowMeshAfterFrames(Renderer renderer, int frameCount)
    {
        for (int i = 0; i < frameCount; i++)
        {
            yield return null; // Wait for one frame
        }
        if (renderer != null) renderer.enabled = true;
    }

    /// <summary>
    /// Stops spawning new characters and destroys each existing one
    /// as it naturally reaches the end of the spline.
    /// </summary>
    public void BeginDrainMode()
    {
        isDraining = true;

        // Stop the spawn coroutine so no new characters are queued
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private void OnFollowerReachedEnd(GameObject character)
    {
        if (!isDraining) return;

        // Unsubscribe before destroying to be safe
        SplineFollower follower = character.GetComponent<SplineFollower>();
        if (follower != null) follower.OnSplineCompleted -= OnFollowerReachedEnd;

        Destroy(character);
    }

    public void ClearCharacters()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if (spawnedCharacters != null)
        {
            foreach (GameObject character in spawnedCharacters)
            {
                if (character != null)
                {
                    Destroy(character);
                }
            }
        }
        spawnedCharacters = null;
    }

    [ContextMenu("Respawn Characters")]
    public void RespawnCharacters()
    {
        SpawnCharacters();
    }

    void OnDestroy()
    {
        ClearCharacters();
    }
}