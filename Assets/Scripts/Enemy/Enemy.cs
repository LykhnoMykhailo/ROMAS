using UnityEngine;
using GameCore.Entities;

public class Enemy : Subject
{
    // Посилання на інші компоненти бота
    public EnemyAI Ai { get; private set; }
    public EnemyMovement Movement { get; private set; }
    // У класі Enemy.cs
    protected override void Die()
    {
        Debug.Log($"<color=green>[Enemy]</color> {gameObject.name} помер, видаю золото.");

        // Нараховуємо золото гравцю
        if (GameManager.Instance != null && GameManager.Instance.currentPlayer != null)
        {
            GameManager.Instance.currentPlayer.AddMoney(3f);
            Debug.Log($"[Drop] +3 золота. Поточний баланс: {GameManager.Instance.currentPlayer.money}");
            GameManager.Instance.currentPlayer.add_exp(3f);
            Debug.Log($"[Drop] + 3 exp");
        }

        // Викликаємо базову логіку знищення об'єкта
        base.Die();
    }
    public override void Initialize(Puppet data)
    {
        base.Initialize(data);

        Movement = GetComponent<EnemyMovement>();
        Ai = GetComponent<EnemyAI>();

        // ЛОГ: Перевірка ініціалізації
        if (Movement != null && Ai != null)
        {
            Debug.Log($"<color=green>[Enemy]</color> {gameObject.name}: Модулі руху та ШІ успішно підключені.");
        }
        else
        {
            Debug.LogError($"<color=red>[Enemy]</color> {gameObject.name}: Відсутній скрипт EnemyMovement або EnemyAI на об'єкті!");
        }

        if (stats.speed <= 0)
        {
            stats.speed = 3.5f;
            Debug.LogWarning($"<color=yellow>[Enemy]</color> Швидкість була 0, встановлено дефолтну: {stats.speed}");
        }

        ApplyVisuals(data);
    }

    private void ApplyVisuals(Puppet data)
    {
        // Налаштування масштабу
        float finalSize = (stats.size > 0.01f) ? stats.size : 1.2f;
        transform.localScale = new Vector3(finalSize, finalSize, 1f);

        // Налаштування спрайту (якщо шлях є в JSON)
        if (data.textures != null && data.textures.Count > 0)
        {
            var sr = GetComponent<SpriteRenderer>();
            Sprite s = Resources.Load<Sprite>(data.textures[0]);
            if (s != null) sr.sprite = s;
        }
    }

}