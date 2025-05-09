using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Meadows.Levels;
using Meadows.Items;
using System;

namespace Meadows.Entities {
    public class Bush : Entity {
        private readonly Rectangle source;
        private readonly Resource drop;
        private readonly Sheet _sheet;
        private int hurtTime = 0;
        private int health = 5;

        public Bush(int ix, int iy, int size, Sheet sheet, int x = 0, int y = 0) {
            this.source = sheet.Source2(ix, iy, size);
            this.xr = this.yr = 8;
            this._sheet = sheet;
            this.x = x;
            this.y = y;

            this.health = 5 + RNG.Next(3);
            var drop = RNG.Next(5);
            switch (drop) {
                case 1: this.drop = Resources.Carrot; break;
                case 2: this.drop = Resources.Beetroot; break;
                case 3: this.drop = Resources.RedBellPepper; break;
                case 4: this.drop = Resources.Pumpkin; break;
                case 0: default: this.drop = Resources.Potato; break;
            }
        }

        public override void Update(GameTime dt) {
            base.Update(dt);
            if (hurtTime > 0) --hurtTime;
        }

        public override void Draw(SpriteBatch batch) {
            var off = hurtOff * (float) Math.Clamp(hurtTime, 0, 3);
            batch.Draw(_sheet.Texture, new Vector2(this.x + off.X - xr * 2 - Tiles.Tiles.xo, this.y + off.Y - yr * 2 - Tiles.Tiles.yo), source, Color.White);
            base.Draw(batch);
        }

        private static readonly Vector2[] _offs = new Vector2[] {
            new Vector2(0, -1),
            new Vector2(-1, 0),
            new Vector2(0, +1),
            new Vector2(+1, 0),
        };

        private Vector2 hurtOff = Vector2.Zero;
        public override void Hurt(Level level, Mob mob, int damage, int direction) {
            if (this.health <= 0) return;
            if (damage > 0) {
                level.Add(new Meadows.Entities.Text(damage.ToString(), x, y, Color.PaleVioletRed));
                level.Add(new Meadows.Entities.BitParticle(x - 2, y - 2, Color.DarkOliveGreen));
                level.Add(new Meadows.Entities.BitParticle(x, y, Color.DarkOliveGreen));
                level.Add(new Meadows.Entities.BitParticle(x + 2, y + 2, Color.DarkOliveGreen));
                level.Add(new Meadows.Entities.BitParticle(x, y + 2, Color.DarkOliveGreen));
                this.health -= damage;
                if (this.health <= 0) {
                    level.Add(new EItem(new ResourceItem(this.drop), x, y));
                    Removed = true;
                }

                this.hurtOff = _offs[direction & 3];
                this.hurtTime = 10;
            }
        }

        public override bool Blocks(Entity e) {
            return true;
        }
    }
}
