using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
[RequireComponent(typeof(TMP_InputField))]
public class NameInputField : MonoBehaviour
{
    [SerializeField] Button AcceptButton;
    private TMP_InputField inputField;
    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        AcceptButton.onClick.AddListener(ChangeName);
    }
    private async void ChangeName()
    {
        var text = inputField.text;

        if (string.IsNullOrWhiteSpace(text)) return;

        Debug.Log($"Ввод завершен. Новое имя: {text}");

        inputField.interactable = false;

        await AuthenticationService.Instance.UpdatePlayerNameAsync(text);

        inputField.interactable = true;
    }
    private void OnDestroy()
    {
        AcceptButton.onClick.RemoveListener(ChangeName);
    }
}
