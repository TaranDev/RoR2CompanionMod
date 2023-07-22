using BepInEx;
using BepInEx.Configuration;
using IL.RoR2.Skills;
using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using Inventory = RoR2.Inventory;

namespace StreamCompanion
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class StreamCompanion : BaseUnityPlugin
    {
        // Mod Info
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "TaranDev";
        public const string PluginName = "RoR2StreamCompanion";
        public const string PluginVersion = "2.2.0";

        // Backend
        public const string url = "https://ror2-sc-api.herokuapp.com/";

        public static string inventoryItemsString = "";
        public static string equipmentString = "";
        public static string skillsString = "";
        public static string bodyColourString = "";

        // Config file entries
        public static ConfigEntry<string> TwitchNameConfigEntry { get; set; }
        public static ConfigEntry<string> SecretKeyConfigEntry { get; set; }
        public static ConfigEntry<double> DelayConfigEntry { get; set; }

        // Vanilla Item List
        public static List<string> VanillaItems = new List<string>() { "ITEM_BOSSDAMAGEBONUS_NAME", "ITEM_SECONDARYSKILLMAGAZINE_NAME", "ITEM_FLATHEALTH_NAME", "ITEM_FIREWORK_NAME", "ITEM_MUSHROOM_NAME", "ITEM_HEALWHILESAFE_NAME", "ITEM_CROWBAR_NAME", "ITEM_FRAGILEDAMAGEBONUS_NAME", "ITEM_SPRINTBONUS_NAME", "ITEM_NEARBYDAMAGEBONUS_NAME", "ITEM_IGNITEONKILL_NAME", "ITEM_SCRAPWHITE_NAME", "ITEM_CRITGLASSES_NAME", "ITEM_MEDKIT_NAME", "ITEM_ATTACKSPEEDANDMOVESPEED_NAME", "ITEM_TOOTH_NAME", "ITEM_OUTOFCOMBATARMOR_NAME", "ITEM_HOOF_NAME", "ITEM_PERSONALSHIELD_NAME", "ITEM_HEALINGPOTION_NAME", "ITEM_REPULSIONARMORPLATE_NAME", "ITEM_GOLDONHURT_NAME", "ITEM_TREASURECACHE_NAME", "ITEM_SYRINGE_NAME", "ITEM_STICKYBOMB_NAME", "ITEM_STUNCHANCEONHIT_NAME", "ITEM_BARRIERONKILL_NAME", "ITEM_BEAR_NAME", "ITEM_BLEEDONHIT_NAME", "ITEM_WARDONLEVEL_NAME", "ITEM_MISSILE_NAME", "ITEM_BANDOLIER_NAME", "ITEM_WARCRYONMULTIKILL_NAME", "ITEM_SLOWONHIT_NAME", "ITEM_DEATHMARK_NAME", "ITEM_EQUIPMENTMAGAZINE_NAME", "ITEM_BONUSGOLDPACKONKILL_NAME", "ITEM_HEALONCRIT_NAME", "ITEM_FEATHER_NAME", "ITEM_MOVESPEEDONKILL_NAME", "ITEM_STRENGTHENBURN_NAME", "ITEM_INFUSION_NAME", "ITEM_SCRAPGREEN_NAME", "ITEM_FIRERING_NAME", "ITEM_SEED_NAME", "ITEM_TPHEALINGNOVA_NAME", "ITEM_EXECUTELOWHEALTHELITE_NAME", "ITEM_PHASING_NAME", "ITEM_ATTACKSPEEDONCRIT_NAME", "ITEM_THORNS_NAME", "ITEM_SPRINTOUTOFCOMBAT_NAME", "ITEM_REGENERATINGSCRAP_NAME", "ITEM_SPRINTARMOR_NAME", "ITEM_ICERING_NAME", "ITEM_FREECHEST_NAME", "ITEM_PRIMARYSKILLSHURIKEN_NAME", "ITEM_SQUIDTURRET_NAME", "ITEM_CHAINLIGHTNING_NAME", "ITEM_ENERGIZEDONEQUIPMENTUSE_NAME", "ITEM_JUMPBOOST_NAME", "ITEM_EXPLODEONDEATH_NAME", "ITEM_CLOVER_NAME", "ITEM_BARRIERONOVERHEAL_NAME", "ITEM_ALIENHEAD_NAME", "ITEM_IMMUNETODEBUFF_NAME", "ITEM_RANDOMEQUIPMENTTRIGGER_NAME", "ITEM_KILLELITEFRENZY_NAME", "ITEM_BEHEMOTH_NAME", "ITEM_DAGGER_NAME", "ITEM_CAPTAINDEFENSEMATRIX_NAME", "ITEM_EXTRALIFE_NAME", "ITEM_ICICLE_NAME", "ITEM_FALLBOOTS_NAME", "ITEM_GHOSTONKILL_NAME", "ITEM_UTILITYSKILLMAGAZINE_NAME", "ITEM_INTERSTELLARDESKPLANT_NAME", "ITEM_SCRAPRED_NAME", "ITEM_CRITDAMAGE_NAME", "ITEM_NOVAONHEAL_NAME", "ITEM_MOREMISSILE_NAME", "ITEM_INCREASEHEALING_NAME", "ITEM_LASERTURBINE_NAME", "ITEM_BOUNCENEARBY_NAME", "ITEM_ARMORREDUCTIONONHIT_NAME", "ITEM_TALISMAN_NAME", "ITEM_DRONEWEAPONS_NAME", "ITEM_PERMANENTDEBUFFONHIT_NAME", "ITEM_SHOCKNEARBY_NAME", "ITEM_HEADHUNTER_NAME", "ITEM_ARTIFACTKEY_NAME", "ITEM_LIGHTNINGSTRIKEONHIT_NAME", "ITEM_MINORCONSTRUCTONKILL_NAME", "ITEM_ROBOBALLBUDDY_NAME", "ITEM_NOVAONLOWHEALTH_NAME", "ITEM_TITANGOLDDURINGTP_NAME", "ITEM_SHINYPEARL_NAME", "ITEM_SCRAPYELLOW_NAME", "ITEM_SPRINTWISP_NAME", "ITEM_SIPHONONLOWHEALTH_NAME", "ITEM_FIREBALLSONHIT_NAME", "ITEM_PEARL_NAME", "ITEM_PARENTEGG_NAME", "ITEM_BEETLEGLAND_NAME", "ITEM_BLEEDONHITANDEXPLODE_NAME", "ITEM_KNURL_NAME", "ITEM_LUNARTRINKET_NAME", "ITEM_GOLDONHIT_NAME", "ITEM_REPEATHEAL_NAME", "ITEM_MONSTERSONSHRINEUSE_NAME", "ITEM_LUNARSUN_NAME", "ITEM_LUNARSPECIALREPLACEMENT_NAME", "ITEM_RANDOMLYLUNAR_NAME", "ITEM_FOCUSEDCONVERGENCE_NAME", "ITEM_AUTOCASTEQUIPMENT_NAME", "ITEM_LUNARSECONDARYREPLACEMENT_NAME", "ITEM_HALFATTACKSPEEDHALFCOOLDOWNS_NAME", "ITEM_RANDOMDAMAGEZONE_NAME", "ITEM_LUNARBADLUCK_NAME", "ITEM_LUNARDAGGER_NAME", "ITEM_HALFSPEEDDOUBLEHEALTH_NAME", "ITEM_LUNARUTILITYREPLACEMENT_NAME", "ITEM_SHIELDONLY_NAME", "ITEM_LUNARPRIMARYREPLACEMENT_NAME", "ITEM_CLOVERVOID_NAME", "ITEM_TREASURECACHEVOID_NAME", "ITEM_CRITGLASSESVOID_NAME", "ITEM_EQUIPMENTMAGAZINEVOID_NAME", "ITEM_BLEEDONHITVOID_NAME", "ITEM_VOIDMEGACRABITEM_NAME", "ITEM_MISSILEVOID_NAME", "ITEM_EXTRALIFEVOID_NAME", "ITEM_CHAINLIGHTNINGVOID_NAME", "ITEM_BEARVOID_NAME", "ITEM_ELEMENTALRINGVOID_NAME", "ITEM_SLOWONHITVOID_NAME", "ITEM_EXPLODEONDEATHVOID_NAME", "ITEM_MUSHROOMVOID_NAME", "ITEM_FRAGILEDAMAGEBONUSCONSUMED_NAME", "ITEM_EXTRALIFECONSUMED_NAME", "ITEM_HEALINGPOTIONCONSUMED_NAME", "ITEM_EXTRALIFEVOIDCONSUMED_NAME", "ITEM_REGENERATINGSCRAPCONSUMED_NAME", "ITEM_TONICAFFLICTION_NAME" };

        // Captain Beacon Name List 
        public static List<string> CaptainBeacons = new List<string>() { "CAPTAIN_SUPPLY_HEAL_NAME", "CAPTAIN_SUPPLY_DEFENSE_NAME", "CAPTAIN_SUPPLY_HACKING_NAME", "CAPTAIN_SUPPLY_EQUIPMENT_RESTOCK_NAME", "CAPTAIN_SUPPLY_SHOCKING_NAME" };
        
        // MUL-T Primary Skill Name List 
        public static List<string> MULTPrimarySkills = new List<string>() { "TOOLBOT_PRIMARY_NAME", "TOOLBOT_PRIMARY_ALT1_NAME", "TOOLBOT_PRIMARY_ALT2_NAME", "TOOLBOT_PRIMARY_ALT3_NAME" };

        // Phase 4 item update status
        public static bool isWaitingForUpdate = false;

        public void Awake()
        {
            Log.Init(Logger);

            TwitchNameConfigEntry = Config.Bind<string>(
            "RoR2 Stream Companion",
            "Twitch Name",
            "exampleTwitchName",
            "The name of your channel on Twitch"
            );
            SecretKeyConfigEntry = Config.Bind<string>(
            "RoR2 Stream Companion",
            "Secret Key",
            "123MySecretKey",
            "The secret key you generated in the extension config page on Twitch"
            );
            DelayConfigEntry = Config.Bind<double>(
            "RoR2 Stream Companion",
            "Delay",
            2.0,
            "The delay of your twitch stream in seconds"
            );

            // Resetting to no items when the game is opened
            SendItemsUpdate(inventoryItemsString, equipmentString, skillsString, bodyColourString);
        }

        // Hooks
        private void OnEnable()
        {
            On.RoR2.Inventory.HandleInventoryChanged += OnInventoryChanged;
            On.RoR2.GameOverController.Awake += OnGameOver;
            On.RoR2.CharacterMaster.OnBodyStart += OnBodyLoaded;
            On.EntityStates.Captain.Weapon.CallSupplyDropBase.OnEnter += OnEnterSupplyDrop;
            On.EntityStates.Toolbot.ToolbotStanceSwap.OnEnter += OnEnterRetool;
            On.EntityStates.Toolbot.ToolbotDualWield.OnEnter += OnEnterPowerMode;
            On.EntityStates.Toolbot.ToolbotDualWield.OnExit += OnExitPowerMode;
        }
        private void OnDisable()
        {
            On.RoR2.Inventory.HandleInventoryChanged -= OnInventoryChanged;
            On.RoR2.GameOverController.Awake -= OnGameOver;
            On.RoR2.CharacterMaster.OnBodyStart -= OnBodyLoaded;
            On.EntityStates.Captain.Weapon.CallSupplyDropBase.OnEnter -= OnEnterSupplyDrop;
            On.EntityStates.Toolbot.ToolbotStanceSwap.OnEnter -= OnEnterRetool;
            On.EntityStates.Toolbot.ToolbotDualWield.OnEnter -= OnEnterPowerMode;
            On.EntityStates.Toolbot.ToolbotDualWield.OnExit -= OnExitPowerMode;
        }

        private static void OnEnterSupplyDrop(On.EntityStates.Captain.Weapon.CallSupplyDropBase.orig_OnEnter orig, EntityStates.Captain.Weapon.CallSupplyDropBase self)
        {
            orig(self);
            // Delay to give time for Captains special to switch over to used up version if there are no more beacons left
            Task.Delay((int)(500)).ContinueWith(t => {
                checkSkillString();
            });
        }

        private static void OnEnterRetool(On.EntityStates.Toolbot.ToolbotStanceSwap.orig_OnEnter orig, EntityStates.Toolbot.ToolbotStanceSwap self)
        {
            orig(self);
            checkSkillString();
        }

        private static void OnEnterPowerMode(On.EntityStates.Toolbot.ToolbotDualWield.orig_OnEnter orig, EntityStates.Toolbot.ToolbotDualWield self)
        {
            orig(self);
            checkSkillString();
        }

        private static void OnExitPowerMode(On.EntityStates.Toolbot.ToolbotDualWield.orig_OnExit orig, EntityStates.Toolbot.ToolbotDualWield self)
        {
            orig(self);
            checkSkillString();
        }

        public static void OnBodyLoaded(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);

            // Delay is to give time for skills like captains supply beacons to be fully loaded in
            Task.Delay((int)(1500)).ContinueWith(t => {
                checkSkillString();
            });
        }

        private static void checkSkillString()
        {
            if (PlayerCharacterMasterController.instances.Count > 0 && PlayerCharacterMasterController.instances[0].master.GetBody())
            {
                var newSkillsString = GetSkillString(inventoryItemsString);
                if (!newSkillsString.Equals(skillsString))
                {
                    skillsString = newSkillsString;
                    bodyColourString = GetColourString();
                    WaitThenSendItemsUpdate(inventoryItemsString, equipmentString, skillsString, bodyColourString);
                }
            }
        }

        private static void OnGameOver(On.RoR2.GameOverController.orig_Awake orig, GameOverController self)
        {
            // Resetting everything to be empty when the game is over
            inventoryItemsString = "";
            equipmentString = "";
            skillsString = "";
            bodyColourString = "";
            WaitThenSendItemsUpdate(inventoryItemsString, equipmentString, skillsString, bodyColourString);
            orig(self);
        }

        private static void OnInventoryChanged(On.RoR2.Inventory.orig_HandleInventoryChanged orig, Inventory self)
        {
            orig(self);

            // If inventory changed matches the local players inventory, and game is not over
            if (PlayerCharacterMasterController.instances.Count > 0 && self.Equals(PlayerCharacterMasterController.instances[0].master.inventory) && !GameOverController.instance)
            {
                // Items

                var items = self.itemAcquisitionOrder;

                var newInventoryItemsString = "";

                // Get internal name and tier of all non hidden items in the form Name[Tier];... or Name{Description}[Tier];
                for (int i = 0; i < items.Count; i++)
                {
                    var itemInfo = ItemCatalog.GetItemDef(items[i]);
                    if (!itemInfo.hidden)
                    {
                        if (VanillaItems.Contains(itemInfo.nameToken) && Language.currentLanguage == Language.english)
                        {
                            // Vanilla English Item Case
                            newInventoryItemsString += itemInfo.nameToken + "[" + itemInfo.tier + "];";
                        } else
                        {
                            // Modded or Non-English Item Case
                            newInventoryItemsString += Language.GetString(itemInfo.nameToken) + "{" + Language.GetString(itemInfo.descriptionToken) + "}" + "[" + itemInfo.tier + "];";
                        }

                    }
                }

                // Removing new line characters
                newInventoryItemsString = newInventoryItemsString.Replace("\n", "");
                newInventoryItemsString = newInventoryItemsString.Replace("\r", "");

                // Equipment

                var equipment = self.currentEquipmentIndex;
                var equipmentInfo = EquipmentCatalog.GetEquipmentDef(equipment);
                var newEquipmentString = "";
                // Get equipment info and description in the form Name{Description}[IsLunar];
                if (equipmentInfo)
                {
                    // Equipment in current active slot
                    newEquipmentString = GetEquipmentString(equipmentInfo);

                    var equipmentAlt = self.alternateEquipmentIndex;
                    var equipmentInfoAlt = EquipmentCatalog.GetEquipmentDef(equipmentAlt);
                    var newEquipmentStringAlt = "";
                    if (equipmentInfoAlt)
                    {
                        // Equipment in alternate slot
                        newEquipmentStringAlt = GetEquipmentString(equipmentInfoAlt);
                    }

                    newEquipmentString += newEquipmentStringAlt;
                }

                // Removing new line characters
                newEquipmentString = newEquipmentString.Replace("\n", "");
                newEquipmentString = newEquipmentString.Replace("\r", "");

                // Skills

                var newSkillsString = "";
                var newBodyColourString = "";
                // If player body exists
                if (PlayerCharacterMasterController.instances[0].master.GetBody())
                {
                    newSkillsString = GetSkillString(newInventoryItemsString);
                    newBodyColourString = GetColourString();
                }

                // If the changed inventory items are different to the previous inventory (gained or lost a unique item)
                if (!newInventoryItemsString.Equals(inventoryItemsString) || !newEquipmentString.Equals(equipmentString) || !newSkillsString.Equals(skillsString))
                {
                    inventoryItemsString = newInventoryItemsString;
                    equipmentString = newEquipmentString;
                    skillsString = newSkillsString;
                    bodyColourString = newBodyColourString;

                    // If in phase 4 mithrix
                    if (PhaseCounter.instance && PhaseCounter.instance.phase == 4)
                    {
                        // If an update isn't already pending to be sent
                        if(!isWaitingForUpdate)
                        {
                            isWaitingForUpdate = true;
                            WaitThenSendItemsUpdatePhase4();
                        } 
                    } else
                    {
                        WaitThenSendItemsUpdate(inventoryItemsString, equipmentString, skillsString, bodyColourString);
                    }
                }
            }
        }
 
        private static string GetEquipmentString(EquipmentDef equipmentInfo)
        {
            var equipmentDescription = Language.GetString(equipmentInfo.descriptionToken);
            if (equipmentDescription == "")
            {
                equipmentDescription = Language.GetString(equipmentInfo.pickupToken);
            }
            string newEquipmentString = Language.GetString(equipmentInfo.nameToken) + "{" + equipmentDescription + "}" + "[" + equipmentInfo.isLunar + "];";

            return newEquipmentString;
        }

        private static string GetSkillString(string InventoryItemsString)
        {

            var primary = PlayerCharacterMasterController.instances[0].master.GetBody().skillLocator.primary;
            var secondary = PlayerCharacterMasterController.instances[0].master.GetBody().skillLocator.secondary;
            var utility = PlayerCharacterMasterController.instances[0].master.GetBody().skillLocator.utility;
            var special = PlayerCharacterMasterController.instances[0].master.GetBody().skillLocator.special;

            if(!primary || !secondary || !utility || !special)
            {
                return skillsString;
            }

            string newSkillsString = "";

            // Primary
            string primaryString;
            if (InventoryItemsString.Contains("ITEM_LUNARPRIMARYREPLACEMENT_NAME"))
            {
                // Visions of Heresy
                primaryString = Language.GetString("SKILL_LUNAR_PRIMARY_REPLACEMENT_NAME") + "{" + Language.GetString("SKILL_LUNAR_PRIMARY_REPLACEMENT_DESCRIPTION") + "};";
            } else if (CaptainBeacons.Contains(primary.baseSkill.GetCurrentNameToken(primary)))
            {
                // If captains supply beacons are overriding primary, send base primary instead
                primaryString = Language.GetString("CAPTAIN_PRIMARY_NAME") + "{" + Language.GetString("CAPTAIN_PRIMARY_DESCRIPTION") + "};";
            } else
            {
                primaryString = Language.GetString(primary.baseSkill.GetCurrentNameToken(primary)) + "{" + Language.GetString(primary.baseSkill.GetCurrentDescriptionToken(primary)) + "};";
            }
            newSkillsString += primaryString;

            // Secondary
            string secondaryString;
            if (MULTPrimarySkills.Contains(secondary.skillDef.GetCurrentNameToken(secondary)))
            {
                // If MUL-T's secondary override is one of its primary's, it's in power mode, so send the overriden secondary instead
                secondaryString = Language.GetString(secondary.skillDef.GetCurrentNameToken(secondary)) + "{" + Language.GetString(secondary.skillDef.GetCurrentDescriptionToken(secondary)) + "};";
            } else if (InventoryItemsString.Contains("ITEM_LUNARSECONDARYREPLACEMENT_NAME"))
            {
                // Hooks of Heresy
                secondaryString = Language.GetString("SKILL_LUNAR_SECONDARY_REPLACEMENT_NAME") + "{" + Language.GetString("SKILL_LUNAR_SECONDARY_REPLACEMENT_DESCRIPTION") + "};";
            } else if (CaptainBeacons.Contains(secondary.baseSkill.GetCurrentNameToken(secondary)))
            {
                // If captains supply beacons are overriding secondary, send base secondary instead
                secondaryString = Language.GetString("CAPTAIN_SECONDARY_NAME") + "{" + Language.GetString("CAPTAIN_SECONDARY_DESCRIPTION") + "};";
            } else
            {
                secondaryString = Language.GetString(secondary.baseSkill.GetCurrentNameToken(secondary)) + "{" + Language.GetString(secondary.baseSkill.GetCurrentDescriptionToken(secondary)) + "};";
            }
            newSkillsString += secondaryString;

            // Utility
            string utilityString;
            if (InventoryItemsString.Contains("ITEM_LUNARUTILITYREPLACEMENT_NAME"))
            {
                // Strides of Heresy
                utilityString = Language.GetString("SKILL_LUNAR_UTILITY_REPLACEMENT_NAME") + "{" + Language.GetString("SKILL_LUNAR_UTILITY_REPLACEMENT_DESCRIPTION") + "};";
            } else
            {
                utilityString = Language.GetString(utility.baseSkill.GetCurrentNameToken(utility)) + "{" + Language.GetString(utility.baseSkill.GetCurrentDescriptionToken(utility)) + "};";
            }
            newSkillsString += utilityString;

            // Special
            string specialString;
            if (InventoryItemsString.Contains("ITEM_LUNARSPECIALREPLACEMENT_NAME"))
            {
                // Essence of Heresy
                specialString = Language.GetString("SKILL_LUNAR_SPECIAL_REPLACEMENT_NAME") + "{" + Language.GetString("SKILL_LUNAR_SPECIAL_REPLACEMENT_DESCRIPTION") + "};";
            } else
            {
                specialString = Language.GetString(special.baseSkill.GetCurrentNameToken(special)) + "{" + Language.GetString(special.baseSkill.GetCurrentDescriptionToken(special)) + "};";
            }
            newSkillsString += specialString;

            return newSkillsString;
        }

        private static string GetColourString()
        {
            var newBodyColour = PlayerCharacterMasterController.instances[0].master.GetBody().bodyColor;
            string newBodyColourString = "rgb(" + Math.Round(newBodyColour.r * 200) + "," + Math.Round(newBodyColour.g * 200) + "," + Math.Round(newBodyColour.b * 200) + ")";
            return newBodyColourString;
        }

            private static void WaitThenSendItemsUpdate(string items, string equipment, string skills, string bodycolour)
        {
            Task.Delay((int)(DelayConfigEntry.Value * 1000)).ContinueWith(t => {
                SendItemsUpdate(items, equipment, skills, bodycolour);
            });
        }

        private static void WaitThenSendItemsUpdatePhase4()
        {
            Task.Delay((int)(DelayConfigEntry.Value * 1000) + 3000).ContinueWith(t => {
                isWaitingForUpdate = false;
                SendItemsUpdate(inventoryItemsString, equipmentString, skillsString, bodyColourString);
            });
        }

        private static async void SendItemsUpdate(string items, string equipment, string skills, string bodycolour)
        {
            // Currently unimplemented, may be used in future to show item images when playing with the HUD off
            var hudstatus = "";

            await Task.Run(() =>
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"twitchName\":\"" + TwitchNameConfigEntry.Value + "\"," +
                                    "\"secretKey\":\"" + SecretKeyConfigEntry.Value + "\"," +
                                    "\"items\":\"" + items + "\"," +
                                    "\"equipment\":\"" + equipment + "\"," +
                                    "\"skills\":\"" + skills + "\"," +
                                    "\"survivor\":\"" + bodycolour + "\"," +
                                    "\"hudstatus\":\"" + hudstatus + "\"}";
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            });
        }

        private void Update()
        {
            // If not in phase 4, make sure waiting for item update is false
            if ((!PhaseCounter.instance || PhaseCounter.instance.phase != 4) && isWaitingForUpdate)
            {
                isWaitingForUpdate = false;
            }

            // If a run is quit, update items to nothing
            if (RoR2.Run.instance == null && (inventoryItemsString.Length > 0 || equipmentString.Length > 0 || skillsString.Length > 0 || bodyColourString.Length > 0))
            {
                inventoryItemsString = "";
                equipmentString = "";
                skillsString = "";
                bodyColourString = "";
                SendItemsUpdate(inventoryItemsString, equipmentString, skillsString, bodyColourString);
            }
        }
    }
}