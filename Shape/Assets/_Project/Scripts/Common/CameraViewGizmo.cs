using UnityEngine;

[ExecuteAlways]
public class CameraViewGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null || !cam.orthographic) return;

        float size = cam.orthographicSize;
        float width = size * 2 * cam.aspect;
        float height = size * 2;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cam.transform.position, new Vector3(width, height, 0.1f));
    }
}
