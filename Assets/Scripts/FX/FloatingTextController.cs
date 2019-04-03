using UnityEngine;
using System.Collections;

public class FloatingTextController : MonoBehaviour
{
    private static FloatingText popupText;
    private static GameObject canvas;
    private static Camera cameraPlayer;

    public static void Initialize(GameObject _canvas, Camera mainCamera)
    {
        canvas = _canvas;
        cameraPlayer = mainCamera;
        if (!popupText)
            popupText = Resources.Load<FloatingText>("FX/PopupTextParent");
        if (popupText == null)
            Debug.LogError("Text not found");
    }

    public static void CreateFloatingText(string text, Transform location, Color color)
    {
        if (!popupText)
            return;
        FloatingText instance = Instantiate(popupText);
        //Vector2 screenPosition = cameraPlayer.WorldToScreenPoint(new Vector2(location.position.x + Random.Range(-.2f, .2f), location.position.y + Random.Range(-.2f, .2f)));
        Vector2 screenPosition = cameraPlayer.WorldToScreenPoint(location.position);
        Debug.Log("Screen position: " + screenPosition);

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = screenPosition;
        instance.SetTextColor(color);
        instance.SetText(text);
    }
}