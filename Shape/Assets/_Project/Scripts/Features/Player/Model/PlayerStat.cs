using UnityEngine;
using System;
using System.Collections.Generic;

public enum StatType
{
    MaxHp,
    MaxMp,
    Attack,
    Speed,
    CriticalProb,
    CriticalValue,
    DrainArea,

}

[Serializable]
public struct Stat
{
    public StatType type;  
    public float value;   
}

[DefaultExecutionOrder(-40)]
public class PlayerStat : MonoBehaviour
{
    public PlayerManager pm;
    [SerializeField] private List<Stat> _statList; // 스텟 인스펙터창에 띄울려고 만듦
    public List<Stat> StatList => _statList; // 읽기전용

    public Dictionary<StatType, float> stat = new Dictionary<StatType, float>();
    public IReadOnlyDictionary<StatType, float> Stat => stat;

    private void Start()
    {
        pm = PlayerManager.Instance;
    }


    // 스텟 dictionry => list 전환용 메서드
    private float GetStat(StatType type)
    {
        if (stat.TryGetValue(type, out float value))
            return value;
        return 0f;
    }

    public void SetStatList()
    {
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            int index = _statList.FindIndex(s => s.type == type);

            if (index == -1)
            {
                // 리스트에 해당 타입이 없으면 새로 추가
                _statList.Add(new Stat { type = type, value = GetStat(type) });
            }
            else
            {
                Stat temp = _statList[index];
                temp.value = GetStat(type); 
                _statList[index] = temp;            
            }
        }
    }
}
