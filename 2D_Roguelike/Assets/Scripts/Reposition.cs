using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.AI;

public class Reposition : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D collision)
    {    //IsTrigger가 체크된 Collider에서 나갔을 때 동작
        if (!collision.CompareTag("Area"))
            return;

        //거리를 구하기 위해 플레이어 위치와 타일맵 위치 미리 저장
        Vector3 playerPos = PlayerController.Instance.transform.position;
        Vector3 myPos = transform.position;
        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        float diffY = Mathf.Abs(playerPos.y - myPos.y);

        Vector3 targetPoint = PlayerController.Instance.targetPoint;
        float dirX = playerPos.x > targetPoint.x ? -1 : 1;
        float dirY = playerPos.y > targetPoint.y ? -1 : 1;

        switch (transform.tag)
        {
            case "Ground":
                if (diffX > diffY)
                {    //X축 이동시
                    transform.Translate(Vector3.right * dirX * 128);
                    NavmeshBaker.Instance.surface.BuildNavMeshAsync();
                }
                else if (diffX < diffY)
                {   //Y축 이동시
                    transform.Translate(Vector3.up * dirY * 128);
                    NavmeshBaker.Instance.surface.BuildNavMeshAsync();
                }
                break;
            case "Enemy":

                break;

        }
    }
}