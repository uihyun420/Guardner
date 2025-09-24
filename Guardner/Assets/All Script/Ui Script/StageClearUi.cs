
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class StageClearUi : GenericWindow
{
    [SerializeField] private Button retry;
    [SerializeField] private Button nextStage;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleUi battleUi;

    private void Start()
    {
        retry.onClick.AddListener(OnRetryButton);
        nextStage.onClick.AddListener(OnNextStageButton);
    }


    public override void Open()
    {
        base.Open();

        // UI가 열릴 때 참조 가져오기
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (stageManager == null)
            stageManager = FindObjectOfType<StageManager>();

        Time.timeScale = 0;
    }
    public override void Close()
    {
        base.Close();
    }

    public void OnNextStageButton()
    {
        Debug.Log("=== OnNextStageButton 시작 ===");

        if (stageManager == null || stageManager.stageData == null)
        {
            Debug.Log("StageManager 또는 StageData가 null입니다.");
            return;
        }

        // 스테이지 정보 가져오기
        int currentStage = stageManager.stageData.Stage;
        int nextStage = currentStage + 1;
        int nextStageId = FindNextStageId(nextStage);

        Debug.Log($"현재 스테이지: {currentStage}, 다음 스테이지: {nextStage}, 다음 스테이지 ID: {nextStageId}");

        if (nextStageId <= 0)
        {
            if (gameManager != null)
                gameManager.GameExit();
            return;
        }

        Time.timeScale = 1;

        battleUi.ResetBattleTimer();

        Close();

        // 스테이지 로드
        stageManager.LoadStage(nextStageId);

        // WindowManager를 통해 Battle UI로 전환
        if (manager != null)
        {
            manager.Open(WindowType.Battle);
        }
        stageManager.StartStage();
    }


    public void OnRetryButton()
    {
        if (stageManager == null || stageManager.stageData == null)
            return;

        int currentStageId = stageManager.stageData.ID;

        // 타임스케일 복원
        Time.timeScale = 1;

        // 스테이지 로드
        stageManager.LoadStage(currentStageId);

        // WindowManager를 통해 Battle UI로 전환
        if (manager != null)
        {
            manager.Open(WindowType.Battle);
        }

        // 스테이지 시작
        stageManager.StartStage();
    }

    // Stage 번호에 해당하는 스테이지 ID 찾기
    private int FindNextStageId(int stageNumber)
    {
        switch (stageNumber)
        {
            case 1: return 1630;
            case 2: return 2640;
            case 3: return 3650;
            case 4: return 4660;
            case 5: return 56100;
            case 6: return 65110;
            case 7: return 75120;
            case 8: return 85130;
            case 9: return 95140;
            case 10: return 105150;
            case 11: return 114160;
            case 12: return 124170;
            case 13: return 134180;
            case 14: return 144190;
            case 15: return 153200;
            default: return -1; // 존재하지 않는 스테이지
        }
    }
}
