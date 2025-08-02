using System;
using SlimeMaster.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;

namespace SlimeMaster.Scenes;

public class TitleScene : Scene
{
    private const string DUNGEON_TEXT = "Slime";
    private const string SLIME_TEXT = "Master";

    private SpriteFont _font;

    private SpriteFont _font5x;

    private Vector2 _dungeonTextPos;

    private Vector2 _dungeonTextOrigin;

    private Vector2 _slimeTextPos;

    private Vector2 _slimeTextOrigin;

    private Texture2D _backgroundPattern;

    private Rectangle _backgroundDestination;

    private Vector2 _backgroundOffset; // Scrolling effect

    private SoundEffect _uiSoundEffect;

    private Panel _titleScreenButtonsPanel;

    private Panel _optionsPanel;

    private float _scrollSpeed = 50.0f;

    private AnimatedButton _optionsButton;

    private AnimatedButton _optionsBackButton;

    private TextureAtlas _atlas;


    private void CreateTitlePanel()
    {
        _titleScreenButtonsPanel = new Panel();
        
        // Fill title panel to whole screen
        _titleScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);

        _titleScreenButtonsPanel.AddToRoot();

        AnimatedButton startButton = new AnimatedButton(_atlas);

        startButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        startButton.Visual.X = 50; // Margin from the left
        startButton.Visual.Y = -12; // Margin from the bottom
        startButton.Text = "Start";
        startButton.Click += HandleStartClicked;
        _titleScreenButtonsPanel.AddChild(startButton);
        _titleScreenButtonsPanel.AddChild(startButton);

        _optionsButton = new AnimatedButton(_atlas);
        _optionsButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsButton.Visual.X = -50;
        _optionsButton.Visual.Y = -12;
        _optionsButton.Text = "Options";
        _optionsButton.Click += HandleOptionsClicked;
        _titleScreenButtonsPanel.AddChild(_optionsButton);

