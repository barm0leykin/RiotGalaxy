using CocosSharp;
using RiotGalaxy.Interface;


namespace RiotGalaxy.Objects
{
    class BonusHpUp : Bonus
    {
        public BonusHpUp(CCPoint pos)
        {
            System.Diagnostics.Debug.WriteLine("=== ObjCreated: BonusBulletUP ===");

            bonusType = BonusType.HP_UP;

            // Sprite
            draw.LoadGraphics("bonusHPUp.png");

            Position = pos;
        }
        override public void ApplyBonus(GameObject obj)
        {
            CommandHpUp cmd = new CommandHpUp();
            cmd.Execute();  

            //GameManager.gameplay.playerShip.Hp = GameManager.player.MaxHp;
            needToDelete = true;
        }
    }
}