using System.Collections.Generic;
using UnityEngine;

public class ShotgunCollisionHandler : MonoBehaviour
{
    private float spawnTime;
    private readonly List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>();

    private void Awake()
    {
        spawnTime = Time.time;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (Time.time - spawnTime > 0.7f)
        {
            return;
        }

        ParticlePhysicsExtensions.GetCollisionEvents(GetComponent<ParticleSystem>(), other, _collisionEvents);

        foreach (ParticleCollisionEvent collisionEvent in _collisionEvents)
        {
            DestroyOnImpact target = collisionEvent.colliderComponent.GetComponentInParent<DestroyOnImpact>();
            if (target != null)
            {
                Destroy(target.gameObject);
            }
        }

        _collisionEvents.Clear();
    }
}

