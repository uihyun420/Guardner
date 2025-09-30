using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillItemUi : MonoBehaviour
{
    public TextMeshProUGUI skillNameText;
    public Image skillIcon; // ��ų ������ �̹���
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
                .Append("��Ÿ�� : ").Append(skillData.CoolTime).Append("��");
            
            skillNameText.text = sb.ToString();
        }

        if (skillIcon != null)
        {
            // �����: �ε� �õ��ϴ� ��� Ȯ��
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

            // �̹� �Ҵ�� ��ų�̸� ��ư ��Ȱ��ȭ
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
