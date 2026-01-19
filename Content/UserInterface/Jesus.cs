using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;

namespace ChaosTerraria.Content.UserInterface
{
    public class Jesus : UIElement
    {
        private Color currentOpacity = Color.Transparent;
        private bool fading = false;

        public override void OnInitialize()
        {
            Width.Set(0, 100f);
            Height.Set(0, 100f);

            base.OnInitialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rect = GetDimensions().ToRectangle();

            spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("ChaosTerraria/Assets/theson"), rect, currentOpacity);
            // base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (fading == true && currentOpacity.A != 0)
            {
                currentOpacity = new Color(currentOpacity.ToVector4() - Vector4.One * 0.03f);
            }
            else if (fading == true)
            {
                currentOpacity = Color.Transparent;
                fading = false;
            }

            base.Update(gameTime);
        }

        public void FadeImageOut()
        {
            SoundEngine.PlaySound(new SoundStyle("ChaosTerraria/Assets/bellsound"));
            fading = true;
            currentOpacity = new Color(1f, 1f, 1f, 1f);
        }
    }
}
