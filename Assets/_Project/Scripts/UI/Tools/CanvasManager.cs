using Assets._Project.Scripts.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


public class CanvasManager
{
    private SwitchableCanvasGroup _currentCanvas;

    public CanvasManager(SwitchableCanvasGroup currentCanvas)
    {
        _currentCanvas = currentCanvas;
    }

    /// <summary>
    /// Переключает на новый CanvasGroup
    /// </summary>
    public void SwitchCanvas(SwitchableCanvasGroup newCanvas)
    {
        if (_currentCanvas != null)
            _currentCanvas.Hide();

        _currentCanvas = newCanvas;

        if (_currentCanvas != null)
            _currentCanvas.Show();
    }

}
