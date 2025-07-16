using UnityEngine;
using UnityEngine.UI;

public class PigCounter : MonoBehaviour
{
    public Button nextSceneButtonPrefab;
    public Transform uiParent;

    private Button spawnedButton;

    void Update()
    {
        if (spawnedButton != null) return;

        if (CountRemainingPigs() == 0)
        {
            SpawnButton();
        }
    }

    int CountRemainingPigs()
    {
        DestroyOnImpact[] targets = FindObjectsOfType<DestroyOnImpact>();
        int count = 0;
        foreach (var t in targets)
        {
            if (t.gameObject.name.Contains("Pig"))
            {
                count++;
            }
        }
        return count;
    }

    void SpawnButton()
    {
        if (nextSceneButtonPrefab == null) return;
        spawnedButton = Instantiate(nextSceneButtonPrefab, uiParent);
        NextSceneButton loader = spawnedButton.GetComponent<NextSceneButton>();
        if (loader == null)
        {
            loader = spawnedButton.gameObject.AddComponent<NextSceneButton>();
        }
        spawnedButton.onClick.AddListener(loader.LoadNextScene);
    }
}
