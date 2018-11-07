using System;
using CocosSharp;
using RiotGalaxy.Objects.Weapons;

namespace RiotGalaxy.Factories
{
    /*class BulletFactory
    {
        static Lazy<BulletFactory> self =
            new Lazy<BulletFactory>(() => new BulletFactory());

        // simple singleton implementation
        public static BulletFactory Self
        {
            get
            {
                return self.Value;
            }
        }

        public event Action<Bullet> BulletCreated;

        private BulletFactory()
        {
        }

        public Bullet CreateNew(CCPoint point)
        {
            Bullet newBullet = new Bullet(point);
            BulletCreated?.Invoke(newBullet);
            GameManager.gameplay.allObjects.Add(newBullet);
            return newBullet;
        }
    }*/
}