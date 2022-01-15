using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace weightmod.src
{
    public class weightmod: ModSystem
    {
        ICoreServerAPI sapi;
        private static Dictionary<int, float> mapIdWeightItems = new Dictionary<int, float>();
        private static Dictionary<int, float> mapIdWeightBlocks = new Dictionary<int, float>();
        private static Dictionary<string, float> mapLastCalculatedPlayerWeight = new Dictionary<string, float>();
        private static Dictionary<string, bool> inventoryWasModified = new Dictionary<string, bool>();
        public static Harmony harmonyInstance;
        public const string harmonyID = "weightmod.Patches";
        static weightmod instance;
        public void OnPlayerNowPlaying(IServerPlayer byPlayer)
        {
            mapLastCalculatedPlayerWeight.Add(byPlayer.PlayerUID, 0);
        }
        public static Dictionary<string, float> getlastCalculatedPlayerWeight()
        {
            return mapLastCalculatedPlayerWeight;
        }
        public static Dictionary<string, bool> getinventoryWasModified()
        {
            return inventoryWasModified;
        }
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterEntityBehaviorClass("affectedByItemsWeight", typeof(EntityBehaviorWeightable));
            harmonyInstance = new Harmony(harmonyID);
            harmonyInstance.Patch(typeof(Vintagestory.GameContent.EntityInAir).GetMethod("Applicable"), prefix: new HarmonyMethod(typeof(harmPatch).GetMethod("Prefix_ApplicableInAir")));
            harmonyInstance.Patch(typeof(Vintagestory.GameContent.EntityInLiquid).GetMethod("Applicable"), prefix: new HarmonyMethod(typeof(harmPatch).GetMethod("Prefix_ApplicableInLiquid")));
            harmonyInstance.Patch(typeof(Vintagestory.GameContent.EntityOnGround).GetMethod("Applicable"), prefix: new HarmonyMethod(typeof(harmPatch).GetMethod("Prefix_ApplicableOnGround")));
        }
        public weightmod()
        {
            instance = this;
        }
        public static weightmod getInstance()
        {
            return instance;
        }
        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            api.Gui.RegisterDialog((GuiDialog)new HudWeightPlayer((ICoreClientAPI)api));
            harmonyInstance = new Harmony(harmonyID);
            harmonyInstance.Patch(typeof(Vintagestory.API.Common.CollectibleObject).GetMethod("GetHeldItemInfo"), postfix: new HarmonyMethod(typeof(harmPatch).GetMethod("Postfix_GetHeldItemInfo")));
        }
        public void PlayerEventDelegate(IClientPlayer byPlayer)
        {
           // byPlayer.Entity.AddBehavior(new EntityBehaviorWeightable(byPlayer.Entity));
        }
        public override void StartServerSide(ICoreServerAPI api)
        {
            sapi = api;
            base.StartServerSide(api);
            loadConfig();
            api.Event.PlayerNowPlaying += OnPlayerNowPlaying;                   
        }
      
        public void loadConfig()
        {
            try
            {
                Config.Current = sapi.LoadModConfig<Config>(this.Mod.Info.ModID + ".json");
                if (Config.Current != null)
                {
                    sapi.StoreModConfig<Config>(Config.Current, this.Mod.Info.ModID + ".json");
                    return;
                }
            }
            catch (Exception e)
            {
                sapi.Logger.Error("loadConfig::" + e.Message);
            }

            Config.Current = new Config();
            sapi.StoreModConfig<Config>(Config.Current, this.Mod.Info.ModID + ".json");
            return;
        }
    }
}
