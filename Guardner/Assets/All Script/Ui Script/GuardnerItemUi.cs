using System.Collections;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GuardnerItemUi : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Image guardnerImage;
    public Button selectButton;


    [SerializeField] private TextMeshProUGUI guardnerSpwawnDiscriptText;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private TextMeshProUGUI dictionaryName;
    [SerializeField] private TextMeshProUGUI dictionaryDiscription;

    public GuardnerData Data { get; private set; }

    private int guardnerId;

    public void SetNameTextColor(Color color)
    {
        nameText.color = color;
        nameText.ForceMeshUpdate();
    }


    public void SetData(GuardnerData data, System.Action<int> onSelect)
    {
        guardnerId = data.Id;
        Data = data;

        nameText.text = data.Name;

        var enhancedData = SaveLoadManager.GetGuardnerStats(data.Id.ToString());
        int attackPower = enhancedData?.AttackPower ?? data.AttackPower;


        guardnerSpwawnDiscriptText.text =
            $"���ݷ�: {attackPower}\n" +
            $"���ݼӵ�: {data.APS}\n" +
            $"��ȯ ���: {data.SummonGold}";


        if (enhancedData != null)
            infoText.text = $"{data.Name} Lv.{enhancedData.Level}\n���ݷ�: {attackPower} (+{attackPower - data.AttackPower})\n���: {data.SummonGold}";
        else
            infoText.text = $"{data.Name}\n���ݷ�: {data.AttackPower}\n���: {data.SummonGold}";


        // 6. �̹���
        string imagePath = $"GuardnerIcons/guardner_{data.GuardenerDrawId}";
        var sprite = Resources.Load<Sprite>(imagePath);
        guardnerImage.sprite = sprite;
        guardnerImage.gameObject.SetActive(sprite != null);


        // 7. ��ư
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelect(data.Id));
    }


    public void SetTextColor(Color color)
    {
        infoText.color = color;
        infoText.ForceMeshUpdate();
    }

    public void DictionarySetData(GuardnerData data, System.Action<int> onSelect)
    {
        guardnerId = data.Id;


        var nameBuilder = new StringBuilder();
        nameBuilder.Append(data.Name);
        dictionaryName.text = nameBuilder.ToString();


        if (dictionaryDiscription != null)
        {
            var descBuilder = new StringBuilder();
            descBuilder.Append(data.Reference).Append("\n").Append("���ݷ�: ").Append(data.AttackPower);
            dictionaryDiscription.text = descBuilder.ToString();
        }

        if (guardnerImage != null)
        {
            string imagePath = $"GuardnerIcons/guardner_{data.GuardenerDrawId}";
            var sprite = Resources.Load<Sprite>(imagePath);

            if (sprite != null)
            {
                guardnerImage.sprite = sprite;
                guardnerImage.gameObject.SetActive(true);
            }
            else
            {
                guardnerImage.gameObject.SetActive(false);
            }
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelect(data.Id));
        }
    }
}
