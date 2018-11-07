using CocosSharp;

namespace RiotGalaxy.Objects.Weapons
{
    class Bullet : Shell
    {
        public Bullet(CCPoint point) : base(point)
        {
            draw.LoadGraphics("bullet.png");
            Hp = 1;
            Damage = 10;

            //CCAudioEngine.SharedEngine.PlayEffect("fire1");
        }
    }
}