using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Meadows.Items;
using System;
using System.Text;
using Meadows.Utility;

namespace Meadows.Entities {
    public class Player : Mob {
        private enum State : int {
            Idle = 0,
            Walking,
            Swimming,
            Attacking,
            Count,
        }

        private readonly static Texture2D InteractSquare = null;
        static Player() {
            int size = 32;
            InteractSquare = new Texture2D(Main.Graphics.GraphicsDevice, size, size);
            Color[] data = new Color[size * size];

            int borderThickness = 3;
            Color borderColor = new Color(233, 233, 233, 233);
            Color innerColor = new Color(255, 255, 255, 88);

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float maxDist = center.Length();

            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    bool isBorderX = x < borderThickness || x >= size - borderThickness;
                    bool isBorderY = y < borderThickness || y >= size - borderThickness;
                    bool isEdge = isBorderX || isBorderY;

                    bool isNearCorner =
                        (x < borderThickness && y < borderThickness) ||
                        (x >= size - borderThickness && y < borderThickness) ||
                        (x < borderThickness && y >= size - borderThickness) ||
                        (x >= size - borderThickness && y >= size - borderThickness);

                    Color pixelColor;

                    if (isEdge) {
                        byte alphab = borderColor.A;

                        if (isNearCorner) {
                            alphab = (byte)(borderColor.A * 0.75f);
                        }

                        pixelColor = new Color(borderColor.R, borderColor.G, borderColor.B, alphab);
                    } else {
                        pixelColor = innerColor;
                    }

                    float alpha = pixelColor.A / 255f;
                    data[y * size + x] = new Color(
                        (byte)(pixelColor.R * alpha),
                        (byte)(pixelColor.G * alpha),
                        (byte)(pixelColor.B * alpha),
                        pixelColor.A
                    );
                }
            }

