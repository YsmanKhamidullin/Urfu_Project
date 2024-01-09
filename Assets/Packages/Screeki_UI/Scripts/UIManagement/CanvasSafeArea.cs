using UnityEngine;
 
[RequireComponent(typeof(Canvas))]
public class CanvasSafeArea : MonoBehaviour
{
    public RectTransform SafeAreaRect;
 
    private Rect lastSafeArea = Rect.zero;
    private Canvas canvas;
 
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
 
    private void Update()
    {
        if (lastSafeArea != Screen.safeArea)
        {
            lastSafeArea = Screen.safeArea;
            ApplySafeArea();
        }
    }
   
    void Start()
    {
        lastSafeArea = Screen.safeArea;
        ApplySafeArea();
    }
 
    void ApplySafeArea()
    {
        if (SafeAreaRect == null)
        {
            return;
        }
 
        Rect safeArea = Screen.safeArea;
 
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;
        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;
 
        SafeAreaRect.anchorMin = anchorMin;
        SafeAreaRect.anchorMax = anchorMax;
    }
}