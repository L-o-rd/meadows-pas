﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using Meadows.Utility;

namespace Meadows.Scenes {
    public class Menu : Scene {
        private readonly string[] each = { "M", "e", "a", "d", "o", "w", "s" };
        private readonly string text = "Meadows";
        private readonly string[] options = {
            "Play", "Settings", "Exit"
        };

        private readonly Option[] actions;
        private delegate void Option();
        private readonly Main mref;

        private SpriteFont title, opt;
        private float bx, by, ts, dft;
        private Texture2D back;
        private float dx, dy;
        private int select;
        private float acc;

        public Menu(Main mref) {
            this.actions = new Option[] {
                this.Play,
                this.Settings,
                this.Quit
            };

            this.bx = this.by = -1f;
            this.dx = this.dy = -1f;
            this.mref = mref;
        }

        private void Play() {
            Main.Switch(Scenes.Home);
        }

        private void Settings() {
            Main.Switch(Scenes.MenuSettings);
        }

        private void Quit() {
            this.mref.Quit();
        }

        public override void Load() {
            this.back = Main.Contents.Load<Texture2D>("Sprites/MenuBackground");
            Sound.Load(Main.Contents, "Confirm", "Sounds/Confirm", 20);
            this.title = Main.Contents.Load<SpriteFont>("Fonts/Logo");
            this.opt = Main.Contents.Load<SpriteFont>("Fonts/Option");
            Sound.Load(Main.Contents, "Select", "Sounds/Select", 20);
            this.select = 0;
            this.acc = 0f;
            this.ts = 1f;
            base.Load();
        }

        public override void Destroy() {
            this.title.Texture.Dispose();
            this.opt.Texture.Dispose();
            this.back.Dispose();
            base.Destroy();
        }

        public override void Update(GameTime dt) {
            if ((this.bx + this.dx) + this.back.Width <= Main.Width) this.dx = 1f;
            else if ((this.bx + this.dx) > -1f) this.dx = -1f;

            if ((this.by + this.dy) + this.back.Height <= Main.Height) this.dy = 1f;
            else if ((this.by + this.dy) > -1f) this.dy = -1f;

            this.acc += (float) dt.ElapsedGameTime.TotalMilliseconds;
            if (this.acc >= 100f) {
                this.bx += this.dx;
                this.by += this.dy;
                this.acc = 0f;
            }

            if (Utility.InputManager.IsKeyPressed(Keys.Down)) {
                this.select = (this.select + 1) % options.Length;
            } else if (Utility.InputManager.IsKeyPressed(Keys.Up)) {
                this.select = (((this.select - 1) % options.Length) + options.Length) % options.Length;
            }

            if (Utility.InputManager.IsKeyPressed(Keys.Enter)) {
                Sound.Play(dt, "Confirm", pitch: -.5f, cooldown: 0.05f);
                this.actions[this.select]();
            }

            this.dft += (float) dt.ElapsedGameTime.TotalMilliseconds * 0.01f;
            this.ts = 1.125f + (float) Math.Sin(this.dft * 0.5f) * 0.125f;
            base.Update(dt);
        }

        public override void Draw(SpriteBatch batch, GameTime dt) {
            var size = this.title.MeasureString(this.text);
            var position = new Vector2((float)(Main.Width - size.X) * 0.5f, 0.1f * Main.Height);
            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null);
            batch.Draw(this.back, new Vector2(bx, by), new Color(244, 244, 244, 175));
            for (int i = 0; i < text.Length; ++i) {
                batch.DrawString(this.title, this.each[i], position + new Vector2(-5f, -5f),
                Color.Black, 0f, Vector2.Zero, 1.15f, SpriteEffects.None, 0f);
                batch.DrawString(this.title, this.each[i], position, Color.FloralWhite);
                position.X += title.MeasureString(each[i]).X * 1.1f;
            }

            var py = 0f;
            for (int i = 0; i < options.Length; ++i) {
                size = this.opt.MeasureString(this.options[i]);
                if (i == this.select) {
                    batch.DrawString(this.opt, this.options[i],
                        new Vector2((float) Main.Width * 0.5f, 0.4f * Main.Height + py + size.Y * 0.5f),
                            Color.PaleVioletRed, 0f, size * 0.5f, ts, SpriteEffects.None, 0f);
                } else {
                    batch.DrawString(this.opt, this.options[i],
                        new Vector2((float)(Main.Width - size.X) * 0.5f, 0.4f * Main.Height + py),
                            Color.FloralWhite);
                }

                py += size.Y + 0.05f * Main.Height;
            }
            
            batch.End();
        }
    }
}
