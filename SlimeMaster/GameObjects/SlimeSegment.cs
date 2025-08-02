using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SlimeMaster.GameObjects;

public class SlimeSegment
{
    public Vector2 At;

    public Vector2 To;

    public Vector2 Direction;

    public Vector2 ReverseDirection => new Vector2(-Direction.X, -Direction.Y);
}
