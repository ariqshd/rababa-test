using RababaTest.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RababaTest.UI
{
    public class EndGamePanel : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private TextMeshProUGUI totalTimeText;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(HandleOnRestartButton);
            exitButton.onClick.AddListener(HandleOnExitButton);
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(HandleOnRestartButton);
            exitButton.onClick.RemoveListener(HandleOnExitButton);
        }

        private void HandleOnExitButton()
        {
            SceneManager.LoadScene("PlayerSetup");
            gameObject.SetActive(false);
        }

        private void HandleOnRestartButton()
        {
            LevelInitializer.Instance.Restart();
            gameObject.SetActive(false);
        }
        
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
        public void SetTotalTime(float time)
        {
            totalTimeText.gameObject.SetActive(true);
            totalTimeText.text = $"Total Time: {time:0.00}";
        }
    }
}
