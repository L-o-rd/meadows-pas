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
        private readonly Sheet _sheet;
        private readonly Animation[] _anims;
        private State _state;
        private float _moveTimer;
        private float _waitTime;
        private int _speed;

        private static readonly Random _rand = new Random();

        public NPC(int x, int y)
        {
            this._sheet = new Sheet("Sprites/Player", Width);
            this._anims = new Animation[2];
            this._anims[(int)State.Idle] = new Animation(_sheet, new Vector2(0, 4), Vector2.One, 1, Width, loop: true);
            this._anims[(int)State.Walking] = new Animation(_sheet, new Vector2(0, 8), Vector2.One, 9, Width, limit: 15f, loop: true);

            this.x = x;
            this.y = y;
            this.xr = (Width >> 1) - 16;
            this.yr = (Height >> 1) - 16;
            this._dir = Direction.Down;

            this._state = State.Idle;
            this._moveTimer = 0f;
            this._waitTime = 2f;
            this._speed = 1;
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);
            float elapsed = (float)dt.ElapsedGameTime.TotalSeconds;
            _moveTimer += elapsed;

            int xa = 0, ya = 0;

            if (_state == State.Idle && _moveTimer >= _waitTime)
            {
                _moveTimer = 0f;
                _waitTime = (float)(_rand.NextDouble() * 2.5 + 1.5);

                int dir = _rand.Next(5);
                if (dir == 0) { ya = -_speed; _dir = Direction.Up; }
                else if (dir == 1) { ya = _speed; _dir = Direction.Down; }
                else if (dir == 2) { xa = -_speed; _dir = Direction.Left; }
                else if (dir == 3) { xa = _speed; _dir = Direction.Right; }

                if (xa != 0 || ya != 0)
                {
                    _state = State.Walking;
                }
            }

            if (_state == State.Walking)
            {
                if (!Move(xa, ya, dt))
                {
                    _state = State.Idle;
                }
                else
                {
                    if (_moveTimer >= _waitTime)
                    {
                        _moveTimer = 0f;
                        _state = State.Idle;
                        _waitTime = (float)(_rand.NextDouble() * 2.5 + 1.5);
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

        public override bool Blocks(Entity e) => true;

        public override bool CanSwim() => true;
    }
}
