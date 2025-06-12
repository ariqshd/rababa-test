using System;
using System.Collections;
using System.Collections.Generic;
using RababaTest.Characters;
using RababaTest.EventBus;
using UnityEngine;
using UnityEngine.Events;

namespace RababaTest.Core
{
    public class LevelInitializer : MonoBehaviour
    {
        [SerializeField] private Transform[] playerSpawnPoints;
        [SerializeField] private Transform enemySpawnPoint;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject enemyPrefab;

        private List<Character> _survivors = new List<Character>();

        public static LevelInitializer Instance;

        private EventBinding<GameReadyEvent> _gamereadyEventBinding;
        private Enemy _boss;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("Multiple player configurations detected");
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            StartCoroutine(Init_Internal());
        }

        public void Restart()
        {
            Destroy(_boss.gameObject);
            foreach (var survivor in _survivors)
            {
                Destroy(survivor.gameObject);
            }
            StartCoroutine(Init_Internal());
        }

        private IEnumerator Init_Internal()
        {
            SpawnEnemy();
            SpawnPlayers();
            EventBus<GameReadyEvent>.Raise(new GameReadyEvent());
            yield return null;
        }

        private void SpawnEnemy()
        {
            var enemyObj = Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation,
                gameObject.transform);
            if (enemyObj.TryGetComponent(out Enemy enemy))
            {
                EventBus<BossSpawnEvent>.Raise(new BossSpawnEvent { Enemy = enemy });
                _boss = enemy;
                enemy.onDie.AddListener(c =>
                {
                    EventBus<BossDefeatedEvent>.Raise(new BossDefeatedEvent());
                });
            }
            
        }

        private void SpawnPlayers()
        {
            if (!PlayerConfigurationManager.Instance)
            {
                Debug.LogError("Player configuration manager not found");
                return;
            };
            
            var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
            
            for (int i = 0; i < playerConfigs.Length; i++)
            {
                if(playerSpawnPoints.Length <= 0) 
                {
                    Debug.LogError("Player spawn points not found");   
                    return;
                }

                var playerObj = Instantiate(playerPrefab, playerSpawnPoints[i].position, playerSpawnPoints[i].rotation,
                    gameObject.transform);

                if (!playerObj.TryGetComponent(out Player player))
                {
                    Debug.LogError("Player input handler not found");
                    return;
                }
                
                player.Init(playerConfigs[i]);
                player.onDie.AddListener(HandleOnPlayerDie);
                _survivors.Add(player);
                EventBus<PlayerSpawnEvent>.Raise(new PlayerSpawnEvent{ Player = player, PlayerConfiguration = playerConfigs[i] });
            }
        }

        private void HandleOnPlayerDie(Character value)
        {
            _survivors.Remove(value);
            if (_survivors.Count == 0)
            {
                EventBus<GameOverEvent>.Raise(new GameOverEvent());
                Debug.Log($"Game over");
            }
        }
    }
}