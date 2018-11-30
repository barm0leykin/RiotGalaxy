using System;
using System.Threading.Tasks;
using CocosSharp;
using RiotGalaxy;
using RiotGalaxy.Factories;
using RiotGalaxy.SFX;
using RiotGalaxy.Weapons;
using RiotGalaxy.Objects.ObjBehavior;

namespace RiotGalaxy.Objects
{
    public class Enemy : GameObject
    {
        public enum EnemyType : int { RND = 0, SM_SCOUT, BLUE, GREEN, RED }
        public EnemyType enemyType = EnemyType.RND;
        public Route route;
        public BehAI ai;
        protected float ShootInterval = 3;
        protected float ActionTime = 0;

        public Enemy()
        {
            objectType = ObjType.ENEMY;
            playerSide = false;
            MaxSpeed = 100;
            CurrentSpeed = 100;

            gun = new WeaponCannon(this);
            coll = new BehHardCollision(this);
            ai = new ObjBehAIDumd(this);
            draw = new ObjBehDrawSprite(this);
            route = new Route();

            GameManager.level.enemySpawned++;            
        }        
        public override bool Collision(GameObject obj)
        {
            if (obj.playerSide && obj.objectType != ObjType.SHELL) // выкл фрэндли-фаер :) и не получаем урон от оружия (оно само нанесет), только от столкновений
                Hit((int)obj.Damage);
            return true;
        }
        public override void Hit(int damage)
        {
            System.Diagnostics.Debug.WriteLine("=== Enemy hit!: " + damage +" xp");
            Hp -= damage;
            if (Hp <= 0)
                Die();
        }
        private void Die()
        {
            //CCAudioEngine.SharedEngine.PlayEffect("explode1");
            //GameManager.player.Score += 10;
            System.Diagnostics.Debug.WriteLine("=== Enemy die...");
            //GameManager.level.enemyKilled++; ///
            //GameManager.level.enemyRemain--;
            needToDelete = true;
        }
    }
}