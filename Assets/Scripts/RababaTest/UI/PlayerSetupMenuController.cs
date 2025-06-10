using RababaTest;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RababaTest.UI
{
    public class PlayerSetupMenuController : MonoBehaviour
    {
        private int _playerIndex;
        private float _ignoreInputTime = 1.5f;
        private bool _inputEnabled;

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private GameObject readyPanel;
        [SerializeField] private GameObject selectMenu;
        [SerializeField] private Button readyButton;

        public void SetPlayerIndex(int playerIndex)
        {
            _playerIndex = playerIndex;
            titleText.text = $"Player {_playerIndex + 1}";
            _ignoreInputTime = Time.time + _ignoreInputTime;
        }
        
        void Update()
        {
            if (Time.time > _ignoreInputTime)
            {
                _inputEnabled = true;
            }
        }

        public void SetColor(Material color)
        {
            if (!_inputEnabled)
            {
                return;
            }
            
            PlayerConfigurationManager.Instance.SetPlayerColor(_playerIndex, color);
            readyPanel.SetActive(true);
            readyButton.Select();
            selectMenu.SetActive(false);
        }

        public void ReadyPlayer()
        {
            if (!_inputEnabled)
            {
                return;
            }
            
            PlayerConfigurationManager.Instance.ReadyPlayer(_playerIndex);
            readyButton.gameObject.SetActive(false);
        }
    }
}
