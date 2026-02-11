using UnityEngine;

public class ObjectAnimation : MonoBehaviour
{
    Animator anim;
    private bool isHeld;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void OnTransformParentChanged()
    {
        if (transform.parent != null && transform.parent.CompareTag("Player"))
        {
            Debug.Log("hold");
            isHeld = true;
        }
        else
            isHeld = false;

        anim.SetBool("isHeld", isHeld);
    }

}
