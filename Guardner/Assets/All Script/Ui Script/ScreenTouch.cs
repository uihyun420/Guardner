using UnityEngine;

public class ScreenTouch : MonoBehaviour
{
    [SerializeField] private ReCellUi reCellUi;
    [SerializeField] private GuardnerSpawnUi guardnerSpawnUi;
    public BoxCollider2D[] touchAreas;
    private int selectedAreaIndex = -1;

    public Camera mainCamera;
    public WindowManager windowManager;

    private bool isUiBlocking = false;
    private GuardnerBehavior lastOpenedGuardner = null;

    private void Update()
    {
        if (isUiBlocking)
            return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(Input.mousePosition);
        }
#else
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleTouch(touch.position);
            }
        }
#endif
    }

    public void HandleTouch(Vector2 screenPosition)
    {
        if (mainCamera == null) return;

        Vector3 worldPos3D = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));
        Vector2 worldPosition = new Vector2(worldPos3D.x, worldPos3D.y);

        selectedAreaIndex = -1;

        // 스폰 영역 터치 체크
        CheckSpawnAreaTouch(worldPosition);

        // 가드너 터치 체크
        CheckGuardnerTouch(worldPosition);
    }

    private void CheckSpawnAreaTouch(Vector2 worldPosition)
    {
        for (int i = 0; i < touchAreas.Length; i++)
        {
            if (touchAreas[i] == null) continue;

            var bounds = touchAreas[i].bounds;
            bool isInBounds = worldPosition.x >= bounds.min.x && worldPosition.x <= bounds.max.x &&
                              worldPosition.y >= bounds.min.y && worldPosition.y <= bounds.max.y;

            if (isInBounds)
            {
                selectedAreaIndex = i;
                OpenGuardnerSpawnUi();
                break;
            }
        }
    }

    private void CheckGuardnerTouch(Vector2 worldPosition)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);
        foreach (var hit in hits)
        {
            GuardnerBehavior guardner = hit.GetComponent<GuardnerBehavior>();
            if (guardner != null)
            {
                guardner.ToggleRangeDisplay();
                HandleGuardnerUI(guardner);
                break;
            }
        }
    }

    private void OpenGuardnerSpawnUi()
    {
        if (windowManager != null)
        {
            windowManager.OpenOverlay(WindowType.GuardnerSpawnUi);
        }
        else if (guardnerSpawnUi != null)
        {
            guardnerSpawnUi.Open();
        }
    }

    private void HandleGuardnerUI(GuardnerBehavior guardner)
    {
        if (reCellUi == null) return;

        if (lastOpenedGuardner == guardner)
        {
            reCellUi.Close();
            lastOpenedGuardner = null;
        }
        else
        {
            reCellUi.Open(guardner);
            lastOpenedGuardner = guardner;
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