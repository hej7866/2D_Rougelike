using UnityEngine;
using System;
using System.Collections.Generic;

public class ExpManager : SingleTon<ExpManager>
{
    [Header("expPool")]
    [SerializeField] private GameObject expPrefab;
    [SerializeField] private int size = 256;
    public Queue<GameObject> _expPool = new();

    void Start()
    {
        for (int i = 0; i < size; i++)
        {
            var go = Instantiate(expPrefab, transform);
            go.SetActive(false);
            _expPool.Enqueue(go);
        }
    }

    public GameObject GetExp()
    {
        var go = _expPool.Dequeue();
        go.SetActive(true);
        return go;
    }

    public void ReturnExp(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(transform);
        _expPool.Enqueue(go);
    }
}
