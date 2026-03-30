using Assets._Project.Scripts.UI.Tools;
using System;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectScope : LifetimeScope
{
    [SerializeField] private bool DebugMode;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<GameManager>(Lifetime.Singleton);
        builder.RegisterEntryPoint<SessionManager>(Lifetime.Singleton).AsSelf().WithParameter(DebugMode);
        builder.RegisterEntryPoint<CursorController>(Lifetime.Singleton);
    }
}
