using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLibrary.Graphics;

public class Animation
{
    public List<TextureRegion> Frames { get; set; }

    public TimeSpan Delay { get; set; }

    public Animation()
    {
        Frames = new List<TextureRegion>();
        Delay = TimeSpan.FromMilliseconds(100);
    }

    public Animation(List<TextureRegion> frames, TimeSpan delay)
    {
        Frames = frames;
        //foreach (var frame in frames)
        //{
        //    Console.Out.WriteLine(frame.Texture);
        //    Console.Out.WriteLine(frame.SourceRectangle);
        //}


        Delay = delay;
    }


}
