using Microsoft.Xna.Framework;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Снаряд лазера. Аналог Laser из CocosSharp — летит насквозь (IsPiercing),
    /// не самоуничтожается при попадании. Логика урона/пробития целей появится
    /// на этапе столкновений (этап 7).
    /// </summary>
    public class Laser : Shell
    {
        public Laser(Vector2 position) : base(position)
        {
            LoadSprite("Images/laser");
            Hp = 1;
            Damage = 10;
            IsPiercing = true;
        }
    }
}
