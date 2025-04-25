using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Meadows.Items;
using System;
using System.Text;

namespace Meadows.Entities {
    public class Player : Mob {
        private enum State : int {
            Idle = 0,
            Walking,
            Swimming,
            Count,
        }

        private readonly static int Height = 64, Width = 64;
        private readonly static Vector2 Actual = 
            new Vector2((Main.Width - Width) >> 1, (Main.Height - 1.75f * Height) * 0.5f);
        private readonly static int SwimmingSpeed = 1;
        private readonly static int WalkingSpeed = 2;
        private readonly static int RunningSpeed = 3;
        private readonly Animation[] anims;
        private readonly Sheet sheet;
        private Item activeItem;
        private State state;
        private int speed;

        public Player() {
            this.sheet = new Sheet("Sprites/Player", Width);
            this.anims = new Animation[(int) State.Count];
            this.anims[(int) State.Idle] = new Animation(sheet, new Vector2(0, 4), Vector2.One, 3, unit: Width, limit: 48f, loop: true);
            this.anims[(int) State.Swimming] = new Animation(sheet, Vector2.Zero, new Vector2(1f, .7f), 5, unit: Width, limit: 26f, loop: true);
            this.anims[(int) State.Walking] = new Animation(sheet, new Vector2(0, 8), Vector2.One, 9, unit: Width, limit: 15f, loop: true);
            this.y = (int) (24.5 * Tiles.Tile.Height);
            this.x = (int) (10.5 * Tiles.Tile.Width);
            this.xr = (Width >> 1) - 16;
            this.yr = (Height >> 1) - 16;
            this._dir = Direction.Right;
            this.state = State.Idle;
            this.speed = 2;
        }

        public override void TouchItem(EItem ie) {
            ie.Take(this);
            // TODO: add to inventory.
        }

        private void Hurt(int x0, int y0, int x1, int y1) {
            List<Entity> entities = level.GetEntities(x0, y0, x1, y1);
            for (int i = 0; i < entities.Count; ++i) {
                var e = entities[i];
                if (e != this) e.Hurt(this.level, this, RNG.Next(3) + 1, (int) _dir);
            }
        }

        private void Attack() {
            if (this.activeItem == null || this.activeItem.CanAttack()) {
                int range = 32;
                int yo = -2;

                if (_dir == Direction.Down) Hurt(x - 16, y + 8 + yo, x + 16, y + range + yo);
                if (_dir == Direction.Up) Hurt(x - 16, y - range + yo, x + 16, y - 8 + yo);
                if (_dir == Direction.Right) Hurt(x + 8, y - 16 + yo, x + range, y + 16 + yo);
                if (_dir == Direction.Left) Hurt(x - range, y - 16 + yo, x - 8, y + 16 + yo);

            }
        }

        private bool Interact(int x0, int y0, int x1, int y1) {
            List<Entity> entities = level.GetEntities(x0, y0, x1, y1);
            for (int i = 0; i < entities.Count; ++i) {
                var e = entities[i];
                if ((e != this) && (e.Interact(this, activeItem, (int) this._dir)))
                    return true;
            }

            return false;
        }

        public override void Update(GameTime dt) {
            base.Update(dt);
            int xa = 0, ya = 0;
            if (this.state == State.Walking) {
                if (Utility.InputManager.IsKeyDown(Keys.LeftShift)) {
                    if (this.speed == Player.WalkingSpeed) {
                        level.Add(new BitParticle(x, y, Color.DimGray));
                        level.Add(new BitParticle(x, y, Color.DimGray));
                        level.Add(new BitParticle(x, y, Color.DimGray));
                        level.Add(new BitParticle(x, y, Color.DimGray));
                    }

                    this.speed = Player.RunningSpeed;
                }

                if (Utility.InputManager.IsKeyReleased(Keys.LeftShift)) {
                    this.speed = Player.WalkingSpeed;
                }
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

            this.Move(xa, ya);
            if (this.state == State.Idle) {
                if ((xa != 0) || (ya != 0))
                    this.state = State.Walking;

                if (Utility.InputManager.IsKeyPressed(Keys.Space))
                    Attack();
            } else if (this.state == State.Walking) {
                var tile = level.GetLastTile(this.x >> 5, this.y >> 5);
                if ((xa == 0) && (xa == ya)) this.state = State.Idle;
                else if ((tile is not null) && tile.Swimmable) {
                    this.speed = Player.SwimmingSpeed;
                    this.state = State.Swimming;
                    for (int iii = 0; iii < 10; ++iii) {
                        level.Add(new BitParticle(x, y, Color.RoyalBlue));
                    }
                }
            } else if (this.state == State.Swimming) {
                var tile = level.GetLastTile(this.x >> 5, this.y >> 5);
                if ((tile is not null) && !tile.Swimmable) {
                    for (int iii = 0; iii < 10; ++iii) {
                        level.Add(new BitParticle(x, y, Color.RoyalBlue));
                    }

                    if ((xa == 0) && (xa == ya)) this.state = State.Idle;
                    else {
                        this.speed = Player.WalkingSpeed;
                        this.state = State.Walking;
                    }
                }
            }

            this.anims[(int) this.state].Update(dt);
        }

        public override void Draw(SpriteBatch batch) {
            var vstate = this.anims[(int) this.state];
            if (this.state == State.Swimming) vstate.DrawDirected(batch, new Vector2(Actual.X, Actual.Y + (Width >> 2)), (int) this._dir);
            else vstate.DrawDirected(batch, Actual, (int) this._dir);
            base.Draw(batch);
        }

        public override bool CanSwim() {
            return true;
        }
    }
}
