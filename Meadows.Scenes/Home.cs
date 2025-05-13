using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Meadows.Entities;
using Meadows.Levels;
using System;
using Meadows.Items;
using Meadows.Utility;
using System.Linq;
using System.Collections.Generic;

namespace Meadows.Scenes
{
    public class Home : Scene
    {
        private enum State : int
        {
            Playing = 0,
            Paused = 1,
            Inventory = 2,
            Trading = 3,
        }
        enum TradeState { Main, BuyMenu, SellMenu }
        TradeState currentTradeState = TradeState.Main;

        private readonly string invText = "Inventory";
        private readonly string text = "Paused";
        private readonly Option[] actions;
        private delegate void Option();

        private TimeSpan currentTime = new TimeSpan(6, 0, 0);
        private const double daySpeed = 180.0;
        private GameClock clock;
        private Texture2D dateBoard;
        private Texture2D day_night_clock;
        private Texture2D arrow;

        private readonly string[] options = {
            "Continue", "Main Menu"
        };

        private int tradeSelect = 0;
        private float tradeTs = 1.0f;
        private float tradeDft = 0f;
        private string[] tradingOptions = new string[] {
            "Buy",
            "Sell",
            "Exit"
        };

        private int select = 0, invsel = 0;
        private SpriteFont title, opt;
        private float its, idft;
        private float ts, dft;

        private readonly Texture2D whole;
        private Player player;
        private State state;
        private Level level;

        private void OptContinue()
        {
            this.state = State.Playing;
        }

        private void OptBackMenu()
        {
            Main.Switch(Scenes.Menu);
        }

        private void GenerateBushes(Random rng, Vector2 top, Vector2 bot)
        {
            for (var x = top.X; x < bot.X; ++x)
            {
                for (var y = top.Y; y < bot.Y; ++y)
                {
                    var prob = rng.NextDouble();
                    if (rng.NextDouble() < 0.35) this.level.Add(new Bush(8, 17, 32, Sheets.Trees, (int)((x + 0.5) * 32), (int)((y + 0.5) * 32)));
                    else if (rng.NextDouble() < 0.55) this.level.Add(new Bush(4, 21, 32, Sheets.Trees, (int)((x + 0.5) * 32), (int)((y + 0.5) * 32)));
                    else if (rng.NextDouble() < 0.75) this.level.Add(new Bush(2, 21, 32, Sheets.Trees, (int)((x + 0.5) * 32), (int)((y + 0.5) * 32)));
                }
            }
        }

        private NPC Sara;

        public Home()
        {
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
            foreach (var tile in _)
            {
                if (tile is null) continue;
                this.level.animated.Add(tile);
                tile.Swimmable = true;
            }

            foreach (var tile in Tiles.Tiles.Zeros)
            {
                if (tile is null) continue;
                this.level.animated.Add(tile);
            }

            this.level.Layer(bpath + "Outside_Outside.csv", Sheets.Outside);
            this.level.Layer(bpath + "Outside_OutsideOver.csv", Sheets.Outside);
            this.level.Layer(bpath + "Outside_Rocks.csv", Sheets.Rocks);
            this.level.Layer(bpath + "Outside_RocksOver.csv", Sheets.Rocks);
            this.player = new Player();
            this.level.Add(player);
            this.level.Add(new Tree(823, 5 * 32, Sheets.Outside, (int)(6.5f * 32), (int)(29.85f * 32)));
            this.level.Add(new Tree(22, 5 * 32, Sheets.Outside, (int)(3.85f * 32), (int)(24.85f * 32), 0.375f));
            this.level.Add(new EItem(Tools.Sickle, (int)(15.5 * Tiles.Tile.Width), (int)(20.5 * Tiles.Tile.Height)));
            this.level.Add(new EItem(Tools.Shovel, (int)(14.5 * Tiles.Tile.Width), (int)(21.5 * Tiles.Tile.Height)));

            this.clock = new GameClock();
            Sara = new NPC(
               (int)(13.5f * Tiles.Tile.Width),
               (int)(24.5f * Tiles.Tile.Height)
            );

            this.level.Add(Sara);

            Random rng = new Random(Environment.TickCount);
            GenerateBushes(rng, new Vector2(11, 4), new Vector2(28, 13));
            GenerateBushes(rng, new Vector2(15, 14), new Vector2(28, 17));
        }

