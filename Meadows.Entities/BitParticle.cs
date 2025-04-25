using Meadows.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Meadows.Entities {
    public class BitParticle : Entity {
        private static readonly Texture2D Blank;
        public double xx, yy, zz;
        public double xa, ya, za;
        private int time = 0;
        private Color color;

        static BitParticle() {
            int radius = 5;
            int diameter = radius * 2;
            Blank = new Texture2D(Main.Graphics.GraphicsDevice, diameter, diameter);
            Color[] data = new Color[diameter * diameter];

            float center = radius;
            float maxDist = radius;

            for (int _y = 0; _y < diameter; _y++) {
                for (int _x = 0; _x < diameter; _x++) {
                    float dx = _x - center;
                    float dy = _y - center;
                    double dist = Math.Sqrt(dx * dx + dy * dy);
                    double normalized = dist / maxDist;
                    float alpha = normalized < 1f ? 1f - MathF.Pow((float) normalized, 3f) : 0f;
                    data[_y * diameter + _x] = new Color((float)alpha, (float)alpha, (float)alpha, (float) alpha);
                }
            }

            Blank.SetData<Color>(data);
        }

        public BitParticle(int x, int y, Color color) {
            this.color = color;
            this.x = x;
            this.y = y;

            xx = x;
            yy = y;
            zz = 2;
            xa = RNG.NextDouble() * 0.7 - 0.35;
            ya = RNG.NextDouble() * 0.5 - 0.25;
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
            x = (int)xx;
            y = (int)yy;
            ++time;
        }

        public override void Draw(SpriteBatch batch) {
            var position = new Vector2(x - 5 - Tiles.Tiles.xo, y - 5 - (float)zz - Tiles.Tiles.yo);
            batch.Draw(Blank, position + Vector2.One, new Rectangle(0, 0, 10, 10), Color.Black);
            batch.Draw(Blank, position, new Rectangle(0, 0, 10, 10), color);
            base.Draw(batch);
        }
    }
}
