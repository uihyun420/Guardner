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

        // GameOverUi에서 지정한 창을 사용
        currentWindow = GameOverUi.NextWindowAfterLoad;
        // 기본값으로 재설정 (다음 씬 로드를 위해)
        GameOverUi.NextWindowAfterLoad = defaultWindow;

        windows[(int)currentWindow].Open();

        if(currentWindow == WindowType.Battle && GameOverUi.RetryStageId > 0)
        {
            if(stageManager != null)
            {
                stageManager.LoadStage(GameOverUi.RetryStageId);

                GameOverUi.RetryStageId = 0; // 사용후 초기화
            }
        }
        // 도어 이벤트 구독
        DoorBehavior.OnDoorDestroyed += ShowGameOver;
    }

    public void OpenOverlay(WindowType id)
    {
        // 이미 같은 타입의 오버레이가 열려있으면 열지 않음
        if (windows[(int)id].gameObject.activeSelf)
            return;

        // 현재 창은 닫지 않고 새 창만 열기
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
        // 이벤트 해제
        DoorBehavior.OnDoorDestroyed -= ShowGameOver;
    }

    private void ShowGameOver()
    {
        Time.timeScale = 0;
        Open(WindowType.GameOver);
    }
}
