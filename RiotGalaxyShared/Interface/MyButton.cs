using RiotGalaxy;
using RiotGalaxy.Objects;
using CocosSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using RiotGalaxy.Commands;

namespace RiotGalaxy.Interface
{
    public class MyButton : CCNode
    {
        protected ICommand cmd;
        public CCSprite sprite;
        protected int delay = 1000;
        bool blocked = false;
        public string name;

        public MyButton(CCPoint pos)
        {
            
            Position = pos;
            cmd = new NoCommand();
            //sprite = GameManager.sLoader.Load("btn_empty.png");
        }
        public void Delete()
        {
            RemoveAllChildren();
        }
        async public void Press()
        {
            if (!blocked)
            {
                System.Diagnostics.Debug.WriteLine("=== Button pressed ===");
                cmd.Execute();
                blocked = true;
                await Task.Delay(delay);
                blocked = false;
            }            
        }
        public bool CheckCollision(CCPoint touchPoint)
        {
            CCRect rect = GetRect();
            if (touchPoint.X > rect.LowerLeft.X && touchPoint.X < rect.UpperRight.X)
                if (touchPoint.Y > rect.LowerLeft.Y && touchPoint.Y < rect.UpperRight.Y)
                    return true;
            return false;
        }
        virtual public CCRect GetRect()
        {
            float w = sprite.ContentSize.Width * sprite.ScaleX, h = sprite.ContentSize.Height * sprite.ScaleY;
            CCRect cr = new CCRect(PositionX - w / 2, PositionY - h / 2, w, h);
            return cr;
        }
    }

    public class ButtonKillAll : MyButton
    {
        public ButtonKillAll(CCPoint pos) : base(pos)
        {
            sprite = GameManager.sLoader.Load("btn_killall.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);

            cmd = new CommandKillAll();
        }
    }

    public class ButtonWin : MyButton
    {
        public ButtonWin(CCPoint pos) : base(pos)
        {
            System.Diagnostics.Debug.WriteLine("=== ButtonWin ===");
            sprite = GameManager.sLoader.Load("btn_win.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);

            cmd = new CommandWin();
        }
    }
    public class ButtonPause : MyButton
    {
        public ButtonPause(CCPoint pos) : base(pos)
        {
            System.Diagnostics.Debug.WriteLine("=== ButtonWin ===");
            sprite = GameManager.sLoader.Load("btn_pause.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);
            
            cmd = new CommandSwitchPause();
        }
    }
    class ButtonPlayerPause : MyButton
    {
        /// <summary>
        /// Это прозрачная кнопка висит на спрайте коробля игрока, по ее нажатии игра ставится на паузу, открывается меню выбора оружия
        /// </summary>
        GameObject owner;
        public ButtonPlayerPause(CCPoint pos, GameObject obj) : base(pos)
        {            
            System.Diagnostics.Debug.WriteLine("=== ButtonPlayerPause ===");
            sprite = GameManager.sLoader.Load("btn_empty.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);

            delay = 500;
            owner = obj;
            cmd = new CommandPauseWeaponMenu();
        }
        override public CCRect GetRect()
        {
            float w = sprite.ContentSize.Width * sprite.ScaleX, h = sprite.ContentSize.Height * sprite.ScaleY;
            
            // тк кнопка движется вместе с объектом, необходимо суммировать координаты владельца
            CCRect cr = new CCRect(PositionX +owner.PositionX - w / 2, PositionY + owner.PositionY - h / 2, w, h);
            return cr;
        }
    }

