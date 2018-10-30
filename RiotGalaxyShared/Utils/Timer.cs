using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace RiotGalaxy
{
    class Timer
    {
        [DllImport("kernel32.dll")]
        private static extern long GetTickCount();

        private int StartTick = 0;
        private int PrevTick = 0;
        private int CurrentTick = 0;

        public Timer()
        {
            Reset();
        }

        public void Reset()
        {
            StartTick = Environment.TickCount; //GetTickCount();
            PrevTick = StartTick;
        }

        public float GetTicks()
        {            
            CurrentTick = Environment.TickCount; //GetTickCount();
            float Ticks = CurrentTick - PrevTick;
            return Ticks;
        }
        public float GetSeconds()
        {
            float Seconds = GetTicks() / 1000000; //по другой версии 4000, надо проверить
            return Seconds;
        }


    }
}