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
        private readonly Type type;
        public readonly int Ticks;

        public Plantable(String name, int nr, int ticks) : base(name, nr) {
            phases = new Rectangle[] {
                Resources.Sheet.Source2(nr, 1),
                Resources.Sheet.Source2(nr, 3),
                Resources.Sheet.Source2(nr, 5),
                Resources.Sheet.Source2(nr, 7),
            };

            _base = Resources.Sheet.Source2(nr, 9);
            this.Ticks = ticks;
            this.column = nr;
        }

        public override bool InteractOn(Tile tile, Level level, int xt, int yt, Player player, int direction) {
            if (tile.Swimmable)
                return false;

            if (level.Occupied(xt, yt))
                return false;

            level.Add(new Plant(this, (int)((xt + 0.5) * 32), (int)((yt + 0.5) * 32)));
            return true;
        }

        public (Rectangle[], Rectangle, Sheet) GetSprites() {
            return (phases, _base, Resources.Sheet);
        }
    }
}
