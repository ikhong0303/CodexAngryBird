using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private int pelletCount = 10;
    [SerializeField] private float spreadAngle = 10f;
    [SerializeField] private float pelletSpeed = 40f;

    private void Start()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0f) * transform.forward;

            GameObject pellet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pellet.transform.position = transform.position;
            pellet.transform.rotation = Quaternion.LookRotation(direction);
            pellet.transform.localScale = Vector3.one * 0.1f;

            Rigidbody rb = pellet.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearVelocity = direction * pelletSpeed;

            pellet.AddComponent<ShotgunPellet>();
        }

        Destroy(gameObject);
    }
}
