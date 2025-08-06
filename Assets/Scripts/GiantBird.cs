using UnityEngine;
using System.Collections;

public class GiantBird : MonoBehaviour
{
    [SerializeField] private float growDelay = 2f;
    [SerializeField] private float growDuration = 0.3f;
    [SerializeField] private float scaleMultiplier = 5f;
    [SerializeField] private float pushRadius = 5f;
    [SerializeField] private float pushForce = 200f;

    private Rigidbody rb;
    private bool launched;
    private bool isGrowing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!launched && rb != null && !rb.isKinematic)
        {
            launched = true;
            Invoke(nameof(StartGrowAndPush), growDelay);
        }
    }

    private void StartGrowAndPush()
    {
        StartCoroutine(GrowAndPush());
    }

    private IEnumerator GrowAndPush()
    {
        isGrowing = true;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * scaleMultiplier;
        float elapsed = 0f;
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / growDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        transform.localScale = targetScale;
        isGrowing = false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, pushRadius);
        foreach (var col in colliders)
        {
            Rigidbody otherRb = col.attachedRigidbody;
            if (otherRb != null && otherRb != rb)
            {
                Vector3 dir = (otherRb.position - transform.position).normalized;
                otherRb.AddForce(dir * pushForce);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isGrowing)
        {
            return;
        }

        DestroyOnImpact target = collision.collider.GetComponentInParent<DestroyOnImpact>();
        if (target != null && target.transform.position.y < transform.position.y)
        {
            Destroy(target.gameObject);
        }
    }
}
