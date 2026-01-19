using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ChaosTerraria
{
    public class ChaosTerrariaPlayer : ModPlayer
    {
        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            base.ModifyMaxStats(out health, out mana);

            health = new StatModifier(1f, ModContent.GetInstance<ChaosTerrariaSystem>().maxHealth);
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();

            Player.moveSpeed = ModContent.GetInstance<ChaosTerrariaSystem>().speed;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (ModContent.GetInstance<ChaosTerrariaSystem>().rezeroActive)
            {
                ModContent.GetInstance<ChaosTerrariaSystem>().rezeroActive = false;
                SoundEngine.PlaySound(new SoundStyle("ChaosTerraria/Assets/rezero"));
                Player.Spawn(PlayerSpawnContext.ReviveFromDeath);
            }

            ModContent.GetInstance<ChaosTerrariaSystem>().speed = 1f;
            ModContent.GetInstance<ChaosTerrariaSystem>().maxHealth = 1f;
            base.Kill(damage, hitDirection, pvp, damageSource);
        }
    }
}
