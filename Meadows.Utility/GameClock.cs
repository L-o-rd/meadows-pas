using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Meadows.Utility
{
    public class GameClock
    {
        private double totalMinutes;

        public GameClock()
        {
            totalMinutes = 6 * 60;
        }

        public int TotalMinutes => (int)totalMinutes;

        public int Day => (int)(totalMinutes / (60 * 24)) + 1;

        public int Hour => ((int)(totalMinutes / 60)) % 24;

        public int Minute => ((int)totalMinutes) % 60;

        private readonly DateTime startDate = new DateTime(2000, 1, 1);

        public void Update(GameTime gameTime)
        {
            totalMinutes += gameTime.ElapsedGameTime.TotalSeconds * 10;
        }

        public string GetTimeString() => $"{Hour:D2}:{Minute:D2}";

        public string GetDayLabel() => $"Day {Day}";

        public string GetDateString()
        {
            DateTime date = startDate.AddDays(Day - 1);
            return date.ToString("d MMMM");
        }

        public float GetNormalizedTime() => ((float)totalMinutes % 1440f) / 1440f; 
    }



}
