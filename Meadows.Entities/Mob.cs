using Microsoft.Xna.Framework;

namespace Meadows.Entities {
    public class Mob : Entity {
        protected enum Direction : int {
            Up = 0, Left = 1, Down = 2, Right = 3
        }

        protected Direction _dir;

        public override void Update(GameTime dt) {
            base.Update(dt);
        }

        public override bool Move(int xa, int ya, GameTime dt) {
            if (xa != 0 || ya != 0) {
                if (xa < 0) _dir = Direction.Left;
                if (xa > 0) _dir = Direction.Right;
                if (ya < 0) _dir = Direction.Up;
                if (ya > 0) _dir = Direction.Down;
            }

            return base.Move(xa, ya, dt);
        }
    }
}
