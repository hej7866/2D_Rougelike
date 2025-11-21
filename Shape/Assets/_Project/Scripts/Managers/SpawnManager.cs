using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;

public class SpawnManager : SingleTon<SpawnManager>
{
    public PoolManager poolManager; 

    [Header("플레이어")]
    public GameObject playerPrefab;
    public GameObject playerInstance;
    public Transform playerTr;

    [Header("보스")]
    public GameObject bossPrefab;
    public GameObject bossInstance;
    public Transform bossSpawnPos;

    [Header("타켓 카메라")]
    [SerializeField] private Camera _targetCamera;
    [SerializeField, Range(0f, 0.5f)] private float _outOfViewMargin = 0.05f; // 카메라 바깥 판정 마진

    [Header("Spawn Control")]
    [SerializeField] private float _spawnInterval = 1.25f;
    [SerializeField] private int _maxAlive = 40;           // 동시에 살아있을 최대 수
    [SerializeField] private int _alive;


    [Header("Spawn Rect (player-centric)")]
    // 플레이어 기준 "카메라보다 살짝 큰" 안쪽 박스 (여기엔 스폰 안 함)
    [SerializeField] private Vector2 _innerRectHalfSize = new Vector2(0f, 0f);
    // 바깥쪽 스폰 박스 (최대 범위)
    [SerializeField] private Vector2 _outerRectHalfSize = new Vector2(0f, 0f);



    protected override void Awake()
    {
        base.Awake();
        if (_targetCamera == null) _targetCamera = Camera.main;
        SpawnPlayer(); // 플레이어 생성
        poolManager.BuildEnemyPools(); // 에너미 풀 생성
    }

    void Start()
    {
        float halfH = _targetCamera.orthographicSize;                // 세로 반쪽
        float halfW = _targetCamera.orthographicSize * _targetCamera.aspect; // 가로 반쪽

        // inner = 카메라 화면 영역 그대로
        _innerRectHalfSize = new Vector2(halfW, halfH);

        // outer = 카메라 크기의 1.5배
        _outerRectHalfSize = _innerRectHalfSize * 1.5f;

        StartCoroutine(SpawnLoop()); // 에너미 생성 루프 실행
    }

    #region Player
    void SpawnPlayer()
    {
        playerInstance = Instantiate(playerPrefab);
        playerTr = playerInstance.transform;
        playerInstance.transform.position = Vector3.zero;
    }
    #endregion

    #region  Boss
    public void SpawnBoss()
    {
        bossInstance = Instantiate(bossPrefab);
        bossInstance.transform.position = bossSpawnPos.position;
    }
    #endregion


    #region EnemySpawning
    private IEnumerator SpawnLoop()
    {
        // 플레이어 준비 대기
        while (playerTr == null) yield return null;

        var wait = new WaitForSeconds(_spawnInterval);
        while (true)
        {
            if (GameManager.Instance.gameState == GameState.General && _alive < _maxAlive)
            {
                TrySpawnEnemy();
            }
            yield return wait;
        }
    }

    private void TrySpawnEnemy()
    {
        if (poolManager.enemyPools.Count == 0 || playerTr == null) return;

        // 디버그 기록 초기화
        _debugTried.Clear();
        _debugAccepted.Clear();
        _debugHasChosen = false;

        var pool = poolManager.enemyPools[Random.Range(0, poolManager.enemyPools.Count)];

        const int MAX_TRIES = 16;
        Vector3 spawnPos = playerTr.position;
        bool found = false;

        for (int i = 0; i < MAX_TRIES; i++)
        {
            // 1) 플레이어 기준 큰 직사각형 안에서 랜덤 위치
            Vector2 local = new Vector2(
                Random.Range(-_outerRectHalfSize.x, _outerRectHalfSize.x),
                Random.Range(-_outerRectHalfSize.y, _outerRectHalfSize.y)
            );

            // 2) 작은 직사각형 안이면(카메라 근처) 버림
            if (Mathf.Abs(local.x) < _innerRectHalfSize.x &&
                Mathf.Abs(local.y) < _innerRectHalfSize.y)
            {
                continue;
            }

            Vector3 candidate = playerTr.position + (Vector3)local;
            candidate.z = 0f;

            _debugTried.Add(candidate);

            // 카메라 밖 보장을 innerRect로 이미 어느 정도 하고 있으니까
            // 필요하면 IsOutsideCameraView는 빼도 되고, 불안하면 한 번 더 체크해도 됨.
            // if (!IsOutsideCameraView(candidate))
            //     continue;

            if (IsEnemyTooClose(candidate))
            {
                if (TryOffsetFromEnemies(candidate, out var adjusted))
                {
                    spawnPos = adjusted;
                    found = true;
                    _debugAccepted.Add(adjusted);
                    _debugChosenPos = adjusted;
                    _debugHasChosen = true;
                    break;
                }
                continue;
            }

            spawnPos = candidate;
            found = true;
            _debugAccepted.Add(candidate);
            _debugChosenPos = candidate;
            _debugHasChosen = true;
            break;
        }


        if (!found) return;

        var enemy = pool.Get(EnemyManager.Instance.SelectEnemy(), PoolManager.Instance.enemyPoolsParent.transform);
        enemy.transform.position = spawnPos;
        enemy.transform.rotation = Quaternion.identity;

        _alive++;
    }

    private bool IsOutsideCameraView(Vector3 worldPos)
    {
        if (_targetCamera == null) return true; // 카메라 없으면 그냥 허용
        var v = _targetCamera.WorldToViewportPoint(worldPos);
        // z < 0 (카메라 뒤)도 ‘밖’으로 취급
        if (v.z < 0f) return true;
        // 뷰포트 박스 (0~1) 밖(마진 포함)인지
        float m = _outOfViewMargin;
        return (v.x < -m || v.x > 1f + m || v.y < -m || v.y > 1f + m);
    }

