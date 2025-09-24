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
        Debug.Log("OnStageClear ȣ���");
        Time.timeScale = 0;
        Debug.Log($"WindowManager�� null�ΰ�? {windowManager == null}");
        Debug.Log("WindowType.StageClear ȣ�� ����");
        windowManager.Open(WindowType.StageClear);
        Debug.Log("WindowType.StageClear ȣ�� �Ϸ�");
        Debug.Log("�������� Ŭ����");
    }

    public void GameExit()
    {
        Application.Quit();
        Debug.Log("��������");
    }

}
