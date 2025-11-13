using UnityEngine;
using UnityEngine.UI;

public enum SkillType { Passive, Active, Utile }

[System.Serializable]
public struct SkillInstance
{
    public Skill skill;      // ScriptableObject(데이터)
    private float _nextReadyTime; // 런타임 상태 (플레이어별)

    public bool IsReady => Time.time >= _nextReadyTime;
    public float Remaining => Mathf.Max(0f, _nextReadyTime - Time.time);
    public float Normalized => (skill && skill.cooldown > 0f)
                               ? Remaining / skill.cooldown : 0f;

    public bool TryConsume() // 사용 시 쿨다운 시작
    {
        if (!IsReady || skill == null) return false;
        _nextReadyTime = Time.time + skill.cooldown;
        return true;
    }

    public void ForceReady() => _nextReadyTime = 0f; // 디버그/버프용
}

[CreateAssetMenu(fileName = "Skill", menuName = "Skill/Skill")]
public class Skill : ScriptableObject
{
    public SkillType skillType;
    public KeyCode key;
    public Sprite skillIcon;
    public string skillName;
    public float cooldown;
}
