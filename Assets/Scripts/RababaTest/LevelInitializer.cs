using System;
using RababaTest.Characters;
using UnityEngine;

namespace RababaTest
{
    public class LevelInitializer : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject playerPrefab;

        private void Start()
        {
            if (!PlayerConfigurationManager.Instance)
            {
                Debug.LogError("Player configuration manager not found");
                return;
            };
            
            var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
            
            for (int i = 0; i < playerConfigs.Length; i++)
            {
                var player = Instantiate(playerPrefab, spawnPoints[i].position, spawnPoints[i].rotation,
                    gameObject.transform);

                if (!player.TryGetComponent(out Player playerInputHandler))
                {
                    Debug.LogError("Player input handler not found");
                    return;
                }
                
                playerInputHandler.Init(playerConfigs[i]);
            }
        }
    }
}