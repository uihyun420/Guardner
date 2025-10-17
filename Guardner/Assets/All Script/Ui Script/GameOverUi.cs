using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUi : GenericWindow
{
    public static WindowType NextWindowAfterLoad = WindowType.MainMenuUi;
    public static int RetryStageId = 0;

    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private GameObject spawnRect;
    
    [SerializeField] private StageManager StageManager;
    [SerializeField] private GameManager gameManager;
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
        SoundManager.soundManager.PlaySFX("GameOverSfx");

    }
    public override void Close()
    {
        base.Close();
        spawnRect.SetActive(true);
    }

    public void OnClickRetryButton()
    {
        SoundManager.soundManager.PlaySFX("UiClick2Sfx");
        // 재시작 시 현재 스테이지 ID를 정적 변수에 저장
        RetryStageId = StageManager.stageData.ID;
        // 씬 로드 후 Battle UI를 열도록 설정
        NextWindowAfterLoad = WindowType.Battle;

        Time.timeScale = 1;
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickMainMenuButton()
    {
        SoundManager.soundManager.PlaySFX("UiClick2Sfx");
        NextWindowAfterLoad = WindowType.MainMenuUi;
        RetryStageId = 0; // 메인 메뉴로 갈 때는 재시작 ID 초기화

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
