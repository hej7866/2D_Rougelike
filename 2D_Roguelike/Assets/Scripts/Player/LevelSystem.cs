// LevelSystem.cs
using UnityEngine;
using System;
using UnityEngine.InputSystem.Composites;

public class LevelSystem : MonoBehaviour
{
    [Header("레벨/경험치")]
    [SerializeField] private int _level = 0; // 현재 레벨
    [SerializeField] private float _requiredExp; // 필요 경험치
    [SerializeField] private float _exp = 0f; // 현재 경험치

    public int Level => _level;
    public float RequiredExp => _requiredExp;
    public float Exp => _exp;

    public event Action<LevelSystem> OnLevelChanged;     // 새 레벨
    public event Action<LevelSystem> OnExpChanged;     // 현재 경험치
    public event Action<int> OnRequiredExpChanged; // (level, required)

    private void Start()
    {
        _requiredExp = 100f;
        _level = 0;

        OnRequiredExpChanged += CalculateRequiredExp;
        OnExpChanged += UIManager.Instance.UpdateUIOnChangePlayerConditon;
        OnLevelChanged += AgumentManager.Instance.SetAgument; // 증강 세팅
        OnLevelChanged += UIManager.Instance.UpdateUIOnLevelUp; // ui 온
        OnLevelChanged += PlayerManager.Instance.statCalculator.CalculateOnLevelUp; // 레벨이 오르면 스펙 재계산
    }

    // 필요 경험치 함수(예시): 선형. 필요하면 커브/테이블로 교체.
    public void CalculateRequiredExp(int level)
    {
        _requiredExp += +(level - 1) * 5f;
    }

    public void AddExp(float amount)
    {
        if (amount <= 0f) return;
        _exp += amount;
        OnExpChanged.Invoke(this);

        TryLevelUpLoop();
    }

    private void TryLevelUpLoop()
    {
        while (_exp >= _requiredExp)
        {
            _exp -= _requiredExp;
            _level++;
            OnLevelChanged.Invoke(this);
            OnRequiredExpChanged?.Invoke(_level);
        }
        // 경험치 변경 브로드캐스트(게이지 갱신용)
        OnExpChanged.Invoke(this);
    }
}
