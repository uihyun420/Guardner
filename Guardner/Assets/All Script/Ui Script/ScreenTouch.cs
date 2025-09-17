using UnityEngine;

public class ScreenTouch : MonoBehaviour
{
    public Rect spawnRect;
    public bool useColliderArea = true;
    public BoxCollider2D touchArea;

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
            bool isInArea = false;


        }
    }
}
