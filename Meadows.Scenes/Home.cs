using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Meadows.Entities;
using Meadows.Levels;
using System;

namespace Meadows.Scenes {
    public class Home : Scene {
        private enum State : int {
            Playing = 0,
            Paused  = 1,
        }

        private readonly string text = "Paused";
        private readonly Option[] actions;
        private delegate void Option();

        private readonly string[] options = {
            "Continue", "Main Menu"
        };

        private SpriteFont title, opt;
        private int select = 0;
        private float ts, dft;

        private readonly static Vector2 BushBot = new Vector2(12, 9);
        private readonly static Vector2 BushTop = new Vector2(9, 4);
        private readonly Texture2D whole;
        private Player player;
        private State state;
        private Level level;

        private void OptContinue() {
            this.state = State.Playing;
        }

        private void OptBackMenu() {
            Main.Switch(Scenes.Menu);
        }

        public Home() {
            this.whole = new Texture2D(Main.Graphics.GraphicsDevice, 1, 1);
            this.whole.SetData<Color>(new Color[] { Color.White });
            this.actions = new Option[] {
                this.OptContinue,
                this.OptBackMenu
            };

            this.state = State.Playing;
            this.level = new Level(32, 32);
            var bpath = "Meadows.Content/Levels/";
            var _ = this.level.Layer(bpath + "Outside_Water.csv", Sheets.Water);
            foreach (var tile in _) {
                if (tile is null) continue;
                this.level.animated.Add(tile);
                tile.Swimmable = true;
            }

            foreach (var tile in Tiles.Tiles.Zeros) {
                if (tile is null) continue;
                this.level.animated.Add(tile);
            }

            this.level.Layer(bpath + "Outside_Outside.csv", Sheets.Outside);
            this.level.Layer(bpath + "Outside_OutsideOver.csv", Sheets.Outside);
            this.level.Layer(bpath + "Outside_Rocks.csv", Sheets.Rocks);
            this.level.Layer(bpath + "Outside_RocksOver.csv", Sheets.Rocks);
            this.player = new Player();
            this.level.Add(player);
            this.level.Add(new Tree(823, 5 * 32, Sheets.Outside, (int) (6.5f * 32), (int) (29.85f * 32)));
            this.level.Add(new Tree(22, 5 * 32, Sheets.Outside, (int) (3.85f * 32), (int)(24.85f * 32), 0.375f));

            Random rng = new Random(Environment.TickCount);
            for (var x = BushTop.X; x < BushBot.X; ++x) {
                for (var y = BushTop.Y; y < BushBot.Y; ++y) {
                    if (rng.NextDouble() < 0.75)
                        this.level.Add(new Bush(2, 21, 32, Sheets.Trees, (int)((x + 0.5) * 32), (int)((y + 0.5) * 32)));
                }
            }
        }

        public override void Load() {
            this.title = Main.Contents.Load<SpriteFont>("Fonts/Logo");
            this.opt = Main.Contents.Load<SpriteFont>("Fonts/Option");
            this.state = State.Playing;
            this.select = 0;
            this.ts = 1f;
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
            if (Utility.InputManager.IsKeyPressed(Keys.Escape)) {
                this.state = (State) ((int) this.state ^ 1);
            }

            if (this.state == State.Playing) this.level.Update(dt);
            else {
                if (Utility.InputManager.IsKeyPressed(Keys.Down)) {
                    this.select = (this.select + 1) % options.Length;
                } else if (Utility.InputManager.IsKeyPressed(Keys.Up)) {
                    this.select = (((this.select - 1) % options.Length) + options.Length) % options.Length;
                }

                if (Utility.InputManager.IsKeyPressed(Keys.Enter)) {
                    this.actions[this.select]();
                }

                this.dft += (float)dt.ElapsedGameTime.TotalMilliseconds * 0.01f;
                this.ts = 1.125f + (float)Math.Sin(this.dft * 0.5f) * 0.125f;
            }
        }

        public override void Draw(SpriteBatch batch, GameTime dt) {
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            var oy = (this.player.y - (Main.Height >> 1));
            var ox = (this.player.x - (Main.Width >> 1));
            this.level.Draw(batch, ox, oy);
            if (state == State.Paused) {
                batch.Draw(whole, new Rectangle(0, 0, Main.Width, Main.Height), Color.Black * 0.35f);
                var size = this.title.MeasureString(this.text);
                var position = new Vector2((float) (Main.Width - size.X) * 0.5f, 0.1f * Main.Height);
                batch.DrawString(this.title, this.text, position, Color.Wheat);
                var py = 0f;
                for (int i = 0; i < options.Length; ++i) {
                    size = this.opt.MeasureString(this.options[i]);
                    if (i == this.select) {
                        batch.DrawString(this.opt, this.options[i],
                            new Vector2((float)Main.Width * 0.5f, 0.4f * Main.Height + py + size.Y * 0.5f),
                                Color.PaleVioletRed, 0f, size * 0.5f, ts, SpriteEffects.None, 0f);
                    } else {
                        batch.DrawString(this.opt, this.options[i],
                            new Vector2((float)(Main.Width - size.X) * 0.5f, 0.4f * Main.Height + py),
                                Color.FloralWhite);
                    }

                    py += size.Y + 0.075f * Main.Height;
                }
            }

            batch.End();
            base.Draw(batch, dt);
        }
    }
}
