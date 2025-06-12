using System;
using RababaTest.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

namespace RababaTest
{
    public class SpawnPlayerSetupMenu : MonoBehaviour
    {
        public GameObject playerSetupMenuPrefab;
        public PlayerInput playerInput;
        private void Awake()
        {
            PlayerSetupHud playerSetupHud = FindAnyObjectByType<PlayerSetupHud>();
            if (playerSetupHud != null)
            {
                var menu = Instantiate(playerSetupMenuPrefab);
                playerSetupHud.AddPlayerCard(menu);
                playerInput.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
                if (menu.TryGetComponent(out PlayerSetupMenuController playerSetup))
                {
                    playerSetup.SetPlayerName($"Player {playerInput.playerIndex + 1}");
                    playerSetup.SetPlayerIndex(playerInput.playerIndex);
                }
            }
        }
    }
}
