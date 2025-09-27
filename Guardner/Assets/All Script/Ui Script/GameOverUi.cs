using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUi : GenericWindow
{
    // 씬 로드 후 열릴 UI 타입을 저장하는 정적 변수
    public static WindowType NextWindowAfterLoad = WindowType.MainMenuUi;
    public static int RetryStageId = 0;

    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private GameObject spawnRect;
    
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
        mainMenuButton.onClick.AddListener(OnClickMainMenuButton);
    }

    private void Update()
    {
        SetGameOverStageText();
    }
    public override void Open()
    {
        base.Open();
        spawnRect.SetActive(false);

    }
    public override void Close()
    {
        base.Close();
        spawnRect.SetActive(true);
    }

    public void OnClickRetryButton()
    {
        // 재시작 시 현재 스테이지 ID 저장
        retryStageId = StageManager.stageData.ID;
        // 씬 로드 후 Battle UI를 열도록 설정
        NextWindowAfterLoad = WindowType.Battle;

        Time.timeScale = 1;
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickMainMenuButton()
    {
        NextWindowAfterLoad = WindowType.MainMenuUi;
        RetryStageId = 0;

        Time.timeScale = 1;
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    private void SetGameOverStageText()
    {
        sb.Clear();
        sb.Append("STAGE LEVEL ").Append(StageManager.stageData.Stage);
        stageText.text = sb.ToString();
    }
}
