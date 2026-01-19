using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ChaosTerraria
{
    public class ChaosTerraria : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte msg = reader.ReadByte();
            string eventName;
            switch (msg)
            {
                case 1:
                    eventName = reader.ReadString();
                    ModContent.GetInstance<ChaosTerrariaSystem>().HandleEventClient(eventName, Main.LocalPlayer);
                    break;
                case 254:
                    eventName = reader.ReadString();
                    ModContent.GetInstance<ChaosTerrariaSystem>().RunEventByName(eventName);
                    break;
                case 255:
                    eventName = reader.ReadString();
                    if (ModContent.GetInstance<ChaosTerrariaConfig>().displayEventName)
                    {
                        Main.NewText(eventName);
                    }
                    break;
            }
        }
    }
}
