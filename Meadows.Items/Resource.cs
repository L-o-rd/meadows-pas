using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Meadows.Entities;
using Meadows.Levels;
using Meadows.Tiles;
using System;

namespace Meadows.Items {
    public class Resource {
        public readonly Rectangle Sprite;
        public readonly String Name;
        
        public Resource(String name, int nr) {
            Sprite = Resources.Sheet.Source(nr);
            Name = name;
        }

        public virtual bool InteractOn(Tile tile, Level level, int xt, int yt, Player player, int direction) {
            return false;
        }
    }

    public static class Resources {
        public static SpriteFont Particle = Main.Contents.Load<SpriteFont>("Fonts/TextParticle");
        public readonly static Sheet Sheet = new Sheet("Sprites/Resources", 32);
        public static Resource Potato = new Plantable("Potato", 0, 60 * 5);
        public static Resource Carrot = new Plantable("Carrot", 6, 60 * 5);
        public static Resource Beetroot = new Plantable("Beetroot", 9, 60 * 5);
    }
}
