using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private WindowManager windowManager;

    [SerializeField] private int startingStageId; // 기본값을 스테이지 1로 설정
    private void Start()
    {
        Time.timeScale = 1f;

        if (GameOverUi.RetryStageId > 0)
        {
            Debug.Log($"재시작 스테이지 ID: {GameOverUi.RetryStageId}");
            stageManager.LoadStage(GameOverUi.RetryStageId);
            GameOverUi.RetryStageId = 0; // 사용 후 초기화
        }
        else if (startingStageId > 0)
        {
            stageManager.LoadStage(startingStageId);
        }
    }

    public void StartGameStage(int stageId)
    {
        stageManager.LoadStage(stageId);
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Escape))
        //{
        //    GameExit();
        //}
    }

    public void OnStageClear()
    {
        SaveStageProgress();

        Time.timeScale = 0;
        windowManager.Open(WindowType.StageClear);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    private void SaveStageProgress()
    {
        int currentStage = stageManager.stageData.Stage;

        SaveLoadManager.Data.CurrentStage = currentStage;
        SaveLoadManager.Data.IsStageCleared = true;

        SaveLoadManager.UpdateStageProgress(currentStage);
        Debug.Log($"스테이지 {currentStage} 클리어 정보가 저장되었습니다.");
    }

}
