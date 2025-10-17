using UnityEngine;
using System.Collections.Generic;

public class WindowManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private List<GenericWindow> windows;
    [SerializeField] private StageManager stageManager;

    private WindowType defaultWindow;
    public WindowType currentWindow { get; private set; }

    private void Start()
    {
        foreach (var window in windows)
        {
            window.Init(this);
            window.gameObject.SetActive(false);
        }

        // GameOverUi���� ������ â�� ���
        currentWindow = GameOverUi.NextWindowAfterLoad;
        // �⺻������ �缳�� (���� �� �ε带 ����)
        GameOverUi.NextWindowAfterLoad = defaultWindow;

        windows[(int)currentWindow].Open();

        if(currentWindow == WindowType.Battle && GameOverUi.RetryStageId > 0)
        {
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
        windows[(int)currentWindow].Close();
        currentWindow = id;
        windows[(int)currentWindow].gameObject.SetActive(true);
        windows[(int)currentWindow].Open();
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
