using Meadows.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Meadows.Entities {
    public class Text : Entity {
        public double xx, yy, zz;
        public double xa, ya, za;
        private int time = 0;
        private Color color;
        private String msg;

        public Text(String msg, int x, int y, Color color) {
            this.color = color;
            this.msg = msg;
            this.x = x;
            this.y = y;

            xx = x;
            yy = y;
            zz = 2;
            xa = RNG.NextDouble() * 0.3;
            ya = RNG.NextDouble() * 0.2;
            za = RNG.NextDouble() * 0.7 + 2;
        }

        public override void Update(GameTime dt) {
            base.Update(dt);
            if (time > 60) {
                Removed = true;
                return;
            }

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
            x = (int) xx;
            y = (int) yy;
            ++time;
        }

        public override void Draw(SpriteBatch batch) {
            var size = Resources.Particle.MeasureString(msg);
            var position = new Vector2(x - (size.X * 0.5f) - Tiles.Tiles.xo, y - (size.Y * 0.5f) - (float) zz - Tiles.Tiles.yo);
            batch.DrawString(Resources.Particle, msg, position + Vector2.One, Color.Black);
            batch.DrawString(Resources.Particle, msg, position, color);
            base.Draw(batch);
        }
    }
}
