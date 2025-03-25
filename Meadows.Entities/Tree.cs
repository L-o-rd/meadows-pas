using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Meadows.Entities {
    public class Tree : Entity {
        private readonly Rectangle source;
        private readonly Sheet _sheet;
        private readonly int size;
        public float ox = 0.5f;

        public Tree(int id, int size, Sheet sheet, int x = 0, int y = 0) {
            this.source = sheet.Source(id, Tiles.Tile.Width, size);
            this.xr = this.yr = 1;
            this._sheet = sheet;
            this.size = size;
            this.x = x;
            this.y = y;
        }

        public override void Draw(SpriteBatch batch) {
            batch.Draw(_sheet.Texture, new Vector2(this.x - (size * this.ox) - Tiles.Tiles.xo, this.y - size - Tiles.Tiles.yo), source, Color.White);
            base.Draw(batch);
        }

        public override bool Blocks(Entity e) {
            return true;
        }
    }
}
