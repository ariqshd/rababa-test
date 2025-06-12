using System;
using System.Collections.Generic;
using System.Linq;
using RababaTest.EventBus;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace RababaTest
{
    public class PlayerConfigurationManager : MonoBehaviour
    {
        [SerializeField] private int maxPlayers = 2;
        private List<PlayerConfiguration> _playerConfigurations;
        private int _playerJoinCount;
        
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

        public void SetPlayerName(int playerIndex, string name)
        {
            var playerConfiguration = _playerConfigurations.Find(x => x.PlayerIndex == playerIndex);
            if (playerConfiguration == null)
            {
                Debug.Log("Player configuration not found");
                return;
            }
            
            playerConfiguration.PlayerName = name;
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
        }

        public void TryStartGame()
        {
            if (IsAllPlayersReady())
            {
                Debug.Log("All players are ready. Let's start the game");
                SceneManager.LoadScene("Game");
            }
            else
            {
                Debug.LogWarning("All players must be ready to start the game");
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
            _playerJoinCount++;
            if (_playerJoinCount >= maxPlayers)
            {
                EventBus<AllPlayersJoinedEvent>.Raise(new AllPlayersJoinedEvent());
            }
        }

        public List<PlayerConfiguration> GetPlayerConfigs()
        {
            return _playerConfigurations;
        }
    }
}

