using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private WindowManager windowManager;

    [SerializeField] private int startingStageId = 1630; // 기본값을 스테이지 1로 설정
    private void Start()
    {
        Time.timeScale = 1f;
        StartGameStage(startingStageId);
    }

    private void StartGameStage(int stageId)
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
