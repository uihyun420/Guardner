using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private WindowManager windowManager;

    [SerializeField] private int startingStageId; // �⺻���� �������� 1�� ����
    private void Start()
    {
        Time.timeScale = 1f;
        //int startingStageId = SaveLoadManager.GetStartingStageId();
        //StartGameStage(startingStageId);
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
        Debug.Log($"�������� {currentStage} Ŭ���� ������ ����Ǿ����ϴ�.");
        //�������� �����Ͱ� �ε���� �ʾҽ��ϴ�.
        //UnityEngine.Debug:Log(object)
        //StageManager: StartStage()(at Assets / All Script / Manager / StageManager.cs:66)
        //BattleUi: Update()(at Assets / All Script / Ui Script / BattleUi.cs:196)
    }

}
