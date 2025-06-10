using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RababaTest.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class Mover : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 50f;
        
        private CharacterController _characterController;
        private Vector3 _moveDirection = Vector3.zero;
        private Vector2 _inputvector = Vector2.zero;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        public void SetInputVector(Vector2 input)
        {
            _inputvector = input;
        }

        private void Update()
        {
            _moveDirection = new Vector3(_inputvector.x, 0, _inputvector.y);
            _moveDirection = transform.TransformDirection(_moveDirection);
            _moveDirection *= moveSpeed;
            _characterController.Move(_moveDirection * Time.deltaTime);
        }
    }
}
