using BepInEx;
using BepInEx.Configuration;
using Facepunch.Steamworks;
using MonoMod.RuntimeDetour;
using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.UIElements.Experimental;
using static RoR2.GivePickupsOnStart;
using Inventory = RoR2.Inventory;

namespace StreamCompanion
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class StreamCompanion : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "TaranDev";
        public const string PluginName = "RoR2StreamCompanion";
        public const string PluginVersion = "1.0.0";

        // Backend
        static readonly HttpClient client = new HttpClient();
        public const string url = "https://ror2-sc-api.herokuapp.com/streamers";

        static string inventoryItemsString = "";
        static string equipmentString = "";
        
        // Config file entries
        public static ConfigEntry<string> TwitchNameConfigEntry { get; set; }
        public static ConfigEntry<string> SecretKeyConfigEntry { get; set; }
        public static ConfigEntry<double> DelayConfigEntry { get; set; }

        // Vanilla Item List
        public static List<string> VanillaItems = new List<string>() { "ITEM_BOSSDAMAGEBONUS_NAME", "ITEM_SECONDARYSKILLMAGAZINE_NAME", "ITEM_FLATHEALTH_NAME", "ITEM_FIREWORK_NAME", "ITEM_MUSHROOM_NAME", "ITEM_HEALWHILESAFE_NAME", "ITEM_CROWBAR_NAME", "ITEM_FRAGILEDAMAGEBONUS_NAME", "ITEM_SPRINTBONUS_NAME", "ITEM_NEARBYDAMAGEBONUS_NAME", "ITEM_IGNITEONKILL_NAME", "ITEM_SCRAPWHITE_NAME", "ITEM_CRITGLASSES_NAME", "ITEM_MEDKIT_NAME", "ITEM_ATTACKSPEEDANDMOVESPEED_NAME", "ITEM_TOOTH_NAME", "ITEM_OUTOFCOMBATARMOR_NAME", "ITEM_HOOF_NAME", "ITEM_PERSONALSHIELD_NAME", "ITEM_HEALINGPOTION_NAME", "ITEM_REPULSIONARMORPLATE_NAME", "ITEM_GOLDONHURT_NAME", "ITEM_TREASURECACHE_NAME", "ITEM_SYRINGE_NAME", "ITEM_STICKYBOMB_NAME", "ITEM_STUNCHANCEONHIT_NAME", "ITEM_BARRIERONKILL_NAME", "ITEM_BEAR_NAME", "ITEM_BLEEDONHIT_NAME", "ITEM_WARDONLEVEL_NAME", "ITEM_MISSILE_NAME", "ITEM_BANDOLIER_NAME", "ITEM_WARCRYONMULTIKILL_NAME", "ITEM_SLOWONHIT_NAME", "ITEM_DEATHMARK_NAME", "ITEM_EQUIPMENTMAGAZINE_NAME", "ITEM_BONUSGOLDPACKONKILL_NAME", "ITEM_HEALONCRIT_NAME", "ITEM_FEATHER_NAME", "ITEM_MOVESPEEDONKILL_NAME", "ITEM_STRENGTHENBURN_NAME", "ITEM_INFUSION_NAME", "ITEM_SCRAPGREEN_NAME", "ITEM_FIRERING_NAME", "ITEM_SEED_NAME", "ITEM_TPHEALINGNOVA_NAME", "ITEM_EXECUTELOWHEALTHELITE_NAME", "ITEM_PHASING_NAME", "ITEM_ATTACKSPEEDONCRIT_NAME", "ITEM_THORNS_NAME", "ITEM_SPRINTOUTOFCOMBAT_NAME", "ITEM_REGENERATINGSCRAP_NAME", "ITEM_SPRINTARMOR_NAME", "ITEM_ICERING_NAME", "ITEM_FREECHEST_NAME", "ITEM_PRIMARYSKILLSHURIKEN_NAME", "ITEM_SQUIDTURRET_NAME", "ITEM_CHAINLIGHTNING_NAME", "ITEM_ENERGIZEDONEQUIPMENTUSE_NAME", "ITEM_JUMPBOOST_NAME", "ITEM_EXPLODEONDEATH_NAME", "ITEM_CLOVER_NAME", "ITEM_BARRIERONOVERHEAL_NAME", "ITEM_ALIENHEAD_NAME", "ITEM_IMMUNETODEBUFF_NAME", "ITEM_RANDOMEQUIPMENTTRIGGER_NAME", "ITEM_KILLELITEFRENZY_NAME", "ITEM_BEHEMOTH_NAME", "ITEM_DAGGER_NAME", "ITEM_CAPTAINDEFENSEMATRIX_NAME", "ITEM_EXTRALIFE_NAME", "ITEM_ICICLE_NAME", "ITEM_FALLBOOTS_NAME", "ITEM_GHOSTONKILL_NAME", "ITEM_UTILITYSKILLMAGAZINE_NAME", "ITEM_INTERSTELLARDESKPLANT_NAME", "ITEM_SCRAPRED_NAME", "ITEM_CRITDAMAGE_NAME", "ITEM_NOVAONHEAL_NAME", "ITEM_MOREMISSILE_NAME", "ITEM_INCREASEHEALING_NAME", "ITEM_LASERTURBINE_NAME", "ITEM_BOUNCENEARBY_NAME", "ITEM_ARMORREDUCTIONONHIT_NAME", "ITEM_TALISMAN_NAME", "ITEM_DRONEWEAPONS_NAME", "ITEM_PERMANENTDEBUFFONHIT_NAME", "ITEM_SHOCKNEARBY_NAME", "ITEM_HEADHUNTER_NAME", "ITEM_ARTIFACTKEY_NAME", "ITEM_LIGHTNINGSTRIKEONHIT_NAME", "ITEM_MINORCONSTRUCTONKILL_NAME", "ITEM_ROBOBALLBUDDY_NAME", "ITEM_NOVAONLOWHEALTH_NAME", "ITEM_TITANGOLDDURINGTP_NAME", "ITEM_SHINYPEARL_NAME", "ITEM_SCRAPYELLOW_NAME", "ITEM_SPRINTWISP_NAME", "ITEM_SIPHONONLOWHEALTH_NAME", "ITEM_FIREBALLSONHIT_NAME", "ITEM_PEARL_NAME", "ITEM_PARENTEGG_NAME", "ITEM_BEETLEGLAND_NAME", "ITEM_BLEEDONHITANDEXPLODE_NAME", "ITEM_KNURL_NAME", "ITEM_LUNARTRINKET_NAME", "ITEM_GOLDONHIT_NAME", "ITEM_REPEATHEAL_NAME", "ITEM_MONSTERSONSHRINEUSE_NAME", "ITEM_LUNARSUN_NAME", "ITEM_LUNARSPECIALREPLACEMENT_NAME", "ITEM_RANDOMLYLUNAR_NAME", "ITEM_FOCUSEDCONVERGENCE_NAME", "ITEM_AUTOCASTEQUIPMENT_NAME", "ITEM_LUNARSECONDARYREPLACEMENT_NAME", "ITEM_HALFATTACKSPEEDHALFCOOLDOWNS_NAME", "ITEM_RANDOMDAMAGEZONE_NAME", "ITEM_LUNARBADLUCK_NAME", "ITEM_LUNARDAGGER_NAME", "ITEM_HALFSPEEDDOUBLEHEALTH_NAME", "ITEM_LUNARUTILITYREPLACEMENT_NAME", "ITEM_SHIELDONLY_NAME", "ITEM_LUNARPRIMARYREPLACEMENT_NAME", "ITEM_CLOVERVOID_NAME", "ITEM_TREASURECACHEVOID_NAME", "ITEM_CRITGLASSESVOID_NAME", "ITEM_EQUIPMENTMAGAZINEVOID_NAME", "ITEM_BLEEDONHITVOID_NAME", "ITEM_VOIDMEGACRABITEM_NAME", "ITEM_MISSILEVOID_NAME", "ITEM_EXTRALIFEVOID_NAME", "ITEM_CHAINLIGHTNINGVOID_NAME", "ITEM_BEARVOID_NAME", "ITEM_ELEMENTALRINGVOID_NAME", "ITEM_SLOWONHITVOID_NAME", "ITEM_EXPLODEONDEATHVOID_NAME", "ITEM_MUSHROOMVOID_NAME", "ITEM_FRAGILEDAMAGEBONUSCONSUMED_NAME", "ITEM_EXTRALIFECONSUMED_NAME", "ITEM_HEALINGPOTIONCONSUMED_NAME", "ITEM_EXTRALIFEVOIDCONSUMED_NAME", "ITEM_REGENERATINGSCRAPCONSUMED_NAME", "ITEM_TONICAFFLICTION_NAME" };

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
            SendItemsUpdate(inventoryItemsString, equipmentString);
        }

        // Hook
        private void OnEnable()
        {
            On.RoR2.Inventory.HandleInventoryChanged += OnInventoryChanged;
            On.RoR2.GameOverController.Awake += OnGameOver;
        }
        private void OnDisable()
        {
            On.RoR2.Inventory.HandleInventoryChanged -= OnInventoryChanged;
            On.RoR2.GameOverController.Awake -= OnGameOver;
        }

        private static void OnGameOver(On.RoR2.GameOverController.orig_Awake orig, GameOverController self)
        {
            inventoryItemsString = "";
            equipmentString = "";
            WaitThenSendItemsUpdate(inventoryItemsString, equipmentString);
            orig(self);
        }

        private static void OnInventoryChanged(On.RoR2.Inventory.orig_HandleInventoryChanged orig, Inventory self)
        {
            orig(self);

            // If inventory changed matches the local players inventort
            if (PlayerCharacterMasterController.instances.Count > 0 && self.Equals(PlayerCharacterMasterController.instances[0].master.inventory) && !GameOverController.instance)
            {
                var items = self.itemAcquisitionOrder;

                var newInventoryItemsString = "";

                // Get internal name and tier of all non hidden items in the form Name[Tier];Name[Tier];...
                for (int i = 0; i < items.Count; i++)
                {
                    var itemInfo = ItemCatalog.GetItemDef(items[i]);
                    if (!itemInfo.hidden) {
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

                var equipment = self.currentEquipmentIndex;
                var equipmentInfo = EquipmentCatalog.GetEquipmentDef(equipment);
                var newEquipmentString = "";
                if (equipmentInfo) {
                    // Equipment in current active slot
                    var equipmentDescription = Language.GetString(equipmentInfo.descriptionToken);
                    if(equipmentDescription == "")
                    {
                        equipmentDescription = Language.GetString(equipmentInfo.pickupToken);
                    }
                    newEquipmentString = Language.GetString(equipmentInfo.nameToken) + "{" + equipmentDescription + "}" +  "[" + equipmentInfo.isLunar + "];";

                    var equipmentAlt = self.alternateEquipmentIndex;
                    var equipmentInfoAlt = EquipmentCatalog.GetEquipmentDef(equipmentAlt);
                    var newEquipmentStringAlt = "";
                    if (equipmentInfoAlt)
                    {
                        // Equipment in alternate active slot
                        var equipmentDescriptionAlt = Language.GetString(equipmentInfoAlt.descriptionToken);
                        if (equipmentDescriptionAlt == "")
                        {
                            equipmentDescriptionAlt = Language.GetString(equipmentInfoAlt.pickupToken);
                        }
                        newEquipmentStringAlt = Language.GetString(equipmentInfoAlt.nameToken) + "{" + equipmentDescriptionAlt + "}" + "[" + equipmentInfoAlt.isLunar + "];";
                    }

                    newEquipmentString += newEquipmentStringAlt;
                }

                // Removing new line characters
                newEquipmentString = newEquipmentString.Replace("\n", "");
                newEquipmentString = newEquipmentString.Replace("\r", "");


                // If the changed inventory items are different to the previous inventory (gained or lost a unique item)
                if (!newInventoryItemsString.Equals(inventoryItemsString) || !newEquipmentString.Equals(equipmentString)) {
                    inventoryItemsString = newInventoryItemsString;
                    equipmentString = newEquipmentString;
                    WaitThenSendItemsUpdate(inventoryItemsString, equipmentString);
                }
            }
        }

        private static void WaitThenSendItemsUpdate(string items, string equipment)
        {
            Task.Delay((int) (DelayConfigEntry.Value*1000)).ContinueWith(t => SendItemsUpdate(items, equipment));
        }

        private static async void SendItemsUpdate(string items, string equipment)
        {
            await Task.Run(() =>
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "/" + TwitchNameConfigEntry.Value);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"secretKey\":\"" + SecretKeyConfigEntry.Value + "\"," +
                                  "\"items\":\"" + items + "\"," +
                                    "\"equipment\":\"" + equipment + "\"}";
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            });
        }

        private void Update()
        {
            // If a run is quit, update items to nothing
            if(RoR2.Run.instance == null && inventoryItemsString.Length > 0) {
                inventoryItemsString = "";
                equipmentString = "";
                SendItemsUpdate(inventoryItemsString, equipmentString);
                Log.Info($"Player items {inventoryItemsString}");
            }

        }

    }
}
