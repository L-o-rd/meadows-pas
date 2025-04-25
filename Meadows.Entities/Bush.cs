using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Meadows.Levels;
using Meadows.Items;

namespace Meadows.Entities {
    public class Bush : Entity {
        private readonly Rectangle source;
        private readonly Resource drop;
        private readonly Sheet _sheet;
        private int health = 5;

        public Bush(int ix, int iy, int size, Sheet sheet, int x = 0, int y = 0) {
            this.source = sheet.Source2(ix, iy, size);
            this.xr = this.yr = 8;
            this._sheet = sheet;
            this.x = x;
            this.y = y;

            var drop = RNG.Next(3);
            switch (drop) {
                case 1: this.drop = Resources.Carrot; break;
                case 2: this.drop = Resources.Beetroot; break;
                case 0: default: this.drop = Resources.Potato; break;
            }
        }

        public override void Draw(SpriteBatch batch) {
            batch.Draw(_sheet.Texture, new Vector2(this.x - xr * 2 - Tiles.Tiles.xo, this.y - yr * 2 - Tiles.Tiles.yo), source, Color.White);
            base.Draw(batch);
        }

        public override void Hurt(Level level, Mob mob, int damage, int direction) {
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
            }
        }

        public override bool Blocks(Entity e) {
            return true;
        }
    }
}
