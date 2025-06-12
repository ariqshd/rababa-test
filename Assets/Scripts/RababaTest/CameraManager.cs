using System;
using RababaTest.Core;
using RababaTest.EventBus;
using Unity.Cinemachine;
using UnityEngine;

namespace RababaTest
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private CinemachineFollow cameraFollow;

        private EventBinding<PlayerSpawnEvent> _playerSpawnEvent;
        private EventBinding<BossSpawnEvent> _bossSpawnEvent;
        private EventBinding<GameReadyEvent> _gamereadyEventBinding;

        private void OnEnable()
        {
            _playerSpawnEvent = new EventBinding<PlayerSpawnEvent>(HandleOnPlayerSpawnEvent);
            EventBus<PlayerSpawnEvent>.Register(_playerSpawnEvent);
            
            _bossSpawnEvent = new EventBinding<BossSpawnEvent>(HandleOnBossSpawnEvent);
            EventBus<BossSpawnEvent>.Register(_bossSpawnEvent);
            
            _gamereadyEventBinding = new EventBinding<GameReadyEvent>(HandleGameReadyEvent);
            EventBus<GameReadyEvent>.Register(_gamereadyEventBinding);
        }

        private void HandleOnBossSpawnEvent(BossSpawnEvent obj)
        {
            targetGroup.AddMember(obj.Enemy.transform, .2f, 0.5f);
        }

        private void HandleOnPlayerSpawnEvent(PlayerSpawnEvent obj)
        {
            targetGroup.AddMember(obj.Player.transform, 1, 0.5f);
        }

        private void OnDisable()
        {
            EventBus<PlayerSpawnEvent>.Deregister(_playerSpawnEvent);
            EventBus<BossSpawnEvent>.Deregister(_bossSpawnEvent);
            EventBus<GameReadyEvent>.Deregister(_gamereadyEventBinding);
        }

        private void HandleGameReadyEvent()
        {
        }
    }
}
