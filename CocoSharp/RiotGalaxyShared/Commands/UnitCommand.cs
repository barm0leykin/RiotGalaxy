using CocosSharp;
using RiotGalaxy;
using RiotGalaxy.Objects;
using System.Collections.Generic;

namespace RiotGalaxy
{
    interface UnitCommand
    {
        void Execute(GameObject obj);
        //public abstract void Undo();
    }

    class CommandFire : UnitCommand
    {
        //Unit receiver;
        public CommandFire()
        {
            //receiver = r;
        }
        public void Execute(GameObject u)
        {
            //u.Fire();
            //receiver.Operaiton();
        }
    }
    class CommandDie : UnitCommand
    {
        public CommandDie()
        {
        }
        public void Execute(GameObject u)
        {
            //u.Die();
            //receiver.Operaiton();
        }
    }
}