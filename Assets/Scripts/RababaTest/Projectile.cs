using System;
using UnityEngine;

namespace RababaTest
{
    public class Projectile : MonoBehaviour
    {
        public float lifeTime = 10f;
        public bool shouldExplode = true;
        public GameObject explosionEffectPrefab;

        private Rigidbody _rigidbody;
        private bool _unexploded = false;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        public void Launch(Vector3 force)
        {
            _rigidbody.AddForce(force);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_unexploded)
            {
                // TODO: Attach projectile to player
                return;
            }
            
            Debug.Log($"Projectile hit with {other.gameObject.name}");
            if (!shouldExplode)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    Destroy(gameObject);
                    return;
                }
                
                _rigidbody.linearVelocity = Vector3.zero;
                _unexploded = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
