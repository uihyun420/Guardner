using UnityEngine;

public class ScreenTouch : MonoBehaviour
{
    public BoxCollider2D[] touchAreas;
    private int selectedAreaIndex = -1; // 선택된 영역의 인덱스

    public Camera mainCamera;
    public WindowManager windowManager;

    private bool isUiBlocking = false;

    private void Update()
    {
        if (isUiBlocking)
            return;

#if UNITY_EDITOR
        // 에디터에서는 마우스만 처리
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            HandleTouch(mousePosition);
        }
#else
    // 디바이스에서는 터치만 처리
    if (Input.touchCount == 1)
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            Vector2 touchPosition = touch.position;
            HandleTouch(touchPosition);
        }
    }
#endif
    }

    public void HandleTouch(Vector2 screenPosition)
    {
        Vector2 worldPosition = Vector2.zero; // 선언 위치를 함수 맨 위로 이동

        if (mainCamera != null)
        {
             worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
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

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);

        foreach (var hit in hits)
        {
            GuardnerBehavior guardner = hit.GetComponent<GuardnerBehavior>();
            if (guardner != null)
            {
                guardner.ToggleRangeDisplay();
                break; // 한 번만 처리
            }
        }

    }

    public int GetSelectedAreaIndex()
    {
        return selectedAreaIndex;
    }

    public void SetUiBlocking(bool blocking)
    {
        isUiBlocking = blocking;
    }
}
