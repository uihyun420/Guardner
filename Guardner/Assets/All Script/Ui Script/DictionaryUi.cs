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
    [SerializeField] private GuardnerSpawner guardnerSpawner; // ���� �߰�

    [SerializeField] private Button closeButton;

    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private MainMenuUi mainMenu;

    [SerializeField] private TextMeshProUGUI guardnerCountText;    
    private int maxGuardnerCount = 12;

    private HashSet<int> ownedGuardnerIds = new HashSet<int>(); // �������ִ� ����� ���


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
        sb.Append("������ ").Append("(").Append(guardnerSpawner.ownedGuardnerIds.Count).Append(" / ").Append(maxGuardnerCount).Append(")");
        guardnerCountText.text = sb.ToString();
    }

    private void OnClickSetButton()
    {
        // ���� ���� ������� �ǵ�����
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

        //��ũ�� ����
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
            // Ŭ�� �� dictionaryInfoUi�� ������ �̺�Ʈ ����
            itemUi.DictionarySetData(data, (id) =>
            {
                dictionaryInfoUi.SetGuardnerInfo(data);
                dictionaryInfoUi.Open();
                SoundManager.soundManager.PlaySFX("UiClick2Sfx");
            });
        }
    }

    // �� ����� �߰� (GuardnerSpawner���� ȣ���)
    public void AddGuardnerToCollection(int guardnerId)
    {
        // �̹� UI�� �����ִٸ� ��� ���ΰ�ħ
        if (gameObject.activeInHierarchy)
        {
            DisplayOwnedGuardners();
        }
    }

    //����� ������ �ִ��� Ȯ��
    public bool HasGuardner(int guardnerId)
    {
        return ownedGuardnerIds.Contains(guardnerId);
    }

    private void SetGoldText()
    {
        goldText.text = $"{mainMenu.mainUiGold}";
    }

}
