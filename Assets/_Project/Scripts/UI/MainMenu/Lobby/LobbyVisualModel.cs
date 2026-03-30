using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Project.Scripts.UI.MainMenu.Lobby
{
    public class LobbyVisualModel : MonoBehaviour
    {
        [Inject] SessionManager sessionManager;

        [SerializeField] TextMeshProUGUI hostName;
        [SerializeField] TextMeshProUGUI playerNumber;
        [SerializeField] Button joinButton;
        private string connectionString;

        public void Init(string roomName, int currentPlayers, int maxPlayers, string sessionId )
        {
            this.connectionString = sessionId;

            this.hostName.text = $"Имя комнаты: {roomName}";

            playerNumber.text = $"Игроки: {currentPlayers}/{maxPlayers}";

            joinButton.onClick.AddListener(OnJoinClicked);
        }
        private async void OnJoinClicked()
        {
            try
            {
                await sessionManager.JoinSessionAsClient(connectionString, ConnectionType.ById);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
