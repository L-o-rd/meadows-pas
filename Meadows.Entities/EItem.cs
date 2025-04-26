using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Meadows.Items;
using Meadows.Utility;

namespace Meadows.Entities {
    public class EItem : Entity {
        public double xa, ya, za;
        public double xx, yy, zz;
        private int lifeTime;
        private int ticks;
        public Item item;

        public EItem(Item item, int x, int y) {
            this.item = item;
            xx = this.x = x;
            yy = this.y = y;
            xr = 3;
            yr = 3;

            zz = 2;
            xa = RNG.NextDouble() * 0.3;
            ya = RNG.NextDouble() * 0.2;
            za = RNG.NextDouble() * 0.7 + 1;

            lifeTime = 60 * 10 + RNG.Next(60);
            ticks = 0;
        }

        public override void Update(GameTime dt) {
            base.Update(dt);
            if (this.ticks >= lifeTime) {
                this.Removed = true;
                return;
            }

            ++ticks;
            xx += xa;
            yy += ya;
            zz += za;
            if (zz < 0) {
                zz = 0;
                za *= -0.5;
                xa *= 0.6;
                ya *= 0.6;
            }

            za -= 0.15;
            int ox = x;
            int oy = y;
            int nx = (int) xx;
            int ny = (int) yy;
            int expectedx = nx - x;
            int expectedy = ny - y;
            Move(nx - x, ny - y, dt);
            int gotx = x - ox;
            int goty = y - oy;
            xx += gotx - expectedx;
            yy += goty - expectedy;
        }

        public override void Draw(SpriteBatch batch) {
            if ((ticks >= lifeTime - 6 * 20) && (ticks / 6 % 2 == 0))
                return;

            item.Draw(batch, this.x - 10, this.y - 10, Color.Black);
            item.Draw(batch, this.x - 10, this.y - 10 - (int) (zz), Color.Wheat);
        }

        protected override void TouchedBy(Entity e, GameTime dt) {
            if (Removed) return;
            if (ticks > 30)
                e.TouchItem(this, dt);
        }

        public void Take(Player player, GameTime dt) {
            Sound.Play(dt, "Collect", pitch: -.5f, cooldown: 0.05f);
            item.OnTake(this);
            Removed = true;
        }
    }
}
