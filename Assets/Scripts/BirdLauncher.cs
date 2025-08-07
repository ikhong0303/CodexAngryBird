using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BirdLauncher : MonoBehaviour
{
    [Tooltip("Type of bird to launch.")]
    public BirdType.Type selectedBird;

    [Tooltip("Prefabs available for the bird that will be launched.")]
    public List<Rigidbody> birdPrefabs;
    public Transform launchPosition;
    public LineRenderer directionLine; // shows predicted direction

    [SerializeField]
    private CameraDirector cameraDirector;

    [Header("Black Hole Settings")]
    public float blackHoleRadius = 5f;
    public float blackHoleForce = 50f;

    private Rigidbody currentBird;
    private Vector3 dragStart;
    private Camera cam;
    private bool blackHoleActive;
    private bool isDragging;

    void Awake()
    {
        if (cameraDirector == null)
        {
            cameraDirector = FindObjectOfType<CameraDirector>();
            if (cameraDirector == null)
            {
                Debug.LogWarning("CameraDirector not found. BirdLauncher may not function correctly.");
            }
        }
    }

    void Start()
    {
        cam = Camera.main;

        if (directionLine != null)
        {
            directionLine.positionCount = 0;
            directionLine.enabled = false;
        }
    }

    void OnEnable()
    {
        if (cameraDirector != null)
        {
            cameraDirector.OnSequenceComplete += SpawnBird;
        }
    }

    void OnDisable()
    {
        if (cameraDirector != null)
        {
            cameraDirector.OnSequenceComplete -= SpawnBird;
        }
    }

    void SpawnBird()
    {
        CancelInvoke(nameof(SpawnBird));
        if (currentBird != null && currentBird.isKinematic)
        {
            Destroy(currentBird.gameObject);
        }
        CancelInvoke(nameof(ActivateBlackHole));
        CancelInvoke(nameof(DeactivateBlackHole));
        blackHoleActive = false;

        // Find the prefab that matches the selected bird type
        Rigidbody prefab = birdPrefabs.Find(b =>
        {
            BirdType bt = b.GetComponent<BirdType>();
            return bt != null && bt.type == selectedBird;
        });

        if (prefab == null)
        {
            Debug.LogWarning($"No prefab found for {selectedBird}");
            currentBird = null;
            return;
        }

        currentBird = Instantiate(prefab, launchPosition.position, Quaternion.identity);
        currentBird.isKinematic = true;

        if (directionLine != null)
        {
            directionLine.positionCount = 0;
            directionLine.enabled = false;
        }
    }

    void Update()
    {
        HandleBirdSelectionInput();

        if (blackHoleActive && currentBird != null && !currentBird.isKinematic)
        {
            AttractObjects();
        }

        if (currentBird == null)
        {
            return;
        }

        HandleInput();
    }

    void HandleBirdSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChooseBasicBird();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChooseBlackHoleBird();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChooseGunnerBird();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChooseGiantBird();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChooseBombBird();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChooseLaserBird();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            dragStart = GetMouseWorldPos();
            isDragging = true;
            if (directionLine != null)
            {
                directionLine.positionCount = 2;
                directionLine.SetPosition(0, currentBird.position);
                directionLine.SetPosition(1, currentBird.position);
                directionLine.enabled = true;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            if (directionLine != null && directionLine.enabled)
            {
                Vector3 dragCurrent = GetMouseWorldPos();
                Vector3 forcePreview = dragStart - dragCurrent;
                directionLine.SetPosition(0, currentBird.position);
                directionLine.SetPosition(1, currentBird.position + forcePreview);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector3 dragEnd = GetMouseWorldPos();
            Vector3 force = dragStart - dragEnd;
            if (directionLine != null)
            {
                directionLine.enabled = false;
            }

            currentBird.isKinematic = false;
            currentBird.AddForce(force * 500f);
            if (cameraDirector != null)
            {
                cameraDirector.FollowBird(currentBird.transform, 4f);
            }

            if (selectedBird == BirdType.Type.BlackHole)
            {
                Invoke(nameof(ActivateBlackHole), 2.5f);
            }

            isDragging = false;
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
        Collider[] colliders = Physics.OverlapSphere(currentBird.position, blackHoleRadius);
        foreach (var col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null && rb != currentBird)
            {
                Vector3 dir = currentBird.position - rb.position;
                rb.AddForce(dir.normalized * blackHoleForce);
            }
        }
    }

    void ActivateBlackHole()
    {
        blackHoleActive = true;
        Invoke(nameof(DeactivateBlackHole), 2f);
    }

    void DeactivateBlackHole()
    {
        blackHoleActive = false;
    }

    public void ChooseBasicBird() => ChooseBird(BirdType.Type.Basic);
    public void ChooseBlackHoleBird() => ChooseBird(BirdType.Type.BlackHole);
    public void ChooseGunnerBird() => ChooseBird(BirdType.Type.Gunner);
    public void ChooseGiantBird() => ChooseBird(BirdType.Type.Giant);
    public void ChooseBombBird() => ChooseBird(BirdType.Type.Bomb);
    public void ChooseLaserBird() => ChooseBird(BirdType.Type.Laser);

    public void ChooseBird(BirdType.Type type)
    {
        selectedBird = type;
        SpawnBird();
    }
}
