using UnityEngine;
using tagName = Globals.TagName;
using static Globals;

public class PlayerDieObject : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(tagName.player))
            GameManager.Instance.playerController.TakeDamage(1000000);
    }
}
