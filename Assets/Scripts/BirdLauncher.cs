using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BirdLauncher : MonoBehaviour
{
    [Header("Launch Settings")]
    public float powerMultiplier = 500f;
    public float maxForceMagnitude = 1000f;

    [Tooltip("Type of bird to launch.")]
    public BirdType.Type selectedBird;

    [Tooltip("Prefabs available for the bird that will be launched.")]
    public List<Rigidbody> birdPrefabs;
    public Transform launchPosition;
    public LineRenderer directionLine;

    [SerializeField]
    private CameraDirector cameraDirector;

    private readonly HashSet<BirdType.Type> availableTypes = new HashSet<BirdType.Type>();

    private Rigidbody currentBird;
    private Vector3 dragStart;
    private Camera cam;
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

        LoadStageConfig();
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

    void Update()
    {
        HandleBirdSelectionInput();
        HandleInput();
    }

    void HandleBirdSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (IsTypeAvailable(BirdType.Type.Basic)) ChooseBasicBird();
            else ShowUnavailable(BirdType.Type.Basic);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (IsTypeAvailable(BirdType.Type.BlackHole)) ChooseBlackHoleBird();
            else ShowUnavailable(BirdType.Type.BlackHole);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (IsTypeAvailable(BirdType.Type.Gunner)) ChooseGunnerBird();
            else ShowUnavailable(BirdType.Type.Gunner);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (IsTypeAvailable(BirdType.Type.Giant)) ChooseGiantBird();
            else ShowUnavailable(BirdType.Type.Giant);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (IsTypeAvailable(BirdType.Type.Bomb)) ChooseBombBird();
            else ShowUnavailable(BirdType.Type.Bomb);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (IsTypeAvailable(BirdType.Type.Laser)) ChooseLaserBird();
            else ShowUnavailable(BirdType.Type.Laser);
        }
    }

    void HandleInput()
    {
        if (currentBird == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

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
            Vector3 dragCurrent = GetMouseWorldPos();
            Vector3 forcePreview = dragStart - dragCurrent;

            if (directionLine != null && directionLine.enabled)
            {
                directionLine.SetPosition(0, currentBird.position);
                directionLine.SetPosition(1, currentBird.position + forcePreview);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector3 dragEnd = GetMouseWorldPos();
            Vector3 rawForce = dragStart - dragEnd;
            Vector3 clampedForce = Vector3.ClampMagnitude(rawForce * powerMultiplier, maxForceMagnitude);

            if (directionLine != null)
            {
                directionLine.enabled = false;
            }

            currentBird.isKinematic = false;
            currentBird.AddForce(clampedForce);

            if (cameraDirector != null)
            {
                cameraDirector.FollowBird(currentBird.transform, 4f);
            }

            isDragging = false;
        }
    }

    void SpawnBird()
    {
        CancelInvoke(nameof(SpawnBird));

        if (currentBird != null && currentBird.isKinematic)
        {
            Destroy(currentBird.gameObject);
        }

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

    // Bird selection wrapper methods
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

    void LoadStageConfig()
    {
        StageBirdConfig config = FindObjectOfType<StageBirdConfig>();

        availableTypes.Clear();

        if (config != null && config.allowedBirdTypes != null && config.allowedBirdTypes.Count > 0)
        {
            foreach (var t in config.allowedBirdTypes)
            {
                availableTypes.Add(t);
            }

            birdPrefabs = birdPrefabs.FindAll(rb =>
            {
                BirdType bt = rb.GetComponent<BirdType>();
                return bt != null && availableTypes.Contains(bt.type);
            });

            if (!availableTypes.Contains(selectedBird) && birdPrefabs.Count > 0)
            {
                BirdType bt = birdPrefabs[0].GetComponent<BirdType>();
                if (bt != null) selectedBird = bt.type;
            }
        }
        else
        {
            foreach (var rb in birdPrefabs)
            {
                BirdType bt = rb.GetComponent<BirdType>();
                if (bt != null) availableTypes.Add(bt.type);
            }
        }
    }

    bool IsTypeAvailable(BirdType.Type type) => availableTypes.Contains(type);

    void ShowUnavailable(BirdType.Type type)
    {
        Debug.Log($"{type} bird is not available in this stage.");
    }
}

