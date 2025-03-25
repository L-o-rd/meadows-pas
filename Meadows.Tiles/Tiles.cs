using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Meadows.Levels;
using Meadows.Entities;
using System;

namespace Meadows.Tiles {
    public class Tile {
        public static readonly int Height = 32;
        public static readonly int Width = 32;
        public Rectangle _source;
        public Texture2D _sheet;
        public readonly int ID;

        public bool Passable { get; set; } = true;

        public Tile(int id, Sheet sheet) {
            this._source = sheet.Source(id);
            this._sheet = sheet.Texture;
            this.ID = id;
        }

        public virtual void Update(GameTime dt) { }
        public void Bumped(Level level, int x, int y, Entity e) { }
        public virtual void Stepped(Level level, int x, int y, Entity e) { }
        public virtual void Draw(SpriteBatch batch, Level level, int x, int y) {
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

        public static int xo = 0;
        public static int yo = 0;
    }
}
