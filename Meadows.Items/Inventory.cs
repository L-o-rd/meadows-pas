using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Meadows.Items {
    public class Inventory {
        public List<Item> Items { get; }

        public Inventory() => this.Items = new List<Item>();

        public void Add(Item item) {
            Add(Items.Count, item);
        }

        public void Add(int slot, Item item) {
            if (item is ResourceItem res) {
                var has = FindResource(res.Resource);
                if (has is null) {
                    Items.Insert(slot, res);
                } else {
                    has.Count += res.Count;
                }
            } else {
                Items.Insert(slot, item);
            }
        }

        private ResourceItem FindResource(Resource resource) {
            foreach (var item in Items) {
                if (item is ResourceItem has) {
                    if (has.Resource == resource)
                        return has;
                }
            }

		    return null;
	    }

        private static readonly int NHotbars = 8;
        public void DrawHotbars(SpriteBatch batch, int active) {
            var len = (38 + 2) * NHotbars - 2;
            var pos = new Vector2(Main.Width >> 1, 0.9f * Main.Height);
            for (int i = 0; i < 8; ++i) {
                var color = i == active ? Color.PaleVioletRed : Color.White;
                var xx = pos.X - len * 0.5f + i * (38 + 2);
                batch.Draw(HotbarSlotTexture, new Vector2(xx, pos.Y), color);
                if (Items.Count > i) Items[i].DrawSlot(batch, (int) (xx + 1), (int) (pos.Y + 3), Color.White);
            }
        }

        public static readonly Texture2D HotbarSlotTexture = null;
        static Inventory() {
            int size = 38;
            HotbarSlotTexture = new Texture2D(Main.Graphics.GraphicsDevice, size, size);
            Color[] data = new Color[size * size];

            int borderThickness = 3;
            Color borderColor = new Color(200, 230, 255, 233);
            Color innerColor = new Color(255, 255, 255, 88);

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float maxDist = center.Length();

            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    bool isBorderX = x < borderThickness || x >= size - borderThickness;
                    bool isBorderY = y < borderThickness || y >= size - borderThickness;
                    bool isEdge = isBorderX || isBorderY;

                    bool isNearCorner =
                        (x < borderThickness && y < borderThickness) ||
                        (x >= size - borderThickness && y < borderThickness) ||
                        (x < borderThickness && y >= size - borderThickness) ||
                        (x >= size - borderThickness && y >= size - borderThickness);

                    Color pixelColor;

                    if (isEdge) {
                        byte alphab = borderColor.A;

                        if (isNearCorner) {
                            alphab = (byte)(borderColor.A * 0.75f);
                        }

                        pixelColor = new Color(borderColor.R, borderColor.G, borderColor.B, alphab);
                    } else {
                        pixelColor = innerColor;
                    }

                    float alpha = pixelColor.A / 255f;
                    data[y * size + x] = new Color (
                        (byte) (pixelColor.R * alpha),
                        (byte) (pixelColor.G * alpha),
                        (byte) (pixelColor.B * alpha),
                        pixelColor.A
                    );
                }
            }

            HotbarSlotTexture.SetData<Color>(data);
        }
    }
}
