using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Meadows.Entities
{
    public class NPC : Mob
    {
        private readonly Sheet _sheet;
        private readonly Animation _idleAnim;

        public NPC(int x, int y)
        {
            this._sheet = new Sheet("Sprites/Player", 64);
            this._idleAnim = new Animation(_sheet, new Vector2(0, 4), Vector2.One, 1, 64, loop: false);
            this.x = x;
            this.y = y;
            this.xr = (64 >> 1) - 16;
            this.yr = (64 >> 1) - 16;
        }

        public override void Update(GameTime dt)
        {

        }

        public override void Draw(SpriteBatch batch)
        {
            _idleAnim.Draw(batch, new Vector2(
                x - Tiles.Tiles.xo - 32,
                y - Tiles.Tiles.yo - 32
            ));
        }
    }
}