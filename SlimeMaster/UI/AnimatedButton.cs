using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Graphics.Animation;
using Gum.Managers;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimeMaster.UI;

// Inherits from Gum button class
public class AnimatedButton : Button
{
    public AnimatedButton(TextureAtlas atlas)
    {
        // Initialize root visual/container. Relative to children with extra padding
        ContainerRuntime topLevelContainer = new ContainerRuntime();
        topLevelContainer.Height = 14f;
        topLevelContainer.HeightUnits = DimensionUnitType.Absolute;

        topLevelContainer.Width = 21f;
        topLevelContainer.WidthUnits = DimensionUnitType.RelativeToChildren;

        // Nineslice background
        NineSliceRuntime nineSliceInstance = new NineSliceRuntime();
        nineSliceInstance.Height = 0f;
        nineSliceInstance.Texture = atlas.Texture;
        nineSliceInstance.TextureAddress = TextureAddress.Custom; // Specify from atlas
        nineSliceInstance.Dock(Gum.Wireframe.Dock.Fill);
        topLevelContainer.Children.Add(nineSliceInstance); // Why is this different from the form AddChild()

        TextRuntime textInstance = new TextRuntime();
        textInstance.Name = "TextInstance";
        textInstance.Blue = 130;
        textInstance.Green = 80;
        textInstance.Red = 70;
        textInstance.UseCustomFont = true;
        textInstance.CustomFontFile = "fonts/04b_30.fnt";
        textInstance.FontScale = 0.25f;
        textInstance.Anchor(Gum.Wireframe.Anchor.Center);
        textInstance.Width = 0;
        textInstance.WidthUnits = DimensionUnitType.RelativeToChildren;
        topLevelContainer.Children.Add(textInstance);

        TextureRegion unfocusedTextureRegion = atlas.GetRegion("unfocused-button");

        AnimationChain unfocusedAnimation = new AnimationChain();
        unfocusedAnimation.Name = nameof(unfocusedAnimation); // stupid
        AnimationFrame unfocusedFrame = new AnimationFrame
        {
            TopCoordinate = unfocusedTextureRegion.TopTextureCoordinate,
            BottomCoordinate = unfocusedTextureRegion.BottomTextureCoordinate,
            LeftCoordinate = unfocusedTextureRegion.LeftTextureCoordinate,
            RightCoordinate = unfocusedTextureRegion.RightTextureCoordinate,
            FrameLength = 0.3f, // 300ms
            Texture = unfocusedTextureRegion.Texture
        };

        unfocusedAnimation.Add(unfocusedFrame);

        Animation focusedAtlasAnimation = atlas.GetAnimation("focused-button-animation");


        AnimationChain focusedAnimation = new AnimationChain();
        focusedAnimation.Name = nameof(focusedAnimation);
        foreach (TextureRegion region in focusedAtlasAnimation.Frames)
        {
            AnimationFrame frame = new AnimationFrame
            {
                TopCoordinate = region.TopTextureCoordinate,
                BottomCoordinate = region.BottomTextureCoordinate,
                LeftCoordinate = region.LeftTextureCoordinate,
                RightCoordinate = region.RightTextureCoordinate,
                FrameLength = (float)focusedAtlasAnimation.Delay.TotalSeconds,
                Texture = region.Texture
            };

            focusedAnimation.Add(frame);
        }

        nineSliceInstance.AnimationChains = new AnimationChainList
        {
            unfocusedAnimation,
            focusedAnimation
        };

        StateSaveCategory category = new StateSaveCategory();
        category.Name = Button.ButtonCategoryName;
        topLevelContainer.AddCategory(category);

        StateSave enabledState = new StateSave();
        enabledState.Name = FrameworkElement.EnabledStateName;

        enabledState.Apply = () => // syntax is weird
        {
            nineSliceInstance.CurrentChainName = unfocusedAnimation.Name;
        };

        category.States.Add(enabledState);

        StateSave focusedState = new StateSave();

        focusedState.Name = FrameworkElement.FocusedStateName;
        focusedState.Apply = () =>
        {
            nineSliceInstance.CurrentChainName = focusedAnimation.Name;
            nineSliceInstance.Animate = true;
        };

        category.States.Add(focusedState);

        StateSave highlightedFocused = focusedState.Clone();
        highlightedFocused.Name = FrameworkElement.HighlightedFocusedStateName;
        category.States.Add(highlightedFocused);

        StateSave highlighted = enabledState.Clone();
        highlighted.Name = FrameworkElement.HighlightedStateName;
        category.States.Add(highlighted);

        KeyDown += HandleKeyDown;

        topLevelContainer.RollOn += HandleRollOn;

        Visual = topLevelContainer;

    }

    private void HandleKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Keys.Left)
        {
            HandleTab(TabDirection.Up, loop: true);
        }
        if (e.Key == Keys.Right)
        {
            HandleTab(TabDirection.Down, loop: true);
        }
    }

    private void HandleRollOn(object sender, EventArgs e)
    {
        IsFocused = true;
    }
}
