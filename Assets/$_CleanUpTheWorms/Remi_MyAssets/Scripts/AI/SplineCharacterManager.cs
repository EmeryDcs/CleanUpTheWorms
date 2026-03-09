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

        spawnedCharacters = new GameObject[numberOfCharacters];
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        float baseSpawnDelay = completionTime / numberOfCharacters;

        for (int i = 0; i < numberOfCharacters; i++)
        {
            GameObject character = Instantiate(characterPrefab, transform);
            spawnedCharacters[i] = character;

            float randomSizeFactor = Random.Range(1f - sizeRandomness, 1f + sizeRandomness);
            character.transform.localScale = characterPrefab.transform.localScale * randomSizeFactor;

            SplineFollower follower = character.GetComponent<SplineFollower>();

            if (follower != null)
            {
                follower.SetSplineContainer(splineContainer);

                float randomTimeFactor = Random.Range(1f - speedRandomness, 1f + speedRandomness);
                follower.SetCompletionTime(completionTime * randomTimeFactor);

                follower.SetProgress(startingOffset);
            }
            else
            {
                Debug.LogWarning($"Character {i} doesn't have a SplineFollower component!");
            }

            float finalDelay = baseSpawnDelay + Random.Range(minRandomDelay, maxRandomDelay);
            yield return new WaitForSeconds(finalDelay);
        }
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