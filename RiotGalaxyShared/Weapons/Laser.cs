using CocosSharp;
using System.Collections;
using RiotGalaxy.Objects;

namespace RiotGalaxy.Weapons
{
    class Laser : Shell
    {
        ArrayList hitTargets; //необходимо хранить список пораженных целей чтобы не бить их постоянно

        public Laser(CCPoint point) : base(point)
        {
            name += "_Laser";
            draw.LoadGraphics("laser.png");

            Hp = 1;
            Damage = 10;

            hitTargets = new ArrayList();
        }
        public override bool Collision(GameObject obj)
        {
            //if (obj.objectType == ObjType.BONUS) // бонусы не подбиваем)
            //    return false;
            if (playerSide == obj.playerSide) // откл фрэндли-файр
                return false;
            foreach (GameObject target in hitTargets) // если этому объекту уже был нанесен урон - выходим
            {
                if (obj.name == target.name)
                    return false;
            }
            hitTargets.Add(obj); // если этому объекту нанесен урон, доблавим его в список, чтобы не фигачить его каждый цикл
            obj.Hit((int)Damage); // наносим урон цели

            return true;            
        }
        public override void Hit(GameObject obj)
        {
            //base.Hit(obj);
            needToDelete = false; // лазер не самоуничтожается, а летит насквозь
        }
    }
}