            InteractSquare.SetData<Color>(data);
        }

        private readonly static int Height = 64, Width = 64;
        private readonly static Vector2 Actual = 
            new Vector2((Main.Width - Width) >> 1, (Main.Height - 1.75f * Height) * 0.5f);
        private readonly static int SwimmingSpeed = 1;
        private readonly static int WalkingSpeed = 2;
        private readonly static int RunningSpeed = 3;
        public readonly Inventory inventory;
        private readonly Animation[] anims;
        private readonly Sheet sheet;
        public int activeSlot;
        private State state;
        private int speed;

        public Player() {
            this.sheet = new Sheet("Sprites/Player", Width);
            this.anims = new Animation[(int) State.Count];
            this.anims[(int) State.Idle] = new Animation(sheet, new Vector2(0, 4), Vector2.One, 3, unit: Width, limit: 48f, loop: true);
            this.anims[(int) State.Swimming] = new Animation(sheet, Vector2.Zero, new Vector2(1f, .7f), 5, unit: Width, limit: 26f, loop: true);
            this.anims[(int) State.Walking] = new Animation(sheet, new Vector2(0, 8), Vector2.One, 9, unit: Width, limit: 15f, loop: true);
            this.anims[(int) State.Attacking] = new Animation(sheet, new Vector2(1, 12), Vector2.One, 5, unit: Width, limit: 10f, loop: true);
            this.y = (int) (24.5 * Tiles.Tile.Height);
            this.x = (int) (10.5 * Tiles.Tile.Width);
            this.inventory = new Inventory();
            this.xr = (Width >> 1) - 16;
            this.yr = (Height >> 1) - 16;
            this._dir = Direction.Right;
            this.state = State.Idle;
            this.activeSlot = 0;
            this.speed = 2;
        }

        public override void TouchItem(EItem ie, GameTime dt) {
            ie.Take(this, dt);
            this.inventory.Add(ie.item);
        }

        private void Hurt(GameTime dt, int x0, int y0, int x1, int y1) {
            List<Entity> entities = level.GetEntities(x0, y0, x1, y1);
            for (int i = 0; i < entities.Count; ++i) {
                var e = entities[i];
                if (e != this) {
                    if (inventory.Items.Count <= activeSlot) e.Hurt(this.level, this, RNG.Next(3) + 1, (int) _dir);
                    else {
                        var item = inventory.Items[activeSlot];
                        if (item is Tool t) e.Hurt(this.level, this, RNG.Next(3) + t.BaseDamage, (int)_dir);
                        else e.Hurt(this.level, this, RNG.Next(3) + 1, (int)_dir);
                    }

                    Sound.Play(dt, "Hit", pitch: -.5f, cooldown: 0.05f);
                }
            }
        }

        private void Attack(GameTime dt) {
            int yo = -2;
            if ((this.inventory.Items.Count <= activeSlot) || this.inventory.Items[activeSlot].CanAttack()) {
                int range = 32;
                if (_dir == Direction.Down) Hurt(dt, x - 16, y + 8 + yo, x + 16, y + range + yo);
                if (_dir == Direction.Up) Hurt(dt, x - 16, y - range + yo, x + 16, y - 8 + yo);
                if (_dir == Direction.Right) Hurt(dt, x + 8, y - 16 + yo, x + range, y + 16 + yo);
                if (_dir == Direction.Left) Hurt(dt, x - range, y - 16 + yo, x - 8, y + 16 + yo);
            } else {
                int range = 28;
                if ((_dir == Direction.Down) && Interact(dt, x - 16, y + 8 + yo, x + 16, y + range + yo)) return;
                if ((_dir == Direction.Up) && Interact(dt, x - 16, y - range + yo, x + 16, y - 8 + yo)) return;
                if ((_dir == Direction.Right) && Interact(dt, x + 8, y - 16 + yo, x + range, y + 16 + yo)) return;
                if ((_dir == Direction.Left) && Interact(dt, x - range, y - 16 + yo, x - 8, y + 16 + yo)) return;

                int r = range;
                int xt = x >> 5;
                int yt = (y + yo) >> 5;
                if (_dir == Direction.Down) yt = (y + r + yo) >> 5;
                if (_dir == Direction.Up) yt = (y - r + yo) >> 5;
                if (_dir == Direction.Left) xt = (x - r) >> 5;
                if (_dir == Direction.Right) xt = (x + r) >> 5;

                if (level.InBounds(xt, yt)) {
                    var item = inventory.Items[activeSlot];
                    if (item.InteractOn(level.GetTile(xt, yt), level, xt, yt, this, (int) _dir))
                        Sound.Play(dt, "Hit", pitch: -.5f, cooldown: 0.05f);

                    if (item.Depleted()) {
                        inventory.Items.Remove(item);
                    }
                }
            }
        }

        private bool Interact(GameTime dt, int x0, int y0, int x1, int y1) {
            List<Entity> entities = level.GetEntities(x0, y0, x1, y1);
            for (int i = 0; i < entities.Count; ++i) {
                var e = entities[i];
                if ((e != this) && (e.Interact(this, inventory.Items[activeSlot], (int)this._dir))) {
                    Sound.Play(dt, "Hit", pitch: -.5f, cooldown: 0.05f);
                    return true;
                }
            }

            return false;
        }

        private void Drop() {
            if (inventory.Items.Count > activeSlot) {
                level.Add(new EItem(inventory.Items[activeSlot], x, y));
                inventory.Items.RemoveAt(activeSlot);
            }
        }

        public override void Update(GameTime dt) {
            base.Update(dt);
            int xa = 0, ya = 0;
            if (this.state == State.Walking) {
                if (Utility.InputManager.IsKeyDown(Keys.LeftShift)) {
                    if (this.speed == Player.WalkingSpeed) {
                        level.Add(new BitParticle(x, y, Color.LightGray));
                        level.Add(new BitParticle(x, y, Color.LightGray));
                        level.Add(new BitParticle(x, y, Color.LightGray));
                        level.Add(new BitParticle(x, y, Color.LightGray));
                        Sound.Play(dt, "Hit", pitch: -.25f, cooldown: 0.05f);
                    }

                    this.speed = Player.RunningSpeed;
                }

                if (Utility.InputManager.IsKeyReleased(Keys.LeftShift)) {
                    this.speed = Player.WalkingSpeed;
                }
            }

            for (int key = (int) Keys.D1; key <= (int) Keys.D8; ++key) {
                if (Utility.InputManager.IsKeyPressed((Keys) key)) {
                    this.activeSlot = key - (int) Keys.D1;
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

            this.Move(xa, ya, dt);
            if (this.state == State.Idle) {
                if ((xa != 0) || (ya != 0))
                    this.state = State.Walking;

                if (Utility.InputManager.IsKeyPressed(Keys.Space)) {
                    this.state = State.Attacking;
                    Attack(dt);
                }
                
                if (Utility.InputManager.IsKeyPressed(Keys.Q)) {
                    Drop();
                }
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
            } else if (this.state == State.Attacking) {
                var anim = this.anims[(int)State.Attacking];
                if (anim.Completed()) {
                    this.state = State.Idle;
                    anim.Reset();
                }
            }

            this.anims[(int) this.state].Update(dt);
        }

        public override void Draw(SpriteBatch batch) {
            var vstate = this.anims[(int) this.state];
            if (this.state == State.Swimming) vstate.DrawDirected(batch, new Vector2(Actual.X, Actual.Y + (Width >> 2)), (int) this._dir);
            else vstate.DrawDirected(batch, Actual, (int) this._dir);
            if ((this.inventory.Items.Count > activeSlot) && !this.inventory.Items[activeSlot].CanAttack()) {
                int range = 28;
                int yo = -2;
                int r = range;
                int xt = x >> 5;
                int yt = (y + yo) >> 5;
                if (_dir == Direction.Down) yt = (y + r + yo) >> 5;
                if (_dir == Direction.Up) yt = (y - r + yo) >> 5;
                if (_dir == Direction.Left) xt = (x - r) >> 5;
                if (_dir == Direction.Right) xt = (x + r) >> 5;

                if (level.InBounds(xt, yt)) {
                    batch.Draw(InteractSquare, new Vector2((xt << 5) - Tiles.Tiles.xo, (yt << 5) - Tiles.Tiles.yo), Color.White);
                }
            }

            base.Draw(batch);
        }
        public override bool Blocks(Entity e) => true;
        public override bool CanSwim() {
            return true;
        }
    }
}
