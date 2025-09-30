using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillItemUi : MonoBehaviour
{
    public TextMeshProUGUI skillNameText;
    public Image skillIcon; // 스킬 아이콘 이미지
    public Button selectButton;
    StringBuilder sb = new StringBuilder();

    private int skillId;

    public void SetData(PlayerSkillData skillData, System.Action<int> onSelect, bool isAlreadyAssigned)
    {
        skillId = skillData.Id;
        

        if (skillNameText != null)
        {
            sb.Clear();
            sb.Append(skillData.Name).Append("\n").Append(skillData.SkillDescription).Append("\n")
                .Append("쿨타임 : ").Append(skillData.CoolTime).Append("초");
            
            skillNameText.text = sb.ToString();
        }

        if (skillIcon != null)
        {
            // 디버그: 로드 시도하는 경로 확인
            string imagePath = $"SkillIcons/skill_{skillData.Id}";

            var sprite = Resources.Load<Sprite>(imagePath);

            if (sprite != null)
            {
                skillIcon.sprite = sprite;
                skillIcon.gameObject.SetActive(true);
            }
            else
            {
                skillIcon.gameObject.SetActive(false);
            }
        }


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
