using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-20)]
public class BattleSystem : MonoBehaviour
{
    [Header("평타 / 스킬")]
    [SerializeField] public AutoAttack autoAttack;
    [SerializeField] public SpecialAttack specialAttack;

    [Header("AAPool")]
    [SerializeField] public AAPool aaPool;

    #region AAPool
    [Serializable]
    public class AAPool
    {  
        [Header("Pool Setting")]
        [SerializeField] private GameObject aaObjPrefab;
        [SerializeField] private int size;
        public bool expandable = true;

        public Queue<GameObject> _aaPool = new();

        public void SetAAPool(Character character)
        {
            aaObjPrefab = character.aaObj;
            BuildPool();
        }

        private void BuildPool()
        {
            for (int i = 0; i < size; i++)
            {
                var go = Instantiate(aaObjPrefab, PlayerManager.Instance.transform);
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

            if (expandable && aaObjPrefab != null)
            {
                var go = Instantiate(aaObjPrefab, PlayerManager.Instance.transform);
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
        Vector3 aim { get; set; }

        public void AA()
        {
            var pm = PlayerManager.Instance;
            var player = pm.player;

            var mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aim = new Vector3(mp.x, mp.y, player.transform.position.z);
            var dir = (aim - player.transform.position).normalized;

            var go = PlayerManager.Instance.battleSystem.aaPool.Get();
            AAObj aaObj = go.GetComponent<AAObj>();

            go.transform.position = player.transform.position;

            aaObj.dir = dir;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            go.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

    }

    [Serializable] // 스킬 로직
    public class SpecialAttack
    {

    }
}
