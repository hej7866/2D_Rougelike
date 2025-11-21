using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public Transform target;
    public PoolManager.EnemyPool OriginPool { get; set; }
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private Transform canvasTr;

    [Header("적 객체")]
    public Enemy enemy;

    [Header("적 스펙")]
    [SerializeField] private float _enemyHp;
    public float EnemyHP
    {
        get => _enemyHp;
        set
        {
            float max = enemy.hp;
            float nv = Mathf.Clamp(value, 0f, max); // 밸류값이 0과 max사이에서 nv에 저장하고
            if (Mathf.Approximately(_enemyHp, nv)) return; // 기존 hp값과 새로운 밸류값이 차이가없으면 리턴해버리고
            float ov = _enemyHp; // 차이가 있다면 기존 hp값을 잠시 넣어둔다음 
            _enemyHp = nv; // 새로운값을 _enemyMaxHp에 넣는다. 
            OnEnemyHpChanged.Invoke(max, _enemyHp); // ui 처리 해야함
            if (_enemyHp <= 0f) Die();
        }
    }
    public float enemySpeed;
    [SerializeField] private float _delay = 1f;

    public event Action<float, float> OnEnemyHpChanged;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private TextMeshProUGUI health_Text;
    private Animator anim;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        health_Text = GetComponentInChildren<TextMeshProUGUI>();
        anim = GetComponent<Animator>();
        OnEnemyHpChanged += UpdateHealthTextUI;
    }

    void Start()
    {    
        target = GameObject.FindWithTag("Player").transform;  
        
        SetColliderSize();
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
    }

    void LateUpdate()
    {
        // 부모의 회전을 상쇄시키기
        canvasTr.rotation = Quaternion.identity;
    }

    // void EnemyAI() // 나중에 좀 똑똑하게 바꿀때 쓸꺼임
    // {
    //     Move();
    // }

    float t = 0.9f;
    public void Attack()
    {
        t += Time.deltaTime;
        if(t >= _delay)
        {
            PlayerManager.Instance.playerController.TakeDamage(enemy.attackDamage);
            t = 0f;
        }
    }

    void Move()
    {
        // 방향 벡터 계산
        Vector3 dir = (target.position - transform.position).normalized;

        // 이동
        rb.linearVelocity = dir * enemySpeed;

            // 플립
        if (transform.position.x > target.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }

    void Rotate()
    {
        Vector3 dir = (target.position - transform.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    void UpdateHealthTextUI(float maxHp, float currentHp)
    {
        health_Text.text = $"{(int)currentHp}";
    }

    public void SetColliderSize()
    {
        var lb = spriteRenderer.localBounds;   // 피벗 반영된 로컬 공간 Bounds
        boxCollider2D.size   = lb.size;       // 가로/세로
        boxCollider2D.offset = lb.center;     // 피벗이 중앙이 아니어도 정렬됨
    }

    public void EnemyInit()
    {
        spriteRenderer.sprite = enemy.sprite;
        EnemyHP = enemy.hp;
        enemySpeed = enemy.speed;
    }

    public void TakeDamage(float amount)
    {
        EnemyHP -= amount;
        anim.SetTrigger("Hit");
    }

    void Die()
    {
        var exp = ExpManager.Instance.GetExp();
        exp.transform.position = transform.position;
        GameManager.Instance.IncreaseThreatGuage(30);
        ParticleSystem ps = PoolManager.Instance.deathEffectPools.GetParticleSystem(transform.position);
        PoolManager.Instance.deathEffectPools.particleQueue.Enqueue(ps); // 생성과 동시에 넣어주기. 근데 왜 disable할때 넣으면 오류나는지 모르겠음.. 트러블슈팅과제
        OriginPool.Return(gameObject);
    }

    public void DespawnEnemy()
    {
        OriginPool.Return(gameObject);
    }

}
