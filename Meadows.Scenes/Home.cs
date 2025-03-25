using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Meadows.Entities;
using Meadows.Levels;

namespace Meadows.Scenes {
    public class Home : Scene {
        private Player player;
        private Level level;

        public Home(Main mref) {
            this.level = new Level(32, 32);
            var bpath = "Meadows.Content/Levels/";
            this.level.Layer(bpath + "Outside_Water.csv", Sheets.Water);
            this.level.Layer(bpath + "Outside_Outside.csv", Sheets.Outside);
            this.level.Layer(bpath + "Outside_OutsideOver.csv", Sheets.Outside);
            this.level.Layer(bpath + "Outside_Rocks.csv", Sheets.Rocks);
            this.level.Layer(bpath + "Outside_RocksOver.csv", Sheets.Rocks);
            this.player = new Player();
            this.level.Add(player);
            this.level.Add(new Tree(823, 5 * 32, Sheets.Outside, (int) (6.5f * 32), (int) (29.85f * 32)));
            var _ = new Tree(22, 5 * 32, Sheets.Outside, (int)(3.85f * 32), (int)(24.85f * 32));
            _.ox = 0.40f;
            this.level.Add(_);
        }

        public override void Load() {
            base.Load();
        }

        public override void Destroy() {
            Sheets.Outside.Texture.Dispose();
            Sheets.Rocks.Texture.Dispose();
            Sheets.Water.Texture.Dispose();
            base.Destroy();
        }

        public override Color Background() {
            return Color.Black;
        }

        public override void Update(GameTime dt) {
            base.Update(dt);
            this.level.Update(dt);
        }

        public override void Draw(SpriteBatch batch, GameTime dt) {
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            var oy = (this.player.y - (Main.Height >> 1));
            var ox = (this.player.x - (Main.Width >> 1));
            this.level.Draw(batch, ox, oy);
            batch.End();

            base.Draw(batch, dt);
        }
    }
}
