using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BlackHoleBird : MonoBehaviour
{
    public float blackHoleRadius = 5f;
    public float blackHoleForce = 50f;
    public float delayBeforeActivate = 2.5f;
    public float activeDuration = 2f;

    private bool isActive = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        isActive = false;
        Invoke(nameof(ActivateBlackHole), delayBeforeActivate);
    }

    void Update()
    {
        if (isActive && !rb.isKinematic)
        {
            AttractNearbyObjects();
        }
    }

    void ActivateBlackHole()
    {
        Debug.Log("[BlackHoleBird] ºí·¢È¦ È°¼ºÈ­!");
        isActive = true;
        Invoke(nameof(DeactivateBlackHole), activeDuration);
    }

    void DeactivateBlackHole()
    {
        isActive = false;
        Debug.Log("[BlackHoleBird] ºí·¢È¦ ºñÈ°¼ºÈ­.");
    }

    void AttractNearbyObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, blackHoleRadius);
        foreach (var col in colliders)
        {
            Rigidbody otherRb = col.attachedRigidbody;
            if (otherRb != null && otherRb != rb)
            {
                Vector3 dir = transform.position - otherRb.position;
                otherRb.AddForce(dir.normalized * blackHoleForce);
                Debug.Log($"[BlackHoleBird] {otherRb.name} ²ø¾î´ç±è");
            }
        }
    }
}
