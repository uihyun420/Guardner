using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillItemUi : MonoBehaviour
{
    public TextMeshProUGUI skillNameText;
    //public Image skillIcon; // 스킬 아이콘 이미지
    public Button selectButton;

    private int skillId;

    public void SetData(PlayerSkillData skillData, System.Action<int> onSelect, bool isAlreadyAssigned)
    {
        skillId = skillData.Id;

        if (skillNameText != null)
        {
            skillNameText.text = skillData.Name;
        }

        //if (skillIcon != null)
        //{
        //    // 실제로는 Resources나 Addressable로 스킬 아이콘 로드
        //    // skillIcon.sprite = Resources.Load<Sprite>($"SkillIcons/{skillData.GardenerSkillDrawId}");
        //}

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();

            // 이미 할당된 스킬이면 버튼 비활성화
            if (isAlreadyAssigned)
            {
                selectButton.interactable = false;
                selectButton.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                selectButton.interactable = true;
                selectButton.onClick.AddListener(() => onSelect(skillId));
            }
        }
    }
}
