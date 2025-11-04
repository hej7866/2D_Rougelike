using UnityEngine;
using System.Collections;
using NavMeshPlus.Components;

public class NavmeshBaker : SingleTon<NavmeshBaker>
{
    public NavMeshSurface surface;

    void Start()
    {
        // 모든 타일/조각 이동이 끝난 다음 한 프레임 미뤄서 베이크
        StartCoroutine(RebakeNextFrame());
    }

    public void StartBake()
    {
        StartCoroutine(RebakeNextFrame());
    }

    public IEnumerator RebakeNextFrame()
    {
        yield return null;
        surface.BuildNavMeshAsync(); // NavMeshPlus 권장 비동기 베이크
    }
}