    public class ButtonCannon : MyButton
    {
        public ButtonCannon(CCPoint pos) : base(pos)
        {
            //System.Diagnostics.Debug.WriteLine("=== ButtonCannon ===");
            GameManager.gameplay.iface.Message("=== ButtonCannon ===");
            name = "btn_cannon";
            sprite = GameManager.sLoader.Load("btn_cannon.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);

            cmd = new CommandChWeaponCannon();
        }
    }
    public class ButtonAutoCannon : MyButton
    {
        public ButtonAutoCannon(CCPoint pos) : base(pos)
        {
            System.Diagnostics.Debug.WriteLine("=== ButtonAutoCannon ===");
            name = "btn_auto_cannon";
            sprite = GameManager.sLoader.Load("btn_auto_cannon.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);

            cmd = new CommandChWeaponAutoCannon();
        }
    }
    public class ButtonMinigun : MyButton
    {
        public ButtonMinigun(CCPoint pos) : base(pos)
        {
            System.Diagnostics.Debug.WriteLine("=== ButtonMinigun ===");
            name = "btn_minigun";
            sprite = GameManager.sLoader.Load("btn_minigun.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);

            cmd = new CommandChWeaponMinigun();
        }
    }
    public class ButtonLaser : MyButton
    {
        public ButtonLaser(CCPoint pos) : base(pos)
        {
            System.Diagnostics.Debug.WriteLine("=== ButtonLaser ===");
            name = "btn_laser";
            sprite = GameManager.sLoader.Load("btn_laser.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);

            cmd = new CommandChWeaponLaser();
        }
    }



    public class ButtonUpgradeGun : MyButton
    {
        public ButtonUpgradeGun(CCPoint pos) : base(pos)
        {
            System.Diagnostics.Debug.WriteLine("=== ButtonUpgradeGun ===");
            sprite = GameManager.sLoader.Load("btn_BulletUp.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);

            cmd = new CommandUpgradeGun();
            //cmd = new CommandPause();
        }
    }

    public class ButtonHpUP : MyButton
    {
        public ButtonHpUP(CCPoint pos) : base(pos)
        {
            System.Diagnostics.Debug.WriteLine("=== ButtonHpUP ===");
            sprite = GameManager.sLoader.Load("btn_hp_up.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);

            cmd = new CommandHpUp ();
            //cmd = new CommandResume();
        }
    }

    //==========================
    class Button : CCNode
    {

        CCSprite buttonSprite;

        public event TriggeredHandler Triggered;
        // A delegate type for hooking up button triggered events
        public delegate void TriggeredHandler(object sender, EventArgs e);


        private Button()
        {
            AttachListener();
        }

        public Button(CCSprite sprite, CCLabel label)
            : this()
        {
            this.ContentSize = sprite.ScaledContentSize;
            sprite.AnchorPoint = CCPoint.AnchorLowerLeft;
            label.Position = sprite.ContentSize.Center;

            // Create the render texture to draw to.  It will be the size of the button background sprite
            var render = new CCRenderTexture(sprite.ContentSize, sprite.ContentSize);

            // Clear it to any background color you want
            render.BeginWithClear(CCColor4B.Transparent);

            // Render the background sprite to the render texture
            sprite.Visit();

            // Render the label to the render texture
            label.Visit();

            // End the rendering
            render.End();

            // Add the button sprite to this node so it can be rendered
            buttonSprite = render.Sprite;
            buttonSprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(this.buttonSprite);
        }

        void AttachListener()
        {
            // Register Touch Event
            var listener = new CCEventListenerTouchOneByOne();
            listener.IsSwallowTouches = true;

            listener.OnTouchBegan = OnTouchBegan;
            listener.OnTouchEnded = OnTouchEnded;
            listener.OnTouchCancelled = OnTouchCancelled;

            AddEventListener(listener, this);
        }

        bool touchHits(CCTouch touch)
        {
            var location = touch.Location;

            var area = buttonSprite.BoundingBox;
            return area.ContainsPoint(buttonSprite.WorldToParentspace(location));

        }

        bool OnTouchBegan(CCTouch touch, CCEvent touchEvent)
        {
            bool hits = touchHits(touch);
            if (hits)
            {
                // undo the rotation that was applied by the action attached.
                Rotation = 0;
                scaleButtonTo(0.9f);
            }

            return hits;
        }

        void OnTouchEnded(CCTouch touch, CCEvent touchEvent)
        {
            bool hits = touchHits(touch);
            if (hits && Triggered != null)
                Triggered(this, EventArgs.Empty);
            scaleButtonTo(1);
        }

        void OnTouchCancelled(CCTouch touch, CCEvent touchEvent)
        {
            scaleButtonTo(1);
        }

        void scaleButtonTo(float scale)
        {
            var action = new CCScaleTo(0.1f, scale);
            action.Tag = 900;
            StopAction(900);
            RunAction(action);
        }
    }
}