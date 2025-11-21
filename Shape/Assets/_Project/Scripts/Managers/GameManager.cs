using UnityEngine;
using System;
using UnityEngine.Tilemaps;
public enum GameState { General, Boss, Prepare, GameOver } // 제네럴 -> 보스 -> 프리페어 -> 제네럴

public class GameManager : SingleTon<GameManager>
{
    [Header("게임 상태")]
    public GameState gameState = GameState.General;
    [SerializeField] private int _stage = 1;
    public int Stage => _stage;

    [Header("분노 게이지")]
    [SerializeField] private float _maxThreatGuage = 100f;
    public float MaxThreatGuage => _maxThreatGuage;
    [SerializeField] private float _threatGuage = 0f;
    public event Action<float> OnThreatGuageChanged; // 게이지 변화할때 발생하는 이벤트

    [Header("그라운드 타일맵 렌더러")]
    [SerializeField] private TilemapRenderer[] _groundTileMapRenderers;

    [Header("보스방 타일맵 그리드")]
    [SerializeField] private GameObject _bossRoomGird;

    [Header("상점맵 타일맵 그리드")]
    [SerializeField] private GameObject _shopGird;

    void Start()
    {
        OnThreatGuageChanged += UIManager.Instance.threatGaugeView.OnUpdateThreatGauge;   
    }

    public float ThreatGuage
    {
        get => _threatGuage;
        set
        {
            float nv = Mathf.Clamp(value, 0f, _maxThreatGuage);
            _threatGuage = nv;
            OnThreatGuageChanged?.Invoke(_threatGuage);
            if (_threatGuage >= _maxThreatGuage) 
            {
                OnBossPhase();
            }
        }
    }

    void Update()
    {
    }

    public void OnGeneralPhase()
    {
        Debug.Log("일반 스테이지");
        gameState = GameState.General;
        _shopGird.SetActive(false);
        foreach(var gtmr in _groundTileMapRenderers)
        {
            gtmr.enabled = true;
        }
        AudioManager.Instance.PlayGeneralBGM();
    }

    public void OnBossPhase()
    {
        Debug.Log("분노게이지 맥스 보스스테이지 입장");
        gameState = GameState.Boss;
        foreach(var gtmr in _groundTileMapRenderers) // 일반 맵 끄고
        {
            gtmr.enabled = false;
        }
        PoolManager.Instance.enemyPools[0].ReturnAllEnemies(); // 적 다 리턴시키고
        _bossRoomGird.transform.position = new Vector3(PlayerManager.Instance.player.transform.position.x, PlayerManager.Instance.player.transform.position.y + 6f, 0); // 보스방의 위치를 현재 플레이어의 위치로 위치시키고
        _bossRoomGird.SetActive(true); // 보스방을 키고
        SpawnManager.Instance.SpawnBoss();
        AudioManager.Instance.PlayBossBGM();
        ThreatGuage = 0f;
    }

    public void OnPreparePhase()
    {
        ClearBossPatterns();
        
        Debug.Log("준비 스테이지");
        gameState = GameState.Prepare;
        _stage++;
        _bossRoomGird.SetActive(false);
        _shopGird.SetActive(true);
        _shopGird.transform.position = new Vector3(PlayerManager.Instance.player.transform.position.x - 0.5f, PlayerManager.Instance.player.transform.position.y, 0); 
    }

    void ClearBossPatterns()
    {
        var objs = GameObject.FindGameObjectsWithTag("BossPattern");
        foreach (var obj in objs)
            Destroy(obj);
    }
    
    public void SwitchGame()
    {
        Time.timeScale =Time.timeScale == 0f ? 1f : 0f;
    }

    public void IncreaseThreatGuage(float amount)
    {
        ThreatGuage += amount;
    }
}
