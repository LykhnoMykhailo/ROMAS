using System;
using UnityEngine;

namespace GameCore.Entities
{
    /// <summary>
    /// Базовий клас ефектів згідно з діаграмою image_76005a.png.
    /// Реалізує систему станів, що накладаються на сутність (Puppet).
    /// </summary>
    [Serializable]
    public class BaseEffect
    {
        [Header("Основні дані")]
        public string effectName; // Назва ефекту
        public int time;    // Тривалість ефекту (в ходах або секундах)
        public string id;   // Унікальний ідентифікатор пресету

        /// <summary>
        /// Тип ефекту. Зроблено як object, щоб система могла сама обирати тип даних 
        /// (наприклад, Enum, інший клас або рядок) залежно від механіки.
        /// </summary>
        [Header("Гнучкі дані")]
        public object type;

        public BaseEffect(string name, int time, string id, object type)
        {
            this.effectName = name;
            this.time = time;
            this.id = id;
            this.type = type;
        }

        /// <summary>
        /// Створює глибоку копію ефекту.
        /// </summary>
        public BaseEffect() { }
        public BaseEffect Clone()
        {
            return new BaseEffect(this.effectName, this.time, this.id, this.type);
        }

        /// <summary>
        /// Метод для застосування логіки ефекту до істоти.
        /// Повертає рядок для логування подій.
        /// </summary>
        public virtual string apply(Puppet target)
        {
            // Базова логіка може бути порожньою або обробляти універсальні типи
            return "";
        }
    }
}