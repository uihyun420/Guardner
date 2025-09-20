using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillItemUi : MonoBehaviour
{
    public TextMeshProUGUI skillNameText;
    //public Image skillIcon; // ��ų ������ �̹���
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
        //    // �����δ� Resources�� Addressable�� ��ų ������ �ε�
        //    // skillIcon.sprite = Resources.Load<Sprite>($"SkillIcons/{skillData.GardenerSkillDrawId}");
        //}

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
