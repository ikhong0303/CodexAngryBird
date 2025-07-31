using UnityEngine;

public class BirdLauncher : MonoBehaviour
{
    public enum BirdType { Basic, BlackHole }

    [Tooltip("Type of bird to launch.")]
    public BirdType birdType = BirdType.Basic;

    [Tooltip("Prefab used for the bird that will be launched.")]
    public Rigidbody birdPrefab;
    public Transform launchPosition;
    public LineRenderer directionLine; // shows predicted direction

    [Header("Black Hole Settings")]
    public float blackHoleRadius = 5f;
    public float blackHoleForce = 50f;

    private Rigidbody currentBird;
    private Rigidbody launchedBird;
    private bool blackHoleActive;
    private Vector3 dragStart;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        SpawnBird();

        if (directionLine != null)
        {
            directionLine.positionCount = 0;
            directionLine.enabled = false;
        }
    }

    void SpawnBird()
    {
        currentBird = Instantiate(birdPrefab, launchPosition.position, Quaternion.identity);
        currentBird.isKinematic = true;

        if (directionLine != null)
        {
            directionLine.positionCount = 0;
            directionLine.enabled = false;
        }
    }

    void Update()
    {
        if (birdType == BirdType.BlackHole && blackHoleActive && launchedBird != null)
        {
            AttractObjects();
        }

        if (Input.GetMouseButtonDown(0))
        {
            dragStart = GetMouseWorldPos();
            if (directionLine != null)
            {
                directionLine.positionCount = 2;
                directionLine.SetPosition(0, currentBird.position);
                directionLine.SetPosition(1, currentBird.position);
                directionLine.enabled = true;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (directionLine != null && directionLine.enabled)
            {
                Vector3 dragCurrent = GetMouseWorldPos();
                Vector3 forcePreview = dragStart - dragCurrent;
                directionLine.SetPosition(0, currentBird.position);
                directionLine.SetPosition(1, currentBird.position + forcePreview);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 dragEnd = GetMouseWorldPos();
            Vector3 force = dragStart - dragEnd;
            if (directionLine != null)
            {
                directionLine.enabled = false;
            }

            launchedBird = currentBird;
            currentBird = null;
            launchedBird.isKinematic = false;
            launchedBird.AddForce(force * 500f);
            blackHoleActive = false;
            if (birdType == BirdType.BlackHole)
            {
                Invoke(nameof(ActivateBlackHole), 3f);
            }

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

    void AttractObjects()
    {
        if (launchedBird == null) return;

        Collider[] colliders = Physics.OverlapSphere(launchedBird.position, blackHoleRadius);
        foreach (var col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null && rb != launchedBird)
            {
                Vector3 dir = launchedBird.position - rb.position;
                rb.AddForce(dir.normalized * blackHoleForce);
            }
        }
    }

    void ActivateBlackHole()
    {
        blackHoleActive = true;
    }
}
