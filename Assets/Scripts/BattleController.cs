using UnityEngine;
using GameCore.Entities;

public class BattleController : MonoBehaviour
{
    private Puppet playerStats;
    public RectTransform battleArea; // Візуальна або логічна межа локації
    public float areaWidth = 20f;
    public float areaHeight = 12f;

    void Start()
    {
        playerStats = GameManager.Instance.currentPlayer;
    }

    void Update()
    {
        if (GameManager.Instance.currentState != GameState.Battle) return;

        HandleMovement();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(moveX, moveY, 0).normalized;
        transform.position += move * playerStats.speed * Time.deltaTime;

        // Обмеження локації (Bounds)
        float clampedX = Mathf.Clamp(transform.position.x, -areaWidth / 2, areaWidth / 2);
        float clampedY = Mathf.Clamp(transform.position.y, -areaHeight / 2, areaHeight / 2);
        transform.position = new Vector3(clampedX, clampedY, 0);
    }
}