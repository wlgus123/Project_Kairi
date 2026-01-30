using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool IsGrounded { get; private set; }
    public bool HasCollided { get; private set; }

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Check(Collision2D collision)
    {
        IsGrounded = false;

        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.7f &&
                contact.point.y < transform.position.y)
            {
                IsGrounded = true;
                break;
            }
        }

        HasCollided = true;

        // y°ª º¸Á¤ (¹Ù´Ú ¶Õ¸² ¹æÁö)
        if (IsGrounded && rigid.linearVelocityY < 0f)
        {
            rigid.linearVelocity = new Vector2(
                rigid.linearVelocity.x, 0f
            );
        }
    }

    public void ResetState()
    {
        IsGrounded = false;
        HasCollided = false;
    }
}
