using TMPro;
using UnityEngine;

public class PlayerVisualModel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;

    public void SetName(string name)
    {
        playerNameText.text = name;
    }
}