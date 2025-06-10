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
            var rootMenu = GameObject.Find("MainLayout");
            if (rootMenu != null)
            {
                var menu = Instantiate(playerSetupMenuPrefab, rootMenu.transform);
                playerInput.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
                menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(playerInput.playerIndex);
            }
        }
    }
}
