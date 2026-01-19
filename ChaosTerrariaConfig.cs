using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace ChaosTerraria
{
    class ChaosTerrariaConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [SeparatePage]
        [DefaultValue(10)]
        [Range(0, 200)]
        [ReloadRequired]
        public int timeBetweenEvents = 10;

        [DefaultValue(false)]
        public bool displayEventName = false;

        [DefaultValue(150)]
        [Range(0, 1000)]
        [ReloadRequired]
        public int SkipEventWeight;

        public class WeightConfig : ModConfig
        {
            public override ConfigScope Mode => ConfigScope.ServerSide;


            [DefaultValue(150)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int Boulders;
            [DefaultValue(30)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SpawnRandomMobs;
            [DefaultValue(3)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int BlowUp;
            [DefaultValue(9)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int RandomTP;
            [DefaultValue(80)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int GiveCoins;
            [DefaultValue(9)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int ZeroHP;
            [DefaultValue(30)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int ChangeMaxHP;
            [DefaultValue(20)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int ChangeWeaponStats;
            [DefaultValue(9)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int JesusAppears;
            [DefaultValue(60)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int ChangeSpeed;
            [DefaultValue(1)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int ExitWorld;
            [DefaultValue(20)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int LaunchPlayer;
            [DefaultValue(80)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int RandomStatusEffect;
            [DefaultValue(3)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int ImpendingDoom;
            [DefaultValue(15)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int FakeImpendingDoom;
            [DefaultValue(20)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int RemoveAllBuffs;
            [DefaultValue(10)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int FlipGrav;
            [DefaultValue(10)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SwapWaterAndLava;
            [DefaultValue(80)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int RandomizeDyes;
            [DefaultValue(6)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int KillAllMobs;
            [DefaultValue(50)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SpawnBunnies;
            [DefaultValue(20)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SpawnDynamiteBunnies;
            [DefaultValue(10)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SpawnPinkies;
            [DefaultValue(35)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SpawnGuides;
            [DefaultValue(3)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SpawnBombs;
            [DefaultValue(25)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int GiveDiscount;
            [DefaultValue(20)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int StartInvasion;
            [DefaultValue(40)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SpawnMeteor;
            [DefaultValue(3)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SpawnDungeonGuardian;
            [DefaultValue(30)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int FakeSpawnDungeonGuardian;
            [DefaultValue(15)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int TorchGod;
            [DefaultValue(10)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int TeleportAllNPCs;
            [DefaultValue(20)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int ReZero;
            [DefaultValue(30)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int GiveFakeZenith;
            [DefaultValue(30)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int TheTerrarianSituation;
            [DefaultValue(10)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int Testing;
            [DefaultValue(10)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int FakePlayer;

            [Header("Calamity")]
            [DefaultValue(2)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SpawnSCal;
            [DefaultValue(2)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SpawnEidolon;
            [DefaultValue(1)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int BossRush;
            [DefaultValue(1)]
            [Range(0, 1000)]
            [ReloadRequired]
            public int SpawnTheLorde;
        }
    }


}
