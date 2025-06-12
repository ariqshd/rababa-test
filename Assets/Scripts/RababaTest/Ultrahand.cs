using System;
using RababaTest.Interfaces;
using UnityEngine;

namespace RababaTest
{
    /// <summary>
    /// Manipulator to get, store, and throw items
    /// </summary>
    public class Ultrahand : MonoBehaviour
    {
        [SerializeField] private GameObject itemHolder;

        private GameObject _heldItem;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<ICollectable>() != null)
            {
                if (other.GetComponent<ICollectable>().CanCollect())
                {
                    Collect(other.gameObject);
                }
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<ICollectable>() != null)
            {
                if (other.gameObject.GetComponent<ICollectable>().CanCollect())
                {
                    Collect(other.gameObject);
                }
            }
        }

        public void Collect(GameObject item)
        {
            item.transform.SetParent(itemHolder.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            _heldItem = item;
            Debug.Log($"Collecting {item.name}");
            Invoke("GiveBack", 2f);
        }

        public void GiveBack()
        {
            Debug.Log("Giving back");
            if (!_heldItem.TryGetComponent(out Projectile projectile)) return;
            
            if (projectile.GetInstigator() != null)
            {
                projectile.transform.SetParent(transform.root);
                projectile.SetCanExplode(true);
                projectile.Launch(projectile.GetInstigator());
            }
        }
    }
}
