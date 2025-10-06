using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RockPush : MonoBehaviour
{
    public float pushSpeed = 3f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 dir = collision.transform.forward;
            rb.linearVelocity = dir * pushSpeed;
        }
    }
}
