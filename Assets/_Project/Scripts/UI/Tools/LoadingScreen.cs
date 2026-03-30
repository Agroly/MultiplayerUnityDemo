using VContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets._Project.Scripts.UI.Tools
{
    [RequireComponent(typeof(SwitchableCanvasGroup))]
    public class LoadingScreen : MonoBehaviour
    {
        private GameManager gameManager;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        SwitchableCanvasGroup canvasGroup;
        private void Start ()
        {
            gameManager.OnGameStateChanged += SwitchLoadingScreen;
            canvasGroup = GetComponent<SwitchableCanvasGroup>();
        }
        private void SwitchLoadingScreen(GameState gameState)
        {
            if (!canvasGroup.Enabled && gameState == GameState.Loading) canvasGroup.Show();
            else if (canvasGroup.Enabled) canvasGroup.Hide();
        }
        private void OnDestroy()
        {
            gameManager.OnGameStateChanged -= SwitchLoadingScreen;
        }
    }
}
