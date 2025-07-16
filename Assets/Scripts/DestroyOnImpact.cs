using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    public float impactThreshold = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > impactThreshold)
        {
            Destroy(gameObject);
        }
    }
}
