using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Gum.DataTypes;
using Gum.Wireframe;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using System;
using System.Reflection.Metadata;
namespace SlimeMaster.Scenes;

public class GameScene : Scene
{
    private AnimatedSprite _slime;

    private AnimatedSprite _bat;

    private Vector2 _slimePosition;

    private const float MOVEMENT_SPEED = 5.0f;

    private Vector2 _batPosition;

    private Vector2 _batVelocity;

    private Tilemap _tilemap;

    private Rectangle _roomBounds;

    private SoundEffect _bounceSoundEffect;

    private SoundEffect _collectSoundEffect;

    private SpriteFont _font;

    private int _score;

    private Vector2 _scoreTextPosition;

    private Vector2 _scoreTextOrigin;

    private KeyboardState _previousKeyboardState;

    private Panel _pausePanel;

    private Button _resumeButton;

    private SoundEffect _uiSoundEffect;

    private void PauseGame()
    {
        _pausePanel.IsVisible = true;
        _resumeButton.IsFocused = true;
    }

    private void InitializeUI()
    {
        GumService.Default.Root.Children.Clear();

        CreatePausePanel();
    }

    private void CreatePausePanel()
    {
        _pausePanel = new Panel();
        _pausePanel.Anchor(Anchor.Center);

        _pausePanel.Visual.WidthUnits = DimensionUnitType.Absolute;
        _pausePanel.Visual.HeightUnits = DimensionUnitType.Absolute;
        _pausePanel.Visual.Height = 70;
        _pausePanel.Visual.Width = 264;
        _pausePanel.IsVisible = false;
        _pausePanel.AddToRoot();

        var background = new ColoredRectangleRuntime();

        background.Dock(Dock.Fill);
        background.Color = Color.DarkBlue;
        _pausePanel.AddChild(background);

        var textInstance = new TextRuntime();
        textInstance.Text = "PAUSED";
        textInstance.X = 10f;
        textInstance.Y = 10f;
        _pausePanel.AddChild(textInstance);

        _resumeButton = new Button();
        _resumeButton.Text = "RESUME";
        _resumeButton.Anchor(Anchor.BottomLeft);
        _resumeButton.Visual.X = 9f;
        _resumeButton.Visual.Y = -9f;
        _resumeButton.Visual.Width = 90;
        _resumeButton.Click += HandleResumeButtonClicked;
        _pausePanel.AddChild(_resumeButton);

        var quitButton = new Button();
        quitButton.Text = "QUIT";
        quitButton.Anchor(Anchor.BottomRight);
        quitButton.Visual.X = -9f;
        quitButton.Visual.Y = -9f;
        quitButton.Width = 80;
        quitButton.Click += HandleQuitButtonClicked;
        _pausePanel.AddChild(quitButton);

    }

    private void HandleResumeButtonClicked(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        _pausePanel.IsVisible = false;
    }

    private void HandleQuitButtonClicked(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        Core.ChangeScene(new TitleScene());

    }
    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = false;


        Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;

        _roomBounds = new Rectangle(
            (int)_tilemap.TileWidth,
            (int)_tilemap.TileHeight,
            screenBounds.Width - (int)_tilemap.TileWidth * 2,
            screenBounds.Height - (int)_tilemap.TileHeight * 2);

        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;

        _slimePosition = new Vector2(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);

        _batPosition = new Vector2(_roomBounds.Left, _roomBounds.Top);

        _scoreTextPosition = new Vector2(_roomBounds.Left, _tilemap.TileHeight * 0.5f);

        float scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
        _scoreTextOrigin = new Vector2(0, scoreTextYOrigin);

        AssignRandomBatVelocity();

