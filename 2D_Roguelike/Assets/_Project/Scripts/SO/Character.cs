using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName="Characters/character")]
public class Character : ScriptableObject
{
    public int characterId;

    [Header("캐릭터 스프라이트")]
    public Sprite sprite;

    [Header("캐릭터 전투스펙")]
    public float baseHp;
    public float baseMp;
    public float baseSpeed;
    public float baseAttack;

    [Header("고정 스펙(어떤 캐릭이든 같음)")]
    public float drainArea = 1.5f;


    [Header("평타 OBJ")]
    public GameObject aaObj;

    [Header("스킬 셋")]
    public Skill D_Skill;
    public Skill Q_Skill;
    public Skill W_Skill;
    public Skill E_Skill;
    public Skill R_Skill;

    [Header("무기")]
    public Weapon weapon;
}
