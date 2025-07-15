using UnityEngine;

public class BallShooter : MonoBehaviour
{
    public GameObject ballPrefab; // Prefab of the ball to shoot
    public Transform spawnPoint;  // Where the balls will spawn from
    public float shootForce = 10f; // Force applied to the ball when shot

    private GameObject[] balls; // Array to store the 3 spawned balls
    private int shotCount = 0;   // Number of balls shot so far

    void Start()
    {
        // Spawn 3 balls at the beginning but keep them inactive
        balls = new GameObject[3];
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i] = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
            balls[i].SetActive(false);
        }
    }

    void Update()
    {
        if (shotCount >= balls.Length) return; // All balls have been shot

        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            ShootNextBall();
        }
    }

    private void ShootNextBall()
    {
        GameObject ball = balls[shotCount];
        ball.SetActive(true);
        ball.transform.position = spawnPoint.position;

        // Determine the click position in world space on the plane of the ball
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, spawnPoint.position);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 targetPoint = ray.GetPoint(enter);
            Vector3 direction = (targetPoint - spawnPoint.position).normalized;
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero; // Reset any previous velocity
            rb.AddForce(direction * shootForce, ForceMode.Impulse);
            shotCount++;
        }
    }
}
