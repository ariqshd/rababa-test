using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace RababaTest.Managers
{
    public class PlayerConfiguration
    {
        public PlayerConfiguration(PlayerInput playerInput)
        {
            PlayerIndex = playerInput.playerIndex;
            PlayerInput = playerInput;
        }

        public PlayerInput PlayerInput;
        public int PlayerIndex;
        public bool IsReady;
        public Material PlayerMaterial;
    }

    public class PlayerConfigurationManager : MonoBehaviour
    {
        private List<PlayerConfiguration> _playerConfigurations;

        [SerializeField] private int maxPlayers = 2;
        
        public static PlayerConfigurationManager Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("Multiple player configurations detected");
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
                _playerConfigurations = new List<PlayerConfiguration>();
            }
        }
        
        public void SetPlayerColor(int playerIndex, Material color)
        {
            var playerConfiguration = _playerConfigurations.Find(x => x.PlayerIndex == playerIndex);
            if (playerConfiguration == null)
            {
                Debug.Log("Player configuration not found");
                return;
            }

            playerConfiguration.PlayerMaterial = color;
        }

        public void ReadyPlayer(int index)
        {
            var playerConfiguration = _playerConfigurations.Find(x => x.PlayerIndex == index);
            if (playerConfiguration == null)
            {
                Debug.Log("Player configuration not found");
                return;
            }
            
            playerConfiguration.IsReady = true;
            
            if (IsAllPlayersReady())
            {
                Debug.Log("All players are ready. Let's start the game");
                SceneManager.LoadScene("Game");
            }
            
        }

        public bool IsAllPlayersReady()
        {
            foreach (var playerConfiguration in _playerConfigurations)
            {
                if (!playerConfiguration.IsReady)
                {
                    return false;
                }
            }
            
            return true;
        }

        public void HandlePlayerJoin(PlayerInput playerInput)
        {
            if (_playerConfigurations.All(p => p.PlayerIndex != playerInput.playerIndex))
            {
                playerInput.transform.SetParent(transform);
                _playerConfigurations.Add(new PlayerConfiguration(playerInput));
            }
            Debug.Log($"Player joined: {playerInput.playerIndex}");
        }
    }
}