        InitializeUI();
        Console.Out.WriteLine("Done game screen initialization");
    }

    public override void LoadContent()
    {
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");


        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4.0f, 4.0f);

        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4.0f, 4.0f);

        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");

        _tilemap.Scale = new Vector2(4.0f, 4.0f);

        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");
        _uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");
    }

    public override void Update(GameTime gameTime)
    {

        GumService.Default.Update(gameTime);

        if (_pausePanel.IsVisible)
        {
            return;
        }
        _slime.Update(gameTime);
        _bat.Update(gameTime);

        CheckKeyboardInput();

        Rectangle screenBounds = new Rectangle(
            0,
            0,
            Core.GraphicsDevice.PresentationParameters.BackBufferWidth,
            Core.GraphicsDevice.PresentationParameters.BackBufferHeight
        );

        Circle slimeBounds = new Circle(
            (int)(_slimePosition.X + (_slime.Width * 0.5f)),
            (int)(_slimePosition.Y + (_slime.Height * 0.5f)),
            (int)(_slime.Width * 0.5f)
        );

        if (slimeBounds.Left < _roomBounds.Left)
        {
            _slimePosition.X = _roomBounds.Left;
        }
        else if (slimeBounds.Right > _roomBounds.Right)
        {
            _slimePosition.X = _roomBounds.Right - _slime.Width;
        }
        if (slimeBounds.Top < _roomBounds.Top)
        {
            _slimePosition.Y = _roomBounds.Top;
        }
        else if (slimeBounds.Bottom > _roomBounds.Bottom)
        {
            _slimePosition.Y = _roomBounds.Bottom - _slime.Height;
        }

        Vector2 newBatPosition = _batPosition + _batVelocity;

        Circle batBounds = new Circle(
            (int)(newBatPosition.X + (_bat.Width * 0.5f)),
            (int)(newBatPosition.Y + (_bat.Height * 0.5f)),
            (int)(_bat.Width * 0.5f)
        );

        Vector2 normal = Vector2.Zero;

        if (batBounds.Left < _roomBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            newBatPosition.X = _roomBounds.Left;
        }
        else if (batBounds.Right > _roomBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            newBatPosition.X = _roomBounds.Right - _bat.Width;
        }

        if (batBounds.Top < _roomBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            newBatPosition.Y = _roomBounds.Top;
        }
        else if (batBounds.Bottom > _roomBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            newBatPosition.Y = _roomBounds.Bottom - _bat.Height;
        }

        if (normal != Vector2.Zero)
        {
            _batVelocity = Vector2.Reflect(_batVelocity, normal);
            Core.Audio.PlaySoundEffect(_bounceSoundEffect);
        }

        _batPosition = newBatPosition;

        if (slimeBounds.Intersects(batBounds))
        {

            int column = Random.Shared.Next(0, _tilemap.Columns - 1);
            int row = Random.Shared.Next(0, _tilemap.Rows - 1);

            _batPosition = new Vector2(column * _bat.Width, row * _bat.Height);

            AssignRandomBatVelocity();
            Core.Audio.PlaySoundEffect(_collectSoundEffect);
            _score += 100;
        }
    }
    private void AssignRandomBatVelocity()
    {
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);
        _batVelocity = direction * MOVEMENT_SPEED;
    }

    private void CheckKeyboardInput()
    {

        KeyboardInfo kb = Core.Input.Keyboard;
        float speed = MOVEMENT_SPEED;

        if (kb.WasKeyJustPressed(Keys.Escape))
        {
            PauseGame();
        }

        if (kb.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up))
        {
            _slimePosition.Y -= speed;
        }
        if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down))
        {
            _slimePosition.Y += speed;
        }

        if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))
        {
            _slimePosition.X -= speed;
        }
        if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right))
        {
            _slimePosition.X += speed;
        }

        if (kb.WasKeyJustPressed(Keys.M))
        {
            Core.Audio.ToggleMute();
        }

        if (kb.WasKeyJustPressed(Keys.OemPlus))
        {
            Core.Audio.SongVolume += 0.1f;
            Core.Audio.SoundEffectVolume += 0.1f;
        }

        if (kb.WasKeyJustPressed(Keys.OemMinus))
        {
            Core.Audio.SongVolume -= 0.1f;
            Core.Audio.SoundEffectVolume -= 0.1f;
        }

    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);


        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _tilemap.Draw(Core.SpriteBatch);
        _slime.Draw(Core.SpriteBatch, _slimePosition);

        _bat.Draw(Core.SpriteBatch, _batPosition);

        Core.SpriteBatch.DrawString(
            _font, // spritefont
            $"Score: {_score}", //text
            _scoreTextPosition, //position
            Color.White, // color
            0.0f,   //rotation
            _scoreTextOrigin,   //origin
            1.0f,   //scale
            SpriteEffects.None, //effects
            0.0f); // layerdepth
        Core.SpriteBatch.End();

        GumService.Default.Draw();
    }
}
