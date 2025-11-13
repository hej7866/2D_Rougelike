using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [Header("스킬 객체")]
    public SkillInstance skillInstance;
    
    [Header("스킬 정보 SO")]
    public Skill skill;
    public Image icon;
    void Awake()
    {
        InitSkillInstance();
    }

    void Start()
    {
        icon = GetComponentInChildren<Image>();
        icon.sprite = skill.skillIcon;
    }

    void InitSkillInstance()
    {
        skillInstance = new SkillInstance { skill = skill };
    }
}
