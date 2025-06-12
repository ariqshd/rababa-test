using RababaTest;
using TMPro;
using UnityEngine;

namespace RababaTest.UI
{
    public class PlayerSetupMenuController : MonoBehaviour
    {
        private int _playerIndex;
        private float _ignoreInputTime = 1.5f;
        private bool _inputEnabled;

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private GameObject selectMenu;

        void Update()
        {
            if (Time.time > _ignoreInputTime)
            {
                _inputEnabled = true;
            }
        }
        
        public void SetPlayerIndex(int playerIndex)
        {
            _playerIndex = playerIndex;
            _ignoreInputTime = Time.time + _ignoreInputTime;
        }

        public void SetPlayerName(string playerName)
        {
            titleText.text = playerName;
            PlayerConfigurationManager.Instance.SetPlayerName(_playerIndex, titleText.text);
        }

        public void SetColor(Material color)
        {
            if (!_inputEnabled)
            {
                return;
            }
            
            PlayerConfigurationManager.Instance.SetPlayerColor(_playerIndex, color);
            ReadyPlayer();
        }

        public void ReadyPlayer()
        {
            if (!_inputEnabled)
            {
                return;
            }
            
            PlayerConfigurationManager.Instance.ReadyPlayer(_playerIndex);
        }
    }
}
