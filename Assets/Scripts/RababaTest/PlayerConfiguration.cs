using UnityEngine;
using UnityEngine.InputSystem;

namespace RababaTest
{
    public class PlayerConfiguration
    {
        public PlayerConfiguration(PlayerInput playerInput)
        {
            PlayerIndex = playerInput.playerIndex;
            PlayerInput = playerInput;
        }

        public string PlayerName;
        public PlayerInput PlayerInput;
        public int PlayerIndex;
        public bool IsReady;
        public Material PlayerMaterial;
    }
}