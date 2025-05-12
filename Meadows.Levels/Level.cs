using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Meadows.Entities;
using Meadows.Tiles;
using System.IO;
using System;

namespace Meadows.Levels {
    public enum Where : int {
        Outside = 0,
    }

    class EntityOrder : Comparer<Entity> {
        public override int Compare(Entity a, Entity b) {
            if (b.y < a.y) return +1;
            if (b.y > a.y) return -1;
            return 0;
        }
    }

    public class Level {
        private Comparer<Entity> Sorter = new EntityOrder();
        public List<Entity>[] InTiles;
        public List<Tile[]> _layers;
        public List<Tile> animated;
        private int width, height;
        public List<Entity> All;
        private Player player;

        public Level(int width, int height, Where where = Where.Outside) {
            this.animated = new List<Tile>();
            this.All = new List<Entity>();
            _layers = new List<Tile[]>();
            this.height = height;
            this.width = width;
            this.player = null;

            this.InTiles = new List<Entity>[width * height];
            for (int i = 0; i < width * height; ++i)
                this.InTiles[i] = new List<Entity>();
        }

        public bool InBounds(int xt, int yt) {
            return (xt >= 0) && (yt >= 0) && (xt < this.width) && (yt < this.height);
        }

        public bool Occupied(int xt, int yt) {
            if (!InBounds(xt, yt)) return true;
            return InTiles[xt + yt * width].Count > 0;
        }

        public void Add(Entity e) {
            if (e is Player)
                this.player = e as Player;

            e.Removed = false;
            this.All.Add(e);
            e.Level(this);

            var x = e.x >> 5;
            var y = e.y >> 5;
            if (x < 0 || y < 0 || x >= width || y >= height)
                return;

            InTiles[x + y * width].Add(e);
        }

        public void Remove(Entity e) {
            this.All.Remove(e);
            var x = e.x >> 5;
            var y = e.y >> 5;
            if (x < 0 || y < 0 || x >= width || y >= height)
                return;

            InTiles[x + y * width].Remove(e);
        }

        private void AddAt(int x, int y, Entity e) {
            if (x < 0 || y < 0 || x >= width || y >= height) return;
            InTiles[x + y * width].Add(e);
        }

        private void RemoveAt(int x, int y, Entity e) {
            if (x < 0 || y < 0 || x >= width || y >= height) return;
            InTiles[x + y * width].Remove(e);
        }

        public Tile[] Layer(String source, Sheet sheet) {
            var reader = new StreamReader(source);
            var _tiles = new Tile[width * height];
            var line = String.Empty;
            var y = 0;

            while ((line = reader.ReadLine()) != null) {
                String[] tiles = line.Split(",");
                for (int x = 0; x < tiles.Length; ++x) {
                    if (int.TryParse(tiles[x], out int tileID)) {
                        if (tileID > -1) {
                            _tiles[x + y * width] = new Tile(tileID, sheet);
                        }
                    }
                }

                ++y;
            }

            _layers.Add(_tiles);
            return _tiles;
        }

        public Tile GetTileLayer(Tile[] layer, int x, int y) {
            if (x < 0 || y < 0)
                return Tiles.Tiles.Zeros[((x % 10 + 10) % 10) + ((y & 3 + 4) & 3) * 10];

            if (x >= width || y >= height)
                return Tiles.Tiles.Zeros[(x % 10) + (y & 3) * 10];
            
            return layer[x + y * width];
        }

        public Tile GetLastTile(int x, int y) {
            if (x < 0 || y < 0)
                return Tiles.Tiles.Zeros[((x % 10 + 10) % 10) + ((y & 3 + 4) & 3) * 10];

            if (x >= width || y >= height)
                return Tiles.Tiles.Zeros[(x % 10) + (y & 3) * 10];

            Tile tile = null;
            for (int l = _layers.Count - 1; l >= 0; --l) {
                tile = _layers[l][x + y * width];
                if (tile is not null)
                    return tile;
            }

            return null;
        }

        public Tile GetTile(int x, int y) {
            if (x < 0 || y < 0)
                return Tiles.Tiles.Zeros[((x % 10 + 10) % 10) + ((y & 3 + 4) & 3) * 10];

            if (x >= width || y >= height)
                return Tiles.Tiles.Zeros[(x % 10) + (y & 3) * 10];

            Tile tile = null;
            foreach (var layer in _layers) {
                tile = layer[x + y * width];
                if (tile is not null)
                    return tile;
            }

            throw new ArgumentException($"GetTile({x}, {y}) should have found something!");
        }

        public bool Passable(int xt, int yt, Entity e) {
            var tile = GetTile(xt, yt);
            if (tile.Swimmable && !e.CanSwim())
                return false;

            return tile.Passable;
        }

        private List<Entity> _rows = new List<Entity>();
        public void Draw(SpriteBatch batch, int ox = 0, int oy = 0) {
            var h = (Main.Height + Tile.Height - 1) >> 5;
            var w = (Main.Width + Tile.Width - 1) >> 5;
            Tiles.Tiles.xo = ox;
            Tiles.Tiles.yo = oy;
            var xo = ox >> 5;
            var yo = oy >> 5;

            foreach (var layer in _layers) {
                for (int y = yo; y <= h + yo; ++y) {
                    for (int x = xo; x <= w + xo; ++x) {
                        var tile = GetTileLayer(layer, x, y);
                        if (tile is null) continue;
                        tile.Draw(batch, this, x, y);
                    }
                }
            }

            for (int y = yo; y <= h + yo; y++) {
                for (int x = xo; x <= w + xo; x++) {
                    if (x < 0 || y < 0 || x >= this.width || y >= this.height) continue;
                    _rows.AddRange(InTiles[x + y * this.width]);
                }

                if (_rows.Count > 0) {
                    _rows.Sort(this.Sorter);
                    foreach (var e in _rows)
                        e.Draw(batch);
                }

                _rows.Clear();
            }

            Tiles.Tiles.xo = Tiles.Tiles.yo = 0;
        }

        public void Update(GameTime dt) {
            for (int i = 0; i < All.Count; ++i) {
                var e = All[i];
                int xto = e.x >> 5;
                int yto = e.y >> 5;
                e.Update(dt);

                if (e.Removed) {
                    All.RemoveAt(i--);
                    this.RemoveAt(xto, yto, e);
                } else {
                    int xt = e.x >> 5;
                    int yt = e.y >> 5;

                    if (xto != xt || yto != yt) {
                        this.RemoveAt(xto, yto, e);
                        this.AddAt(xt, yt, e);
                    }
                }
            }

            foreach (var tile in animated) {
                Tiles.Tiles.AnimateWater(tile, dt);
            }
        }

        public List<Entity> GetEntities(int x0, int y0, int x1, int y1) {
            List<Entity> result = new List<Entity>();
            int xt0 = (x0 >> 5) - 1;
            int yt0 = (y0 >> 5) - 1;
            int xt1 = (x1 >> 5) + 1;
            int yt1 = (y1 >> 5) + 1;
            for (int y = yt0; y <= yt1; ++y) {
                for (int x = xt0; x <= xt1; ++x) {
                    if (x < 0 || y < 0 || x >= width || y >= height) continue;
                    List<Entity> entities = InTiles[x + y * this.width];
                    for (int i = 0; i < entities.Count; i++) {
                        var e = entities[i];
                        if (e.Intersects(x0, y0, x1, y1)) result.Add(e);
                    }
                }
            }

            return result;
        }
    };
}
