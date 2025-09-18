using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    private void Start()
    {
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
}
