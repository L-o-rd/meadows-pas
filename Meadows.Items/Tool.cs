using Microsoft.Xna.Framework;
using Meadows.Entities;
using Meadows.Levels;
using Meadows.Tiles;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Meadows.Items {
    public class Tool : Item {
        private readonly static Random RNG = new Random(Environment.TickCount);
        public int BaseDamage { get; } = 1;
        public readonly Rectangle Sprite;

        public override bool CanAttack() {
            return true;
        }

        public Tool(String name, int ix, int iy, int damage = 1) {
            Sprite = Tools.Sheet.Source2(ix, iy);
            BaseDamage = damage;
            Name = name;
        }

        public override void Draw(SpriteBatch batch, int x, int y, Color color) {
            batch.Draw(Tools.Sheet.Texture, new Vector2(x - Tiles.Tiles.xo, y - Tiles.Tiles.yo), this.Sprite, color);
        }

        public override void DrawSlot(SpriteBatch batch, int x, int y, Color color) {
            batch.Draw(Tools.Sheet.Texture, new Vector2(x + 1, y + 1), this.Sprite, Color.Black);
            batch.Draw(Tools.Sheet.Texture, new Vector2(x, y), this.Sprite, color);
        }
    }

    public static class Tools {
        public readonly static Sheet Sheet = new Sheet("Sprites/Tools", 32);
        public readonly static Tool Sickle = new Tool("Sickle", 3, 9, 5);
        public readonly static Tool Shovel = new Tool("Shovel", 5, 2, 3);
    }
}
