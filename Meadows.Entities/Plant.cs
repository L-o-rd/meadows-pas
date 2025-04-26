using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Meadows.Levels;
using Meadows.Items;
using System;

namespace Meadows.Entities {
    public class Plant : Entity {
        private readonly Rectangle[] phases;
        private readonly Rectangle _base;
        private readonly Plantable drop;
        private readonly Sheet _sheet;
        private bool grown = false;
        private int hurtTime = 0;
        private int health = 1;
        private int phase = 0;
        private int time = 0;

        public Plant(Plantable drop, int x = 0, int y = 0) {
            (this.phases, this._base, this._sheet) = drop.GetSprites();
            this.health = 1 + RNG.Next(2);
            this.xr = this.yr = 8;
            this.drop = drop;
            this.x = x;
            this.y = y;
        }

        public override void Update(GameTime dt) {
            base.Update(dt);
            if (hurtTime > 0) --hurtTime;
            if (grown) return;
            if (time >= drop.Ticks) {
                phase = (phase + 1) % phases.Length;
                ++health;
                time = 0;
                return;
            }

            if (phase == phases.Length - 1) {
                grown = true;
            }

            ++time;
        }

        public override void Draw(SpriteBatch batch) {
            var off = hurtOff * (float)Math.Clamp(hurtTime, 0, 3);
            batch.Draw(_sheet.Texture, new Vector2(this.x + off.X - xr * 2 - Tiles.Tiles.xo, this.y + off.Y - yr * 2 - Tiles.Tiles.yo), _base, Color.White);
            batch.Draw(_sheet.Texture, new Vector2(this.x + off.X - xr * 2 - Tiles.Tiles.xo - 1, this.y + off.Y - yr * 2 - Tiles.Tiles.yo - 2
                ), phases[phase], Color.White);
            base.Draw(batch);
        }

        private static readonly Vector2[] _offs = new Vector2[] {
            new Vector2(0, -1),
            new Vector2(-1, 0),
            new Vector2(0, +1),
            new Vector2(+1, 0),
        };

        private Vector2 hurtOff = Vector2.Zero;
        public override void Hurt(Level level, Mob mob, int damage, int direction) {
            if (this.health <= 0) return;
            if (damage > 0) {
                level.Add(new Meadows.Entities.Text(damage.ToString(), x, y, Microsoft.Xna.Framework.Color.PaleVioletRed));
                level.Add(new Meadows.Entities.BitParticle(x - 2, y - 2, Color.DarkOliveGreen));
                level.Add(new Meadows.Entities.BitParticle(x, y, Color.DarkOliveGreen));
                level.Add(new Meadows.Entities.BitParticle(x + 2, y + 2, Color.DarkOliveGreen));
                level.Add(new Meadows.Entities.BitParticle(x, y + 2, Color.DarkOliveGreen));
                this.health -= damage;
                if (this.health <= 0) {
                    if (grown) {
                        level.Add(new EItem(new ResourceItem(this.drop), x, y));
                        if (RNG.NextDouble() < 0.65)
                            level.Add(new EItem(new ResourceItem(this.drop), x, y));
                    }

                    Removed = true;
                }

                this.hurtOff = _offs[direction & 3];
                this.hurtTime = 10;
            }
        }

        public override bool Blocks(Entity e) {
            return true;
        }
    }
}
