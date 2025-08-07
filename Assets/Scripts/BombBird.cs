using UnityEngine;

public class BombBird : MonoBehaviour
{
    [SerializeField] private float explosionDelay = 2f;
    [SerializeField] private float pushRadius = 5f;
    [SerializeField] private float destroyRadius = 3f;
    [SerializeField] private float pushForce;

    public GameObject explodeParticlePrefab;
    public Material blackMaterial;

    private Rigidbody rb;
    private bool launched;
    private bool exploded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!launched && rb != null && !rb.isKinematic)
        {
            launched = true;
            Invoke(nameof(Explode), explosionDelay);
        }
    }

    private void Explode()
    {
        if (exploded)
        {
            return;
        }

        exploded = true;

        if (explodeParticlePrefab != null)
        {
            Instantiate(explodeParticlePrefab, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, pushRadius);
        foreach (Collider col in colliders)
        {
            Rigidbody otherRb = col.attachedRigidbody;
            if (otherRb != null && otherRb != rb)
            {
                Vector3 dir = (otherRb.position - transform.position).normalized;
                otherRb.AddForce(dir * pushForce);
            }

            if (Vector3.Distance(transform.position, col.transform.position) <= destroyRadius)
            {
                DestroyOnImpact target = col.GetComponentInParent<DestroyOnImpact>();
                if (target != null)
                {
                    Destroy(target.gameObject);
                }
            }
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && blackMaterial != null)
        {
            renderer.material = blackMaterial;
        }
    }
}
