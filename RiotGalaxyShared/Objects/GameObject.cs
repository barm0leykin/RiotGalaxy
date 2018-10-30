using CocosSharp;
using RiotGalaxy.Objects.Weapons;
using RiotGalaxy.Objects.ObjBehavior;
using System;

namespace RiotGalaxy.Objects
{
    public class GameObject : CCNode
    {
        public string name;        
        public Weapon gun;
        public ObjBehShoot shoot;
        public BehMove move;
        public ObjBehCollision coll;
        public ObjBehDraw draw;

        public enum ObjType : int{ OBJECT = 0, SHELL, BONUS, PLAYER, ENEMY, SFX }
        public ObjType objectType;
        public int Damage { get; set; } // Сила с которой ударяет своей тушкой по другим объектам
        public int Hp { get; set; }
        public float Width, Height;
        public float CurrentSpeed, MaxSpeed;
        public bool playerSide;                
        //public CCSprite sprite;
        //public CCLabel label;

        //sprite.contentSize.width* sprite.scaleX
        public bool needToDelete;

        public GameObject()
        {
            objectType = ObjType.OBJECT;
            name = this.objectType.ToString();
            Damage = 0;
            Hp = 0;
            playerSide = false;
            needToDelete = false;            

            shoot = new ObjBehNoShoot(this);
            gun = new NoWeapon(this);
            move = new BehMoveNone(this);
            coll = new ObjBehNoCollision(this);
            draw = new ObjBehNoDraw(this);

            /*label = new CCLabel("", "Arial", 20, CCLabelFormat.SystemFont);
            label.Color = CCColor3B.Gray;
            label.PositionY = 50;
            AddChild(label);*/

            //GameManager.gameplay.allObjects.Add(this);
            GameManager.ScGame.gameplayLayer.AddChild(this);
        }
        public void Delete()
        {
            //System.Diagnostics.Debug.WriteLine("=== Delete() === ");            
            UnscheduleAll();
            //GameManager.gameplay.allObjects.Remove(this); //
            GameManager.ScGame.gameplayLayer.RemoveChild(this);
            //ai = null;
            gun = null;
            shoot = null;
            move = null;            
            coll = null;
            /*if(sprite != null)
                sprite.Dispose();
            sprite = null;*/

            RemoveAllChildren(true);
            RemoveFromParent(true);
            RemoveAllListeners();
            needToDelete = true;            
        }
        virtual public void Activity(float time)
        {
        }
        virtual public bool Collision(GameObject obj)
        {
            return false;
        }
        virtual public void Hit(int damage)
        {
            //Hp -= damage;
        }
        public void Kill()
        {
            Hit(Hp);
            //needToDelete = true;
        }
        virtual public CCRect GetRect()
        {
            /*if(sprite == null)
            {
                CCRect p = new CCRect(PositionX - 1, PositionY - 1, 1, 1);//создадим мазонькую область
                return p;
            }*/
            return draw.GetRect();
            //float w = sprite.ContentSize.Width * sprite.ScaleX, h = sprite.ContentSize.Height * sprite.ScaleY;
            //CCRect cr = new CCRect(PositionX - w/2, PositionY - h/2, w,h);
            //return cr;
        }
    }//class
}