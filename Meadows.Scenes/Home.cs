// no loader

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Meadows.Entities;
using Meadows.Levels;
using System;
using Meadows.Items;
using Meadows.Utility;

namespace Meadows.Scenes {
    public class Home : Scene {
        private enum State : int {
            Playing     = 0,
            Paused      = 1,
            Inventory   = 2,
        }

        private readonly string invText = "Inventory";
        private readonly string text = "Paused";
        private readonly Option[] actions;
        private delegate void Option();
        private TimeSpan currentTime = new TimeSpan(6, 0, 0); 
        private const double daySpeed = 180.0; 

        private readonly string[] options = {
            "Continue", "Main Menu"
        };

        private int select = 0, invsel = 0;
        private SpriteFont title, opt;
        private float its, idft;
        private float ts, dft;

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

        private void GenerateBushes(Random rng, Vector2 top, Vector2 bot) {
            for (var x = top.X; x < bot.X; ++x) {
                for (var y = top.Y; y < bot.Y; ++y) {
                    var prob = rng.NextDouble();
                    if (rng.NextDouble() < 0.35) this.level.Add(new Bush(8, 17, 32, Sheets.Trees, (int)((x + 0.5) * 32), (int)((y + 0.5) * 32)));
                    else if (rng.NextDouble() < 0.55) this.level.Add(new Bush(4, 21, 32, Sheets.Trees, (int)((x + 0.5) * 32), (int)((y + 0.5) * 32)));
                    else if (rng.NextDouble() < 0.75) this.level.Add(new Bush(2, 21, 32, Sheets.Trees, (int)((x + 0.5) * 32), (int)((y + 0.5) * 32)));
                }
            }
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
            this.level.Add(new EItem(Tools.Sickle, (int) (15.5 * Tiles.Tile.Width), (int) (20.5 * Tiles.Tile.Height)));
            this.level.Add(new EItem(Tools.Shovel, (int) (14.5 * Tiles.Tile.Width), (int) (21.5 * Tiles.Tile.Height)));

            Random rng = new Random(Environment.TickCount);
            GenerateBushes(rng, new Vector2(11, 4), new Vector2(28, 13));
            GenerateBushes(rng, new Vector2(15, 14), new Vector2(28, 17));
        }

        public override void Load() {
            this.title = Main.Contents.Load<SpriteFont>("Fonts/Logo");
            this.opt = Main.Contents.Load<SpriteFont>("Fonts/Option");
            Sound.Load(Main.Contents, "Collect", "Sounds/Collect", 20);
            Sound.Load(Main.Contents, "Hit", "Sounds/Hit", 20);
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
            currentTime += TimeSpan.FromMinutes(dt.ElapsedGameTime.TotalSeconds * (24 * 60 / daySpeed));

            if (currentTime.TotalHours >= 24)
                currentTime -= TimeSpan.FromHours(24);

            base.Update(dt);
            if (Utility.InputManager.IsKeyPressed(Keys.Escape)) {
                if (this.state == State.Playing || this.state == State.Paused) this.state = (State)((int)this.state ^ 1);
                else this.state = State.Playing;
            }

            if (Utility.InputManager.IsKeyPressed(Keys.I) &&
                (this.state == State.Playing || this.state == State.Inventory)) {
                this.state = this.state == State.Playing ? State.Inventory : State.Playing;
            }

            if (this.state == State.Playing) this.level.Update(dt);
            else if (this.state == State.Paused) {
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
            } else { /* State.Inventory */
                var len = player.inventory.Items.Count;
                if (len > 0) {
                    if (Utility.InputManager.IsKeyPressed(Keys.Down)) {
                        this.invsel = (this.invsel + 1) % len;
                    } else if (Utility.InputManager.IsKeyPressed(Keys.Up)) {
                        this.invsel = (((this.invsel - 1) % len) + len) % len;
                    }

                    if (Utility.InputManager.IsKeyPressed(Keys.Enter)) {
                        var inv = player.inventory.Items;
                        var temp = inv[0];
                        inv[0] = inv[this.invsel];
                        inv[this.invsel] = temp;
                    }
                }

                this.idft += (float) dt.ElapsedGameTime.TotalMilliseconds * 0.01f;
                this.its = 1.125f + (float) Math.Sin(this.idft * 0.5f) * 0.125f;
            }
        }

