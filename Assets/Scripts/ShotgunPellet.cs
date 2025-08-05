using UnityEngine;

public class ShotgunPellet : MonoBehaviour
{
    private float spawnTime;
    private Collider pelletCollider;

    private void Awake()
    {
        spawnTime = Time.time;
        pelletCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (pelletCollider != null && Time.time - spawnTime >= 0.7f)
        {
            pelletCollider.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - spawnTime >= 0.7f)
        {
            return;
        }

        DestroyOnImpact target = collision.gameObject.GetComponent<DestroyOnImpact>();
        if (target != null)
        {
            Destroy(collision.gameObject);
        }
    }
}
