using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Meadows.Scenes {
    public class Menu : Scene {
        Texture2D tex;
        public Menu(GraphicsDevice g) {
            tex = new Texture2D(g, 50, 50);
            Color[] col = new Color[50 * 50];
            for (int i = 0; i < 50 * 50; ++i)
                col[i] = Color.White;

            tex.SetData<Color>(col);
        }

        public override void Draw(SpriteBatch batch, GameTime dt) {
            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null);
            batch.Draw(tex, new Rectangle(10, 10, 50, 50), Color.White);
            batch.End();
        }
    }
}
