using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    private MeshRenderer render;
    public float speed = 0.5f;
    private float offset;

    void Start()
    {
        render = GetComponent<MeshRenderer>();
    }

    void Update()
    { 
        // 플레이어 입력값 사용
        float inputX = GameManager.Instance.playerController.inputVec.x;

        offset += inputX * speed * Time.deltaTime;
        render.material.mainTextureOffset = new Vector2(offset, 0);
    }
}
