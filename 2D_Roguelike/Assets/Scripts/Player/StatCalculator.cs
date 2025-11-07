// StatCalculator.cs
using UnityEngine;
using System;



[DefaultExecutionOrder(-50)]
public class StatCalculator : MonoBehaviour
{
    public PlayerManager pm;

    [Header("계산용 스텟 구조체")]
    [SerializeField] private Stat s;
    
    // 이벤트 
    public event Action<Stat> OnDefaultCalculated;
    public event Action<Stat> OnRecalculated;


    private void Start()
    {
        pm = PlayerManager.Instance;

        OnDefaultCalculated += pm.playerStats.OnApplyStat; // 1. 캐릭터 선택시 "스텟 계산기"에서 기초 스텟을 적용한 뒤 스텟결과창에 적용 
        OnRecalculated += pm.playerStats.OnApplyStat; // 2. 스텟이 변화할때 "스텟 계산기"에서 계산이 완료되면 스텟결과창에 적용
    }


    /// <summary>
    /// 캐릭터 기본 스펙 세팅용 메서드
    /// </summary>
    #region  Calculator
    public void DefaultCalculate()
    {
        s = pm.playerStats.Stat; // 기존 스텟을 복사해온다.

        if (pm.character == null || pm.levelSystem == null)
        {
            Debug.LogWarning("StatCalculator: character/levelSystem null");
            return;
        }

        // 1. 캐릭터 디폴트 스탯 임시 변수에 담고 
        s.MaxHp = pm.character.baseHp;
        s.MaxMp = pm.character.baseMp;
        s.Speed = pm.character.baseSpeed;
        s.Attack = pm.character.baseAttack;
        s.DrainArea = pm.character.drainArea;

        // 2. 읽기 변수에 담은다음 플레이어스탯 으로 보낸다.
        OnDefaultCalculated.Invoke(s);
    }

    /// <summary>
    /// 스탯 변화시 계산용 메서드
    /// </summary>
    public void ReCalculate(Stat s)
    {  
        OnRecalculated.Invoke(s);
    }
    #endregion

    public void CalculateOnLevelUp(LevelSystem levelSystem)
    {
        s = pm.playerStats.Stat; // 기존 스텟을 복사해온다.

        int L = Mathf.Max(1, levelSystem.Level);
        int idx = L - 1; // 커브 입력용

        // 1) 기본 + 성장 (예: 커브/테이블 사용) 
        s.MaxHp += 10f;
        s.MaxMp += 10f;
        s.Speed += 0.5f;
        s.Attack += +2f;
        s.DrainArea += 0f;

        ReCalculate(s);
    }
    
    public void CalculateOnSelecetAgument()
    {
        s = pm.playerStats.Stat; // 기존 스텟을 복사해온다.

        ReCalculate(s);
    }

    private void OnDestroy()
    {
        OnDefaultCalculated -= pm.playerStats.OnApplyStat;
        OnRecalculated -= pm.playerStats.OnApplyStat;
    }
}
