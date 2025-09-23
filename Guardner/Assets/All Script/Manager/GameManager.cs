using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private WindowManager windowManager;

    [SerializeField] private int startingStageId = 1630; // �⺻���� �������� 1�� ����
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
        Debug.Log("�������� Ŭ����");
    }

    public void GameExit()
    {
        Application.Quit();
        Debug.Log("��������");
    }

}
