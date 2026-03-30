using VContainer;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;

namespace Assets._Project.Scripts.UI
{
    public class SessionCreateUI : MonoBehaviour 
    {
        [Inject] SessionManager sessionManager;
        [SerializeField] private Button confirmButton;
        [SerializeField] private TMP_Text maxPlayers;
        [SerializeField] private TMP_InputField sessionName;
        [SerializeField] private Toggle isPrivate;

        private void Awake()
        {
            confirmButton.onClick.AddListener(StartSession);
        }
        private async void StartSession()
        {
            var options = SerializeInputs();
            await sessionManager.StartSessionAsHost(options);
        }
        private  SessionOptions SerializeInputs()
        {
            var options = new SessionOptions();
            options.MaxPlayers = int.TryParse(maxPlayers.text, out var v) ? v : 2;
            options.IsPrivate = isPrivate.isOn;
            options.Name = string.IsNullOrWhiteSpace(sessionName.text) ? "Room": sessionName.text;
            return options
                .WithRelayNetwork("europe-central2")
                .WithPlayerName(VisibilityPropertyOptions.Public);
        }
        private void OnDestroy()
        {
            confirmButton.onClick.RemoveAllListeners();
        }
    }
        
}