        public override void Draw(SpriteBatch batch, GameTime dt) {
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            var oy = (this.player.y - (Main.Height >> 1));
            var ox = (this.player.x - (Main.Width >> 1));
            this.level.Draw(batch, ox, oy);
            this.player.inventory.DrawHotbars(batch, player.activeSlot);
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
            } else if (state == State.Inventory) {
                batch.Draw(whole, new Rectangle(0, 0, Main.Width, Main.Height), Color.Black * 0.35f);
                var size = this.title.MeasureString(this.invText);
                var position = new Vector2((float) (Main.Width - size.X) * 0.5f, 0.1f * Main.Height);
                batch.DrawString(this.title, this.invText, position, Color.Wheat);
                if (this.invsel < 3) {
                    var py = 0f;
                    for (int i = 0; i < 3; ++i) {
                        if (i >= player.inventory.Items.Count) break;
                        var item = player.inventory.Items[i];
                        var str = $"{i + 1}. {item.Name}";
                        if (item is ResourceItem res) str = $"{str} ({res.Count})";
                        size = this.opt.MeasureString(str);
                        if (i == this.invsel) {
                            batch.DrawString(this.opt, str,
                                new Vector2((float)Main.Width * 0.5f, 0.4f * Main.Height + py + size.Y * 0.5f),
                                    Color.PaleVioletRed, 0f, size * 0.5f, its, SpriteEffects.None, 0f);
                        } else {
                            batch.DrawString(this.opt, str,
                                new Vector2((float)(Main.Width - size.X) * 0.5f, 0.4f * Main.Height + py),
                                    Color.FloralWhite);
                        }

                        py += size.Y + 0.075f * Main.Height;
                    }
                } else {
                    var st = this.invsel - 2;
                    var mx = this.invsel;
                    var py = 0f;
                    for (int i = st; i <= mx; ++i) {
                        if (i >= player.inventory.Items.Count) break;
                        var item = player.inventory.Items[i];
                        var str = $"{i + 1}. {item.Name}";
                        if (item is ResourceItem res) str = $"{str} ({res.Count})";
                        size = this.opt.MeasureString(str);
                        if (i == this.invsel) {
                            batch.DrawString(this.opt, str,
                                new Vector2((float)Main.Width * 0.5f, 0.4f * Main.Height + py + size.Y * 0.5f),
                                    Color.PaleVioletRed, 0f, size * 0.5f, its, SpriteEffects.None, 0f);
                        } else {
                            batch.DrawString(this.opt, str,
                                new Vector2((float)(Main.Width - size.X) * 0.5f, 0.4f * Main.Height + py),
                                    Color.FloralWhite);
                        }

                        py += size.Y + 0.075f * Main.Height;
                    }
                }
            }

            var timeString = currentTime.ToString(@"hh\:mm");
            var timeFont = this.opt; 
            batch.DrawString(timeFont, $"Time: {timeString}", new Vector2(10, 10), Color.FloralWhite);

            float overlayAlpha = 0f;
            if (currentTime.TotalHours >= 18 || currentTime.TotalHours < 6)
            {
                overlayAlpha = 0.5f; 
            }
            else if (currentTime.TotalHours >= 6 && currentTime.TotalHours < 8)
            {
                overlayAlpha = 0.5f - (float)((currentTime.TotalHours - 6) / 2) * 0.5f;
            }
            else if (currentTime.TotalHours >= 16 && currentTime.TotalHours < 18)
            {
                overlayAlpha = (float)((currentTime.TotalHours - 16) / 2) * 0.5f;
            }

            if (overlayAlpha > 0f)
            {
                batch.Draw(whole, new Rectangle(0, 0, Main.Width, Main.Height), Color.Black * overlayAlpha);
            }



            batch.End();
            base.Draw(batch, dt);
        }
    }
}
