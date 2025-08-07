using UnityEngine;
using System.Collections;
using System;

public class CameraDirector : MonoBehaviour
{
    public Transform MainCamPos;
    public Transform FirstCamPos;
    public float slowSpeed = 2f;
    public float fastSpeed = 5f;
    public float waitTime = 1f;

    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 5f;

    private BirdLauncher launcher;
    private Transform targetBird;
    private bool returning;
    private bool isSequenceRunning;

    public event Action OnSequenceComplete;

    void Start()
    {
        launcher = FindObjectOfType<BirdLauncher>();
        if (launcher != null)
        {
            launcher.enabled = false;
        }

        isSequenceRunning = true;
        StartCoroutine(CameraSequence());
    }

    void LateUpdate()
    {
        if (targetBird != null)
        {
            // X만 새를 따라가고, Y/Z는 현재 카메라 위치 + offset 유지
            Vector3 targetPos = new Vector3(
                targetBird.position.x + offset.x,
                transform.position.y,
                transform.position.z
            );
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
        else if (returning && MainCamPos != null)
        {
            transform.position = Vector3.Lerp(transform.position, MainCamPos.position, fastSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, MainCamPos.position) < 0.1f)
            {
                transform.position = MainCamPos.position;
                returning = false;
                isSequenceRunning = false;
                if (launcher != null)
                {
                    launcher.enabled = true;
                }
                OnSequenceComplete?.Invoke();
            }
        }
    }

    private IEnumerator CameraSequence()
    {
        if (MainCamPos != null)
        {
            transform.position = MainCamPos.position;
            transform.rotation = MainCamPos.rotation;
        }

        if (MainCamPos != null && FirstCamPos != null)
        {
            while (Vector3.Distance(transform.position, FirstCamPos.position) > 0.01f ||
                   Quaternion.Angle(transform.rotation, FirstCamPos.rotation) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    FirstCamPos.position,
                    slowSpeed * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    FirstCamPos.rotation,
                    slowSpeed * 100f * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);

            while (Vector3.Distance(transform.position, MainCamPos.position) > 0.01f ||
                   Quaternion.Angle(transform.rotation, MainCamPos.rotation) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    MainCamPos.position,
                    fastSpeed * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    MainCamPos.rotation,
                    fastSpeed * 100f * Time.deltaTime);
                yield return null;
            }
        }

        isSequenceRunning = false;
        if (launcher != null)
        {
            launcher.enabled = true;
        }
        OnSequenceComplete?.Invoke();
    }

    public void FollowBird(Transform bird, float returnDelay = 0f)
    {
        targetBird = bird;
        returning = false;
        isSequenceRunning = true;
        if (launcher != null)
        {
            launcher.enabled = false;
        }
        if (returnDelay > 0f)
        {
            StartCoroutine(ReturnAfterDelay(returnDelay));
        }
    }

    public void ReturnToMain()
    {
        targetBird = null;
        returning = true;
        isSequenceRunning = true;
        if (launcher != null)
        {
            launcher.enabled = false;
        }
    }

    private IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToMain();
    }

    public bool IsSequenceRunning => isSequenceRunning;
}

