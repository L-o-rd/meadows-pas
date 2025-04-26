using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Meadows.Entities;
using Meadows.Levels;
using Meadows.Tiles;

namespace Meadows.Items {
    public class Item {
        public string Name { get; set; }

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
        public virtual void DrawSlot(SpriteBatch batch, int x, int y, Color color) { }
    }

    public class ResourceItem : Item {
        public int Count { get; set; } = 1;
        public readonly Resource Resource;

        public ResourceItem(Resource resource) {
            Name = resource.Name;
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

        public override void DrawSlot(SpriteBatch batch, int x, int y, Color color) {
            var str = Count.ToString();
            var size = Resources.Particle.MeasureString(str);
            batch.Draw(Resources.Sheet.Texture, new Vector2(x, y), this.Resource.Sprite, color);
            batch.DrawString(Resources.Particle, str, new Vector2(x + 23 - (size.X * 0.5f), y + 14), Color.Black);
            batch.DrawString(Resources.Particle, str, new Vector2(x + 22 - (size.X * 0.5f), y + 13), Color.White);
        }
    }
}
