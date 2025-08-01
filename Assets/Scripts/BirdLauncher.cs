using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BirdLauncher : MonoBehaviour
{
    [Tooltip("Type of bird to launch.")]
    public BirdType.Type selectedBird;

    [Tooltip("Prefabs available for the bird that will be launched.")]
    public List<Rigidbody> birdPrefabs;
    public Transform launchPosition;
    public LineRenderer directionLine; // shows predicted direction

    [Header("Black Hole Settings")]
    public float blackHoleRadius = 5f;
    public float blackHoleForce = 50f;

    private Rigidbody currentBird;
    private Vector3 dragStart;
    private Camera cam;
    private bool blackHoleActive;
    private Canvas buttonCanvas;

    void Start()
    {
        cam = Camera.main;
        SpawnBird();

        CreateSelectionUI();

        if (directionLine != null)
        {
            directionLine.positionCount = 0;
            directionLine.enabled = false;
        }
    }

    void CreateSelectionUI()
    {
        if (buttonCanvas != null)
        {
            return;
        }

        GameObject canvasObj = new GameObject("BirdSelectionCanvas");
        buttonCanvas = canvasObj.AddComponent<Canvas>();
        buttonCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        CreateButton("Basic", BirdType.Type.Basic, new Vector2(-160f, 80f));
        CreateButton("BlackHole", BirdType.Type.BlackHole, new Vector2(0f, 80f));
        CreateButton("Gunner", BirdType.Type.Gunner, new Vector2(160f, 80f));
    }

    void CreateButton(string label, BirdType.Type type, Vector2 anchoredPosition)
    {
        GameObject buttonObj = new GameObject(label + "Button");
        buttonObj.transform.SetParent(buttonCanvas.transform);
        buttonObj.AddComponent<Image>();
        Button btn = buttonObj.AddComponent<Button>();
        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160f, 30f);
        rect.anchoredPosition = anchoredPosition;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        Text txt = textObj.AddComponent<Text>();
        txt.text = label;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        btn.onClick.AddListener(() => ChooseBird(type));
    }

    void SpawnBird()
    {
        CancelInvoke(nameof(ActivateBlackHole));
        CancelInvoke(nameof(DeactivateBlackHole));
        blackHoleActive = false;

        Rigidbody prefab = null;
        foreach (var b in birdPrefabs)
        {
            BirdType bt = b.GetComponent<BirdType>();
            if (bt != null && bt.type == selectedBird)
            {
                prefab = b;
                break;
            }
        }

        if (prefab == null)
        {
            Debug.LogWarning($"No prefab found for {selectedBird}");
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
        if (blackHoleActive && currentBird != null && !currentBird.isKinematic)
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

            currentBird.isKinematic = false;
            currentBird.AddForce(force * 500f);

            if (selectedBird == BirdType.Type.BlackHole)
            {
                Invoke(nameof(ActivateBlackHole), 2.5f);
            }

            Invoke(nameof(SpawnBird), 4f);
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

    public void ChooseBird(BirdType.Type type)
    {
        selectedBird = type;
        SpawnBird();
    }
}
