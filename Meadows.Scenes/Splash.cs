using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Meadows.Scenes {
    public class Splash : Scene {
        private readonly string[] each = { "M", "e", "a", "d", "o", "w", "s" };
        private float[] dfts = { 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        private readonly string text = "Meadows";
        private SpriteFont font;
        private float dft, fd;
        public Splash() {
            this.dft = 0f;
            this.fd = 1f;
        }

        public override void Load() {
            this.font = Main.Contents.Load<SpriteFont>("Fonts/Logo");
        }

        public override void Destroy() {
            this.font.Texture.Dispose();
        }

        public override void Update(GameTime dt) {
            for (int i = 0; i < dfts.Length; ++i)
                dfts[i] = (float)(Math.Sin(this.dft + Math.PI * 0.5 * i) * 5);

            this.fd -= (float)dt.ElapsedGameTime.TotalMilliseconds * 0.0002f;
            if (this.fd <= -0.1f) {
                this.Destroy();
                Main.Switch(Scenes.Menu);
            }

            this.dft += (float) dt.ElapsedGameTime.TotalMilliseconds * 0.01f;
        }

        public override void Draw(SpriteBatch batch, GameTime dt) {
            var whole = font.MeasureString(text);
            var position = new Vector2((Main.Width - whole.X) * 0.5f, (Main.Height - whole.Y) * 0.5f);
            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null);
            var org = position.Y;
            position.Y = org + dfts[0];
            batch.DrawString(font, each[0], position, Color.AntiqueWhite * this.fd);
            for (int i = 1; i < text.Length; ++i) {
                position.Y = org + dfts[i];
                position.X += font.MeasureString(each[i - 1]).X;
                batch.DrawString(font, each[i], position, Color.AntiqueWhite * this.fd);
            }

            batch.End();
        }
    }
}
