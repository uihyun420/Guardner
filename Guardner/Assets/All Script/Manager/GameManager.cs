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
            Debug.Log($"스테이지 {stageId}로드 실패");
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
        Debug.Log("스테이지 클리어");
    }

    public void GameExit()
    {
        Application.Quit();
        Debug.Log("게임종료");
    }

}
