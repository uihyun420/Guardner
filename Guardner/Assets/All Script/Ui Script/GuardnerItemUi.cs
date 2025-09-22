using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuardnerItemUi : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Image guardnerImage; // 스킬 아이콘 이미지
    public Button selectButton;

    private int guardnerId;

    public void SetData(GuardnerData data, System.Action<int> onSelect)
    {
        guardnerId = data.Id;

        if (infoText != null)
        {
            infoText.text = $"{data.Name}\n공격력: {data.AttackPower}\n골드: {data.SummonGold}";
        }

        if(guardnerImage != null)
        {
            string imagePath = $"GuardnerIcons/guardner_{data.GuardenerDrawId}";
            Debug.Log($"이미지 로드 경로: {imagePath}");
            var sprite = Resources.Load<Sprite>(imagePath);

            if(sprite != null)
            {
                guardnerImage.sprite = sprite;
                guardnerImage.gameObject.SetActive(true);
                Debug.Log("이미지 로드 성공");
            }
            else
            {
                guardnerImage.gameObject.SetActive(false);
                Debug.LogWarning("이미지 로드 실패: 파일이 없거나 경로가 잘못됨");
            }
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelect(data.Id));
        }
    }
}
