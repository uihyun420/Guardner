using UnityEngine;
using System.Collections.Generic;

public class WindowManager : MonoBehaviour
{
    public List<GenericWindow> windows;

    public WindowType defaultWindow;
    public WindowType CurrentWindow { get; private set; }

    private GenericWindow currentWindow;

    private void Start()
    {
        foreach (var window in windows)
        {
            window.Init(this);
            window.gameObject.SetActive(false);
        }

        // GameOverUi���� ������ â�� ���
        CurrentWindow = GameOverUi.NextWindowAfterLoad;
        // �⺻������ �缳�� (���� �� �ε带 ����)
        GameOverUi.NextWindowAfterLoad = defaultWindow;

        windows[(int)CurrentWindow].Open();

        if(CurrentWindow == WindowType.Battle && GameOverUi.RetryStageId > 0)
        {
            StageManager stageManager = FindObjectOfType<StageManager>();
            if(stageManager != null)
            {
                stageManager.LoadStage(GameOverUi.RetryStageId);

                GameOverUi.RetryStageId = 0; // ����� �ʱ�ȭ
            }
        }
        // ���� �̺�Ʈ ����
        DoorBehavior.OnDoorDestroyed += ShowGameOver;
    }

    public void OpenOverlay(WindowType id)
    {
        // �̹� ���� Ÿ���� �������̰� ���������� ���� ����
        if (windows[(int)id].gameObject.activeSelf)
            return;

        // ���� â�� ���� �ʰ� �� â�� ����
        windows[(int)id].Open();
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
