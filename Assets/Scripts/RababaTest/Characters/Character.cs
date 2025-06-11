using System;
using RababaTest.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace RababaTest.Characters
{
    public class Character : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 3;

        private float _currentHealth;

        [HideInInspector] public UnityEvent onHealthChange;
        [HideInInspector] public UnityEvent onDie;
        
        private void Start()
        {
            SetHealth(maxHealth);
            onHealthChange.AddListener(HandleOnHealthChange);
        }

        protected virtual void HandleOnHealthChange()
        {
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void TakeDamage(float damage)
        {
            ChangeHealth(-damage);
        }

        protected virtual void Die()
        {
            onDie?.Invoke();
        }

        protected void ChangeHealth(float health)
        {
            _currentHealth += health;
            onHealthChange?.Invoke();
        }

        protected void SetHealth(float health)
        {
            _currentHealth = health;
            onHealthChange?.Invoke();
        }
    }
}