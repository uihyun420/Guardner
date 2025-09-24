using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUi : GenericWindow
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI stageText;

    [SerializeField] private StageManager StageManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleUi battleUi;
    [SerializeField] private MonsterSpawner monsterSpawner;

    private StringBuilder sb = new StringBuilder();
    private void Start()
    {
        retryButton.onClick.AddListener(OnClickRetryButton);
        mainMenuButton.onClick.AddListener(OnMainMenuButton);
    }

    private void Update()
    {
        SetGameOverStageText();
    }
    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();
    }

    public void OnClickRetryButton()
    {
        int currentStagId = StageManager.stageData.ID;

        Time.timeScale = 1;

        battleUi.ResetBattleTimer();

        Close();

        StageManager.LoadStage(currentStagId);

        if (manager != null)
        {
            manager.Open(WindowType.Battle);
        }

        StageManager.StartStage();
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1;

        Close();

        StageManager.StageStop();
        monsterSpawner.ClearMonster();

        if (manager != null)
        {
            manager.Open(WindowType.MainMenuUi);
        }
    }

    private void SetGameOverStageText()
    {        
        
        sb.Clear();
        sb.Append("스테이지 LEVEL ").Append(StageManager.stageData.Stage);
        stageText.text = sb.ToString();
        
    }
}
