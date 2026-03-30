using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Scripts.UI.Tools
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SwitchableCanvasGroup : MonoBehaviour
    {
        [field:SerializeField] public bool Enabled { get; private set; }
        private CanvasGroup _canvasGroup;
        private void Awake()
        {
           _canvasGroup = GetComponent<CanvasGroup>();
        }
        /// <summary>
        /// Включить CanvasGroup
        /// </summary>
        public void Show()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            Enabled = true;
        }

        /// <summary>
        /// Выключить CanvasGroup
        /// </summary>
        public void Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            Enabled = false;
        }
    }
}
