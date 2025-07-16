using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    public float impactThreshold = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > impactThreshold)
        {
            Destroy(gameObject);
        }
    }
}
