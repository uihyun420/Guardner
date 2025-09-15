using UnityEngine;

public class ScreenTouch : MonoBehaviour
{
    private void Update()
    {
        if(Input.touchCount ==1)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = touch.position;
            }
        }
    }
}
