using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Meadows {
    public class Sheet {
        public readonly Texture2D Texture;
        private readonly int unit = 1;
        private readonly int width;

        public Sheet(String source, int unit = 1) {
            this.Texture = Main.Contents.Load<Texture2D>(source);
            this.width = this.Texture.Width / unit;
            this.unit = unit;
        }

        public Rectangle Source(int id, int nunit, int actual) {
            var x = (id % width) * nunit;
            var y = (id / width) * nunit;
            return new Rectangle(x, y, actual, actual);
        }

        public Rectangle Source(int id, int actual) {
            var x = (id % width) * actual;
            var y = (id / width) * actual;
            return new Rectangle(x, y, actual, actual);
        }

        public Rectangle Source(int id) {
            var x = (id % width) << 5;
            var y = (id / width) << 5;
            return new Rectangle(x, y, Tiles.Tile.Width, Tiles.Tile.Height);
        }
    }

    public class Sheets {
        public static readonly Sheet Outside = new Sheet("Sprites/Beach", 32);
        public static readonly Sheet Water = new Sheet("Sprites/Water", 32);
        public static readonly Sheet Rocks = new Sheet("Sprites/Rocks", 32);
    }
}
