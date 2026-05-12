using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    [Header("Статистика")]
    public float health = 100f;

    // Метод, який викликатиме куля при влучанні
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"<color=orange>[Dummy]</color> Шкода: {amount}. Життя: {health}");

        if (health <= 0)
        {
            Debug.Log("<color=red>[Dummy]</color> Манекен знищено!");
            // Можна замінити на анімацію смерті або просто видалити
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Додаткова перевірка через тег
        if (collision.CompareTag("Bullet"))
        {
            Debug.Log("[Dummy] Куля влучила в тригер!");
        }
    }
}