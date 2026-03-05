using UnityEngine;

public class S_ElvatorDoorAnimation : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;

    public float speed;
    public float damping = 1f;

    [Header("door animation")]
    public GameObject door;
    public Vector3 doorStartPos;
    public Vector3 doorEndPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, endPos, damping * Time.deltaTime);

        if(Vector3.Distance(transform.position, endPos) < 0.01f)
        {
            door.transform.localPosition = Vector3.Lerp(door.transform.localPosition, doorEndPos, damping * Time.deltaTime);
        }
    }
}
