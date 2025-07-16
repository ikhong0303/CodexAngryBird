using UnityEngine;

public class BirdLauncher : MonoBehaviour
{
    public Rigidbody birdPrefab;
    public Transform launchPosition;

    private Rigidbody currentBird;
    private Vector3 dragStart;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        SpawnBird();
    }

    void SpawnBird()
    {
        currentBird = Instantiate(birdPrefab, launchPosition.position, Quaternion.identity);
        currentBird.isKinematic = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStart = GetMouseWorldPos();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 dragEnd = GetMouseWorldPos();
            Vector3 force = dragStart - dragEnd;

            currentBird.isKinematic = false;
            currentBird.AddForce(force * 500f);

            Invoke(nameof(SpawnBird), 2f);
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
