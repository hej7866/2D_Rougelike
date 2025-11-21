using UnityEngine;

public class Aim : MonoBehaviour
{
    void FixedUpdate()
    {
        Look();
    }

    public void Look()
    {
        // 마우스의 월드 좌표 구하기
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 원 중심(this.transform) → 마우스 방향 벡터
        Vector3 dirVec = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    public Vector3 GetAimPos()
    {
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return aimPos;
    }
}
