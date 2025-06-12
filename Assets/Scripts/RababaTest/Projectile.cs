using System;
using System.Collections;
using RababaTest.Interfaces;
using UnityEngine;

namespace RababaTest
{
    public class Projectile : MonoBehaviour, ICollectable
    {
        public float projectileSpeed = 750;
        public float lifeTime = 10f;
        public bool shouldExplode = true;
        public ParticleSystem particle;

        private Rigidbody _rigidbody;
        private bool _canExplode = true;
        private GameObject _instigator;
        
        private Coroutine _lifeTimeCoroutine;
        private float _remainingTime;
        private bool _isPaused = false;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _remainingTime = lifeTime;
        }

        private void Start()
        {
            StartTimer();
        }

        public void SetInstigator(GameObject instigator)
        {
            _instigator = instigator;
        }
        
        private IEnumerator CountdownLifetime()
        {
            while (_remainingTime > 0f)
            {
                if (!_isPaused)
                {
                    _remainingTime -= Time.deltaTime;
                }

                yield return null;
            }

            OnLifetimeEnd();
        }

        public void StartTimer()
        {
            _lifeTimeCoroutine = StartCoroutine(CountdownLifetime());
        }
        
        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }
        
        public void ResetTimer()
        {
            _remainingTime = lifeTime;
            // _isPaused = false;
            if (_lifeTimeCoroutine != null)
            {
                StopCoroutine(_lifeTimeCoroutine);
            }
            _lifeTimeCoroutine = StartCoroutine(CountdownLifetime());
        }
        
        private void OnLifetimeEnd()
        {
            if (shouldExplode)
            {
                Explode();
            }
        }
        
        private void Explode()
        {
            if (particle != null)
            {
                particle.Play();
            }

            Destroy(gameObject);
        }

        public void Launch(Vector3 force)
        {
            _rigidbody.AddForce(force * projectileSpeed);
        }
        
        public void Launch(GameObject target)
        {
            if (_rigidbody == null || target == null) return;

            Vector3 direction = (target.transform.position - transform.position).normalized;
            _rigidbody.linearVelocity = direction * projectileSpeed * 0.1f;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!_canExplode) return;
            
            Debug.Log($"Projectile hit with {other.gameObject.name}");
            if (other.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(1);
                Explode();
            }
            else
            {
                if (shouldExplode)
                {
                    Explode();
                }
                else
                {
                    _rigidbody.linearVelocity = Vector3.zero;
                    _canExplode = false;
                }
            }
        }

        public void SetCanExplode(bool canExplode)
        {
            _canExplode = canExplode;
        }

        public GameObject GetInstigator()
        {
            return _instigator;
        }

        public bool CanCollect()
        {
            return !_canExplode;
        }
    }
}
