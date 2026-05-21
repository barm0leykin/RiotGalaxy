using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RiotGalaxy.GameObjects;

namespace RiotGalaxy.Components
{
    /// <summary>
    /// Базовый класс для компонентов стрельбы
    /// Реализует паттерн Стратегия
    /// </summary>
    public abstract class ShootingComponent
    {
        protected GameObject _owner;
        protected float _fireRate;
        protected float _fireTimer = 0f;
        
        public float FireRate 
        { 
            get => _fireRate; 
            set 
            { 
                _fireRate = value <= 0 ? 0.1f : value; 
            } 
        }
        
        public ShootingComponent(GameObject owner, float fireRate)
        {
            _owner = owner;
            _fireRate = fireRate;
        }
        
        public abstract void Update(GameTime gameTime);
        
        /// <summary>
        /// Попытка выстрелить
        /// </summary>
        public virtual bool TryShoot()
        {
            if (_fireTimer <= 0f)
            {
                _fireTimer = 1f / _fireRate;
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Создание снаряда
        /// </summary>
        protected virtual Bullet CreateBullet(Vector2 position, Vector2 direction)
        {
            Bullet bullet = new Bullet(position, direction, _owner);
            return bullet;
        }
    }
    
    /// <summary>
    /// Компонент стрельбы для игрока
    /// </summary>
    public class PlayerShootingComponent : ShootingComponent
    {
        private WeaponType _currentWeapon;
        private List<Bullet> _bullets;
        
        public WeaponType CurrentWeapon 
        { 
            get => _currentWeapon; 
            set 
            { 
                _currentWeapon = value;
                UpdateFireRate();
            } 
        }
        
        public List<Bullet> Bullets => _bullets;
        
        public PlayerShootingComponent(GameObject owner) : base(owner, 2.0f)
        {
            _currentWeapon = WeaponType.Cannon;
            _bullets = new List<Bullet>();
            UpdateFireRate();
        }
        
        public override void Update(GameTime gameTime)
        {
            // Обновляем таймер стрельбы
            if (_fireTimer > 0f)
                _fireTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Обновляем все пули
            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                Bullet bullet = _bullets[i];
                bullet.Update(gameTime);
                
                // Удаляем пули за пределами экрана или неактивные
                if (!bullet.IsActive)
                {
                    _bullets.RemoveAt(i);
                }
            }
        }
        
        /// <summary>
        /// Выстрел игроком
        /// </summary>
        public void Fire()
        {
            if (!TryShoot())
                return;
                
            Vector2 bulletPosition = _owner.Position;
            
            switch (_currentWeapon)
            {
                case WeaponType.Cannon:
                    CreateCannonShot(bulletPosition);
                    break;
                case WeaponType.MachineGun:
                    CreateMachineGunShot(bulletPosition);
                    break;
                case WeaponType.Laser:
                    CreateLaserShot(bulletPosition);
                    break;
            }
        }
        
        private void CreateCannonShot(Vector2 position)
        {
            Vector2 direction = -Vector2.UnitY; // Вверх
            Bullet bullet = CreateBullet(position, direction);
            bullet.Damage = 10;
            bullet.Speed = 10f;
            bullet.Size = new Vector2(8, 16);
            _bullets.Add(bullet);
        }
        
        private void CreateMachineGunShot(Vector2 position)
        {
            Vector2 direction = -Vector2.UnitY; // Вверх
            Bullet bullet = CreateBullet(position, direction);
            bullet.Damage = 3;
            bullet.Speed = 15f;
            bullet.Size = new Vector2(6, 12);
            _bullets.Add(bullet);
        }
        
        private void CreateLaserShot(Vector2 position)
        {
            Vector2 direction = -Vector2.UnitY; // Вверх
            Bullet bullet = CreateBullet(position, direction);
            bullet.Damage = 20;
            bullet.Speed = 20f;
            bullet.Size = new Vector2(4, 30);
            bullet.IsPiercing = true; // Лазер проходит через врагов
            _bullets.Add(bullet);
        }
        
        private void UpdateFireRate()
        {
            switch (_currentWeapon)
            {
                case WeaponType.Cannon:
                    FireRate = 2.0f;
                    break;
                case WeaponType.MachineGun:
                    FireRate = 10.0f;
                    break;
                case WeaponType.Laser:
                    FireRate = 1.0f;
                    break;
            }
        }
    }
    
    /// <summary>
    /// Компонент стрельбы для врагов
    /// </summary>
    public class EnemyShootingComponent : ShootingComponent
    {
        private ShootingPattern _pattern;
        private float[] _patternParameters;
        private float _patternTimer = 0f;
        private Vector2 _targetPosition;
        
        public ShootingPattern Pattern 
        { 
            get => _pattern; 
            set 
            { 
                _pattern = value; 
                _patternTimer = 0f;
            } 
        }
        
        public Vector2 TargetPosition 
        { 
            get => _targetPosition; 
            set => _targetPosition = value; 
        }
        
        public EnemyShootingComponent(GameObject owner, float fireRate, ShootingPattern pattern = ShootingPattern.Direct) 
            : base(owner, fireRate)
        {
            _pattern = pattern;
            _patternParameters = new float[0];
            _targetPosition = Vector2.Zero;
        }
        
        public override void Update(GameTime gameTime)
        {
            _patternTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Обновляем таймер стрельбы
            if (_fireTimer > 0f)
                _fireTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Автоматическая стрельба в зависимости от паттерна
            AutoFire(gameTime);
        }
        
        private void AutoFire(GameTime gameTime)
        {
            switch (_pattern)
            {
                case ShootingPattern.Direct:
                    // Стрельба прицеленной прямо на игрока
                    break;
                case ShootingPattern.Fixed:
                    // Стрельба в фиксированном направлении
                    break;
                case ShootingPattern.Pattern:
                    // Стрельба по определенному паттерну
                    FireByPattern();
                    break;
            }
        }
        
        private void FireByPattern()
        {
            // Реализация стрельбы по паттерну
            // Например, веерная стрельба или круговые волны
        }
        
        /// <summary>
        /// Создание снаряда врага
        /// </summary>
        protected virtual Bullet CreateEnemyBullet(Vector2 position, Vector2 direction)
        {
            Bullet bullet = new Bullet(position, direction, _owner);
            bullet.Damage = 5;
            bullet.Speed = 5f;
            bullet.Size = new Vector2(10, 10);
            bullet.IsEnemyBullet = true;
            return bullet;
        }
    }
    
    /// <summary>
    /// Паттерны стрельбы врагов
    /// </summary>
    public enum ShootingPattern
    {
        Direct,    // Стрельба прямо в цель
        Fixed,     // Стрельба в фиксированном направлении
        Pattern    // Стрельба по заданному паттерну
    }
}
