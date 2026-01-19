using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChaosTerraria.Content.UserInterface
{
    public class CrazyUIState : UIState
    {
        public Crazy crazy;
        public override void OnInitialize()
        {
            Width.Set(0, 100f);
            Height.Set(0, 100f);

            crazy = new Crazy();
            Append(crazy);

            base.OnInitialize();
        }
    }
}
