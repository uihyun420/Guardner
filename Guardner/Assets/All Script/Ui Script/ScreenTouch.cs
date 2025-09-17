using UnityEngine;

public class ScreenTouch : MonoBehaviour
{
    public BoxCollider2D[] touchAreas;
    private int selectedAreaIndex = -1; // 선택된 영역의 인덱스

    public Camera mainCamera;
    public WindowManager windowManager;

    private void Update()
    {
        if(Input.touchCount ==1)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = touch.position;
                HandleTouch(touchPosition);
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            HandleTouch(mousePosition);
        }
    }

    public void HandleTouch(Vector2 screenPosition)
    {
        if(mainCamera != null)
        {
            Vector2 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            selectedAreaIndex = -1;

            for(int i =0;  i < touchAreas.Length; i++)
            {
                if (touchAreas[i] != null && touchAreas[i].bounds.Contains(worldPosition))
                {
                    selectedAreaIndex = i;
                    if(windowManager != null)
                    {
                        windowManager.OpenOverlay(WindowType.GuardnerSpawnUi);
                    }
                    break;
                }
            }
        }
    }

    public int GetSelectedAreaIndex()
    {
        return selectedAreaIndex;
    }
}
