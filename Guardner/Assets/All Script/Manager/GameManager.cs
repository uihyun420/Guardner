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
        Debug.Log("OnStageClear 호출됨");
        Time.timeScale = 0;
        Debug.Log($"WindowManager가 null인가? {windowManager == null}");
        Debug.Log("WindowType.StageClear 호출 시작");
        windowManager.Open(WindowType.StageClear);
        Debug.Log("WindowType.StageClear 호출 완료");
        Debug.Log("스테이지 클리어");
    }

    public void GameExit()
    {
        Application.Quit();
        Debug.Log("게임종료");
    }

}
