using UnityEngine;

public class S_GrabbableState : MonoBehaviour
{
    [SerializeField] bool canBeTrashed = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            return;
        }

        canBeTrashed = false;
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Robot"))
        {
            return;
        }

        canBeTrashed = false;
    }

    public bool CanBeTrashed()
    {
        return canBeTrashed;
    }

    public void SetCanBeTrashed(bool value)
    {
        canBeTrashed = value;
    }
}
