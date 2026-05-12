using UnityEngine;

// Це корінь всієї твоєї системи
public abstract class Object : MonoBehaviour
{
    [Header("Base Object Data")]
    public string id;
    public string displayName;

    // Спільний метод для всіх: від кулі до монстра
    public virtual void Initialize()
    {
        // Базова логіка ініціалізації
    }
}