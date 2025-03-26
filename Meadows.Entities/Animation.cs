using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meadows.Entities {
    public class Animation {
        private readonly Vector2 corner;
        private readonly Vector2 scale;
        private readonly Sheet _sheet;
        private readonly float limit;
        private readonly bool _loop;
        private readonly int unit;
        private int frame, frames;
        private float counter;
        private bool once;

        public Animation(Sheet sheet, Vector2 corner, Vector2 scale, int frames, int unit = 32, int frame = 0, float limit = 1000f / 60f, bool loop = false) {
            this.frames = frames;
            this.corner = corner;
            this.scale = scale;
            this.frame = frame;
            this.limit = limit;
            this.counter = 0f;
            this.once = false;
            this.unit = unit;
            _sheet = sheet;
            _loop = loop;
        }

        public void Update(GameTime dt) {
            if (!this._loop && this.once) return;
            this.counter += (float) dt.ElapsedGameTime.TotalMilliseconds * 0.1f;
            if (this.counter >= this.limit) {
                this.counter -= this.limit;
                if (++this.frame >= this.frames) {
                    this.once = true;
                    this.frame = 0;
                }
            }
        }

        public void DrawDirected(SpriteBatch batch, Vector2 at, int dir = 0) {
            batch.Draw(this._sheet.Texture, at, this._sheet.Source(this.corner, this.unit, scale, this.frame, dir), Color.White);
        }

        public void Draw(SpriteBatch batch, Vector2 at) {
            batch.Draw(this._sheet.Texture, at, this._sheet.Source(this.corner, this.unit, scale, offx: this.frame), Color.White);
        }
    }
}
