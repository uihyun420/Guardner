using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private WindowManager windowManager;

    [SerializeField] private int startingStageId; // �⺻���� �������� 1�� ����
    private void Start()
    {
        Time.timeScale = 1f;

        if (GameOverUi.RetryStageId > 0)
        {
            Debug.Log($"����� �������� ID: {GameOverUi.RetryStageId}");
            stageManager.LoadStage(GameOverUi.RetryStageId);
            GameOverUi.RetryStageId = 0; // ��� �� �ʱ�ȭ
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
        Debug.Log($"�������� {currentStage} Ŭ���� ������ ����Ǿ����ϴ�.");
    }

}
