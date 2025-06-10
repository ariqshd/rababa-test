using UnityEngine;
using UnityEngine.InputSystem;

namespace RababaTest.Characters
{
    public class PlayerInputHandler : MonoBehaviour
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
    }
}