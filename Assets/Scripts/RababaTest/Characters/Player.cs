using RababaTest.EventBus;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RababaTest.Characters
{
    public class Player : Character
    {
        [SerializeField]
        private MeshRenderer playerMesh;
        
        private PlayerConfiguration _playerConfiguration;
        private Mover _mover;
        private InputSystem_Actions _control;

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _control = new InputSystem_Actions();
        }

        public void Init(PlayerConfiguration config)
        {
            _playerConfiguration = config;
            playerMesh.material = _playerConfiguration.PlayerMaterial;
            config.PlayerInput.onActionTriggered += Input_onActionTriggered;
        }

        private void Input_onActionTriggered(InputAction.CallbackContext ctx)
        {
            if (ctx.action.name == _control.Player.Move.name)
            {
                OnMove(ctx);
            }
        }
        
        public void OnMove(InputAction.CallbackContext ctx)
        {
            if(_mover != null)
                _mover.SetInputVector(ctx.ReadValue<Vector2>());
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            
            EventBus<PlayerTakeDamageEvent>.Raise(new PlayerTakeDamageEvent { PlayerIndex = _playerConfiguration.PlayerIndex, CurrentHealth = GetHealth()});
        }

        protected override void Die()
        {
            base.Die();
        }
    }
}