        startButton.IsFocused = true;
    }
    
    private void CreateOptionsPanel()
    {
        _optionsPanel = new Panel();
        _optionsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _optionsPanel.IsVisible = false;
        _optionsPanel.AddToRoot();

        TextRuntime optionsText = new TextRuntime();
        optionsText.Y = 10;
        optionsText.X = 10;
        optionsText.Text = "OPTIONS";
        optionsText.UseCustomFont = true;
        optionsText.FontScale = 0.5f;
        optionsText.CustomFontFile = @"fonts/04B_30.fnt";
        _optionsPanel.AddChild(optionsText);

        OptionsSlider musicSlider = new OptionsSlider(_atlas);
        musicSlider.Name = "MusicSlider";
        musicSlider.Text = "MUSIC";
        musicSlider.Anchor(Gum.Wireframe.Anchor.TopRight);
        musicSlider.Visual.Y = 30f;
        musicSlider.Minimum = 0;
        musicSlider.Maximum = 1;
        musicSlider.SmallChange = .1;
        musicSlider.LargeChange = .2;
        musicSlider.ValueChanged += HandleMusicSliderValueChanged;
        _optionsPanel.AddChild(musicSlider);

        OptionsSlider sfxSlider = new OptionsSlider(_atlas);
        sfxSlider.Name = "SfxSlider";
        sfxSlider.Text = "SFX";
        sfxSlider.Anchor(Gum.Wireframe.Anchor.Top);
        sfxSlider.Visual.Y = 93;
        sfxSlider.Minimum = 0;
        sfxSlider.Maximum = 1;
        sfxSlider.Value = Core.Audio.SoundEffectVolume;
        sfxSlider.SmallChange = .1;
        sfxSlider.LargeChange = .2;
        sfxSlider.ValueChanged += HandleSfxSliderValueChanged;
        _optionsPanel.AddChild(sfxSlider);

        _optionsBackButton = new AnimatedButton(_atlas);
        _optionsBackButton.Text = "BACK";
        _optionsBackButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsBackButton.X = -28f;
        _optionsBackButton.Y = -10f;
        _optionsBackButton.Click += HandleOptionsButtonBack;
        _optionsPanel.AddChild(_optionsBackButton);

    }
    private void HandleStartClicked(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        Core.ChangeScene(new GameScene());
    }

    private void HandleOptionsClicked(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        _titleScreenButtonsPanel.IsVisible = false;

        _optionsPanel.IsVisible = true;
        _optionsBackButton.IsFocused = true;
    }
    private void HandleMusicSliderValueChangeCompleted(object sender, EventArgs e)
    {

        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }
    private void HandleSfxSliderValueChanged(object sender, EventArgs e)
    {
        var slider = (Slider)sender;

        Core.Audio.SoundEffectVolume = (float)slider.Value;
    }

    private void HandleSfxSliderValueChangeCompleted(object sender, EventArgs e)
    {

        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }
    private void HandleMusicSliderValueChanged(object sender, EventArgs e)
    {
        var slider = (Slider) sender;

        Core.Audio.SongVolume = (float) slider.Value;
    }

    private void HandleOptionsButtonBack(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        _titleScreenButtonsPanel.IsVisible = true;

        _optionsPanel.IsVisible = false;

        _optionsButton.IsFocused = true;

    }

    private void InitializeUI()
    {
        // Clear UI tree
        GumService.Default.Root.Children.Clear();

        CreateTitlePanel();

        CreateOptionsPanel();
    }
    public override void Initialize()
    {
        
        base.Initialize();
        Core.ExitOnEscape = true;
        Vector2 size = _font5x.MeasureString(DUNGEON_TEXT);
        _dungeonTextPos = new Vector2(640, 100);
        _dungeonTextOrigin = size * 0.5f;

        size = _font5x.MeasureString(SLIME_TEXT);
        _slimeTextPos = new Vector2(757, 207);
        _slimeTextOrigin = size * 0.5f;

        //size = _font.MeasureString(PRESS_ENTER_TEXT);
        //_pressEnterPos = new Vector2(640, 620);
        //_pressEnterOrigin = size * 0.5f;

        Console.Out.WriteLine("Done title screen initialization");

        _backgroundOffset = Vector2.Zero;

        _backgroundDestination = Core.GraphicsDevice.PresentationParameters.Bounds;

        InitializeUI();
    }

    public override void LoadContent()
    {

        _atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");
        _font5x = Content.Load<SpriteFont>("fonts/04B_30_5x");

        _backgroundPattern = Content.Load<Texture2D>("images/background-pattern");
        _uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");
    }

    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            Core.ChangeScene(new GameScene());
        }

        float offset = _scrollSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds;
        _backgroundOffset.X -= offset;
        _backgroundOffset.Y -= offset;

        _backgroundOffset.X %= _backgroundPattern.Width;
        _backgroundOffset.Y %= _backgroundPattern.Height;

        GumService.Default.Update(gameTime);
    }


    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        Core.SpriteBatch.Draw(_backgroundPattern,
            _backgroundDestination,
            new Rectangle(_backgroundOffset.ToPoint(), _backgroundDestination.Size),
            Color.White * 0.5f);
        Core.SpriteBatch.End();

        if (_titleScreenButtonsPanel.IsVisible)
        {
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Color dropShadowColor = Color.Black * 0.5f;

            Core.SpriteBatch.DrawString(
                _font5x,
                DUNGEON_TEXT,
                _dungeonTextPos + new Vector2(10, 10), // slight offset
                dropShadowColor,
                0.0f,
                _dungeonTextOrigin,
                1.0f,
                SpriteEffects.None,
                1.0f);

            Core.SpriteBatch.DrawString(
                _font5x,
                DUNGEON_TEXT,
                _dungeonTextPos,
                Color.White,
                0.0f,
                _dungeonTextOrigin,
                1.0f,
                SpriteEffects.None,
                1.0f);

            Core.SpriteBatch.DrawString(
                _font5x,
                SLIME_TEXT,
                _slimeTextPos + new Vector2(10, 10),
                dropShadowColor,
                0.0f,
                _slimeTextOrigin,
                1.0f,
                SpriteEffects.None,
                1.0f);

            Core.SpriteBatch.DrawString(
                _font5x,
                SLIME_TEXT,
                _slimeTextPos,
                Color.White,
                0.0f,
                _slimeTextOrigin,
                1.0f,
                SpriteEffects.None,
                1.0f);

            //Core.SpriteBatch.DrawString(
            //    _font,
            //    PRESS_ENTER_TEXT,
            //    _pressEnterPos,
            //    Color.White,
            //    0.0f,
            //    _pressEnterOrigin,
            //    1.0f,
            //    SpriteEffects.None,
            //    1.0f);

            Core.SpriteBatch.End();
        }

        GumService.Default.Draw();

    }
}
