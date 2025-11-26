// StatCalculator.cs
using UnityEngine;
using System;
using System.Collections.Generic;



[DefaultExecutionOrder(-50)]
public class StatCalculator : MonoBehaviour
{
    public PlayerManager pm;
    private Dictionary<StatType, float> _stat;
    private Dictionary<StatType, float> _baseStat;

    public event Action OnCalculate;
    public event Action<Dictionary<StatType, float>> OnStatChanged;


    private void Start()
    {
        pm = PlayerManager.Instance;

        OnCalculate += pm.playerStat.SetStatList;
        OnStatChanged += pm.drain.ChangeCircleSize;
        OnStatChanged += pm.playerController.OnApplyVital;
        OnStatChanged += UIManager.Instance.playerView.UpdateUIOnChangePlayerStat;
    }

    void Modifier()
    {
        OnCalculate.Invoke();
        OnStatChanged.Invoke(_stat);
    }

    /// <summary>
    /// 캐릭터 기본 스펙 세팅용 메서드
    /// </summary>
    #region  Calculator
    public void DefaultCalculate()
    {
        _baseStat = pm.playerStat.baseStat; // 기존 스텟 딕셔너리를 가져온다.

        if (pm.character == null || pm.levelSystem == null)
        {
            Debug.LogWarning("StatCalculator: character/levelSystem null");
            return;
        }

        // 1. 캐릭터 디폴트 스탯 임시 변수에 담고 
        _baseStat[StatType.MaxHp] = pm.character.baseHp;
        _baseStat[StatType.MaxMp] = pm.character.baseMp;
        _baseStat[StatType.MaxStamina] = pm.character.baseStamina;
        _baseStat[StatType.Speed] = pm.character.baseSpeed;
        _baseStat[StatType.Attack] = pm.character.baseAttack;
        _baseStat[StatType.CriticalProb] = pm.character.CriticalProb;
        _baseStat[StatType.CriticalValue] = pm.character.CriticalValue;
        _baseStat[StatType.DrainArea] = pm.character.drainArea;

        Recalculate(ShapeGrowthManager.Instance.shapeGrowth.shapeGrowthDic);
    } 

    /// <summary>
    /// 스탯 변화시 계산용 메서드
    /// </summary>
    public void CalculateOnLevelUp(LevelSystem levelSystem)
    {
        _baseStat = pm.playerStat.baseStat; // 기존 스텟 딕셔너리를 가져온다.

        int L = Mathf.Max(1, levelSystem.Level);
        int idx = L - 1; // 커브 입력용

        // 1) 성장테이블 기반 스펙업 레벨 디자인 이후 수정
        _baseStat[StatType.MaxHp] += 10f;
        _baseStat[StatType.MaxMp] += 10f;
        _baseStat[StatType.MaxStamina] += 10f;
        _baseStat[StatType.Speed] += 0.1f;
        _baseStat[StatType.Attack] += +2f;

        Recalculate(ShapeGrowthManager.Instance.shapeGrowth.shapeGrowthDic);
    }
    
    public void CalculateOnSelecetAgument(AgumentData agumentData)
    {
        _baseStat = pm.playerStat.baseStat; // 기존 스텟 딕셔너리를 가져온다.
        StatType statType = agumentData.agument.statType;
        OperationType operationType = agumentData.agument.operationType;
        float value = agumentData.agument.value;

        switch(operationType)
        {
            case OperationType.add:
                _baseStat[statType] += value;
                break;
            case OperationType.mul:
                _baseStat[statType] *= (1 + value);
                break;
        }

        Recalculate(ShapeGrowthManager.Instance.shapeGrowth.shapeGrowthDic);
    }

    public void Recalculate(Dictionary<StatType, int> shapeGrowthDic)
    {
        _baseStat = pm.playerStat.baseStat;
        _stat = pm.playerStat.stat; 

        foreach (var kv in _baseStat)
        {
            StatType type = kv.Key;
            float baseValue = kv.Value;

            int lv = 0;
            if (shapeGrowthDic != null && shapeGrowthDic.TryGetValue(type, out var v))
                lv = v; // 0~5강

            float factor = 1f + 0.1f * lv; // 예: 1강당 +10%
            if(type != StatType.CriticalProb)
                _stat[type] = baseValue * factor;
            else if(type == StatType.CriticalProb) // 치명타 확률은 20%씩 증가하는걸로 왜냐하면 치명타확률은 Shape 성장시스템으로 밖에 얻지못함. 그리고 애초에 0이기때문에 baseValue * factor 안됨
                _stat[type] = 20f * lv;
        }

        Modifier(); // 이벤트 쏴서 UI, HP 등 갱신
    }
    #endregion
}