    [SerializeField] private float enemyAvoidRadius = 1f; // 적과 최소 거리
    [SerializeField] private int enemyOffsetChecks = 8;     // 주변 체크 횟수
    private bool IsEnemyTooClose(Vector3 pos)
    {
        Collider2D hit = Physics2D.OverlapCircle(pos, enemyAvoidRadius, LayerMask.GetMask("Enemy"));
        return hit != null;
    }

    private bool TryOffsetFromEnemies(Vector3 center, out Vector3 result)
    {
        float r = enemyAvoidRadius * 1.2f;

        for (int i = 0; i < enemyOffsetChecks; i++)
        {
            float angle = 360f / enemyOffsetChecks * i;
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * r;
            Vector3 checkPos = center + offset;

            if
            (
                IsOutsideCameraView(checkPos) &&
                !IsEnemyTooClose(checkPos)
            )
            {
                result = checkPos;
                return true;
            }
        }

        result = center;
        return false;
    }
    #endregion

    #region Gizmo
    // SpawnManager 내부 아무 필드 영역
    [Header("Debug Draw")]
    [SerializeField] private bool debugDraw = true;
    [SerializeField] private float gizmoPointRadius = 0.2f;

    [SerializeField] private Color colRingMin = new Color(0f, 1f, 1f, 1f);  // cyan
    [SerializeField] private Color colRingMax = new Color(0f, 0.5f, 1f, 1f); // blue
    [SerializeField] private Color colCamRect = new Color(1f, 1f, 0f, 1f);   // yellow
    [SerializeField] private Color colCandBad = new Color(1f, 0f, 0f, 1f);   // red
    [SerializeField] private Color colCandGood = new Color(0f, 1f, 0f, 1f);  // green

    // 디버그용 후보/채택 위치 기록
    private readonly List<Vector3> _debugTried = new();
    private readonly List<Vector3> _debugAccepted = new();
    private Vector3 _debugChosenPos;
    private bool _debugHasChosen;

    private void OnDrawGizmosSelected()
    {
        if (!debugDraw) return;

        // 1) 플레이어 중심 스폰 링
       if (playerTr != null)
        {
            // 안쪽 박스 (카메라 근처, 미스폰 영역)
            DrawRect(playerTr.position, _innerRectHalfSize, Color.yellow);
            // 바깥 박스 (최대 스폰 영역)
            DrawRect(playerTr.position, _outerRectHalfSize, Color.cyan);
        }


        // 2) 카메라 뷰포트 사각형 (마진 포함)
        if (_targetCamera != null)
        {
            // z깊이: 카메라→그려줄 평면까지의 거리 (2D라면 보통 player z 또는 0 기준)
            float refZ = playerTr != null ? playerTr.position.z : 0f;
            float depth = refZ - _targetCamera.transform.position.z;

            float m = _outOfViewMargin;
            Vector3 bl = _targetCamera.ViewportToWorldPoint(new Vector3(0f - m, 0f - m, depth));
            Vector3 tl = _targetCamera.ViewportToWorldPoint(new Vector3(0f - m, 1f + m, depth));
            Vector3 tr = _targetCamera.ViewportToWorldPoint(new Vector3(1f + m, 1f + m, depth));
            Vector3 br = _targetCamera.ViewportToWorldPoint(new Vector3(1f + m, 0f - m, depth));

            Gizmos.color = colCamRect;
            Gizmos.DrawLine(bl, tl);
            Gizmos.DrawLine(tl, tr);
            Gizmos.DrawLine(tr, br);
            Gizmos.DrawLine(br, bl);

            // (선택) 에디터에 반투명 면 채우기
            #if UNITY_EDITOR
            UnityEditor.Handles.color = new Color(colCamRect.r, colCamRect.g, colCamRect.b, 0.08f);
            UnityEditor.Handles.DrawAAConvexPolygon(bl, tl, tr, br);
            #endif
        }

        // 3) 최근 스폰 시도 후보들 - 빨강(실패), 초록(허용 후보), 별표(최종 선택)
        Gizmos.color = colCandBad;
        foreach (var p in _debugTried)
            Gizmos.DrawSphere(p, gizmoPointRadius);

        Gizmos.color = colCandGood;
        foreach (var p in _debugAccepted)
            Gizmos.DrawSphere(p, gizmoPointRadius * 1.2f);

        if (_debugHasChosen)
        {
            // 최종 선택점 강조 + 회피 반경 시각화
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(_debugChosenPos, gizmoPointRadius * 1.6f);

            #if UNITY_EDITOR
            UnityEditor.Handles.color = new Color(1f, 1f, 1f, 0.6f);
            UnityEditor.Handles.DrawWireDisc(_debugChosenPos, Vector3.forward, enemyAvoidRadius);
            #else
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(_debugChosenPos, enemyAvoidRadius);
            #endif
        }
    }

    void DrawRect(Vector3 center, Vector2 halfSize, Color c)
    {
        Gizmos.color = c;
        Vector3 bl = center + new Vector3(-halfSize.x, -halfSize.y, 0);
        Vector3 tl = center + new Vector3(-halfSize.x,  halfSize.y, 0);
        Vector3 tr = center + new Vector3( halfSize.x,  halfSize.y, 0);
        Vector3 br = center + new Vector3( halfSize.x, -halfSize.y, 0);

        Gizmos.DrawLine(bl, tl);
        Gizmos.DrawLine(tl, tr);
        Gizmos.DrawLine(tr, br);
        Gizmos.DrawLine(br, bl);
    }

    #endregion
}