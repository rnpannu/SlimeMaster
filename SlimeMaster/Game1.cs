using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using SlimeMaster.Scenes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonoGameGum;
using MonoGameGum.Forms.Controls;

namespace SlimeMaster;

public class Game1 : Core
{

    private Song _themeSong;

    public void InitializeGum()
    {
        GumService.Default.Initialize(this);

        GumService.Default.ContentLoader.XnaContentManager = Core.Content;

        FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);

        FrameworkElement.TabReverseKeyCombos.Add(
            new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Up });

        FrameworkElement.TabKeyCombos.Add(
            new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Down });

        GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
        GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;

        GumService.Default.Renderer.Camera.Zoom = 4.0f;

    }
    public Game1() : base("Dungeon Slime", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {

        base.Initialize();

        InitializeGum();
        Audio.PlaySong(_themeSong);


        ChangeScene(new TitleScene());
    }

    protected override void LoadContent()
    {


        //Song theme = Content.Load<Song>("audio/theme");

        //if (MediaPlayer.State == MediaState.Playing)
        //{
        //    MediaPlayer.Stop();
        //}

        //MediaPlayer.Play(theme);

        //MediaPlayer.IsRepeating = true;

        _themeSong = Content.Load<Song>("audio/theme");


    }

    //protected override void Update(GameTime gameTime)
    //{
    //    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
    //        Exit();


    //    base.Update(gameTime);
    //}
    


}
