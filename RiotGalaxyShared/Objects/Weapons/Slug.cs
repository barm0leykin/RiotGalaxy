using CocosSharp;

namespace RiotGalaxy.Objects.Weapons
{
    class Slug : Shell
    {
        public Slug(CCPoint point) : base(point)
        {
            draw.LoadGraphics("slug.png");
            Hp = 1;
            Damage = 4;

            //CCAudioEngine.SharedEngine.PlayEffect("fire1");
        }
    }
}
