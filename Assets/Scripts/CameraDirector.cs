using UnityEngine;
using System.Collections;

public class CameraDirector : MonoBehaviour
{
    public Transform MainCamPos;
    public Transform FirstCamPos;
    public float slowSpeed = 2f;
    public float fastSpeed = 5f;
    public float waitTime = 1f;

    private BirdLauncher launcher;
    private Transform targetBird;
    private bool returning;

    void Start()
    {
        launcher = FindObjectOfType<BirdLauncher>();
        if (launcher != null)
        {
            launcher.enabled = false;
        }

        StartCoroutine(CameraSequence());
    }

    void LateUpdate()
    {
        if (targetBird != null)
        {
            Vector3 pos = transform.position;
            pos.x = targetBird.position.x;
            transform.position = pos;
        }
        else if (returning && MainCamPos != null)
        {
            transform.position = Vector3.Lerp(transform.position, MainCamPos.position, fastSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, MainCamPos.position) < 0.1f)
            {
                transform.position = MainCamPos.position;
                returning = false;
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

        if (launcher != null)
        {
            launcher.enabled = true;
        }
    }

    public void FollowBird(Transform bird)
    {
        targetBird = bird;
        returning = false;
    }

    public void ReturnToMain()
    {
        targetBird = null;
        returning = true;
    }
}

