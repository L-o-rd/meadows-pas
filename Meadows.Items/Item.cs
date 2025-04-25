using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Meadows.Entities;
using Meadows.Levels;
using Meadows.Tiles;

namespace Meadows.Items {
    public class Item {
        public virtual void OnTake(EItem e) { }
        public virtual bool Interact(Player player, Entity entity, int direction) {
            return false;
        }

        public virtual bool InteractOn(Tile tile, Level level, int xt, int yt, Player player, int direction) {
            return false;
        }

        public virtual bool CanAttack() {
            return false;
        }

        public virtual bool Depleted() {
            return false;
        }

        public virtual void Draw(SpriteBatch batch, int x, int y, Color color) { }
    }

    public class ResourceItem : Item {
        public int Count { get; set; } = 1;
        public readonly Resource Resource;

        public ResourceItem(Resource resource) {
            Resource = resource;
        }

        public override bool InteractOn(Tile tile, Level level, int xt, int yt, Player player, int direction) {
            if (Resource.InteractOn(tile, level, xt, yt, player, direction)) {
                --Count;
                return true;
            }

            return false;
        }

        public override bool Depleted() {
            return Count <= 0;
        }

        public override void Draw(SpriteBatch batch, int x, int y, Color color) {
            batch.Draw(Resources.Sheet.Texture, new Vector2(x - Tiles.Tiles.xo, y - Tiles.Tiles.yo), this.Resource.Sprite, color);
        }
    }
}
