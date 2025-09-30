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

    private void Update()
    {
        SetGoldText();
        SetGuardnerText();
    }

    private void Start()
    {
        closeButton.onClick.AddListener(OnClickCloseButton);
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
    }

    private void SetGuardnerText()
    {
        var sb = new StringBuilder();
        sb.Clear();
        sb.Append("정원사 ").Append("(").Append(guardnerSpawner.ownedGuardnerIds.Count).Append(" / ").Append(maxGuardnerCount).Append(")");
        guardnerCountText.text = sb.ToString();
    }

    private void InitializeOwnedGuardenr()
    {
        // 나중에는 세이브 데이터에서 로드
        ownedGuardnerIds.Add(11120);
        ownedGuardnerIds.Add(11125);
        ownedGuardnerIds.Add(11135);
        ownedGuardnerIds.Add(11235);
        ownedGuardnerIds.Add(11245);
        ownedGuardnerIds.Add(11250);
        ownedGuardnerIds.Add(11255);
        ownedGuardnerIds.Add(11260);
        ownedGuardnerIds.Add(11270);
        ownedGuardnerIds.Add(11310);   
        ownedGuardnerIds.Add(11315);
        ownedGuardnerIds.Add(11320);
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

        if(itemUi != null)
        {
            itemUi.DictionarySetData(data, null);
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
