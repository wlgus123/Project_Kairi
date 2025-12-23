using UnityEngine;

public class Hooking : MonoBehaviour
{
    GrapplingHook grappling;
    public DistanceJoint2D joint2D;

    private Transform hookedEnemy;

    void Start()
    {
        grappling = GameObject.Find("Player").GetComponent<GrapplingHook>();
        joint2D = GetComponent<DistanceJoint2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ceiling"))
        {
            joint2D.enabled = true;

            // 플레이어가 갈고리를 건 위치가 Joint DIstance의 Distance
            float dist = Vector2.Distance(grappling.transform.position, transform.position);
            joint2D.distance = dist - 0.3f;

            grappling.isAttach = true;
            grappling.isHookActive = false;
            grappling.isLineMax = false;
        }
        if (collision.CompareTag("Enemy") || collision.CompareTag("ThrowingEnemy"))
        {
            grappling.AttachEnemy(collision.transform);
        }
    }
}
