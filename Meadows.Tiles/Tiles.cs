using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Meadows.Entities;
using Meadows.Levels;

namespace Meadows.Tiles {
    public class Tile {
        public static readonly int Height = 32;
        public static readonly int Width = 32;
        public Rectangle _source;
        public Texture2D _sheet;
        public readonly int ID;
        public int frame = 0;
        public float dt = 0f;

        public bool Swimmable { get; set; } = false;
        public bool Passable { get; set; } = true;

        public Tile(int id, Sheet sheet) {
            this._source = sheet.Source(id);
            this._sheet = sheet.Texture;
            this.ID = id;
        }

        public void Bumped(Level level, int x, int y, Entity e) { }
        public void Stepped(Level level, int x, int y, Entity e) { }
        public void Draw(SpriteBatch batch, Level level, int x, int y) {
            batch.Draw(this._sheet, new Vector2(x * Tile.Width - Tiles.xo, y * Tile.Height - Tiles.yo), 
                this._source, Color.White);
        }
    }

    public static class Tiles {
        public static readonly Tile[] Zeros = null;

        static Tiles() {
            Tiles.Zeros = new Tile[40];
            for (int i = 0; i < 40; ++i) {
                Tiles.Zeros[i] = new Tile(i, Sheets.Water);
                Tiles.Zeros[i].Passable = false;
            }
        }

        public static void AnimateWater(Tile tile, GameTime dt) {
            tile.dt += (float) dt.ElapsedGameTime.TotalMilliseconds * 0.1f;
            if (tile.dt >= 17f) {
                tile.frame = (tile.frame + 1) % 20;
                tile._source = Sheets.Water.Source(tile.ID + 50 * tile.frame);
                tile.dt = 0f;
            }
        }

        public static int xo = 0;
        public static int yo = 0;
    }
}
