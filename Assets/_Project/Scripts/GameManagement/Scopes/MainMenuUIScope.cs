using Assets._Project.Scripts.UI.MainMenu;
using Assets._Project.Scripts.UI.Tools;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class MainMenuUIScope : LifetimeScope
{
    [SerializeField] SwitchableCanvasGroup initialCanvas;
    protected override void Configure(IContainerBuilder builder)
    {
        var canvasManager = new CanvasManager(initialCanvas);
        builder.RegisterInstance(canvasManager);
    }
}
