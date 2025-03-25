using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Meadows.Entities {
    public class Player : Mob {
        private readonly static int Height = 64, Width = 64;
        private readonly static Vector2 Actual = new Vector2((Main.Width - Width) >> 1, (Main.Height - 1.75f * Height) * 0.5f);
        private readonly Sheet sheet;
        private Rectangle source;
        private int speed;

        public Player() {
            this.sheet = new Sheet("Sprites/Player", Width);
            this.y = (int) (24.5 * Tiles.Tile.Height);
            this.x = (int) (10.5 * Tiles.Tile.Width);
            this.xr = (Width >> 1) - 16;
            this.yr = (Height >> 1) - 1;
            this._dir = Direction.Right;
            this.speed = 2;
        }

        float acc = 0f;
        int frame = 0;
        public override void Update(GameTime dt) {
            base.Update(dt);
            int xa = 0, ya = 0;
            if (Utility.InputManager.IsKeyDown(Keys.LeftShift)) {
                this.speed = 3;
            }

            if (Utility.InputManager.IsKeyReleased(Keys.LeftShift)) {
                this.speed = 2;
            }

            if (Utility.InputManager.IsKeyDown(Keys.W)) {
                ya -= speed;
            }

            if (Utility.InputManager.IsKeyDown(Keys.A)) {
                xa -= speed;
            }

            if (Utility.InputManager.IsKeyDown(Keys.S)) {
                ya += speed;
            }

            if (Utility.InputManager.IsKeyDown(Keys.D)) {
                xa += speed;
            }

            this.source = this.sheet.Source(((int) _dir << 3) + frame, Width);
            acc += (float) dt.ElapsedGameTime.TotalMilliseconds * 0.1f;
            if ((xa != 0 || ya != 0)) {
                if (acc >= 15f) {
                    frame = (frame + 1) & 7;
                    acc = 0f;
                }
            } else {
                frame = 0;
            }

            this.Move(xa, ya);
        }

        public override void Draw(SpriteBatch batch) {
            batch.Draw(this.sheet.Texture, Actual, this.source, Color.White);
            base.Draw(batch);
        }

        public override bool CanSwim() {
            return true;
        }
    }
}
