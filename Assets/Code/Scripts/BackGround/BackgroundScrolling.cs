using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    private MeshRenderer render;
    public float speed = 0.5f;
    private float offset;
    private Transform player;
    private float prevX;

    void Start()
    {
        render = GetComponent<MeshRenderer>();
        player = GameManager.Instance.playerController.transform;
        prevX = player.position.x;
    }

    void Update()
    {
        // 플레이어의 실제 이동량 기준으로 배경 스크롤
        float deltaX = player.position.x - prevX;
        offset += deltaX * speed;
        render.material.mainTextureOffset = new Vector2(offset, 0);
        prevX = player.position.x;
    }
}
