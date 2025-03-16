﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Meadows.Scenes {
    public enum Scenes : int {
        Splash, Menu, Count
    }

    public class Scene {
        public virtual void Load() { }

        public virtual void Destroy() { }

        public virtual void Update(GameTime dt) { }

        public virtual void Draw(SpriteBatch batch, GameTime dt) { }
    }
}
