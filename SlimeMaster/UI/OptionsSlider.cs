using System;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Managers;
using Microsoft.Xna.Framework;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;
namespace SlimeMaster.UI;

public class OptionsSlider : Slider
{
    // Reference to slider caption
    private TextRuntime _textInstance;

    private ColoredRectangleRuntime _fillRectangle;

    public string Text
    {
        get => _textInstance.Text;
        set => _textInstance.Text = value;
    }

    public OptionsSlider(TextureAtlas atlas)
    {
        ContainerRuntime topLevelContainer = new ContainerRuntime();

        topLevelContainer.Height = 55f;
        topLevelContainer.Width = 264f;

        TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

        NineSliceRuntime background = new NineSliceRuntime();
        background.Texture = atlas.Texture;
        background.TextureAddress = TextureAddress.Custom;
    }
}
