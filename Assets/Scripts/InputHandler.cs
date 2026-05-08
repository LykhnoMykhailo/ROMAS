using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputHandler : MonoBehaviour
{
    // Подія, на яку будуть підписуватися менеджери (світу або локацій)
    public static event Action<Vector2Int> OnMoveInput;

    [Header("Налаштування затримки кроку")]
    public float moveDelay = 0.2f;
    private float nextMoveTime = 0f;

    private void Update()
    {
        // Перевірка таймера затримки
        if (Time.time < nextMoveTime) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector2Int direction = Vector2Int.zero;

        // Зчитування натискань (isPressed дозволяє затиснути кнопку для бігу)
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) direction.y = 1;
         if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) direction.y = -1;
         if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) direction.x = -1;
         if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) direction.x = 1;

        if (direction != Vector2Int.zero)
        {
            nextMoveTime = Time.time + moveDelay;
            // Розсилаємо сигнал усім, хто слухає (наприклад, GameManager)
            OnMoveInput?.Invoke(direction);
        }
    }
}