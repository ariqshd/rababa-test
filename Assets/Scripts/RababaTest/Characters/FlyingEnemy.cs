using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RababaTest.Characters
{
    public class FlyingEnemy : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WaypointHolder waypointHolder;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;


        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float rotationSpeed = 7.5f;
        [SerializeField] private float circleDuration = 5f;
        [SerializeField] private float waypointDistanceThreshold = 2f;


        [Header("Attack Settings")]
        [SerializeField] private float attackRange = 35f;
        [SerializeField] private float attackDuration = 3f;
        [SerializeField] private float shootInterval = 0.5f;


        [Header("Projectile Settings")]
        [SerializeField] private float projectileSpeed = 60f;
        [SerializeField] private float angleToShootAtPlayer = 0.1f;


        private Transform _currentWaypointTarget;
        private Transform[] _waypoints;
        private List<GameObject> _playersInRange = new List<GameObject>();
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


    private void FaceTarget(Vector3 targetPos)
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


        private IEnumerator RotateUntilFacingPlayer(float angleThreshold)
        {
            while (!IsFacingPlayer(angleThreshold))
            {
                FaceTarget(_currentTarget.transform.position);
                yield return null;
            }
        }


        private void PickRandomWaypoint()
        {
            if (_waypoints != null && _waypoints.Length > 0)
            {
                _currentWaypointTarget = _waypoints[Random.Range(0, _waypoints.Length)];
            }
        }


        private bool ReachedWaypoint()
        {
            if (!_currentWaypointTarget) return false;
            return Vector3.Distance(transform.position, _currentWaypointTarget.position) < waypointDistanceThreshold;
        }


        private void MoveTowardsTarget(Vector3 targetPos)
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


        private void FireProjectile()
        {
            if (!projectilePrefab) return;
            var spawn = projectileSpawnPoint ? projectileSpawnPoint : transform;


            var proj = Instantiate(projectilePrefab, spawn.position, spawn.rotation);
            var rb = proj.GetComponent<Rigidbody>();


            if (rb)
                rb.linearVelocity = spawn.forward * projectileSpeed;
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
            _currentTarget = GetRandomPlayerInRange();
            if(!_currentTarget) yield break;
            yield return StartCoroutine(RotateUntilFacingPlayer(angleToShootAtPlayer));
            FireProjectile();


            float timer = 0f;
            float shootTimer = 0f;


            while (timer < duration)
            {
                timer += Time.deltaTime;
                shootTimer += Time.deltaTime;

                if (!_currentTarget) yield break;

                FaceTarget(_currentTarget.transform.position);


                if (DistanceToPlayer() > attackRange)
                {
                    MoveTowardsTarget(_currentTarget.transform.position);
                }

                if (shootTimer >= shootInterval)
                {
                    shootTimer = 0f;
                    FireProjectile();
                }

                yield return null;
            }
        }


        private IEnumerator StateMachine()
        {
            while (true)
            {
                yield return StartCoroutine(CircleState(circleDuration));
                yield return StartCoroutine(AttackState(attackDuration));
            }
        }

        private GameObject GetRandomPlayerInRange()
        {
            if (_playersInRange.Count < 1)
            {
                Debug.Log("No players in range");
                return null;
            }
            
            int randomIndex = UnityEngine.Random.Range(0, _playersInRange.Count);
            return _playersInRange[randomIndex];
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;

            if (!_playersInRange.Contains(other.gameObject))
            {
                _playersInRange.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;

            _playersInRange.Remove(other.gameObject); 
        }
    }
}
