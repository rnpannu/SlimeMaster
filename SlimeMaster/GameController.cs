using MonoGameLibrary;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace SlimeMaster;

public static class GameController
{
    private static KeyboardInfo s_keyboard => Core.Input.Keyboard;
    public static bool MoveUp()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Up) ||
            s_keyboard.WasKeyJustPressed(Keys.W);
    }

    public static bool MoveDown()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Down) ||
            s_keyboard.WasKeyJustPressed(Keys.Up);
    }

    public static bool MoveLeft()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Left) ||
               s_keyboard.WasKeyJustPressed(Keys.A);
    }

    public static bool MoveRight()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Right) ||
               s_keyboard.WasKeyJustPressed(Keys.D);
    }

    public static bool Pause()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Escape);
    }

    public static bool Action()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Enter);
    }
}

