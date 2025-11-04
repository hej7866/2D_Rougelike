using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;    // Player Transform
    public float smoothSpeed = 5f;
    public Vector3 offset;      // 카메라 위치 오프셋

    void Start()
    {
        target = PlayerController.Instance.transform;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
