using Assets._Project.Scripts.UI.Tools;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class JoinByCodeWindow : MonoBehaviour
{
    [Inject] SessionManager sessionManager;

    [SerializeField] TextMeshProUGUI ErrorText;
    [SerializeField] TMP_InputField InputField;
    [SerializeField] Button conifrmButton;

    private void Awake()
    {
        conifrmButton.onClick.AddListener(JoinSessionByCode);
    }
    private void OnDestroy()
    {
        conifrmButton.onClick.RemoveListener(JoinSessionByCode);
    }
    private async void JoinSessionByCode()
    {
        try
        {
            await sessionManager.JoinSessionAsClient(InputField.text, ConnectionType.ByCode);

            ErrorText.text = "";
        }
        catch (System.Exception e)
        {
            // Если произошла ошибка, выводим сообщение в UI (или просто ничего не делаем)
            Debug.LogWarning($"Failed to join session by code: {e.Message}");
            ErrorText.text = "Не удалось присоединиться к сессии. Проверьте код!";
        }
    }
}