        public override void Load()
        {
            this.title = Main.Contents.Load<SpriteFont>("Fonts/Logo");
            this.opt = Main.Contents.Load<SpriteFont>("Fonts/Option");
            this.dateBoard = Main.Contents.Load<Texture2D>("Sprites/Box");
            this.day_night_clock = Main.Contents.Load<Texture2D>("Sprites/Clock");
            this.arrow = Main.Contents.Load<Texture2D>("Sprites/Arrow");
            Sound.Load(Main.Contents, "Collect", "Sounds/Collect", 20);
            Sound.Load(Main.Contents, "Hit", "Sounds/Hit", 20);
            this.state = State.Playing;
            this.select = 0;
            this.ts = 1f;
            base.Load();
        }

        public override void Destroy()
        {
            Sheets.Outside.Texture.Dispose();
            Sheets.Rocks.Texture.Dispose();
            Sheets.Water.Texture.Dispose();
            base.Destroy();
        }

        public override Color Background()
        {
            return Color.Black;
        }

        private bool IsNearNPC()
        {
            const int range = 48;
            return level.GetEntities(
                player.x - range,
                player.y - range,
                player.x + range,
                player.y + range
            ).Any(e => e is NPC);
        }

        private int GetSellPrice(Item item)
        {
            if (item is Tool tool)
            {
                return tool.Name switch
                {
                    "Shovel" => 40,
                    "Sickle" => 60,
                    _ => 10
                };
            }

            if (item is ResourceItem resource)
            {
                return resource.Name switch
                {
                    "Potato" => 10,
                    "Carrot" => 15,
                    "Beetroot" => 20,
                    "RedBellPepper" => 20,
                    "Pumpkin" => 25,
                    _ => 5
                };
            }

            return 0;
        }

