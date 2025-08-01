using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MonoGameLibrary.Graphics;

public class TextureAtlas
{
    private Dictionary<string, TextureRegion> _regions;

    public Texture2D Texture { get; set; }

    public Dictionary<string, Animation> _animations;

    public TextureAtlas()
    {
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    public TextureAtlas(Texture2D texture)
    {
        Texture = texture; 
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    public void AddRegion(string name, int x, int y, int width, int height)
    {
        TextureRegion region = new TextureRegion(Texture, x, y, width, height);
        _regions.Add(name, region);
    }

    public TextureRegion GetRegion(string name)
    {
        //Console.Out.WriteLine(_regions.ContainsKey(name));
        return _regions[name];
    }

    public bool RemoveRegion(string name)
    {
        return _regions.Remove(name);
    }

    public void Clear()
    {
        _regions.Clear();
    }

    public Sprite CreateSprite(string regionName)
    {
        TextureRegion region = GetRegion(regionName);
        return new Sprite(region);
    }

    public void AddAnimation(string animationName, Animation animation)
    {
        _animations.Add(animationName, animation);
    }

    public Animation GetAnimation(string animationName)
    {
        return _animations[animationName];
    }

    public bool RemoveAnimation(string animationName)
    {
        return _animations.Remove(animationName);
    }

    public AnimatedSprite CreateAnimatedSprite(string animationName)
    {
        Animation animation = GetAnimation(animationName);
        return new AnimatedSprite(animation);

    }
    public static TextureAtlas FromFile(ContentManager content, string filename)
    {
        TextureAtlas atlas = new TextureAtlas();

        string filepath = Path.Combine(content.RootDirectory, filename);

        using (Stream stream = TitleContainer.OpenStream(filepath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;


                // The <Texture> element contains the content path for the Texture2D to load.
                string texturePath = root.Element("Texture").Value;
                atlas.Texture = content.Load<Texture2D>(texturePath);

                var regions = root.Element("Regions")?.Elements("Region");

                List<string> tempNames1 = [];
                if (regions != null)
                {
                    foreach (var region in regions)
                    {
                        string name = region.Attribute("name")?.Value;
                        int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                        int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                        int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                        int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                        if (!string.IsNullOrEmpty(name))
                        {
                            //Console.Out.WriteLine("Add region: " + name);
                            atlas.AddRegion(name, x, y, width, height);
                            tempNames1.Add(name);
                        }
                    }
                }

                var animations = root.Element("Animations")?.Elements("Animation");

                if (animations != null)
                {
                    foreach (var animationElement in animations)
                    {
                        string name = animationElement.Attribute("name")?.Value;
                        float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                        TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                        List<TextureRegion> frames = new List<TextureRegion>();

                        var frameElements = animationElement.Elements("Frame");

                        if (frameElements != null)
                        {
                            foreach (var frameElement in frameElements)
                            {
                                string regionName = frameElement.Attribute("region").Value;
                                //Console.Out.WriteLine("Frame name: " + frameElement.Attribute("region").Value);
                                
                                //Console.Out.WriteLine(tempNames1.Contains(regionName));
                                TextureRegion region = atlas.GetRegion(regionName);
                                //Console.Out.WriteLine(region.Texture);
                                //Console.Out.WriteLine(region.SourceRectangle);
                                frames.Add(region);
                                
                            }
                        }

                        Animation animation = new Animation(frames, delay);
                        atlas.AddAnimation(name, animation);

                    }
                }
                return atlas;
            }

        }
    }
}
