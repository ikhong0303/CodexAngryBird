using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    [Tooltip("Relative collision velocity required to destroy this object.")]
    public float impactThreshold = 10f;

    [Tooltip("Downward speed on landing that also causes destruction.")]
    public float fallSpeedThreshold = 8f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;

        float downSpeed = 0f;
        if (rb != null)
        {
            downSpeed = Mathf.Abs(Vector3.Dot(rb.velocity, Vector3.down));
        }

        if (impactVelocity > impactThreshold || downSpeed > fallSpeedThreshold)
        {
            Destroy(gameObject);
        }
    }
}
