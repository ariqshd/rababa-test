using System;
using RababaTest.Interfaces;
using UnityEngine;

namespace RababaTest
{
    public class NetFall : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("NET FALL TRIGGER");
            if (other.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(100);
            }
        }
    }
}