        public override void Update(GameTime dt)
        {
            currentTime += TimeSpan.FromMinutes(dt.ElapsedGameTime.TotalSeconds * (24 * 60 / daySpeed));

            if (currentTime.TotalHours >= 24)
                currentTime -= TimeSpan.FromHours(24);
            base.Update(dt);

            if (Utility.InputManager.IsKeyPressed(Keys.Escape))
            {
                if (this.state == State.Playing || this.state == State.Paused)
                    this.state = (State)((int)this.state ^ 1);
                else
                    this.state = State.Playing;
            }

            if (Utility.InputManager.IsKeyPressed(Keys.I) &&
                (this.state == State.Playing || this.state == State.Inventory))
            {
                this.state = this.state == State.Playing ? State.Inventory : State.Playing;
            }

            if (this.state == State.Playing)
            {
                if (IsNearNPC() && Utility.InputManager.IsKeyPressed(Keys.B))
                {
                    this.state = State.Trading;
                    this.tradeSelect = 0;
                    Sara.SetPortrait(-1);
                }
                this.clock.Update(dt);

                this.level.Update(dt);
            }
            else if (this.state == State.Paused)
            {
                if (Utility.InputManager.IsKeyPressed(Keys.Down))
                {
                    this.select = (this.select + 1) % options.Length;
                }
                else if (Utility.InputManager.IsKeyPressed(Keys.Up))
                {
                    this.select = (((this.select - 1) % options.Length) + options.Length) % options.Length;
                }

                if (Utility.InputManager.IsKeyPressed(Keys.Enter))
                {
                    this.actions[this.select]();
                }

                this.dft += (float)dt.ElapsedGameTime.TotalMilliseconds * 0.01f;
                this.ts = 1.125f + (float)Math.Sin(this.dft * 0.5f) * 0.125f;
            }
            else if (this.state == State.Inventory)
            {
                var len = player.inventory.Items.Count;
                if (len > 0)
                {
                    if (Utility.InputManager.IsKeyPressed(Keys.Down))
                    {
                        this.invsel = (this.invsel + 1) % len;
                    }
                    else if (Utility.InputManager.IsKeyPressed(Keys.Up))
                    {
                        this.invsel = (((this.invsel - 1) % len) + len) % len;
                    }

                    if (Utility.InputManager.IsKeyPressed(Keys.Enter))
                    {
                        var inv = player.inventory.Items;
                        var temp = inv[0];
                        inv[0] = inv[this.invsel];
                        inv[this.invsel] = temp;
                    }
                }

                this.idft += (float)dt.ElapsedGameTime.TotalMilliseconds * 0.01f;
                this.its = 1.125f + (float)Math.Sin(this.idft * 0.5f) * 0.125f;
            }
            else if (this.state == State.Trading)
            {
                if (Utility.InputManager.IsKeyPressed(Keys.Escape))
                {
                    if (currentTradeState == TradeState.Main)
                        this.state = State.Playing;
                    else
                        currentTradeState = TradeState.Main;
                }

                if (currentTradeState == TradeState.Main)
                {
                    if (Utility.InputManager.IsKeyPressed(Keys.Down))
                        tradeSelect = (tradeSelect + 1) % tradingOptions.Length;
                    else if (Utility.InputManager.IsKeyPressed(Keys.Up))
                        tradeSelect = (((tradeSelect - 1) % tradingOptions.Length) + tradingOptions.Length) % tradingOptions.Length;
                    else if (Utility.InputManager.IsKeyPressed(Keys.Enter))
                    {
                        var selected = tradingOptions[tradeSelect];
                        if (selected == "Buy")
                        {
                            currentTradeState = TradeState.BuyMenu;
                            tradeSelect = 0;
                        }
                        else if (selected == "Sell")
                        {
                            currentTradeState = TradeState.SellMenu;
                            tradeSelect = 0;
                        }
                        else if (selected == "Exit")
                        {
                            this.state = State.Playing;
                        }
                    }
                }
                else if (currentTradeState == TradeState.BuyMenu)
                {
                    var itemsForSale = new List<Item>() { Tools.Shovel, Tools.Sickle };
                    var prices = new Dictionary<string, int> { { "Shovel", 100 }, { "Sickle", 200 } };
                    var optionsCount = itemsForSale.Count + 1;

                    if (Utility.InputManager.IsKeyPressed(Keys.Down))
                        tradeSelect = (tradeSelect + 1) % optionsCount;
                    else if (Utility.InputManager.IsKeyPressed(Keys.Up))
                        tradeSelect = (((tradeSelect - 1) % optionsCount) + optionsCount) % optionsCount;
                    else if (Utility.InputManager.IsKeyPressed(Keys.Enter))
                    {
                        if (tradeSelect == itemsForSale.Count)
                        {
                            currentTradeState = TradeState.Main;
                            tradeSelect = 0;
                        }
                        else
                        {
                            var item = itemsForSale[tradeSelect];
                            int price = prices[item.Name];

                            if (player.inventory.Gold >= price)
                            {
                                player.inventory.Gold -= price;
                                player.inventory.Add(item);
                            }
                        }
                    }
                }
                else if (currentTradeState == TradeState.SellMenu)
                {
                    var invItems = player.inventory.Items;
                    var optionsCount = invItems.Count + 1;

                    if (Utility.InputManager.IsKeyPressed(Keys.Down))
                        tradeSelect = (tradeSelect + 1) % optionsCount;
                    else if (Utility.InputManager.IsKeyPressed(Keys.Up))
                        tradeSelect = (((tradeSelect - 1) % optionsCount) + optionsCount) % optionsCount;
                    else if (Utility.InputManager.IsKeyPressed(Keys.Enter))
                    {
                        if (tradeSelect == invItems.Count)
                        {
                            currentTradeState = TradeState.Main;
                            tradeSelect = 0;
                        }
                        else
                        {
                            var item = invItems[tradeSelect];
                            var basePrice = GetSellPrice(invItems[tradeSelect]);

                            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                            {
                                // Sell entire stack
                                if (item is ResourceItem res)
                                {
                                    player.inventory.Gold += basePrice * res.Count;
                                    invItems.RemoveAt(tradeSelect);
                                }
                                else
                                {
                                    player.inventory.Gold += basePrice;
                                    invItems.RemoveAt(tradeSelect);
                                }
                            }
                            else
                            {
                                // Sell single item
                                if (item is ResourceItem res)
                                {
                                    player.inventory.Gold += basePrice;
                                    res.Count--;
                                    if (res.Count <= 0)
                                    {
                                        invItems.RemoveAt(tradeSelect);
                                    }
                                }
                                else
                                {
                                    player.inventory.Gold += basePrice;
                                    invItems.RemoveAt(tradeSelect);
                                }
                            }

                            tradeSelect = Math.Min(tradeSelect, invItems.Count);
                        }
                    }
                }

                this.tradeDft += (float)dt.ElapsedGameTime.TotalMilliseconds * 0.01f;
                this.tradeTs = 1.125f + (float)Math.Sin(this.tradeDft * 0.5f) * 0.125f;
            }
        }

