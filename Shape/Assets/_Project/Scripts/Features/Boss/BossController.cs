using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class BossController : MonoBehaviour
{
    public GameObject target;
    public Boss[] boss;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    [SerializeField] private TextMeshProUGUI health_Text;
    private Animator anim;
    private Rigidbody2D rb;


    [Header("보스 스펙")]
    [SerializeField] private float _bossHp;
    public float BossHP
    {
        get => _bossHp;
        set
        {
            float max = boss[GameManager.Instance.Stage - 1].bossHp;
            float nv = Mathf.Clamp(value, 0f, max); // 밸류값이 0과 max사이에서 nv에 저장하고
            if (Mathf.Approximately(_bossHp, nv)) return; // 기존 hp값과 새로운 밸류값이 차이가없으면 리턴해버리고
            float ov = _bossHp; // 차이가 있다면 기존 hp값을 잠시 넣어둔다음 
            _bossHp = nv; // 새로운값을 _enemyMaxHp에 넣는다. 
            OnBossHpChanged.Invoke(max, _bossHp); // ui 처리 해야함
            if (_bossHp <= 0f) OnBossDie.Invoke();
        }
    }

    public event Action<float,float> OnBossHpChanged;
    public event Action OnBossDie;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        health_Text = GetComponentInChildren<TextMeshProUGUI>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        OnBossHpChanged += UpdateBossHealthTextUI;
        OnBossDie += BossDie;
        OnBossDie += GameManager.Instance.OnPreparePhase;
    }


    void Start()
    {
        BossInit();
        SetColliderSize();
        target = GameObject.FindWithTag("Player");  
    }

    [SerializeField] private float _patternDelay = 3f;
    private float _t = 0f;
    void Update()
    {
        _t += Time.deltaTime;
        if(_patternDelay <= _t)
        {
            int ran = UnityEngine.Random.Range(0,boss[GameManager.Instance.Stage - 1].patterns.Length);    
            StartCoroutine(boss[GameManager.Instance.Stage - 1].patterns[ran].pattern.ExecutePattern(this, boss[GameManager.Instance.Stage - 1].patterns[ran].parameters));
            _t = 0f;
        }
    }

    void UpdateBossHealthTextUI(float maxHp, float currentHp)
    {
        health_Text.text = $"{(int)currentHp}";
    }

    public void TakeDamage(float amount)
    {
        BossHP -= amount; 
        anim.SetTrigger("Hit");
    }

    public void SetColliderSize()
    {
        var lb = spriteRenderer.localBounds;   // 피벗 반영된 로컬 공간 Bounds
        boxCollider2D.size   = lb.size;       // 가로/세로
        boxCollider2D.offset = lb.center;     // 피벗이 중앙이 아니어도 정렬됨
    }

    public void BossInit()
    {
        spriteRenderer.sprite = boss[GameManager.Instance.Stage - 1].sprite;
        BossHP = boss[GameManager.Instance.Stage - 1].bossHp;
    }

    public void BossDie()
    {
        Destroy(gameObject);
    }
}
