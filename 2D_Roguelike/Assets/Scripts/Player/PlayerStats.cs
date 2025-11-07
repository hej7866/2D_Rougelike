// PlayerStats.cs
using UnityEngine;
using System;

// 메인 스탯
[Serializable]
public struct Stat
{
    public float MaxHp;
    public float MaxMp;
    public float Attack;
    public float Speed;
    public float DrainArea;
}

[DefaultExecutionOrder(-40)]
public class PlayerStats : MonoBehaviour
{
    public PlayerManager pm;

    [Header("플레이어 스탯")]
    [SerializeField] private Stat _stat;
    public Stat Stat => _stat; // 읽기 전용

    public event Action<Stat> OnStatChanged;


    private void Start()
    {
        pm = PlayerManager.Instance;

        OnStatChanged += pm.drain.ChangeCircleSize;
    }

    public void OnApplyStat(Stat newStat)
    {
        _stat = newStat;
        OnStatChanged?.Invoke(_stat);
    }

}
