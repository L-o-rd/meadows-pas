using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace Meadows {
    public class Main : Game {
        private static readonly String Title = "Meadows";
        private static GraphicsDeviceManager Graphics;
        private static readonly int[,] Dimensions = {
            {768, 432}, {1344, 756}, {1536, 864},
        };

        private Rectangle renderScaleRectangle;
        public static ContentManager Contents;
        private static Scenes.Scene[] scenes;
        private RenderTarget2D renderTarget;
        private static Scenes.Scene scene;
        private SpriteBatch batch;
        private float aspectRatio;
        public static int Height;
        public static int Width;

        public Main() {
            Main.Graphics = new GraphicsDeviceManager(this);
            Main.Graphics.SynchronizeWithVerticalRetrace = true;

            this.aspectRatio = Main.Dimensions[0, 0] / (float) Main.Dimensions[0, 1];
            this.Content.RootDirectory = "Meadows.Content";
            this.Window.AllowUserResizing = false;
            Main.Height = Main.Dimensions[0, 1];
            Main.Width = Main.Dimensions[0, 0];
            this.Window.AllowAltF4 = true;
            this.IsFixedTimeStep = true;
            this.IsMouseVisible = true;
        }

        protected override void Initialize() {
            this.Window.Title = Main.Title;

            if (Main.Graphics is not null) {
                Main.Graphics.PreferredBackBufferWidth = Main.Dimensions[0, 0];
                Main.Graphics.PreferredBackBufferHeight = Main.Dimensions[0, 1];
                Main.Graphics.ApplyChanges();

                this.renderTarget = new RenderTarget2D(Main.Graphics.GraphicsDevice, Main.Dimensions[0, 0], Main.Dimensions[0, 1], false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
                this.renderScaleRectangle = GetScaleRectangle();
            }

            Main.Contents = new ContentManager(this.Content.ServiceProvider, "Meadows.Content");
            Main.scenes = new Scenes.Scene[(int) Scenes.Scenes.Count];
            Main.scenes[(int)Scenes.Scenes.Splash] = new Scenes.Splash();
            Main.scenes[(int)Scenes.Scenes.Menu] = new Scenes.Menu(Main.Graphics.GraphicsDevice);
            Main.scene = Main.scenes[(int)Scenes.Scenes.Splash];
            base.Initialize();
        }

        protected override void LoadContent() {
            this.batch = new SpriteBatch(this.GraphicsDevice);
            Main.scene.Load();
        }

        private Rectangle GetScaleRectangle() {
            var actualAspectRatio = this.Window.ClientBounds.Width / (float)this.Window.ClientBounds.Height;
            var variance = 0;

            Rectangle scaleRectangle = default;
            if (actualAspectRatio <= this.aspectRatio) {
                var presentHeight = (int)(this.Window.ClientBounds.Width / this.aspectRatio + variance);
                var barHeight = (this.Window.ClientBounds.Height - presentHeight) >> 1;
                scaleRectangle = new Rectangle(0, barHeight, this.Window.ClientBounds.Width, presentHeight);
            } else {
                var presentWidth = (int)(this.Window.ClientBounds.Height * this.aspectRatio + variance);
                var barWidth = (this.Window.ClientBounds.Width - presentWidth) >> 1;
                scaleRectangle = new Rectangle(barWidth, 0, presentWidth, this.Window.ClientBounds.Height);
            }

            return scaleRectangle;
        }

        private void Resolution(int index) {
            if ((index < 0) || (index >= Main.Dimensions.GetLength(0)))
                return;

            if ((Main.Graphics is not null) && !Main.Graphics.IsFullScreen) {
                Main.Graphics.PreferredBackBufferWidth = Main.Dimensions[index, 0];
                Main.Graphics.PreferredBackBufferHeight = Main.Dimensions[index, 1];
                Main.Height = Main.Dimensions[index, 1];
                Main.Width = Main.Dimensions[index, 0];

                Main.Graphics.ApplyChanges();
                this.renderScaleRectangle = this.GetScaleRectangle();
            }
        }

        private void ToggleFullscreen() {
            if (Main.Graphics is not null) {
                if (Main.Graphics.IsFullScreen == true) {
                    Main.Graphics.IsFullScreen = false;
                    this.Resolution(0);
                } else {
                    Main.Graphics.IsFullScreen = true;
                    Main.Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    Main.Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    Main.Graphics.ApplyChanges();
                    Main.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    Main.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    this.renderScaleRectangle = this.GetScaleRectangle();
                }
            }
        }

        protected override void Update(GameTime dt) {
            if (this.IsActive == true) {
                Utility.InputManager.Update();
                if (Utility.InputManager.IsKeyPressed(Keys.Escape)) {
                    Main.Contents.Unload();
                    this.Exit();
                }

                Main.scene.Update(dt);
            }

            base.Update(dt);
        }

        public static void Switch(Scenes.Scenes scene) {
            Main.scene = Main.scenes[(int)scene];
        }

        protected override void Draw(GameTime dt) {
            this.GraphicsDevice.SetRenderTarget(this.renderTarget);
            this.GraphicsDevice.Clear(Color.Black);
            Main.scene.Draw(batch, dt);

            this.GraphicsDevice.SetRenderTarget(null);
            this.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            this.batch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
            this.batch.Draw(this.renderTarget, this.renderScaleRectangle, Color.White);
            this.batch.End();

            base.Draw(dt);
        }
    }
}
