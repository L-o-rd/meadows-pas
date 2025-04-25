using Microsoft.Xna.Framework;
using Meadows.Entities;
using Meadows.Levels;
using Meadows.Tiles;
using System;

namespace Meadows.Items {
    public class Plantable : Resource {
        private readonly Rectangle[] phases;
        private readonly Rectangle _base;
        private readonly int column;

        public Plantable(String name, int nr) : base(name, nr) {
            phases = new Rectangle[] {
                Resources.Sheet.Source2(nr, 1),
                Resources.Sheet.Source2(nr, 3),
                Resources.Sheet.Source2(nr, 5),
                Resources.Sheet.Source2(nr, 7),
            };

            _base = Resources.Sheet.Source2(nr, 9);
            this.column = nr;
        }

        public override bool InteractOn(Tile tile, Level level, int xt, int yt, Player player, int direction) {
            if (tile.Swimmable)
                return false;

            // TODO: actually plant it.
            return true;
        }
    }
}
