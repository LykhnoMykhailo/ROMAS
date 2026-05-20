using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Enemy _enemy;
    private Transform _player;

    public float detectionRange = 10f;
    public bool isAlerted = false;

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");

        if (pObj != null)
        {
            _player = pObj.transform;
            Debug.Log($"<color=green>[AI]</color> {gameObject.name}: √равц€ знайдено. √отовий до ви€вленн€ в рад≥ус≥ {detectionRange}.");
        }
        else
        {
            Debug.LogError($"<color=red>[AI]</color> {gameObject.name}:  –»“»„Ќќ! ќб'Їкт з тегом 'Player' не знайдено на сцен≥.");
        }
    }

    void Update()
    {
        if (_player == null || !_enemy.stats.is_alive()) return;
        // —творюЇмо чист≥ Vector2 координати, щоб Z-в≥сь не ламала математику
        Vector2 enemyPos2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 playerPos2D = new Vector2(_player.position.x, _player.position.y);

        float distance = Vector2.Distance(enemyPos2D, playerPos2D);

        // “имчасовий спам у консоль, щоб побачити –≈јЋ№Ќ” цифру в≥дстан≥ п≥д час руху
        if (!isAlerted && distance <= detectionRange)
        {
            isAlerted = true;
            Debug.Log($"<color=red>![AI EVENT]!</color> {_enemy.stats.Pname} пом≥тив гравц€! ƒистанц≥€: {distance:F1}");
        }

        if (isAlerted)
        {
            float distance_attack = Vector2.Distance(transform.position, _player.position);

            // якщо п≥д≥йшли на дистанц≥ю атаки (наприклад, 8 одиниць)
            if (distance_attack <= 8f)
            {
                GetComponent<EnemyAttack>().TryAttack();
            }

            _enemy.Movement.SetTarget(_player.position, _enemy.stats.speed);
        }
    }
}