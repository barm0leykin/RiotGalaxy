using CocosSharp;
using System;
using RiotGalaxy;
using RiotGalaxy.Objects.Weapons;

namespace RiotGalaxy.Object.Weapons
{
    class Bullet : Shell
    {
        //CCSprite sprite_op;
        public Bullet(CCPoint point) : base(point)
        {
            /*sprite_op = GameManager.sLoader.Load("bullet_glow.png");            
            sprite_op.AnchorPoint = CCPoint.AnchorMiddle;  // Making the Sprite be centered makes positioning easier.
            sprite_op.Opacity = 50;
            sprite_op.Scale = 1.5f;
            AddChild(sprite_op);*/

            draw.LoadGraphics("bullet.png");
            /*sprite = GameManager.sLoader.Load("bullet.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;  // Making the Sprite be centered makes positioning easier.
            AddChild(sprite);*/
            Hp = 1;
            Damage = 10;

            //CCAudioEngine.SharedEngine.PlayEffect("fire1");
        }
    }
}