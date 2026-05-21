using CocosSharp;
using RiotGalaxy.Interface;

namespace RiotGalaxy.Objects
{
    class BonusNukeBomb : Bonus
    {
        //private int damage = 100;
        public BonusNukeBomb(CCPoint pos)
        {
            bonusType = BonusType.NUKE_BOMB;
            Position = pos;

            // Sprite
            draw.LoadGraphics("bonusNukeBomb.png");
        }
        override public void ApplyBonus(GameObject obj)
        {
            CommandKillAll cmd = new CommandKillAll();
            cmd.Execute();
            needToDelete = true;
        }
    }   
    
}