using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-20)]
public class BattleSystem : MonoBehaviour
{
    [Header("평타 / 유틸")]
    [SerializeField] public AutoAttack autoAttack;
    [SerializeField] public Utile utile;

    // 스킬 실행기
    public void SkillExecutor(Skill skill)
    {
        if( PlayerManager.Instance.playerController.Mp < skill.manaCost) return; // 마나부족하면 리턴

        int actionLenght = skill.actions.Length;
        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 tgt = new Vector3(m.x, m.y, 0f);
        SkillContext ctx = new SkillContext
        {
            caster = gameObject,
            castOrigin = gameObject.transform.position,
            targetPoint = tgt
        };
        PlayerManager.Instance.playerController.Mp -= skill.manaCost;
        for(int i=0; i<actionLenght; i++)
        {
            StartCoroutine(skill.actions[i].action.Execute(ctx, skill.actions[i].parameters));
        }
    }

    #region AA
    [Header("AAPool")]
    [SerializeField] public AAPool aaPool;

    [Serializable]
    public class AAPool
    {  
        [Header("Pool Setting")]
        [SerializeField] private GameObject _aaObjPrefab;
        [SerializeField] private int _size;
        public bool expandable = true;

        public Queue<GameObject> _aaPool = new();

        public void SetAAPool(Character character)
        {
            _aaObjPrefab = character.aaObj;
            BuildPool();
        }

        private void BuildPool()
        {
            for (int i = 0; i < _size; i++)
            {
                var go = Instantiate(_aaObjPrefab, PlayerManager.Instance.transform);
                go.SetActive(false);
                _aaPool.Enqueue(go);
            }
        }

        public GameObject Get()
        {
            if (_aaPool.Count > 0)
            {
                var go = _aaPool.Dequeue();
                go.SetActive(true);
                return go;
            }

            if (expandable && _aaObjPrefab != null)
            {
                var go = Instantiate(_aaObjPrefab, PlayerManager.Instance.transform);
                go.SetActive(true);
                return go;
            }

            return null;
        }


        public void Return(GameObject go)
        {
            if (!go) return;
            go.SetActive(false);
            go.transform.SetParent(PlayerManager.Instance.transform);
            _aaPool.Enqueue(go);
        }
    }
    #endregion


    [Serializable] // 평타 로직
    public class AutoAttack
    {
        Vector3 _aim { get; set; }
        [SerializeField] private GameObject gun;

        public void AA(PlayerManager pm)
        {
            var player = pm.player;

            // 방향
            var mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _aim = new Vector3(mp.x, mp.y, player.transform.position.z);
            var dir = (_aim - player.transform.position).normalized;

            // 오브젝트 생성
            var go = pm.battleSystem.aaPool.Get();
            AAProj aaObj = go.GetComponent<AAProj>();

            // 치명타 판정 체크
            bool isCritical = UnityEngine.Random.Range(0f, 1f) < (pm.playerStat.Stat[StatType.CriticalProb] / 100f);

            // proj정보 입력
            aaObj.InitAAProj(pm.battleSystem.aaPool, dir, isCritical);
            
            // 생성위치 및 방향 지정
            go.transform.position = gun.transform.position;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            go.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

    }


    [Serializable]
    public class Utile
    {
        #region Dash
        [Header("Dash params")]
        [SerializeField] private float _dashDuration = 0.12f;
        [SerializeField] private float _dashDistance = 2f;

        public IEnumerator DashRoutine(Rigidbody2D rb)
        {
            // 마우스 방향 계산
            Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = ((Vector2)m - rb.position).normalized;

            Vector2 start = rb.position;
            Vector2 end = start + dir * _dashDistance;

            float t = 0f;
            while (t < _dashDuration)
            {
                t += Time.fixedDeltaTime;
                rb.MovePosition(Vector2.Lerp(start, end, t / _dashDuration));
                yield return new WaitForFixedUpdate();
            }

            // 실제 물리 위치 기반으로 보정
            Vector2 actualEnd = rb.position;

            // 플레이어 컨트롤러의 targetPoint를 현재 물리 위치로 세팅
            PlayerManager.Instance.playerController.targetPoint = new Vector3(actualEnd.x, actualEnd.y, 0f);
        }
        #endregion
    }
}
