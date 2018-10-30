using CocosSharp;
using RiotGalaxy.Commands;
using RiotGalaxy.Interface;
using RiotGalaxy.Objects.ObjBehavior;

namespace RiotGalaxy.Objects
{
    class BonusBulletUP : Bonus
    {
        public BonusBulletUP(CCPoint pos)
        {
            System.Diagnostics.Debug.WriteLine("=== ObjCreated: BonusBulletUP ===");

            bonusType = BonusType.BULLET_UP;
            Position = pos;
            
            draw.LoadGraphics("bonusBulletUp.png");
            // Sprite
            /*sprite = GameManager.sLoader.Load("bonusBulletUp.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);*/
            //SetRect();
        }
        override public void ApplyBonus(GameObject obj)
        {
            CommandUpgradeGun cmd = new CommandUpgradeGun();
            cmd.Execute();

            if (GameManager.player.GunFireRate > 0.3f )
                GameManager.player.GunFireRate -= 0.1f; // уменьшаем интервал между выстрелами
            if (GameManager.player.GunFireRate < 0.3f)
                GameManager.player.GunFireRate = 0.3f;

            GameManager.gameplay.playerShip.Upgrade();
            needToDelete = true;
        }
    }
}