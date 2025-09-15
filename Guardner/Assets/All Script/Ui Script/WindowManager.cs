using UnityEngine;
using System.Collections.Generic;

public class WindowManager : MonoBehaviour
{
    public List<GenericWindow> windows;

    public WindowType defaultWindow;
    public WindowType CurrentWindow { get; private set; }

    private void Start()
    {
        foreach (var window in windows)
        {
            window.Init(this);
            //window.Close();
            window.gameObject.SetActive(false);
        }

        CurrentWindow = defaultWindow;
        windows[(int)CurrentWindow].Open();

        // ���� �̺�Ʈ ����
        DoorBehavior.OnDoorDestroyed += ShowGameOver;
    }

    public void Open(WindowType id)
    {
        windows[(int)CurrentWindow].Close();
        CurrentWindow = id;
        windows[(int)CurrentWindow].Open();
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ����
        DoorBehavior.OnDoorDestroyed -= ShowGameOver;
    }

    private void ShowGameOver()
    {
        Time.timeScale = 0;
        Open(WindowType.GameOver);
    }
}
