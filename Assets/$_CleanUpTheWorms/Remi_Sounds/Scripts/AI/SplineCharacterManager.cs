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
        float spawnDelay = completionTime / numberOfCharacters;
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

        for (int i = 0; i < numberOfCharacters; i++)
        {
            GameObject character = Instantiate(characterPrefab, transform);
            spawnedCharacters[i] = character;

            SplineFollower follower = character.GetComponent<SplineFollower>();

            if (follower != null)
            {
                follower.SetSplineContainer(splineContainer);
                follower.SetCompletionTime(completionTime);
                follower.SetProgress(startingOffset);
            }
            else
            {
                Debug.LogWarning($"Character {i} doesn't have a SplineFollower component!");
            }

            yield return wait;
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