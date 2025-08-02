using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SlimeMaster.GameObjects;

public class Slime
{
    // Movement threshold
    private static readonly TimeSpan s_movementTime = TimeSpan.FromMilliseconds(200);

    // Accumulates time until movment is forced.
    private TimeSpan _movementTimer;

    
    private float _movementProgress;

    private Vector2 _nextDirection;

    // # of pixels to move head
    private float _stride;

    private List<SlimeSegment> _segments;

    private AnimatedSprite _sprite;

    public event EventHandler BodyCollision;

    public Slime(AnimatedSprite sprite)
    {
        _sprite = sprite;
    }

    public void Initialize(Vector2 startingPosition, float stride)
    {
        _segments = new List<SlimeSegment>();

        _stride = stride;

        SlimeSegment head = new SlimeSegment();
        head.At = startingPosition;
        head.To = startingPosition + new Vector2(_stride, 0);
        head.Direction = Vector2.UnitX;




    }

}

