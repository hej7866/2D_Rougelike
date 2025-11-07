using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Scripting.APIUpdating;

public class EnemyController : MonoBehaviour
{
    public Transform target;

    [Header("적 객체")]
    public Enemy enemy;

    [Header("적 스펙")]
    [SerializeField] private float _enemyMaxHp;
    public float EnemyMaxHP
    {
        get => _enemyMaxHp;
        set
        {
            float max = enemy.hp;
            float nv = Mathf.Clamp(value, 0f, max); // 밸류값이 0과 max사이에서 nv에 저장하고
            if (Mathf.Approximately(_enemyMaxHp, nv)) return; // 기존 hp값과 새로운 밸류값이 차이가없으면 리턴해버리고
            float ov = _enemyMaxHp; // 차이가 있다면 기존 hp값을 잠시 넣어둔다음 
            _enemyMaxHp = nv; // 새로운값을 _enemyMaxHp에 넣는다. 
            OnEnemyHpChanged.Invoke(max, _enemyMaxHp); // ui 처리 해야함
            if (_enemyMaxHp <= 0f) Die();
        }
    }
    public float enemySpeed;

    public event Action<float, float> OnEnemyHpChanged;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private Slider slider;
    private Rigidbody2D rb;

    void Awake()
    {
        OnEnemyHpChanged += UpdateHealthBarUI;
    }

    void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        slider = GetComponentInChildren<Slider>();
        target = GameObject.FindWithTag("Player").transform;
        
        EnemyInit();
    }

    void FixedUpdate()
    {
        EnemyAI();
    }

    void EnemyAI()
    {
        Move();

        // 플립
        if (transform.position.x > target.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }

    void Move()
    {
        // 방향 벡터 계산
        Vector3 dir = (target.position - transform.position).normalized;

        // 이동
        rb.linearVelocity = dir * enemySpeed;
    }

    void UpdateHealthBarUI(float maxHp, float currentHp)
    {
        slider.value = currentHp / maxHp;
    }

    void EnemyInit()
    {
        var lb = spriteRenderer.localBounds;   // 피벗 반영된 로컬 공간 Bounds
        boxCollider2D.size   = lb.size;       // 가로/세로
        boxCollider2D.offset = lb.center;     // 피벗이 중앙이 아니어도 정렬됨

        spriteRenderer.sprite = enemy.sprite;
        EnemyMaxHP = enemy.hp;
        enemySpeed = enemy.speed;
    }

    public void TakeDamage(float amount)
    {
        EnemyMaxHP -= amount;
    }
    
    void Die()
    {
        var exp = ExpManager.Instance.GetExp();
        exp.transform.position = transform.position;
        
        SpawnManager.Instance.Despawn(gameObject);
    }
}
