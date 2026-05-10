using System;
namespace GameCore.Entities
{
    [Serializable]
    public class Item
    {
        public string id;
        public string Pname; // Назва, яка буде відображатися в тексті
        public float price;
        public string description; // Короткий текст про предмет
    }
}