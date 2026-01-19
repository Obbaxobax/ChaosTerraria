using ChaosTerraria.Content.Items;
using ChaosTerraria.Content.Projectiles;
using ChaosTerraria.Content.UserInterface;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static ChaosTerraria.ChaosTerrariaConfig;
using static Mono.Cecil.Cil.OpCodes;

namespace ChaosTerraria
{

    public class ChaosTerrariaSystem : ModSystem
    {
        // ------------------------------------------------- Set up various UI stuff ----------------------------------------
        private static JesusUIState jesusUIState;
        private UserInterface _jesusUIState;

        private static CrazyUIState crazyUIState;
        private UserInterface _crazyUIState;
        private int crazyCountdown = 0;

        public override void Load()
        {
            IL_Player.GetItemExpectedPrice += HookExpectedPrice;
        }

        private static void HookExpectedPrice(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);

                c.GotoNext(i => i.MatchCallvirt(typeof(Terraria.Item).GetMethod(nameof(Item.GetStoreValue))));

                //var label = il.DefineLabel();
                c.Index++;

                c.Emit(Conv_R4);

                c.Emit(Ldsfld, typeof(ChaosTerrariaSystem).GetField(nameof(ChaosTerrariaSystem.priceModifier)));
                c.Emit(Conv_R4);
                c.Emit(Div);
                c.Emit(Conv_I8);
            }
            catch (Exception e)
            {
                MonoModHooks.DumpIL(ModContent.GetInstance<ChaosTerraria>(), il);

                throw new ILPatchFailureException(ModContent.GetInstance<ChaosTerraria>(), il, e);
            }
        }

        public override void OnModLoad()
        {
            jesusUIState = new JesusUIState();
            jesusUIState.Activate();

            _jesusUIState = new UserInterface();
            _jesusUIState.SetState(jesusUIState);

            crazyUIState = new CrazyUIState();
            crazyUIState.Activate();

            _crazyUIState = new UserInterface();
            _crazyUIState.SetState(null);
        }
        public override void UpdateUI(GameTime gameTime)
        {
            _jesusUIState?.Update(gameTime);
            _crazyUIState?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            //Add the UI's to their own layers
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Cursor"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex + 1, new LegacyGameInterfaceLayer(
                    "ChaosTerraria: The Son",
                    delegate
                    {
                        _jesusUIState.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );

                layers.Insert(mouseTextIndex + 1, new LegacyGameInterfaceLayer(
                    "ChaosTerraria: Critikal",
                    delegate
                    {
                        _crazyUIState.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public void ShowJesus()
        {
            jesusUIState.jesus.FadeImageOut();
        }

        public void ShowCrazy()
        {
            _crazyUIState.SetState(crazyUIState);

            crazyCountdown = 240;
        }

        // ----------------------------------------------------- Set Up Chaos ----------------------------------------------
        private Dictionary<string, int> eventList = new Dictionary<string, int>()
        {
            { "Boulders", 150 },
            { "SpawnRandomMobs", 30 },
            { "BlowUp", 3 },
            { "RandomTP", 9 },
            { "GiveCoins", 80 },
            { "ZeroHP", 9 },
            { "ChangeMaxHP", 30 },
            { "ChangeWeaponStats", 20 },
            { "JesusAppears", 9 },
            { "ChangeSpeed", 60 },
            { "ExitWorld", 1 },
            { "LaunchPlayer", 20 },
            { "RandomStatusEffect", 80 },
            { "ImpendingDoom", 3 },
            { "FakeImpendingDoom", 15 },
            { "RemoveAllBuffs", 20 },
            { "FlipGrav", 10 },
            { "SwapWaterAndLava", 10 },
            { "RandomizeDyes", 80 },
            { "KillAllMobs", 6 },
            { "SpawnBunnies", 50 },
            { "SpawnDynamiteBunnies", 20 },
            { "SpawnPinkies", 10 },
            { "SpawnGuides", 35 },
            { "SpawnBombs", 3 },
            { "GiveDiscount", 25 },
            { "StartInvasion", 20 },
            { "SpawnMeteor", 40 },
            { "SpawnDungeonGuardian", 3 },
            { "FakeSpawnDungeonGuardian", 30 },
            { "TorchGod", 15 },
            { "TeleportAllNPCs", 10 },
            { "ReZero", 20 },
            { "GiveFakeZenith", 30 },
            { "TheTerrarianSituation", 30 },
            { "Testing", 10 },
            { "FakePlayer", 10 }
        };

        private Random ran = new Random(); //Get random number
        private int updateTime = 0;
        private int ticksPerUpdate = 200;
        private int totalEventWeight = 0;

        public float maxHealth = 1f;
        public float speed = 1f;
        public bool rezeroActive = false;

        public bool fakeImpendingDoomActive = false;
        private bool realImpendingDoomActive = false;
        public int MoonLordCountdown = 0;

        public static float priceModifier = 1.0f;


        public override void PostSetupContent()
        {
            ticksPerUpdate = ModContent.GetInstance<ChaosTerrariaConfig>().timeBetweenEvents * 60;
            totalEventWeight = ModContent.GetInstance<ChaosTerrariaConfig>().SkipEventWeight;

            if (ModLoader.HasMod("CalamityMod"))
            {
                CalamityModSetup();
            }

            foreach (var element in eventList)
            {
                var weight = (int)ModContent.GetInstance<WeightConfig>().GetType().GetField(element.Key).GetValue(ModContent.GetInstance<WeightConfig>());
                eventList[element.Key] = weight;
                totalEventWeight += weight;
            }
        }

        [JITWhenModsEnabled("CalamityMod")]
        private void CalamityModSetup()
        {
            var calList = new Dictionary<string, int>() 
            {
                { "SpawnSCal", 2 },
                { "SpawnEidolon", 2 },
                { "BossRush", 1 },
                { "SpawnTheLorde", 1 }
            };

            eventList = eventList.Concat(calList).ToDictionary<string, int>();
        }

        public override void OnWorldLoad()
        {
            MoonLordCountdown = 0;
            priceModifier = 0;
        }

        public override void PostUpdateTime()
        {
            if (crazyCountdown > 0)
            {
                crazyCountdown--;
                if (crazyCountdown == 0)
                {
                    _crazyUIState.SetState(null);
                }
            }

            if (fakeImpendingDoomActive)
            {
                if (!Main.dedServ)
                {
                    if (MoonLordCountdown > 0)
                    {
                        float num5 = MathHelper.Clamp((float)Math.Sin((float)MoonLordCountdown / 60f * 0.5f) * 2f, 0f, 1f);
                        num5 *= 0.75f - 0.5f * ((float)MoonLordCountdown / (float)3600);
                        if (!Filters.Scene["MoonLordShake"].IsActive())
                        {
                            Filters.Scene.Activate("MoonLordShake", Main.LocalPlayer.position, Array.Empty<object>());
                        }
                        Filters.Scene["MoonLordShake"].GetShader().UseIntensity(num5);
                    }
                    else if (Filters.Scene["MoonLordShake"].IsActive())
                    {
                        Filters.Scene.Deactivate("MoonLordShake", Array.Empty<object>());
                    }
                }
                if (MoonLordCountdown > 0)
                {
                    MoonLordCountdown--;
                    if (MoonLordCountdown <= 0)
                    {
                        fakeImpendingDoomActive = false;
                    }
                }
            }
            else if (realImpendingDoomActive)
            {
                if (MoonLordCountdown > 0)
                {
                    MoonLordCountdown--;
                    if (MoonLordCountdown <= 0)
                    {
                        realImpendingDoomActive = false;
                    }
                }
            }
        }

        public override void PreUpdateWorld()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            
            if (updateTime < ticksPerUpdate)
            {
                updateTime++; 
                return;
            }

            var num = ran.Next(0, totalEventWeight);
            var start = 20;

            for (int i = 0; i < eventList.Count; i++)
            {
                var element = eventList.ElementAt(i);
                var end = start + element.Value;

                if (num >= start && num < end)
                {
                    RunEventByName(eventList.Keys.ToList()[i]);
                    break;
                }

                start += element.Value;
            }

            updateTime = 0;
        }

        public void RunEventByName(string name)
        {
            var ran = new Random();
            Main.NewText(Main.netMode);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                if (ModContent.GetInstance<ChaosTerrariaConfig>().displayEventName)
                {
                    Main.NewText(name);
                }
                
                HandleEventClient(name, Main.LocalPlayer);
            }
            else
            {
                ModPacket packet = Mod.GetPacket();

                foreach (var player in Main.ActivePlayers)
                {
                    switch (name)
                    {
                        //Server only
                        case "SpawnRandomMobs": //check
                        case "RandomTP": //check
                        case "GiveCoins": //check
                        case "Boulders": //check
                        case "ImpendingDoom": //check
                        case "KillAllMobs": //check
                        case "SpawnBunnies": //check
                        case "SpawnDynamiteBunnies": //check
                        case "SpawnPinkies": //check
                        case "SpawnGuides": //check
                        case "SpawnBombs": //check
                        case "StartInvasion": //check
                        case "SpawnDungeonGuardian": //check
                        case "TeleportAllNPCs": //check
                        case "SpawnSCal": //check
                        case "SpawnEidolon": //check
                        case "SpawnTheLorde": //check
                        case "BossRush": //check
                        case "GiveFakeZenith": //check
                        case "SpawnMeteor": // check (only works if there is not already meteorite)
                            HandleEventClient(name, player);
                            packet.Write((byte)255);
                            packet.Write(name);
                            packet.Send();
                            break;
                        // Client Only
                        case "ChangeMaxHP": //check
                        case "ChangeWeaponStats": //check
                        case "ChangeSpeed": // check
                        case "LaunchPlayer": //check
                        case "RandomStatusEffect": //check
                        case "RemoveAllBuffs": //check
                        case "JesusAppears": //check
                        case "BlowUp": //check
                        case "ExitWorld": //check
                        case "FlipGrav": //check
                        case "SwapWaterAndLava": //check
                        case "RandomizeDyes": //check
                        case "FakeSpawnDungeonGuardian": //check
                        case "FakeImpendingDoom": //check
                        case "TheTerrarianSituation": // check
                        case "Testing": // check
                        case "FakePlayer": // check
                            packet.Write((byte)1);
                            packet.Write(name);
                            packet.Send();
                            break;
                        //Both Server and Client
                        case "ReZero": // check
                        case "ZeroHP": // check
                        case "TorchGod": // check
                        case "GiveDiscount": // check
                            HandleEventClient(name, player);
                            packet.Write((byte)1);
                            packet.Write(name);
                            packet.Send();
                            break;
                    }
                }
            }
        }

        public void HandleEventClient(string name, Player player)
        {
            if (ModContent.GetInstance<ChaosTerrariaConfig>().displayEventName)
            {
                Main.NewText(name);
            }

            switch (name)
            {
                case "Boulders":
                    for (int i = 0; i < ran.Next(5, 20); i++)
                    {
                        Projectile.NewProjectile(new EntitySource_WorldEvent(), player.position + new Vector2(ran.Next(-10, 10), -64), new Vector2(ran.Next(-10, 10), ran.Next(-10, 10)), ProjectileID.Boulder, 10, 0);
                    }

                    break;
                case "SpawnRandomMobs":
                    for (int i = 0; i < ran.Next(1, 10); i++)
                    {
                        NPC.NewNPC(new EntitySource_WorldEvent(), (int)(player.position.X + ran.Next(-20, 20)), (int)(player.position.Y + ran.Next(-20, 20)), ran.Next(NPCID.NegativeIDCount + 1, TextureAssets.Npc.Length + NPCID.NegativeIDCount));
                    }

                    break;
                case "BlowUp":
                    for (int i = 0; i < ran.Next(1, 10); i++)
                    {
                        Projectile.NewProjectile(new EntitySource_WorldEvent(), player.position, Vector2.Zero, ModContent.ProjectileType<Explosion>(), 10000000, 0);
                    }

                    break;
                case "RandomTP":
                    player.TeleportationPotion();
                    break;
                case "GiveCoins":
                    CommonCode.DropItem(player.position, new EntitySource_WorldEvent(), 71 + ran.Next(0, 3), ran.Next(0, 10));
                    break;
                case "ZeroHP":
                    player.statLife = 1;
                    break;
                case "ChangeMaxHP":
                    var baseHealth = 100 + player.ConsumedLifeCrystals * 20 + player.ConsumedLifeFruit * 5;
                    var singleVal = ran.NextSingle();
                    var healthAdjust = Math.Clamp((1f / singleVal - 1f) * 0.1f, 20f / baseHealth, 5f);
                    maxHealth = healthAdjust;
                    break;
                case "ChangeWeaponStats":
                    player.HeldItem.damage = ran.Next(0, 500);
                    player.HeldItem.crit = (int)Math.Clamp(1f / ran.NextDouble() - 1, 1, 100);
                    player.HeldItem.scale = ran.Next(1, 100) / 10f;

                    var useTime = ran.Next(0, 100);
                    player.HeldItem.useAnimation = useTime;
                    player.HeldItem.useTime = useTime;

                    player.HeldItem.knockBack = ran.Next(0, 20);
                    player.HeldItem.Prefix(ran.Next(0, PrefixID.Count - 1));
                    break;
                case "JesusAppears":
                    jesusUIState.jesus.FadeImageOut();
                    break;
                case "ChangeSpeed":
                    var num9 = ran.NextSingle();
                    num9 = Math.Clamp((2f / num9) - 2, 0.1f, 10f);
                    speed = num9;
                    break;
                case "ExitWorld":
                    WorldGen.SaveAndQuit();
                    break;
                case "LaunchPlayer":
                    player.velocity += new Vector2(ran.Next(-100, 100), ran.Next(-100, 100));
                    break;
                case "RandomStatusEffect":
                    var num = ran.Next(0, BuffID.Count - 1);
                    while (BuffID.Sets.BasicMountData[num] != null)
                    {
                        num = ran.Next(0, BuffID.Count - 1);
                    }
                    player.AddBuff(num, ran.Next(1, 300) * 60);
                    break;
                case "ImpendingDoom":
                    if (fakeImpendingDoomActive || realImpendingDoomActive) break;

                    realImpendingDoomActive = true;
                    MoonLordCountdown = 3600;
                    WorldGen.StartImpendingDoom(3600);
                    break;
                case "FakeImpendingDoom":
                    if (fakeImpendingDoomActive || realImpendingDoomActive) break;

                    fakeImpendingDoomActive = true;
                    MoonLordCountdown = 3600;

                    Main.NewText(NetworkText.FromKey(Lang.misc[52].Key).ToString(), 50, 255, 130);
                    break;
                case "RemoveAllBuffs":
                    for (int i = 0; i < player.buffType.Length; i++)
                    {
                        player.DelBuff(i);
                    }
                    break;
                case "FlipGrav":
                    player.forcedGravity = ran.Next(60, 1200);
                    break;
                case "SwapWaterAndLava":
                    _ = Task.Run(() => SwapLava(player));
                    break;
                case "RandomizeDyes":
                    for (int i = 0; i < player.dye.Length; i++)
                    {
                        player.dye[i].dye = GameShaders.Armor.GetShaderIdFromItemId(ItemLists.DyeList[ran.Next(0, ItemLists.DyeList.Length - 1)]);
                    }
                    break;
                case "KillAllMobs":
                    foreach(NPC npc in Main.ActiveNPCs)
                    {
                        npc.life = 0;
                    }
                    break;
                case "SpawnBunnies":
                    for (int i = 0; i < ran.Next(1, 100); i++)
                    {
                        NPC.NewNPC(new EntitySource_WorldEvent(), (int)(player.position.X + ran.Next(-40, 40)), (int)(player.position.Y + ran.Next(-40, 40)), NPCID.Bunny);
                    }
                    break;
                case "SpawnDynamiteBunnies":
                    for (int i = 0; i < ran.Next(1, 100); i++)
                    {
                        NPC.NewNPC(new EntitySource_WorldEvent(), (int)(player.position.X + ran.Next(-40, 40)), (int)(player.position.Y + ran.Next(-40, 40)), NPCID.ExplosiveBunny);
                    }
                    break;
                case "SpawnPinkies":
                    for (int i = 0; i < ran.Next(1, 100); i++)
                    {
                        NPC.NewNPC(new EntitySource_WorldEvent(), (int)(player.position.X + ran.Next(-40, 40)), (int)(player.position.Y + ran.Next(-40, 40)), NPCID.Pinky);
                    }
                    break;
                case "SpawnGuides":
                    for (int i = 0; i < ran.Next(1, 100); i++)
                    {
                        NPC.NewNPC(new EntitySource_WorldEvent(), (int)(player.position.X + ran.Next(-40, 40)), (int)(player.position.Y + ran.Next(-40, 40)), NPCID.Guide);
                    }
                    break;
                case "SpawnBombs":
                    for (int i = 0; i < ran.Next(1, 100); i++)
                    {
                        Projectile.NewProjectile(new EntitySource_WorldEvent(), player.position + new Vector2(ran.Next(-10, 10), -64), new Vector2(ran.Next(-10, 10), ran.Next(-10, 10)), ProjectileID.Bomb, 10000000, 0);
                    }
                    break;
                case "GiveDiscount":
                    priceModifier = ran.Next(7, 100) / 10f;
                    break;
                case "StartInvasion":
                    var roll = ran.Next(1, 6);
                    if (roll <= 4)
                    {
                        Main.StartInvasion(roll);
                    }
                    else if (roll == 5)
                    {
                        Main.startSnowMoon();
                    }
                    else
                    {
                        Main.startSnowMoon();
                    }
                    break;
                case "SpawnMeteor":
                    WorldGen.dropMeteor();
                    break;
                case "SpawnDungeonGuardian":
                    float fraction = ran.NextSingle();
                    float theta = float.Pi * fraction * 2;
                    Vector2 spawnPosition = new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                    spawnPosition = spawnPosition * 1600 + player.position;
                    //NPC.NewNPC(new EntitySource_WorldEvent(), (int)spawnPosition.X, (int)spawnPosition.Y, NPCID.DungeonGuardian);
                    NPC.SpawnBoss((int)spawnPosition.X, (int)spawnPosition.Y, NPCID.DungeonGuardian, 0);
                    break;
                case "FakeSpawnDungeonGuardian":
                    Main.NewText("Dungeon Guardian has awoken!", 175, 75);
                    SoundEngine.PlaySound(SoundID.Roar);
                    break;
                case "TorchGod":
                    player.happyFunTorchTime = true;
                    break;
                case "TeleportAllNPCs":
                    int activePlayerCount = 0;
                    foreach(Player play in Main.ActivePlayers)
                    {
                        activePlayerCount++;
                    }

                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if ((npc.whoAmI % activePlayerCount) != player.whoAmI) continue;
                        npc.position = player.position;
                    }
                    break;
                case "ReZero":
                    rezeroActive = true;
                    break;
                case "GiveFakeZenith":
                    CommonCode.DropItem(player.position, new EntitySource_WorldEvent(), ModContent.ItemType<Zenith>(), 1);
                    break;
                case "TheTerrarianSituation":
                    ShowCrazy();
                    break;
                case "Testing":
                    Main.NewText("A Fatal Error has occuered", Color.Red);
                    Mod.Logger.Error("\r\n⠀⠀⠀⠀⢀⣴⣶⠿⠟⠻⠿⢷⣦⣄⠀⠀⠀\r\n⠀⠀⠀⠀⣾⠏⠀⠀⣠⣤⣤⣤⣬⣿⣷⣄⡀\r\n⠀⢀⣀⣸⡿⠀⠀⣼⡟⠁⠀⠀⠀⠀⠀⠙⣷\r\n⢸⡟⠉⣽⡇⠀⠀⣿⡇⠀⠀⠀⠀⠀⠀⢀⣿\r\n⣾⠇⠀⣿⡇⠀⠀⠘⠿⢶⣶⣤⣤⣶⡶⣿⠋\r\n⣿⠂⠀⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠃\r\n⣿⡆⠀⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀\r\n⢿⡇⠀⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⠀\r\n⠘⠻⠷⢿⡇⠀⠀⠀⣴⣶⣶⠶⠖⠀⢸⡟⠀\r\n⠀⠀⠀⢸⣇⠀⠀⠀⣿⡇⣿⡄⠀⢀⣿⠇⠀\r\n⠀⠀⠀⠘⣿⣤⣤⣴⡿⠃⠙⠛⠛⠛⠋⠀⠀");
                    break;
                case "FakePlayer":
                    Thread t = new Thread(() => FakeMessage(ran.Next(5)));
                    t.Start();
                    break;
                // ---- Calamity Stuff ---- //
                case "SpawnSCal":
                case "SpawnEidolon":
                case "SpawnTheLorde":
                case "BossRush":
                    CalamityStuff(name, player);
                    break;
            }
        }

        private void FakeMessage(int num)
        {
            switch (num) {
                case 0:
                    Main.NewText("Herobrine has joined!", Color.Yellow);
                    Thread.Sleep(2000);
                    Main.NewText("<Herobrine> oops wrong game mb");
                    Thread.Sleep(1000);
                    Main.NewText("Herobrine has left.", Color.Yellow);
                    break;
                case 1:
                    Main.NewText("Abba has joined!", Color.Yellow);
                    Thread.Sleep(2000);
                    Main.NewText("<Abba> hey");
                    Thread.Sleep(1000);
                    Main.NewText("<Abba> how are you");
                    Thread.Sleep(10000);
                    Main.NewText("<Abba> im gonna build my house in the snow bime if you dont mind");
                    break;
                case 2:
                    Main.NewText("God has joined!", Color.Yellow);
                    Thread.Sleep(2000);
                    Main.NewText("<God> yo whats up its god");
                    Thread.Sleep(2000);
                    Main.NewText("<God> just remember i am always watching over you");
                    Thread.Sleep(2000);
                    Main.NewText("<God> anyways im a busy man so i gtg");
                    Thread.Sleep(500);
                    Main.NewText("God has left.", Color.Yellow);
                    break;
                case 3:
                    Main.NewText("TotallyRealPlayer has joined!", Color.Yellow);
                    Thread.Sleep(2000);
                    Main.NewText("<TotallyRealPlayer> Your build suck bro quit the game.");
                    Thread.Sleep(1000);
                    Main.NewText("TotallyRealPlayer has left.", Color.Yellow);
                    break;
                case 4:
                    Main.NewText("Obbax has joined!", Color.Yellow);
                    Thread.Sleep(2000);
                    Main.NewText("<Obbax> Yo thank you for installing my virus");
                    Thread.Sleep(2000);
                    Main.NewText("<Obbax> Finally finished transferring all the files and just wnated to let you know");
                    Thread.Sleep(1000);
                    Main.NewText("Obbax has left.", Color.Yellow);
                    break;
            }
        }

        [JITWhenModsEnabled("CalamityMod")]
        public void CalamityStuff(string name, Player player)
        {
            var id = 0;

            float fraction = ran.NextSingle();
            float theta = float.Pi * fraction * 2;
            Vector2 spawnPosition = new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
            spawnPosition = spawnPosition * 800 + player.position;

            switch (name)
            {
                case "SpawnSCal":
                    id = ModContent.NPCType<CalamityMod.NPCs.SupremeCalamitas.SupremeCalamitas>();
                    NPC.SpawnBoss((int)player.position.X, (int)player.position.Y, id, 0);
                    break;
                case "SpawnEidolon":
                    id = ModContent.NPCType<CalamityMod.NPCs.PrimordialWyrm.PrimordialWyrmHead>();
                    NPC.SpawnBoss((int)player.position.X, (int)player.position.Y, id, 0);
                    break;
                case "SpawnTheLorde":
                    id = ModContent.NPCType<CalamityMod.NPCs.Other.THELORDE>();
                    NPC.SpawnBoss((int)player.position.X, (int)player.position.Y, id, 0);
                    break;
                case "BossRush":
                    CalamityMod.Events.BossRushEvent.SyncStartTimer(120);
                    CalamityMod.Events.BossRushEvent.BossRushStage = 0;
                    CalamityMod.Events.BossRushEvent.BossRushActive = true;
                    break;
            }
        }

        private void SwapLava(Player player)
        {
            Point playerTilePosition = player.position.ToTileCoordinates();
            for (int x = -50; x < 50; x++)
            {
                for (int y = -50; y < 50; y++)
                {
                    Point tilePosition = playerTilePosition + new Point(x, y);
                    Tile tile = Main.tile[tilePosition];

                    if (tile.LiquidAmount <= 0) continue;

                    tile.LiquidType = tile.LiquidType == 1 ? 0 : 1;
                }
            }
        }
    }
    class StartCommand : ModCommand
    {
        public override string Command => "test";
        public override CommandType Type => CommandType.Chat;
        public override bool IsCaseSensitive => true;
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            //Starts the server
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                ModContent.GetInstance<ChaosTerrariaSystem>().RunEventByName(args[0]);
            }
            else
            {
                ModPacket packet = Mod.GetPacket();

                packet.Write((byte)254);
                packet.Write(args[0]);
                packet.Send();
            }
        }
    }
}
