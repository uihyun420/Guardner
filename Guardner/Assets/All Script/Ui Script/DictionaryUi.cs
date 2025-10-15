using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryUi : GenericWindow
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject dictionarySlot;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GuardnerSpawner guardnerSpawner; // 참조 추가

    [SerializeField] private Button closeButton;

    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private MainMenuUi mainMenu;

    [SerializeField] private TextMeshProUGUI guardnerCountText;    
    private int maxGuardnerCount = 12;

    private HashSet<int> ownedGuardnerIds = new HashSet<int>(); // 가지고있는 가드너 목록


    [SerializeField] private DictionaryInfoUi dictionaryInfoUi;
    [SerializeField] private Button dictionaryInfoUiButton;
    [SerializeField] private WindowManager windowManager;

    [SerializeField] private Button setButton;
    [SerializeField] private DictionarySetUi dictionarySetUi;
    private void Update()
    {
        SetGoldText();
        SetGuardnerText();
    }

    private void Start()
    {
        closeButton.onClick.AddListener(OnClickCloseButton);
        setButton.onClick.AddListener(OnClickSetButton);
    }

    public override void Open()
    {
        base.Open();
        DisplayOwnedGuardners();
    }

    public override void Close()
    {
        base.Close();
    }

    public void OnClickCloseButton()
    {
        Close();
        SoundManager.soundManager.PlaySFX("UiClickSfx");
    }

    private void SetGuardnerText()
    {
        var sb = new StringBuilder();
        sb.Clear();
        sb.Append("정원사 ").Append("(").Append(guardnerSpawner.ownedGuardnerIds.Count).Append(" / ").Append(maxGuardnerCount).Append(")");
        guardnerCountText.text = sb.ToString();
    }

    private void OnClickSetButton()
    {
        // 직접 열기 방식으로 되돌리기
        if (dictionarySetUi != null)
        {
            dictionarySetUi.Open();
        }

        SoundManager.soundManager.PlaySfxButton1();
    }
    private void DisplayOwnedGuardners()
    {
        foreach (Transform guardner in contentParent)
        {
            Destroy(guardner.gameObject);
        }

        if (guardnerSpawner != null)
        {
            foreach (var guardnerId in guardnerSpawner.ownedGuardnerIds)
            {
                var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
                if (guardnerData != null)
                {
                    CreationDictionarySlot(guardnerData);
                }
            }
        }

        //스크롤 설정
        if (scrollRect != null)
        {
            var contentHeight = scrollRect.content.rect.height;
            var viewportHeight = scrollRect.viewport.rect.height;
            scrollRect.vertical = contentHeight > viewportHeight;
        }
    }

    private void CreationDictionarySlot(GuardnerData data)
    {
        var slot = Instantiate(dictionarySlot, contentParent);
        var itemUi = slot.GetComponent<GuardnerItemUi>();

        if (itemUi != null)
        {
            // 클릭 시 dictionaryInfoUi를 열도록 이벤트 연결
            itemUi.DictionarySetData(data, (id) =>
            {
                dictionaryInfoUi.SetGuardnerInfo(data);
                dictionaryInfoUi.Open();
                SoundManager.soundManager.PlaySFX("UiClick2Sfx");
            });
        }
    }

    // 새 가드너 추가 (GuardnerSpawner에서 호출됨)
    public void AddGuardnerToCollection(int guardnerId)
    {
        // 이미 UI가 열려있다면 즉시 새로고침
        if (gameObject.activeInHierarchy)
        {
            DisplayOwnedGuardners();
        }
    }

    //가드너 가지고 있는지 확인
    public bool HasGuardner(int guardnerId)
    {
        return ownedGuardnerIds.Contains(guardnerId);
    }

    private void SetGoldText()
    {
        goldText.text = $"{mainMenu.mainUiGold}";
    }

}
