using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using System;
using UnityEditor;
using TMPro;

public class PoolManager : SingleTon<PoolManager>
{
    [Header("Enemy Pools")]
    public GameObject enemyPoolsParent;
    public EnemyPoolConfig[] enemyGroups; // 프리팹/초기수 설정
    public List<EnemyPool> enemyPools = new();


    [Header("deathEffect Pools")]
    public ParticlePool deathEffectPools;
    [SerializeField] private ParticleSystem _deathEffect;
    [SerializeField] public GameObject deathEffectPoolsParent;

    [Header("HitText Pools")]
    public HitTextPool hitTextPools;
    [SerializeField] private GameObject _hitText;
    [SerializeField] public GameObject hitTextPoolsParent;

    public void BuildEnemyPools()
    {
        enemyPools.Clear();
        foreach (var cfg in enemyGroups)
        {
            if (cfg.prefab == null) continue;
            enemyPools.Add(new EnemyPool(cfg.prefab, Mathf.Max(0, cfg.initialSize), Mathf.Max(cfg.initialSize, cfg.maxSize), enemyPoolsParent.transform));
        }
    }

    public void BuildPools()
    {
        deathEffectPools = new ParticlePool(_deathEffect, 32, deathEffectPoolsParent.transform);
        hitTextPools = new HitTextPool(_hitText, 64, hitTextPoolsParent.transform);
    }

    protected override void Awake()
    {
        base.Awake();
        BuildPools();
    }

    [System.Serializable]
    public class EnemyPoolConfig
    {
        public GameObject prefab;
        public int initialSize;
        public int maxSize;
    }

    public class EnemyPool
    {
        public GameObject prefab;
        public int maxSize;
        private readonly Queue<GameObject> q = new();
        private readonly HashSet<GameObject> active = new(); // 살아있는 애들 목록

        public EnemyPool(GameObject prefab, int initial, int max, Transform parent = null)
        {
            this.prefab = prefab;
            maxSize = Mathf.Max(initial, max);
            for (int i = 0; i < initial; i++)
            {
                var go = Instantiate(prefab, parent);
                go.SetActive(false);
                q.Enqueue(go);
            }
        }

        public GameObject Get(Enemy enemy, Transform parent = null)
        {
            GameObject go;
            if (q.Count > 0)
                go = q.Dequeue();
            else
                go = Instantiate(prefab, parent);

            var enemyController = go.GetComponent<EnemyController>();
            enemyController.enemy = enemy;
            enemyController.OriginPool = this; 

            if (parent != null)
                go.transform.SetParent(parent);

            go.SetActive(true);
            enemyController.EnemyInit();
            active.Add(go);      // 살아있는 리스트에 추가
            return go;
        }

        public void Return(GameObject go)
        {
            go.SetActive(false);
            active.Remove(go);   // 살아있는 리스트에서 제거
            q.Enqueue(go);
        }

        public void ReturnAllEnemies()
        {
            // active를 복사해서 도는 게 안전
            var snapshot = new List<GameObject>(active);
            foreach (var go in snapshot)
            {
                Return(go);
            }
        }
    }

    [Serializable]
    public class ParticlePool
    {
        [SerializeField] private ParticleSystem _particlePrefab;
        [SerializeField] private int _size;
        public Queue<ParticleSystem> particleQueue = new Queue<ParticleSystem>();

        public ParticlePool(ParticleSystem ParticlePrefab, int size, Transform parent = null)
        {
            _particlePrefab = ParticlePrefab;
            _size = size;
            for (int i = 0; i < size; i++)
            {
                var p = Instantiate(ParticlePrefab, parent);
                p.gameObject.SetActive(false);
                particleQueue.Enqueue(p);
            }
        }

        public ParticleSystem GetParticleSystem(Vector3 pos)
        {
            var p = particleQueue.Dequeue();
            p.transform.position = pos;
            p.gameObject.SetActive(true);
            return p;
        }
    }

    [Serializable]
    public class HitTextPool
    {
        [SerializeField] private GameObject _hitTextPrefab;
        [SerializeField] private int _size = 64;
        public Queue<GameObject> hitTextQueue = new Queue<GameObject>();

        public HitTextPool(GameObject hitTextPrefab, int size, Transform parent = null)
        {
            _hitTextPrefab = hitTextPrefab;
            _size = size;
            for (int i = 0; i < size; i++)
            {
                var p = Instantiate(_hitTextPrefab, parent);
                p.gameObject.SetActive(false);
                hitTextQueue.Enqueue(p);
            }
        }

        public GameObject GetHitText(EnemyController enemy, float damage)
        {
            var p = hitTextQueue.Dequeue();
            p.transform.position = enemy.transform.position + new Vector3(0, 1.6f, 0);
            p.GetComponentInChildren<TextMeshProUGUI>().text = $"{(int)damage}";
            p.gameObject.SetActive(true);
            return p;
        }

        public void ReturnHitText(GameObject go)
        {
            go.SetActive(false);
            hitTextQueue.Enqueue(go);
        }
    }
}
