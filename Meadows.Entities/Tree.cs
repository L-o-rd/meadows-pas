using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Meadows.Entities {
    public class Tree : Entity {
        private readonly Rectangle source;
        private readonly float _ox, _oy;
        private readonly Sheet _sheet;

        public Tree(int id, int size, Sheet sheet, int x = 0, int y = 0, float ox = 0.45f, float oy = 0.95f) {
            this.source = sheet.Source(id, Tiles.Tile.Width, size);
            this.xr = this.yr = 5;
            this._ox = size * ox;
            this._oy = size * oy;
            this._sheet = sheet;
            this.x = x;
            this.y = y;
        }

        public override void Draw(SpriteBatch batch) {
            batch.Draw(_sheet.Texture, new Vector2(this.x - this._ox - Tiles.Tiles.xo, this.y - this._oy - Tiles.Tiles.yo), source, Color.White);
            base.Draw(batch);
        }

        public override bool Blocks(Entity e) {
            return true;
        }
    }
}
