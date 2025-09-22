using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuardnerItemUi : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Image guardnerImage; // ��ų ������ �̹���
    public Button selectButton;

    private int guardnerId;

    public void SetData(GuardnerData data, System.Action<int> onSelect)
    {
        guardnerId = data.Id;

        if (infoText != null)
        {
            infoText.text = $"{data.Name}\n���ݷ�: {data.AttackPower}\n���: {data.SummonGold}";
        }

        if(guardnerImage != null)
        {
            string imagePath = $"GuardnerIcons/guardner_{data.GuardenerDrawId}";
            Debug.Log($"�̹��� �ε� ���: {imagePath}");
            var sprite = Resources.Load<Sprite>(imagePath);

            if(sprite != null)
            {
                guardnerImage.sprite = sprite;
                guardnerImage.gameObject.SetActive(true);
                Debug.Log("�̹��� �ε� ����");
            }
            else
            {
                guardnerImage.gameObject.SetActive(false);
                Debug.LogWarning("�̹��� �ε� ����: ������ ���ų� ��ΰ� �߸���");
            }
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelect(data.Id));
        }
    }
}
