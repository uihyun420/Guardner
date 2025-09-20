using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private WindowManager windowManager;
    private void Start()
    {
        Time.timeScale = 1f;
        StartGameStage(1630);
    }

    private void StartGameStage(int stageId)
    {
        stageManager.LoadStage(stageId);
        if(stageManager.stageData !=null)
        {
            stageManager.StartStage();
        }
        else
        {
            Debug.Log($"�������� {stageId}�ε� ����");
        }
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
        Time.timeScale = 0;
        windowManager.Open(WindowType.StageClear);
        Debug.Log("�������� Ŭ����");
    }

    public void GameExit()
    {
        Application.Quit();
        Debug.Log("��������");
    }

}
