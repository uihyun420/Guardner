using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MainMenuUi : GenericWindow
{
    [SerializeField] private Button battleStartButton;
    [SerializeField] private Button DictionaryButton;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BattleUi battleUi;
    [SerializeField] private DictionaryUi dictionaryUi;
    [SerializeField] private InventoryUi inventoryUi;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button EnhanceButton;
    [SerializeField] private GuardnerEnhanceUi guardnerEnhanceUi;
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private GameObject tilemap;
    [SerializeField] private GameObject spawnRect;

    public int mainUiGold
    {
        get => SaveLoadManager.Data.Gold;
        set => SaveLoadManager.Data.Gold = value;
    }

    private WindowManager windowManager;

    private void Start()
    {
        battleStartButton.onClick.AddListener(OnBattleStartButton);
        DictionaryButton.onClick.AddListener(OnClickDictionaryButton);
        EnhanceButton.onClick.AddListener(OnClickEnhanceButton);
        inventoryButton.onClick.AddListener(OnClickInventoryButton);

        SetMainUiGoldText();
    }
    private void Update()
    {
        SetMainUiGoldText();
    }

    public override void Open()
    {
        base.Open();
        tilemap.SetActive(false);
        spawnRect.SetActive(false);

        SetMainUiGoldText();
    }

    public override void Close()
    {
        base.Close();
        tilemap.SetActive(true);
        spawnRect.SetActive(true);
    }

    private void OnBattleStartButton()
    {
        Close();
        int startingStageId = SaveLoadManager.GetStartingStageId();
        gameManager.StartGameStage(startingStageId);

        if (windowManager != null)
        {
            windowManager.Open(WindowType.Battle);
        }
        else if (battleUi != null)
        {
            battleUi.Open();
        }
    }

    private void OnClickDictionaryButton()
    {
        dictionaryUi.Open();
    }

    public void AddMainUiGold(int amount)
    {
        mainUiGold += amount;
    }
    public void SetMainUiGoldText()
    {
        var sb = new StringBuilder();
        sb.Append(mainUiGold).Append(" G");
        goldText.text = sb.ToString();
    }
    public void ResetMainUiGold()
    {
        mainUiGold = 0;
    }

    public void OnClickEnhanceButton()
    {
        guardnerEnhanceUi.Open();
    }

    public void OnClickInventoryButton()
    {
        inventoryUi.Open();
    }
}
