using UnityEngine;

public class GunnerBird : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject shotgunPrefab;

    private Rigidbody rb;
    private bool launched;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!launched && rb != null && !rb.isKinematic)
        {
            launched = true;
            Invoke(nameof(FireShotgun), 2f);
        }
    }

    private void FireShotgun()
    {
        if (shotgunPrefab == null || spawnPoint == null)
        {
            return;
        }

        GameObject shotgun = Instantiate(shotgunPrefab, spawnPoint.position, spawnPoint.rotation);
        Destroy(shotgun, 2f);
    }
}
