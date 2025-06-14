using System;
using RababaTest.EventBus;
using TMPro;
using UnityEngine;

namespace RababaTest.UI
{
    public class GameHud : MonoBehaviour
    {
        [SerializeField] private PlayerInfo _leftPlayerInfo;
        [SerializeField] private PlayerInfo _rightPlayerInfo;
        [SerializeField] private PlayerInfo _bossInfo;
        [SerializeField] private EndGamePanel _gameOverPanel;
        [SerializeField] private EndGamePanel _bossDefeatedPanel;
        [SerializeField] private TextMeshProUGUI _timerText;
        
        private EventBinding<PlayerSpawnEvent> _playerSpawnEvent;
        private EventBinding<PlayerTakeDamageEvent> _playerTakeDamageEvent;
        private EventBinding<BossSpawnEvent> _bossSpawnEvent;
        private EventBinding<BossTakeDamageEvent> _bossTakeDamageEvent;
        private EventBinding<BossDefeatedEvent> _bossDefeatedEvent;
        private EventBinding<GameOverEvent> _gameOverEvent;
        
        float _timer;
        private float _timerCache;
        private bool _canTickTimer = false;
        private void OnEnable()
        {
            _playerSpawnEvent = new EventBinding<PlayerSpawnEvent>(HandleOnPlayerSpawnEvent);
            EventBus<PlayerSpawnEvent>.Register(_playerSpawnEvent);
            
            _bossSpawnEvent = new EventBinding<BossSpawnEvent>(HandleBossSpawnEvent);
            EventBus<BossSpawnEvent>.Register(_bossSpawnEvent);
            
            _playerTakeDamageEvent = new EventBinding<PlayerTakeDamageEvent>(PlayerTakeDamageEvent);
            EventBus<PlayerTakeDamageEvent>.Register(_playerTakeDamageEvent);
            
            _bossTakeDamageEvent = new EventBinding<BossTakeDamageEvent>(HandleBossTakeDamageEvent);
            EventBus<BossTakeDamageEvent>.Register(_bossTakeDamageEvent);
            
            _bossDefeatedEvent = new EventBinding<BossDefeatedEvent>(BossDefeatedEvent);
            EventBus<BossDefeatedEvent>.Register(_bossDefeatedEvent);

            _gameOverEvent = new EventBinding<GameOverEvent>(HandleGameOverEvent);
            EventBus<GameOverEvent>.Register(_gameOverEvent);
        }

        private void Update()
        {
            if (_canTickTimer)
            {
                _timer += Time.deltaTime;
                _timerText.text = $"{Mathf.RoundToInt(_timer)}";                
            }
        }

        private void HandleGameOverEvent()
        {
            _gameOverPanel.SetActive(true);
            _bossInfo.Flush();
            _leftPlayerInfo.Flush();
            _rightPlayerInfo.Flush();
            _canTickTimer = false;
            _timerCache = _timer;
            _timer = 0;
        }

        private void BossDefeatedEvent()
        {
            _bossInfo.Flush();
            _leftPlayerInfo.Flush();
            _rightPlayerInfo.Flush();
            _canTickTimer = false;
            _timerCache = _timer;
            _timer = 0;
            _bossDefeatedPanel.SetTotalTime(_timerCache);
            _bossDefeatedPanel.SetActive(true);
        }

        private void HandleBossTakeDamageEvent(BossTakeDamageEvent obj)
        {
            _bossInfo.SetHeart((int)obj.CurrentHealth);
            Debug.Log($"Boss current health: {obj.CurrentHealth}");
        }

        private void PlayerTakeDamageEvent(PlayerTakeDamageEvent obj)
        {
            switch (obj.PlayerIndex)
            {
                case 0:
                    _leftPlayerInfo.SetHeart((int)obj.CurrentHealth);
                    break;
                case 1:
                    _rightPlayerInfo.SetHeart((int)obj.CurrentHealth);
                    break;
            }
        }

        private void HandleBossSpawnEvent(BossSpawnEvent obj)
        {
            _bossInfo.Set(3);
            _canTickTimer = true;
        }

        private void HandleOnPlayerSpawnEvent(PlayerSpawnEvent obj)
        {
            switch (obj.PlayerConfiguration.PlayerIndex)
            {
                case 0:
                    _leftPlayerInfo.Set(obj.PlayerConfiguration.PlayerIndex, 3);
                    break;
                case 1:
                    _rightPlayerInfo.Set(obj.PlayerConfiguration.PlayerIndex, 3);
                    break;
            }
        }

        private void OnDisable()
        {
            EventBus<PlayerSpawnEvent>.Deregister(_playerSpawnEvent);
            EventBus<BossSpawnEvent>.Deregister(_bossSpawnEvent);
            EventBus<PlayerTakeDamageEvent>.Deregister(_playerTakeDamageEvent);
            EventBus<BossTakeDamageEvent>.Deregister(_bossTakeDamageEvent);
            EventBus<BossDefeatedEvent>.Deregister(_bossDefeatedEvent);
            EventBus<GameOverEvent>.Deregister(_gameOverEvent);
        }
    }
}
