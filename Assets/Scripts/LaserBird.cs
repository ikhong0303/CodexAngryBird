using System.Collections;
using UnityEngine;

public class LaserBird : MonoBehaviour
{
    public Transform laserSpawnPoint;
    public LineRenderer lineRenderer;
    public float delay = 2f;
    public float beamDuration = 2f;
    public float tickInterval = 0.05f;
    public float maxDistance = 1000f;

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
            Invoke(nameof(StartFiring), delay);
        }
    }

    private void StartFiring()
    {
        StartCoroutine(FireLaser());
    }

    private IEnumerator FireLaser()
    {
        if (lineRenderer == null || laserSpawnPoint == null)
        {
            yield break;
        }

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;

        float elapsed = 0f;
        while (elapsed < beamDuration)
        {
            Vector3 start = laserSpawnPoint.position;
            Vector3 direction = laserSpawnPoint.right;

            if (Physics.Raycast(start, direction, out RaycastHit hit, maxDistance))
            {
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, hit.point);

                DestroyOnImpact target = hit.collider.GetComponentInParent<DestroyOnImpact>();
                if (target != null)
                {
                    Destroy(target.gameObject);
                }
            }
            else
            {
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, start + direction * maxDistance);
            }

            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        lineRenderer.enabled = false;
    }
}
