using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace MonoGameLibrary.Graphics;

public class Tilemap
{
    private readonly Tileset _tileset;
    private readonly int[] _tiles;

    public int Rows { get; }
    public int Columns { get; }

    public int Count {  get; }

    public Vector2 Scale { get; set; }

    public float TileWidth => _tileset.TileWidth * Scale.X;

    public float TileHeight => _tileset.TileHeight * Scale.Y;

    public Tilemap(Tileset tileset, int columns, int rows)
    {
        _tileset = tileset;
        Rows = rows;
        Columns = columns;
        Count = Columns * Rows;
        Scale = Vector2.One;
        _tiles = new int[Count];
    }

    public void SetTile (int index, int tileSetID)
    {
        _tiles[index] = tileSetID;
    }

    public void SetTile(int column, int row, int tilesetID)
    {
        int index = row * Columns + column;
        SetTile(index, tilesetID);
    }


    public TextureRegion GetTile(int index)
    {
        return _tileset.GetTile(_tiles[index]);
    }

    public TextureRegion GetTile(int column, int row)
    {
        int index = row * Columns + column;
        return GetTile(index);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for(int i= 0; i < Count; i++)
        {
            int tileSetIndex = _tiles[i];
            TextureRegion tile = _tileset.GetTile(tileSetIndex);

            int x = i % Columns;
            int y = i / Columns;

            Vector2 position = new Vector2(x * TileWidth, y * TileHeight);
            tile.Draw(spriteBatch, position, Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 1.0f);
        }
    }

    public static Tilemap FromFile(ContentManager content, string filename)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                /*
                 * <Tileset> contains tileset information used by the tilemap
                 * 
                 * 
                 * 
                 * <Tilemap>
                        <Tileset region="0 40 80 80" tileWidth="20" tileHeight="20">images/atlas</Tileset>
                        <Tiles>
                            00 01 02 01 02 01 02 01 02 01 02 01 02 01 02 03
                            04 05 05 06 05 05 06 05 05 06 05 05 06 05 05 07
                            08 09 09 09 09 09 09 09 09 09 09 09 09 09 09 11
                            04 09 09 09 09 09 09 09 10 09 09 09 09 10 09 07
                            08 09 10 09 09 09 09 09 09 09 09 09 09 09 09 11
                            04 09 09 09 09 09 09 09 09 09 09 09 09 09 09 07
                            08 10 09 09 09 09 09 09 09 09 10 09 09 09 09 11
                            04 09 09 09 09 09 10 09 09 09 09 09 09 09 09 07
                            12 13 14 13 14 13 14 13 14 13 14 13 14 13 14 15
                        </Tiles>
                    </Tilemap>
                 */

                XElement tilesetElement = root.Element("Tileset");
                // Parse info about the tileset, what region of the texture the tileset is in (x, y, width, height),
                // tile width and height of each tile in the region, and the content path to the texture that contains the tileset
                string regionAttribute = tilesetElement.Attribute("region").Value;
                string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                int x = int.Parse(split[0]);
                int y = int.Parse(split[1]);
                int width = int.Parse(split[2]);
                int height = int.Parse(split[3]);

                int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                string contentPath = tilesetElement.Value;


                Texture2D texture = content.Load<Texture2D>(contentPath);

                TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);


                Tileset tileset = new Tileset(textureRegion, tileWidth, tileHeight);

                // Begin parsing tiles
                XElement tilesElement = root.Element("Tiles");

                // Split at newline to get a list of rows
                string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

                Tilemap tilemap = new Tilemap(tileset, columnCount, rows.Length);

                for(int row = 0; row < rows.Length; row++)
                {
                    // Split at space to get a list of entries at each column
                    string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    // Put each column entry into tilemap object
                    for(int column = 0; column < columnCount; column++)
                    {
                        int tilesetIndex = int.Parse(columns[column]);
                        TextureRegion region = tileset.GetTile(tilesetIndex);

                        tilemap.SetTile(column, row, tilesetIndex);
                        Console.Out.WriteLine("Column: " + column.ToString() + "Row: " +
                            row.ToString() + "Tileset index: " + tilesetIndex.ToString());
                    }

                }

                return tilemap;

            }
        }
    }
}
