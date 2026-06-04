using Microsoft.Xna.Framework;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Снаряд пулемёта (minigun). Аналог Slug из CocosSharp.
    /// </summary>
    public class Slug : Shell
    {
        public Slug(Vector2 position) : base(position)
        {
            LoadSprite("Images/slug");
            Hp = 1;
            Damage = 4;
        }
    }
}
