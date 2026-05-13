using UnityEngine;
using GameCore.Entities;

public class Enemy : Subject
{
    [Header("AI Settings")]
    public bool isAlerted = false;
    public float detectionRange = 5f;
    public float stopDistance = 1.2f;

    private Transform playerTransform;

    public override void Initialize(Puppet data)
    {
        base.Initialize(data);

        // Пошук гравця
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            Debug.Log($"<color=green>[AI]</color> {gameObject.name}: Гравець знайдений.");
        }
        else
        {
            Debug.LogError($"<color=red>[AI Помилка]</color> {gameObject.name}: Об'єкт з тегом 'Player' не знайдено!");
        }
    }

    void Update()
    {
        // Перевіряємо, чи ініціалізовані стати та чи живий ворог
        if (stats == null || !stats.is_alive()) return;

        HandleAI();
    }

    protected virtual void HandleAI()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // 1. Стан виявлення
        if (!isAlerted && distanceToPlayer <= detectionRange)
        {
            isAlerted = true;
            // Виводимо повідомлення в консоль
            Debug.Log($"<color=yellow>![ПОМІТИВ]!</color> {stats.Pname} побачив гравця на відстані {distanceToPlayer:F1}");
        }

        // 2. Рух
        if (isAlerted)
        {
            MoveTowardsPlayer(distanceToPlayer);
        }
    }

    private void MoveTowardsPlayer(float distance)
    {
        if (distance > stopDistance)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;

            // Використовуємо швидкість зі статів, але якщо вона 0 — даємо базову 3.0 для тесту
            float moveSpeed = (stats.speed > 0) ? stats.speed : 3.0f;

            // РУХ: спробуй цей варіант, він працює і з Rigidbody, і без
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

            // Поворот спрайту
            if (direction.x > 0.01f) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            else if (direction.x < -0.01f) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
        else
        {
            // Якщо ми вже близько, можна додати логіку атаки
            Debug.Log($"[AI] {stats.Pname} готується до атаки!");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}