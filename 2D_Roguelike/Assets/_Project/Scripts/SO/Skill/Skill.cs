using UnityEngine;
using UnityEngine.UI;
using System;

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
        if(PlayerManager.Instance.playerController.Mp < skill.manaCost) return false; // 마나 부족하면 리턴

        if (!IsReady || skill == null) return false;
        _nextReadyTime = Time.time + skill.cooldown;
        return true;
    }

    public void ForceReady() => _nextReadyTime = 0f; // 디버그/버프용
}

[Serializable]
public struct SkillDefinition
{
    public SkillType skillType;
    public KeyCode key;
    public Sprite skillIcon;
    public string skillName;
    public int id;
}

[CreateAssetMenu(fileName = "Skill", menuName = "Skills/Skill")]
public class Skill  : ScriptableObject
{
    public SkillDefinition skillDefinition;
    public float cooldown;
    public float manaCost;


    [Serializable]
    public class ActionEntry
    {
        public SkillAction action;                           // 어떤 액션을 쓸지
        [SerializeReference] public ActionParams parameters; // 액션별 파라미터(다형 직렬화)
    }

    public ActionEntry[] actions;                            // 리스트/배열 타입이 이 엔트리여야 함

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (actions == null) return;

        foreach (var e in actions)
        {
            if (e == null || e.action == null) continue;

            var pType = e.action.ParamsType;
            if (e.parameters == null || e.parameters.GetType() != pType)
            {
                e.parameters = e.action.CreateDefaultParams();    // 자동 생성
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
    }
#endif
}

