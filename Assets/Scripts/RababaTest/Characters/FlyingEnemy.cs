using System;
using System.Collections;
using System.Collections.Generic;
using RababaTest.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace RababaTest.Characters
{
    public class FlyingEnemy : Character
    {
        [Header("References")]
        [SerializeField] private WaypointHolder waypointHolder;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private GameObject shadowPrefab; // Prefab for the target shadow
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private ParticleSystem flameParticleSystem;
        [SerializeField] private LayerMask groundLayer;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float rotationSpeed = 7.5f;
        [SerializeField] private float circleDuration = 5f;
        [SerializeField] private float waypointDistanceThreshold = 2f;

        [Header("Attack Settings")]
        [SerializeField] private float attackDuration = 3f;
        [SerializeField] private float eagleStrikeHeight = 50f; // Height to fly up before striking
        [SerializeField] private float strikeSpeed = 40f; // Speed of the downward strike
        [SerializeField] private float strikeImpactRadius = 5f; // Radius of the strike impact
        [SerializeField] private float strikePreparationTime = 1f; // Time before executing the strike
        [SerializeField] private float strikeRecoveryTime = 0.5f; // Time to recover after strike
        [SerializeField] private float angleToShootAtPlayer = 0.1f;

        private Transform _currentWaypointTarget;
        private Transform[] _waypoints;
        private GameObject _currentTarget;

        private void Start()
        {
            if (waypointHolder != null)
            {
                waypointHolder.RefreshWaypoints();
                _waypoints = waypointHolder.waypoints;
            }

            if (_waypoints == null || _waypoints.Length == 0) return;

            StartCoroutine(StateMachine());
        }


        public void FaceTarget(Vector3 targetPos)
        {
            Vector3 dir = targetPos - transform.position;
            if (dir.sqrMagnitude < 0.0001f) return;

            dir.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotationSpeed);
        }

        private bool IsFacingPlayer(float angleThreshold)
        {
            if (!_currentTarget) return true;
            Vector3 toPlayer = (_currentTarget.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, toPlayer);
            return angle <= angleThreshold;
        }

        public bool IsFacingTarget(GameObject target, float angleThreshold)
        {
            if (!target) return true;
            Vector3 toPlayer = (target.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, toPlayer);
            return angle <= angleThreshold;
        }


        public IEnumerator RotateUntilFacingPlayer(GameObject target, float angleThreshold)
        {
            while (!IsFacingPlayer(angleThreshold))
            {
                FaceTarget(target.transform.position);
                yield return null;
            }
        }

        public Transform PickRandomWaypoint()
        {
            if (_waypoints is not { Length: > 0 }) return null;
            
            _currentWaypointTarget = _waypoints[Random.Range(0, _waypoints.Length)];
            return _currentWaypointTarget;

        }

        private bool ReachedWaypoint()
        {
            if (!_currentWaypointTarget) return false;
            return Vector3.Distance(transform.position, _currentWaypointTarget.position) < waypointDistanceThreshold;
        }

        public void MoveTowardsTarget(Vector3 targetPos)
        {
            Vector3 dir = targetPos - transform.position;
            if (dir.sqrMagnitude < 0.0001f) return;


            dir.Normalize();
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * rotationSpeed
            );
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }

        private float DistanceToPlayer()
        {
            if (!_currentTarget) return float.MaxValue;
            return Vector3.Distance(transform.position, _currentTarget.transform.position);
        }

        public IEnumerator RocketLaunch()
        {
            if (!projectilePrefab) yield return null;
            Debug.Log("Launch rockets");
            
            _currentTarget = GetRandomPlayerInRange(100);
            if(!_currentTarget) yield break;
            yield return StartCoroutine(RotateUntilFacingPlayer(_currentTarget,angleToShootAtPlayer));
            
            var spawn = projectileSpawnPoint ? projectileSpawnPoint : transform;
            
            // Launch angles for the three rockets
            Vector3[] spreadAngles = new Vector3[] {
                Vector3.zero,
                new Vector3(0, 5, 0),  // 15 degrees to the right
                new Vector3(0, -5, 0)  // 15 degrees to the left
            };

            for (int i = 0; i < 3; i++)
            {
                Quaternion spreadRotation = spawn.rotation * Quaternion.Euler(spreadAngles[i]);
                var proj = Instantiate(projectilePrefab, spawn.position, spreadRotation);
                
                Debug.Log($"Projectile {i+1} launched");
        
                // Get the Projectile component
                var projectile = proj.GetComponent<Projectile>();
                if (projectile == null) yield return null;

                projectile.SetInstigator(gameObject);
                
                // Make the third rocket not explode
                projectile.shouldExplode = (i < 2);

                projectile.Launch(spreadRotation * Vector3.forward);
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.5f));
            }
        }

        public IEnumerator FireFlame()
        {
            Debug.Log("Fire Flame");

            _currentTarget = GetRandomPlayerInRange(100);
            if(!_currentTarget) yield break;
            yield return StartCoroutine(RotateUntilFacingPlayer(_currentTarget,angleToShootAtPlayer));
            
            RaycastHit hit;
            Vector3 direction = transform.forward;
            
            Debug.DrawRay(projectileSpawnPoint.transform.position, direction * 100, Color.red, 5f);
            
            Collider[] hitPlayers = Physics.OverlapSphere(transform.position + direction * 10, 2f, 1 << 6);
            foreach (var hitCollider in hitPlayers)
            {
                Debug.Log($"hit player {hitCollider.gameObject.name}");
                if (hitCollider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(1);
                }
            }
            
            if (flameParticleSystem != null)
            {
                flameParticleSystem.Play();
            }
        }
        
        private IEnumerator EagleStrikeState()
        {
            Debug.Log("Eagle strike");

            // Store the initial position for recovery
            Vector3 initialPosition = transform.position;
            
            Vector3 heightPosition = transform.position + Vector3.up * eagleStrikeHeight;

            // Move up to strike position
            while (Vector3.Distance(transform.position, heightPosition) > 0.1f)
            {
                if (!IsFacingPlayer(angleToShootAtPlayer))
                {
                    FaceTarget(heightPosition);
                }
                
                transform.position = Vector3.MoveTowards(transform.position, heightPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            
            _currentTarget = GetRandomPlayerInRange(100);
            if(!_currentTarget) yield break;
            yield return StartCoroutine(RotateUntilFacingPlayer(_currentTarget, angleToShootAtPlayer));
            
            // Move above the target
            Vector3 strikePosition = _currentTarget.transform.position;
            
            // Raycast downward to find floor
            RaycastHit hit;
            Vector3 finalStrikePosition = strikePosition;
            Quaternion shadowRotation = Quaternion.identity;
            
            if (Physics.Raycast(strikePosition, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                finalStrikePosition = hit.point;
                shadowRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
            else
            {
                // Fallback: just place on Y=0 or keep original Y
                finalStrikePosition = new Vector3(strikePosition.x, 0, strikePosition.z);
            }
            
            // Create shadow indicator
            GameObject shadowObj = null;
            if (shadowPrefab != null)
            {
                Vector3 shadowSpawnPosition = finalStrikePosition;
                shadowObj = Instantiate(shadowPrefab, shadowSpawnPosition, shadowRotation);
                var shadow = shadowObj.GetComponent<StrikeShadow>();
                if (shadow == null) yield return null;
                shadow.SetRadius(strikeImpactRadius);
            }

            // Wait before striking
            yield return new WaitForSeconds(strikePreparationTime);

            // Execute the strike
            while (Vector3.Distance(transform.position, strikePosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, strikePosition, strikeSpeed * Time.deltaTime);
                yield return null;
            }
            
            Destroy(shadowObj);

            // Impact effect
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, strikeImpactRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(1);
                }
            }

            // Recovery period
            yield return new WaitForSeconds(strikeRecoveryTime);

            // Return to initial position
            while (Vector3.Distance(transform.position, initialPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }


        private IEnumerator CircleState(float duration)
        {
            float timer = 0f;
            PickRandomWaypoint();
            yield return new WaitForSeconds(UnityEngine.Random.Range(0, duration));

            while (timer < duration)
            {
                timer += Time.deltaTime;
                if (_currentWaypointTarget)
                    MoveTowardsTarget(_currentWaypointTarget.position);

                if (ReachedWaypoint()) PickRandomWaypoint();


                yield return null;
            }
        }


        private IEnumerator AttackState(float duration)
        {
            var randomAttack = UnityEngine.Random.Range(0, 3);
            switch (randomAttack)
            {
                case 0:
                    yield return RocketLaunch();
                    // yield return FireFlame();
                    // yield return EagleStrikeState();
                    break;
                case 1:
                    // yield return RocketLaunch();
                    yield return FireFlame();
                    // yield return EagleStrikeState();
                    break;
                case 2:
                    // yield return RocketLaunch();
                    // yield return FireFlame();
                    yield return EagleStrikeState();
                    break;
            }

            yield return null;
        }


        private IEnumerator StateMachine()
        {
            while (true)
            {
                yield return StartCoroutine(CircleState(circleDuration));
                yield return StartCoroutine(AttackState(attackDuration));
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 20f);
        }

        public GameObject GetRandomPlayerInRange(float radius)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            List<GameObject> playersFound = new List<GameObject>();

            foreach (var hit in hitColliders)
            {
                if (hit.gameObject.CompareTag("Player"))
                {
                    playersFound.Add(hit.gameObject);
                }
            }

            if (playersFound.Count > 0)
            {
                int randomIndex = Random.Range(0, playersFound.Count);
                return playersFound[randomIndex];
            }

            Debug.Log("No players in range");
            return null;
        }

        public override void TakeDamage(float damage)
        {
            
        }

        protected override void Die()
        {
            base.Die();
        }
    }
}
