using Assets._Project.Scripts.UI.Tools;
using VContainer;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class SwitchUIButton : MonoBehaviour
{
    [SerializeField] SwitchableCanvasGroup targetCanvas;
    [Inject] private CanvasManager canvasManager;
    private void Start()
    {
        if (targetCanvas == null)
            Debug.LogError($"Target canvas not assigned on {gameObject.name}");
        var button = GetComponent<Button>();
        button.onClick.AddListener(SwitchToTargetCanvas);
    }
    private void SwitchToTargetCanvas()
    {
        canvasManager.SwitchCanvas(targetCanvas);
    }
}
