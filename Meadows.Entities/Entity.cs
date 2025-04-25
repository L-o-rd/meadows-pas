using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Meadows.Levels;
using System;
using Meadows.Items;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Meadows.Entities {
    public abstract class Entity {
        public static readonly Random RNG = new Random((int) DateTime.Now.Ticks);
        public bool Removed { get; set; }
        public int xr = 5, yr = 5;
        protected Level level;
        public int x, y;

        public void Level(Level level) {
            this.level = level;
        }

        public virtual void Update(GameTime dt) { }
        public virtual void TouchItem(EItem ie) { }
        protected virtual void TouchedBy(Entity e) { }
        public virtual void Draw(SpriteBatch batch) { }
        public virtual bool CanSwim() { return false; }
        public virtual void Hurt(Level level, Mob mob, int damage, int direction) {}

        public virtual bool Interact(Player player, Item item, int direction) {
            return item.Interact(player, this, direction);
        }

        public virtual bool Blocks(Entity e) {
            return false;
        }

        public bool Intersects(int x0, int y0, int x1, int y1) {
            return !(x + xr < x0 || y + yr < y0 || x - xr > x1 || y - yr > y1);
        }

        public virtual bool Move(int xa, int ya) {
            if (xa != 0 || ya != 0) {
                var stopped = true;
                if (xa != 0 && _Move(xa, 0)) stopped = false;
                if (ya != 0 && _Move(0, ya)) stopped = false;
                if (!stopped) {
                    int xt = x >> 5;
                    int yt = y >> 5;
                    level.GetTile(xt, yt).Stepped(level, xt, yt, this);
                }

                return !stopped;
            }

            return true;
        }

        protected bool _Move(int xa, int ya) {
            if (xa != 0 && ya != 0) 
                throw new ArgumentException($"_Move({xa}, {ya})!");

            int xto0 = ((x) - xr) >> 5;
            int yto0 = ((y) - yr) >> 5;
            int xto1 = ((x) + xr) >> 5;
            int yto1 = ((y) + yr) >> 5;

            int xt0 = ((x + xa) - xr) >> 5;
            int yt0 = ((y + ya) - yr) >> 5;
            int xt1 = ((x + xa) + xr) >> 5;
            int yt1 = ((y + ya) + yr) >> 5;
            var blocked = false;

            for (int yt = yt0; yt <= yt1; yt++) {
                for (int xt = xt0; xt <= xt1; xt++) {
                    if (xt >= xto0 && xt <= xto1 && yt >= yto0 && yt <= yto1) continue;
                    level.GetTile(xt, yt).Bumped(level, xt, yt, this);
                    if (!level.Passable(xt, yt, this)) {
                        blocked = true;
                        return false;
                    }
                }
            }

            if (blocked) 
                return false;

            List<Entity> wasInside = level.GetEntities(x - xr, y - yr, x + xr, y + yr);
            List<Entity> isInside = level.GetEntities(x + xa - xr, y + ya - yr, x + xa + xr, y + ya + yr);
            for (int i = 0; i < isInside.Count; ++i) {
                var e = isInside[i];
                if (e == this) continue;
                e.TouchedBy(this);
            }

            isInside.RemoveAll(item => wasInside.Contains(item));
            for (int i = 0; i < isInside.Count; ++i) {
                var e = isInside[i];
                if (e == this) continue;

                if (e.Blocks(this))
                    return false;
            }

            x += xa;
            y += ya;
            return true;
        }
    }
}
