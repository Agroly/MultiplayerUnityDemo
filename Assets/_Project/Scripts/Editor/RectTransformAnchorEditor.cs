using UnityEngine;
using UnityEditor;

public class RectTransformAnchorEditor
{
    // Клавиша K сработает, если выделен хотя бы один объект в иерархии
    [MenuItem("Tools/UI/Anchors to Corners _k")]
    [MenuItem("CONTEXT/RectTransform/Anchors to Corners")]
    public static void AnchorsToCorners()
    {
        foreach (Transform transform in Selection.transforms)
        {
            RectTransform t = transform as RectTransform;
            if (t == null || t.parent == null) continue;

            RectTransform p = t.parent as RectTransform;

            Undo.RecordObject(t, "Anchors to Corners");

            // Расчет новых якорей
            Vector2 newMin = new Vector2(
                t.anchorMin.x + t.offsetMin.x / p.rect.width,
                t.anchorMin.y + t.offsetMin.y / p.rect.height);

            Vector2 newMax = new Vector2(
                t.anchorMax.x + t.offsetMax.x / p.rect.width,
                t.anchorMax.y + t.offsetMax.y / p.rect.height);

            t.anchorMin = newMin;
            t.anchorMax = newMax;

            // Сброс оффсетов
            t.offsetMin = Vector2.zero;
            t.offsetMax = Vector2.zero;
        }
    }
}