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
    [SerializeField] private MainMenuUi mainMenuUi;
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private GuardnerSpawner guardnerSpawner;


    private int retryStageId;
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
        //string sceneName = SceneManager.GetActiveScene().name;
        //SceneManager.LoadScene(sceneName);

        //int currentStagId = StageManager.stageData.ID;

        //Time.timeScale = 1;

        //battleUi.ResetBattleTimer();

        //Close();

        //StageManager.LoadStage(currentStagId);

        //if (manager != null)
        //{
        //    manager.Open(WindowType.Battle);
        //}

        //StageManager.StartStage();
        Time.timeScale = 1;
        retryStageId = StageManager.stageData.ID;

        SceneManager.sceneLoaded += OnSceneLoaded;
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        WindowManager newWindowManager = FindObjectOfType<WindowManager>();
        StageManager newStageManager = FindObjectOfType<StageManager>();
        BattleUi newBattleUi = FindObjectOfType<BattleUi>();
        MainMenuUi newMainMenuUi = FindObjectOfType<MainMenuUi>();
        if(newMainMenuUi != null)
        {
            newMainMenuUi.gameObject.SetActive(false);
        }

        if (newStageManager != null && newBattleUi != null)
        {
            newStageManager.LoadStage(retryStageId);
            newBattleUi.ResetBattleTimer();           
        }

        if (newWindowManager != null)
        {
            newWindowManager.Open(WindowType.Battle);
        }
        // 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void OnMainMenuButton()
    {
        Time.timeScale = 1;
        Close();
        StageManager.StageStop();
        monsterSpawner.ClearMonster();
        guardnerSpawner.ClearGuardner();

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
