using UnityEngine;

public class S_GrabbableState : MonoBehaviour
{
    [SerializeField] bool canBeTrashed = false;
    [SerializeField] bool isGrabbed = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Robot")) return;
        if (isGrabbed)  return;
        if (!other.gameObject.CompareTag("Ground")) return;
        canBeTrashed = false;
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Robot")) return;
        if (isGrabbed)  return;
        if (!other.gameObject.CompareTag("Ground")) return;
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
    
    public void SetIsGrabbed(bool value)
    {
        isGrabbed = value;
    }
}
