using UnityEngine;

public class ShotgunPellet : MonoBehaviour
{
    private float spawnTime;

    private void Awake()
    {
        spawnTime = Time.time;
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
