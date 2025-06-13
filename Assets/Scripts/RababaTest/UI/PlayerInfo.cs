using System;
using System.Collections.Generic;
using RababaTest.EventBus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RababaTest.UI
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private GameObject heartContainer;
        [SerializeField] private Color fullHeartColor;
        [SerializeField] private Color emptyHeartColor;
        [SerializeField] private GameObject heartPrefab;

        private List<GameObject> _hearts = new List<GameObject>();
        
        private int _playerIndex;

        private void OnEnable()
        {

        }



        public void Set(int playerIndex, int maxHeart)
        {
            _playerIndex = playerIndex;
            for (int i = 0; i < maxHeart; i++)
            {
                var obj = Instantiate(heartPrefab, heartContainer.transform);
                _hearts.Add(obj);
            }
        }
        
        public void Set(int maxHeart)
        {
            for (int i = 0; i < maxHeart; i++)
            {
                var obj = Instantiate(heartPrefab, heartContainer.transform);
                _hearts.Add(obj);
            }
        }

        public void Flush()
        {
            foreach (var heart in _hearts)
            {
                Destroy(heart);
            }
            _hearts.Clear();
        }

        public void SetHeart(int value)
        {
            int countToEmpty = _hearts.Count - value;
            foreach (var heart in _hearts)
            {
                if (countToEmpty <= 0) continue;
                if (!heart.TryGetComponent(out Image image)) continue;
                
                image.color = emptyHeartColor;
                countToEmpty--;
            }
        }
    }
}