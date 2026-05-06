using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace map_test
{
    public class Location_tile
    {
        // Унікальний ідентифікатор локації
        public string Id { get; set; }

        // Шлях до текстури для відображення
        public string Texture { get; set; }

        // Координати розташування локації на сітці мапи
        public int cords_x { get; set; }
        public int cords_y { get; set; }

        // Посилання на конкретне завдання (нейтральне або агресивне)
        public Task_Location Task { get; set; }

        /// <summary>
        /// Метод для повної ініціалізації локації. 
        /// Аналог твого методу для завантаження тайлів.
        /// </summary>
        public void Setup_Location(string id, string texture, int x, int y, Task_Location task = null)
        {
            this.Id = id;
            this.Texture = texture;
            this.cords_x = x;
            this.cords_y = y;
            this.Task = task;
        }

        /// <summary>
        /// Взаємодія з локацією (наприклад, при натисканні клавіші дії)
        /// </summary>
        public void Interact()
        {
            if (Task != null)
            {
                Task.OnInteract();
            }
            else
            {
                Console.WriteLine($"Локація {Id} не має призначеного завдання.");
            }
        }
    }
}
