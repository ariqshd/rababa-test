using System;
using RababaTest.EventBus;
using TMPro;
using UnityEngine;

namespace RababaTest.UI
{
    public class PlayerSetupHud : MonoBehaviour
    {
        [SerializeField] private GameObject holder;
        [SerializeField] private GameObject playerCardHolder;
        [SerializeField] private TextMeshProUGUI prompt;

        private int _totalPlayer;

        private EventBinding<AllPlayersJoinedEvent> _allPlayersJoinedEvent;
        
        private void OnEnable()
        {
            _allPlayersJoinedEvent = new EventBinding<AllPlayersJoinedEvent>(HandleOnAllPlayersJoinedEvent);
            EventBus<AllPlayersJoinedEvent>.Register(_allPlayersJoinedEvent);
        }

        private void HandleOnAllPlayersJoinedEvent()
        {
            prompt.text = "Press ENTER to start the game";
        }

        private void OnDisable()
        {
            EventBus<AllPlayersJoinedEvent>.Deregister(_allPlayersJoinedEvent);
        }

        public void AddPlayerCard(GameObject obj)
        {
            obj.transform.SetParent(playerCardHolder.transform);
        }
        
    }
}