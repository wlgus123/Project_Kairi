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
            grappling.isAttach = true;
        }
        if (collision.CompareTag("Enemy"))
        {
            grappling.AttachEnemy(collision.transform);
        }
    }
}
