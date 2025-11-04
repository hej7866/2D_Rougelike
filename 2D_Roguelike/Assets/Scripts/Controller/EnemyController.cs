using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform target;            // 플레이어 Transform
    private NavMeshAgent agent;
    private SpriteRenderer sr;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sr = GetComponent<SpriteRenderer>();

        // 2D 세팅 핵심
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
        target = PlayerController.Instance.transform;
    }

    void Update()
    {
        if (target == null) return;

        agent.SetDestination(target.position);

        // 좌우 반전(스프라이트가 좌우만 바뀌면 되는 경우)
        if (sr != null && agent.velocity.sqrMagnitude > 0.0001f)
            sr.flipX = agent.velocity.x > 0f;
    }
}
