// StatCalculator.cs
using UnityEngine;
using System;
using System.Collections.Generic;



[DefaultExecutionOrder(-50)]
public class StatCalculator : MonoBehaviour
{
    public PlayerManager pm;
    private Dictionary<StatType, float> _stat;

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
        _stat = pm.playerStat.stat; // 기존 스텟 딕셔너리를 가져온다.

        if (pm.character == null || pm.levelSystem == null)
        {
            Debug.LogWarning("StatCalculator: character/levelSystem null");
            return;
        }

        // 1. 캐릭터 디폴트 스탯 임시 변수에 담고 
        _stat[StatType.MaxHp] = pm.character.baseHp;
        _stat[StatType.MaxMp] = pm.character.baseMp;
        _stat[StatType.Speed] = pm.character.baseSpeed;
        _stat[StatType.Attack] = pm.character.baseAttack;
        _stat[StatType.CriticalProb] = pm.character.CriticalProb;
        _stat[StatType.CriticalValue] = pm.character.CriticalValue;
        _stat[StatType.DrainArea] = pm.character.drainArea;

        Modifier();
    } 

    /// <summary>
    /// 스탯 변화시 계산용 메서드
    /// </summary>
    public void CalculateOnLevelUp(LevelSystem levelSystem)
    {
        _stat = pm.playerStat.stat; // 기존 스텟 딕셔너리를 가져온다.

        int L = Mathf.Max(1, levelSystem.Level);
        int idx = L - 1; // 커브 입력용

        // 1) 성장테이블 기반 스펙업 레벨 디자인 이후 수정
        _stat[StatType.MaxHp] += 10f;
        _stat[StatType.MaxMp] += 10f;
        _stat[StatType.Speed] += 0.1f;
        _stat[StatType.Attack] += +2f;

        Modifier();
    }
    
    public void CalculateOnSelecetAgument(AgumentData agumentData)
    {
        _stat = pm.playerStat.stat; // 기존 스텟 딕셔너리를 가져온다.
        StatType statType = agumentData.agument.statType;
        OperationType operationType = agumentData.agument.operationType;
        float value = agumentData.agument.value;

        switch(operationType)
        {
            case OperationType.add:
                _stat[statType] += value;
                break;
            case OperationType.mul:
                _stat[statType] *= (1 + value);
                break;
        }

        Modifier();
    }
    #endregion
}
