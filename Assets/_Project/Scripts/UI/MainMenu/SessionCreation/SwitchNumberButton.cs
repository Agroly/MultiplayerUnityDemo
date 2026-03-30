using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI.MainMenu.SessionCreation
{
    [RequireComponent(typeof(Button))]
    public class SwitchNumberButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text text;

        [Header("Settings")]
        [SerializeField] private bool isIncreasing = true;
        [SerializeField] private int minValue = 1;
        [SerializeField] private int maxValue = 10;

        protected void Awake()
        {
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(SwitchNumber);
        }

        private void SwitchNumber()
        {
            if (!int.TryParse(text.text, out int value))
                value = minValue;

            value += isIncreasing ? 1 : -1;

            value = Mathf.Clamp(value, minValue, maxValue);

            text.text = value.ToString();
        }
    }
}