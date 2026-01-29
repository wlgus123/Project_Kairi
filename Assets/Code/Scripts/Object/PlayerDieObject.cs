using UnityEngine;
using tagName = Globals.TagName;

public class PlayerDieObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(tagName.player))
        {
            GameManager.Instance.playerController.TakeDamage(1000000);
            Debug.Log("³«»çÇÔ ¤µ¤¡");
        } 
    }
}