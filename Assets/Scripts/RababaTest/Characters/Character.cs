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
        [HideInInspector] public UnityEvent<Character> onDie;
        
        protected virtual void Start()
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
            if(_currentHealth <= 0) return;
            ChangeHealth(-damage);
        }

        protected virtual void Die()
        {
            Debug.Log($"{gameObject.name} dies");
            gameObject.SetActive(false);
            onDie?.Invoke(this);
        }

        protected void ChangeHealth(float health)
        {
            _currentHealth += health;
            if (_currentHealth > maxHealth)
            {
                _currentHealth = maxHealth;
            }
            else if (_currentHealth < 0)
            {
                _currentHealth = 0;
            }

            onHealthChange?.Invoke();
        }

        protected void SetHealth(float health)
        {
            _currentHealth = health;
            onHealthChange?.Invoke();
        }

        public float GetHealth()
        {
            return _currentHealth;
        }
    }
}