        public override void Draw(SpriteBatch batch, GameTime dt)
        {
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            var oy = (this.player.y - (Main.Height >> 1));
            var ox = (this.player.x - (Main.Width >> 1));
            this.level.Draw(batch, ox, oy);
            this.player.inventory.DrawHotbars(batch, player.activeSlot);
            if (state == State.Paused)
            {
                batch.Draw(whole, new Rectangle(0, 0, Main.Width, Main.Height), Color.Black * 0.35f);
                var size = this.title.MeasureString(this.text);
                var position = new Vector2((float)(Main.Width - size.X) * 0.5f, 0.1f * Main.Height);
                batch.DrawString(this.title, this.text, position, Color.Wheat);
                var py = 0f;
                for (int i = 0; i < options.Length; ++i)
                {
                    size = this.opt.MeasureString(this.options[i]);
                    if (i == this.select)
                    {
                        batch.DrawString(this.opt, this.options[i],
                            new Vector2((float)Main.Width * 0.5f, 0.4f * Main.Height + py + size.Y * 0.5f),
                                Color.PaleVioletRed, 0f, size * 0.5f, ts, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        batch.DrawString(this.opt, this.options[i],
                            new Vector2((float)(Main.Width - size.X) * 0.5f, 0.4f * Main.Height + py),
                                Color.FloralWhite);
                    }

                    py += size.Y + 0.075f * Main.Height;
                }
            }
            else if (state == State.Inventory)
            {
                batch.Draw(whole, new Rectangle(0, 0, Main.Width, Main.Height), Color.Black * 0.35f);
                var size = this.title.MeasureString(this.invText);
                var position = new Vector2((float)(Main.Width - size.X) * 0.5f, 0.1f * Main.Height);
                batch.DrawString(this.title, this.invText, position, Color.Wheat);
                if (this.invsel < 3)
                {
                    var py = 0f;
                    for (int i = 0; i < 3; ++i)
                    {
                        if (i >= player.inventory.Items.Count) break;
                        var item = player.inventory.Items[i];
                        var str = $"{i + 1}. {item.Name}";
                        if (item is ResourceItem res) str = $"{str} ({res.Count})";
                        size = this.opt.MeasureString(str);
                        if (i == this.invsel)
                        {
                            batch.DrawString(this.opt, str,
                                new Vector2((float)Main.Width * 0.5f, 0.4f * Main.Height + py + size.Y * 0.5f),
                                    Color.PaleVioletRed, 0f, size * 0.5f, its, SpriteEffects.None, 0f);
                        }
                        else
                        {
                            batch.DrawString(this.opt, str,
                                new Vector2((float)(Main.Width - size.X) * 0.5f, 0.4f * Main.Height + py),
                                    Color.FloralWhite);
                        }

                        py += size.Y + 0.075f * Main.Height;
                    }
                }
                else
                {
                    var st = this.invsel - 2;
                    var mx = this.invsel;
                    var py = 0f;
                    for (int i = st; i <= mx; ++i)
                    {
                        if (i >= player.inventory.Items.Count) break;
                        var item = player.inventory.Items[i];
                        var str = $"{i + 1}. {item.Name}";
                        if (item is ResourceItem res) str = $"{str} ({res.Count})";
                        size = this.opt.MeasureString(str);
                        if (i == this.invsel)
                        {
                            batch.DrawString(this.opt, str,
                                new Vector2((float)Main.Width * 0.5f, 0.4f * Main.Height + py + size.Y * 0.5f),
                                    Color.PaleVioletRed, 0f, size * 0.5f, its, SpriteEffects.None, 0f);
                        }
                        else
                        {
                            batch.DrawString(this.opt, str,
                                new Vector2((float)(Main.Width - size.X) * 0.5f, 0.4f * Main.Height + py),
                                    Color.FloralWhite);
                        }

                        py += size.Y + 0.075f * Main.Height;
                    }
                }
            }
            else if (state == State.Trading)
            {
                batch.Draw(whole, new Rectangle(0, 0, Main.Width, Main.Height), Color.Black * 0.35f);
                Sara.Portrait(batch, 10, 100);

                string titleText = "Trading";
                Color titleColor = Color.Gold;
                List<string> optionsToShow = tradingOptions.ToList();
                List<int> prices = new List<int>();

                if (currentTradeState == TradeState.BuyMenu)
                {
                    titleText = "Buy Items";
                    titleColor = Color.LightGreen;
                    optionsToShow = new List<string> { "Shovel - 100G", "Sickle - 200G", "Exit" };
                }
                else if (currentTradeState == TradeState.SellMenu)
                {
                    titleText = "Sell Items";
                    titleColor = Color.IndianRed;

                    optionsToShow = player.inventory.Items.Select(i =>
                    {
                        int sellPricePerUnit = GetSellPrice(i);
                        if (i is ResourceItem res)
                        {
                            return $"{i.Name} ({res.Count}) - {sellPricePerUnit}G";
                        }
                        return $"{i.Name} - {sellPricePerUnit}G";
                    }).ToList();

                    optionsToShow.Add("Exit");
                }

                //string goldText = $"Gold: {player.inventory.Gold}g";
                //var goldSize = opt.MeasureString(goldText);
                //batch.DrawString(opt, goldText,
                //    new Vector2((Main.Width - goldSize.X) * 0.5f, 0.15f * Main.Height),
                //    Color.Gold);

                // Draw gold information at the top
                string goldText = $"Gold: {player.inventory.Gold}g";
                var goldSize = opt.MeasureString(goldText);
                batch.DrawString(opt, goldText,
                    new Vector2(Main.Width - goldSize.X - 20, 20),
                    Color.Gold);

                var size = title.MeasureString(titleText);
                var position = new Vector2((Main.Width - size.X) * 0.5f, 0.2f * Main.Height);
                batch.DrawString(title, titleText, position, titleColor);

                var py = 0f;
                if (currentTradeState == TradeState.SellMenu) {
                    if (optionsToShow.Count > 3) {
                        var last = optionsToShow.Last();
                        var sliceSize = Math.Min(2, optionsToShow.Count - tradeSelect - 1);
                        optionsToShow = optionsToShow.Slice(Math.Min(tradeSelect, optionsToShow.Count - 1), sliceSize);
                        if ((optionsToShow.Count == 0) || (optionsToShow.Last() != last))
                            optionsToShow.Add(last);

                        for (int i = 0; i < optionsToShow.Count; ++i) {
                            size = opt.MeasureString(optionsToShow[i]);
                            if (i == 0) {
                                batch.DrawString(opt, optionsToShow[i],
                                    new Vector2((float)Main.Width * 0.5f, 0.4f * Main.Height + py + size.Y * 0.5f),
                                    Color.LightGreen, 0f, size * 0.5f, tradeTs, SpriteEffects.None, 0f);
                            } else {
                                batch.DrawString(opt, optionsToShow[i],
                                    new Vector2((Main.Width - size.X) * 0.5f, 0.4f * Main.Height + py),
                                    Color.FloralWhite);
                            }

                            py += size.Y + 0.075f * Main.Height;
                        }
                    } else {
                        for (int i = 0; i < optionsToShow.Count; ++i) {
                            size = opt.MeasureString(optionsToShow[i]);
                            if (i == tradeSelect) {
                                batch.DrawString(opt, optionsToShow[i],
                                    new Vector2((float)Main.Width * 0.5f, 0.4f * Main.Height + py + size.Y * 0.5f),
                                    Color.LightGreen, 0f, size * 0.5f, tradeTs, SpriteEffects.None, 0f);
                            } else {
                                batch.DrawString(opt, optionsToShow[i],
                                    new Vector2((Main.Width - size.X) * 0.5f, 0.4f * Main.Height + py),
                                    Color.FloralWhite);
                            }

                            py += size.Y + 0.075f * Main.Height;
                        }
                    }
                } else {
                    for (int i = 0; i < optionsToShow.Count; ++i) {
                        size = opt.MeasureString(optionsToShow[i]);
                        if (i == tradeSelect) {
                            batch.DrawString(opt, optionsToShow[i],
                                new Vector2((float)Main.Width * 0.5f, 0.4f * Main.Height + py + size.Y * 0.5f),
                                Color.LightGreen, 0f, size * 0.5f, tradeTs, SpriteEffects.None, 0f);
                        } else {
                            batch.DrawString(opt, optionsToShow[i],
                                new Vector2((Main.Width - size.X) * 0.5f, 0.4f * Main.Height + py),
                                Color.FloralWhite);
                        }

                        py += size.Y + 0.075f * Main.Height;
                    }
                }
            }
            else
            {
                float globalScale = 0.6f;

                int panelWidth = 123;
                int panelHeight = 107;
                int clockWidth = 71;
                int clockHeight = 107;

                float scaledPanelWidth = panelWidth * globalScale;
                float scaledPanelHeight = panelHeight * globalScale;
                float scaledClockWidth = clockWidth * globalScale;
                float scaledClockHeight = clockHeight * globalScale;

                float panelX = Main.Width - scaledPanelWidth - 10f;
                float panelY = 10f;

                float clockX = panelX - scaledClockWidth;
                float clockY = panelY;

                float scale = 0.45f * globalScale;

                string time = clock.GetTimeString();
                string dayLabel = clock.GetDayLabel();
                string dateStr = clock.GetDateString();

                Vector2 timeSize = opt.MeasureString(time) * scale;
                Vector2 daySize = opt.MeasureString(dayLabel) * scale;
                Vector2 dateSize = opt.MeasureString(dateStr) * scale;

                float centerX = panelX + scaledPanelWidth / 2f;
                float totalTextHeight = timeSize.Y + daySize.Y + dateSize.Y;
                float spacingY = (scaledPanelHeight - totalTextHeight) / 4f;

                float timeY = panelY + spacingY;
                float dayY = timeY + timeSize.Y + spacingY;
                float dateY = dayY + daySize.Y + spacingY;

                batch.Draw(this.day_night_clock, new Vector2(clockX, clockY), null, Color.White, 0f, Vector2.Zero, globalScale, SpriteEffects.None, 0f);
                batch.Draw(this.dateBoard, new Vector2(panelX, panelY), null, Color.White, 0f, Vector2.Zero, globalScale, SpriteEffects.None, 0f);

                batch.DrawString(opt, time, new Vector2(centerX - timeSize.X / 2f, timeY), Color.LightYellow, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                batch.DrawString(opt, dayLabel, new Vector2(centerX - daySize.X / 2f, dayY), Color.LightYellow, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                batch.DrawString(opt, dateStr, new Vector2(centerX - dateSize.X / 2f, dateY), Color.LightYellow, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                Texture2D arrowTex = this.arrow;

                Vector2 clockCenter = new Vector2(clockX + scaledClockWidth, clockY + scaledClockHeight / 2f);

                Vector2 arrowOrigin = new Vector2(arrowTex.Width - 6f, arrowTex.Height / 2f);

                Vector2 arrowPos = clockCenter;

                float timeNormalized = clock.GetNormalizedTime();
                float shiftedTime = (timeNormalized + 0.25f) % 1f;
                float sinValue = (float)Math.Sin(shiftedTime * MathHelper.TwoPi);
                float angle = sinValue * MathHelper.PiOver2;

                batch.Draw(
                    arrowTex,
                    arrowPos,
                    null,
                    Color.White,
                    angle,
                    arrowOrigin,
                    globalScale,
                    SpriteEffects.None,
                    0f
                );

                float normalizedTime = clock.GetNormalizedTime();
                double currentHour = normalizedTime * 24.0;

                float overlayAlpha = 0f;

                if (currentHour >= 18 || currentHour < 6)
                {
                    overlayAlpha = 0.5f;
                }
                else if (currentHour >= 6 && currentHour < 8)
                {
                    overlayAlpha = 0.5f - (float)((currentHour - 6) / 2.0) * 0.5f;
                }
                else if (currentHour >= 16 && currentHour < 18)
                {
                    overlayAlpha = (float)((currentHour - 16) / 2.0) * 0.5f;
                }

                if (overlayAlpha > 0f)
                {
                    batch.Draw(whole, new Rectangle(0, 0, Main.Width, Main.Height), Color.Black * overlayAlpha);
                }
            }

            batch.End();
            base.Draw(batch, dt);
        }
    }
}
