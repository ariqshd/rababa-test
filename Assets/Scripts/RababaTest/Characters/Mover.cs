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
        [SerializeField]
        private float gravity = -9.81f;
        private CharacterController _characterController;
        private Vector3 _moveDirection = Vector3.zero;
        private Vector2 _inputvector = Vector2.zero;
        private float _verticalVelocity = 0f;

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
            
            if (_characterController.isGrounded)
            {
                _verticalVelocity = -1f; // Small ground contact velocity
            }
            else
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }

            // Apply movement
            Vector3 velocity = _moveDirection;
            velocity.y = _verticalVelocity;
            
            _characterController.Move(velocity * Time.deltaTime);
        }
    }
}
