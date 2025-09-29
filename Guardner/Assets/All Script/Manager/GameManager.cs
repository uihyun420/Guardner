using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private WindowManager windowManager;

    [SerializeField] private int startingStageId; // 기본값을 스테이지 1로 설정
    private void Start()
    {
        Time.timeScale = 1f;
        int startingStageId = SaveLoadManager.GetStartingStageId();
        StartGameStage(startingStageId);
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
        Debug.Log("게임종료");
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
