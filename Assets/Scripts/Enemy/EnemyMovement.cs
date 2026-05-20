using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float stopDistance = 1.2f;

    void Awake() => _rb = GetComponent<Rigidbody2D>();

    public void SetTarget(Vector2 targetPos, float speed)
    {
        float distance = Vector2.Distance(transform.position, targetPos);

        if (distance > stopDistance)
        {
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            _rb.linearVelocity = direction * speed;

            // Поворот (Flip)
            if (direction.x != 0)
            {
                float sX = Mathf.Abs(transform.localScale.x) * (direction.x > 0 ? 1 : -1);
                transform.localScale = new Vector3(sX, transform.localScale.y, 1);
            }
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }
}