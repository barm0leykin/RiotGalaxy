using Microsoft.Xna.Framework;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Снаряд пушки (cannon). Аналог Bullet из CocosSharp.
    /// </summary>
    public class Bullet : Shell
    {
        public Bullet(Vector2 position) : base(position)
        {
            LoadSprite("Images/bullet");
            Hp = 1;
            Damage = 10;
        }
    }
}
