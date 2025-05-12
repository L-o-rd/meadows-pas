using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Meadows.Utility;

namespace Meadows.Entities
{
    public class NPC : Mob
    {
        private enum State
        {
            Idle = 0,
            Walking
        }

        private readonly static int Width = 64, Height = 64;
        private readonly Sheet _portraits;
        private readonly Sheet _sheet;
        private readonly Animation[] _anims;
        private State _state;
        private float _moveTimer;
        private float _waitTime;
        private int _speed;

        public NPC(int x, int y)
        {
            this._portraits = new Sheet("Sprites/SaraPortrait");
            this._sheet = new Sheet("Sprites/Sara", Width);
            this._anims = new Animation[2];
            this._anims[(int)State.Idle] = new Animation(_sheet, new Vector2(0, 12), Vector2.One, 3, Width, limit: 35f, loop: true);
            this._anims[(int)State.Walking] = new Animation(_sheet, new Vector2(0, 8), Vector2.One, 9, Width, limit: 15f, loop: true);

            this.x = x;
            this.y = y;
            this.xr = (Width >> 1) - 16;
            this.yr = (Height >> 1) - 16;
            this._dir = Direction.Down;

            this._state = State.Idle;
            this._moveTimer = 0f;
            this._waitTime = 3f;
            this._speed = 1;
            xa = ya = 0;
        }

        private int xa, ya;
        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if (_state == State.Idle) {
                _moveTimer += (float) dt.ElapsedGameTime.TotalSeconds;
                if (_moveTimer >= _waitTime) {
                    _waitTime = (float) (RNG.NextDouble() * 2.5 + 1.5);
                    _moveTimer = 0f;

                    int dir = RNG.Next(5);
                    if (dir == 0) { ya = -_speed; _dir = Direction.Up; } 
                    else if (dir == 1) { ya = +_speed; _dir = Direction.Down; } 
                    else if (dir == 2) { xa = -_speed; _dir = Direction.Left; } 
                    else if (dir == 3) { xa = +_speed; _dir = Direction.Right; }
                    else xa = ya = 0;

                    if (xa != 0 || ya != 0) {
                        _state = State.Walking;
                    }
                }
            } else if (_state == State.Walking) {
                var anim = _anims[(int) State.Walking];
                if (anim.Completed()) {
                    anim.Reset();
                    _state = State.Idle;
                    xa = ya = 0;
                } else {
                    if (!this.Move(xa, ya, dt)) {
                        _state = State.Idle;
                        xa = ya = 0;
                    }
                }
            }

            _anims[(int)_state].Update(dt);
        }

        public override void Draw(SpriteBatch batch)
        {
            var anim = _anims[(int)_state];
            anim.DrawDirected(batch, new Vector2(
                x - Tiles.Tiles.xo - (Width >> 1),
                y - Tiles.Tiles.yo - (Height >> 1)
            ), (int)_dir);
        }

        private int pidx = 0, pidy = 0;

        public void SetPortrait(int n) {
            int tx = 0, ty = 1;
            if ((n < 0) || (n > 3)) n = RNG.Next(4);
            if (n == 0) tx = ty = 0;
            else if (n == 1) { tx = 0; ty = 1; }
            else if (n == 2) { tx = 1; ty = 1; }
            else { tx = 2; ty = 1; }
            pidx = tx; pidy = ty;
        }

        public void Portrait(SpriteBatch batch, int x, int y) {
            batch.Draw(_portraits.Texture, new Vector2(x, y), _portraits.Source3(pidx, pidy, 300, 380, 300, 380), Color.White);
        }

        public override bool Blocks(Entity e) => true;

        public override bool CanSwim() => false;
    }
}
