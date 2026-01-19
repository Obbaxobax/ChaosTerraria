using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChaosTerraria.Content.UserInterface
{
    public class Crazy : UIElement
    {
        public override void OnInitialize()
        {
            Width.Set(0, 100f);
            Height.Set(0, 100f);

            base.OnInitialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rect = GetDimensions().ToRectangle();

            spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("ChaosTerraria/Assets/crazy"), rect, Color.White);
            Utils.DrawBorderStringBig(spriteBatch, "The " + Main.LocalPlayer.name + " Situation is Crazy", new Vector2(0.12f * Main.screenWidth, 0.8f * Main.screenHeight), Color.Black);
            // base.Draw(spriteBatch);
        }
    }
}
