using UnityEngine;

public class GunnerBird : MonoBehaviour
{
    [SerializeField] private GameObject shotgunPrefab;

    private void Start()
    {
        Invoke(nameof(Shoot), 2f);
    }

    private void Shoot()
    {
        if (shotgunPrefab != null)
        {
            Instantiate(shotgunPrefab, transform.position, transform.rotation);
        }
    }
}
