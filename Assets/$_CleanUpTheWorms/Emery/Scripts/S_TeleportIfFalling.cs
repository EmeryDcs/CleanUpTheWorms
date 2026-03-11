using UnityEngine;

public class S_TeleportIfFalling : MonoBehaviour
{
    Vector3 startPosition;
    Rigidbody rb;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.position.y < -10f)
        {
            transform.position = startPosition;
            GetComponent<Collider>().enabled = true;
            GetComponent<Collider>().isTrigger = false;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}