using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.Common;
using Vintagestory.GameContent;

namespace weightmod.src
{
    public class EntityBehaviorWeightable : EntityBehavior
    {
        bool shouldUpdate = true;
        float accum = 0;
        float currentCalculatedWeight = 0;
        float lastCalculatedWeight = 0;
        ITreeAttribute weightTree;
        ITreeAttribute healthTree;
        float lastHealth;
        float lastMaxHealth;
        bool shouldSendPacket = false;
        float lastWeightBonusBags = 0;
        float currentWeightBonusBags = 0;
        public float maxWeight
        {
            get { return weightTree.GetFloat("maxweight"); }
            set
            {
                weightTree.SetFloat("maxweight", value);
                entity.WatchedAttributes.MarkPathDirty("weightmod");
            }
        }

        public float weight
        {
            get { return weightTree.GetFloat("currentweight"); }
            set { weightTree.SetFloat("currentweight", value); entity.WatchedAttributes.MarkPathDirty("weightmod"); }
        }

        /*public float baseMaxWeight
        {
            get { return weightTree.GetFloat("basemaxweight"); }
            set
            {
                weightTree.SetFloat("basemaxweight", value);
                entity.WatchedAttributes.MarkPathDirty("weightmod");
            }
        }*/
        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            weightTree = entity.WatchedAttributes.GetTreeAttribute("weightmod");
            healthTree = entity.WatchedAttributes.GetTreeAttribute("health");

            if (weightTree == null)
            {
                entity.WatchedAttributes.SetAttribute("weightmod", weightTree = new TreeAttribute());

                weight = attributes["currentweight"].AsFloat(0);
                maxWeight = attributes["maxweight"].AsFloat(Config.Current.MAX_PLAYER_WEIGHT.Val);
                lastWeightBonusBags = 0;
                MarkDirty();
                return;
            }
            lastHealth = healthTree.GetFloat("currenthealth");
            lastMaxHealth = healthTree.GetFloat("maxhealth");

            weight = weightTree.GetFloat("currentweight");
            maxWeight = Config.Current.MAX_PLAYER_WEIGHT.Val;
            //baseMaxWeight = weightTree.GetFloat("basemaxweight");
            lastWeightBonusBags = weightTree.GetFloat("weighbonus");
            //if (baseMaxWeight == 0) baseMaxWeight = attributes["maxweight"].AsFloat(Config.Current.MAX_PLAYER_WEIGHT.Val);

            MarkDirty();
        }
       /* public void UpdateMaxWeight()
        {
            float totalMaxWeight = baseMaxWeight;
            maxWeight = baseMaxWeight;
        }*/
        public void MarkDirty()
        {
            //UpdateMaxWeight();
            entity.WatchedAttributes.MarkPathDirty("weightmod");
        }
        public EntityBehaviorWeightable(Entity entity) : base(entity)
        {

        }

        public bool isOverloaded()
        {
            return this.weight > this.maxWeight;
        }
        public override string PropertyName()
        {
            return "affectedByItemsWeight";
        }
        public bool calculateWeightOfInventories()
        {
            currentCalculatedWeight = 0;
            currentWeightBonusBags = 0;
            shouldUpdate = false;
            //Backpacks
            InventoryPlayerBackPacks playerBackpacks = (InventoryPlayerBackPacks)((this.entity as EntityPlayer).Player as IServerPlayer).InventoryManager.GetOwnInventory("backpack");
            for(int i = 0; i < 4; ++i)
            {
                if(playerBackpacks[i] != null)
                {
                    ItemSlot itemSlot = playerBackpacks[i];
                    ItemStack itemStack = itemSlot.Itemstack;
                    if (itemStack != null)
                    {
                        if (itemStack.Collectible.Attributes != null && itemStack.Collectible.Attributes["weightbonusbags"].Exists)
                        {
                            currentWeightBonusBags += itemStack.Collectible.Attributes["weightbonusbags"].AsFloat() * itemStack.StackSize;
                            continue;
                        }
                    }
                }
            }

            {
                shouldUpdate = true;
               
                for (int i = 0; i < playerBackpacks.Count; i++)
                {
                    ItemSlot itemSlot = playerBackpacks[i];
                    if (itemSlot != null)
                    {
                        ItemStack itemStack = itemSlot.Itemstack;
                        if (itemStack != null)
                        {
                            if (itemStack.Collectible.Attributes != null && itemStack.Collectible.Attributes["weightmod"].Exists)
                            {
                                currentCalculatedWeight += itemStack.Collectible.Attributes["weightmod"].AsFloat() * itemStack.StackSize;
                                continue;
                            }
                        }
                    }
                }               
            }
            //Hotbar
            IInventory playerHotbar = (IInventory)((this.entity as EntityPlayer).Player as IServerPlayer).InventoryManager.GetHotbarInventory();
            {

                shouldUpdate = true;
                for (int i = 0; i < playerHotbar.Count; i++)
                {
                    ItemSlot itemSlot = playerHotbar[i];
                    if (itemSlot != null)
                    {
                        ItemStack itemStack = itemSlot.Itemstack;
                        if (itemStack != null)
                        {
                            if (itemStack.Collectible.Attributes != null && itemStack.Collectible.Attributes["weightmod"].Exists)
                            {
                                currentCalculatedWeight += itemStack.Collectible.Attributes["weightmod"].AsFloat() * itemStack.StackSize;
                                    continue;  
                            }
                        }
                    }
                }
            }
            //Character inventory
            IInventory charakterInv =  ((this.entity as EntityPlayer).Player as IServerPlayer).InventoryManager.GetOwnInventory("character");

            {
                shouldUpdate = true;
                for (int i = 0; i < charakterInv.Count; i++)
                {
                    ItemSlot itemSlot = charakterInv[i];
                    if (itemSlot != null)
                    {
                        ItemStack itemStack = itemSlot.Itemstack;
                        if (itemStack != null)
                        {
                            if (itemStack.Collectible.Attributes != null && itemStack.Collectible.Attributes["weightmod"].Exists)
                            {
                                currentCalculatedWeight += itemStack.Collectible.Attributes["weightmod"].AsFloat() * itemStack.StackSize;
                                continue;
                            }
                        }
                    }
                }
            }
           IInventory mouseInv = ((this.entity as EntityPlayer).Player as IServerPlayer).InventoryManager.GetOwnInventory("mouse");
            {
                shouldUpdate = true;
                for (int i = 0; i < mouseInv.Count; i++)
                {
                    ItemSlot itemSlot = mouseInv[i];
                    if (itemSlot != null)
                    {
                        ItemStack itemStack = itemSlot.Itemstack;
                        if (itemStack != null)
                        {
                            if (itemStack.Collectible.Attributes != null && itemStack.Collectible.Attributes["weightmod"].Exists)
                            {
                                currentCalculatedWeight += itemStack.Collectible.Attributes["weightmod"].AsFloat() * itemStack.StackSize;
                                continue;
                            }
                        }
                    }
                }
            }
            if(currentCalculatedWeight < 0)
            {
                currentCalculatedWeight = 0;
            }
            if((entity as EntityPlayer).Player.WorldData.CurrentGameMode == EnumGameMode.Creative)
            {
                currentCalculatedWeight = 0;
            }
            return shouldUpdate;
        }
        public override void OnReceivedServerPacket(int packetid, byte[] data, ref EnumHandling handled)
        {
            if (packetid == 6166)
            {
                ITreeAttribute treeAttribute = entity.WatchedAttributes.GetTreeAttribute("weightmod");
                this.weight = treeAttribute.GetFloat("currentweight");
                this.maxWeight = treeAttribute.GetFloat("maxweight");
            }
        }
        public override void OnGameTick(float deltaTime)
        {            
            accum += deltaTime;
            if (accum >= Config.Current.ACCUM_TIME_WEIGHT_CHECK.Val)
            {
                accum = 0;
                if (entity.World.Api.Side == EnumAppSide.Server)
                {
                    //currentCalculatedWeight updated
                    calculateWeightOfInventories();

                    //Check if health has been changed, maxweight depends on ration current/max
                    ITreeAttribute treeAttribute = entity.WatchedAttributes.GetTreeAttribute("weightmod");                   
                    healthTree = entity.WatchedAttributes.GetTreeAttribute("health");
                    float tmpHealth = healthTree.GetFloat("currenthealth");
                    float tmpMaxHealth = healthTree.GetFloat("maxhealth");
                    if (tmpHealth != lastHealth || tmpMaxHealth != lastMaxHealth)
                    {
                        //should send packet to let client know when to update maxweight
                        shouldSendPacket = true;
                        maxWeight = Config.Current.MAX_PLAYER_WEIGHT.Val * (tmpHealth / tmpMaxHealth);

                        //there is lower border of maxweight, using ratio from config
                        if (maxWeight < Config.Current.MAX_PLAYER_WEIGHT.Val * Config.Current.RATIO_MIN_MAX_WEIGHT_PLAYER_HEALTH.Val)
                        {
                            maxWeight = Config.Current.MAX_PLAYER_WEIGHT.Val * Config.Current.RATIO_MIN_MAX_WEIGHT_PLAYER_HEALTH.Val;
                        }
                        lastHealth = tmpHealth;
                        lastMaxHealth = tmpMaxHealth;
                    }

                    //currentWeightBonus = treeAttribute.GetFloat("weightbonus");
                    if (currentWeightBonusBags != lastWeightBonusBags)
                    {
                        maxWeight -= lastWeightBonusBags;
                        shouldSendPacket = true;
                        //lastCalculatedWeight = currentWeightBonusBags;
                        maxWeight = maxWeight + currentWeightBonusBags;
                        if(maxWeight < 0)
                        {
                            maxWeight = 0;
                        }
                        lastWeightBonusBags = currentWeightBonusBags;
                    }

                    if (currentCalculatedWeight > maxWeight)
                    {
                        //Processed in harmPatch (Prefix_DoApplyOnGround/Prefix_DoApplyInLiquid) using isOverloaded
                        entity.Stats.Set("walkspeed", "weightmod", (float)(-2), true);
                    }
                    //when player is not overburden yet but currentweight is across threshold value from config,
                    //slower movespeed
                    else if (currentCalculatedWeight > maxWeight * Config.Current.WEIGH_PLAYER_THRESHOLD.Val)
                    {
                        entity.Stats.Set("walkspeed", "weightmod", (float)(-0.2 * (weight / maxWeight)), true);
                    }
                    else
                    {
                        entity.Stats.Set("walkspeed", "weightmod", 0);
                    }

                   
                    //if health ratio was changed or current and last weight is not the same = send packet and also update
                    //treeAttribute with maxweight and currentweight
                    if (shouldSendPacket || lastCalculatedWeight != currentCalculatedWeight)
                    {
                        
                        treeAttribute.SetFloat("currentweight", currentCalculatedWeight);
                        treeAttribute.SetFloat("maxweight", maxWeight);
                        entity.WatchedAttributes.MarkPathDirty("weightmod");
                        (entity.Api as ICoreServerAPI).Network.SendEntityPacket((entity as EntityPlayer).Player as IServerPlayer, entity.EntityId, 6166, null);
                    }

                    //current now last
                    lastCalculatedWeight = currentCalculatedWeight;

                }
                else
                {                  
                    if (weight > maxWeight)
                    {
                        var ft = entity.AnimManager;
                    }
                }
            }
        }

        public override void OnEntityDeath(DamageSource damageSourceForDeath)
        {
            if (this.entity.World.Side == EnumAppSide.Server)
            {
                calculateWeightOfInventories();
                if (currentCalculatedWeight < 0)
                {
                    currentCalculatedWeight = 0;
                }
                if ((entity as EntityPlayer).Player.WorldData.CurrentGameMode == EnumGameMode.Creative)
                {
                    currentCalculatedWeight = 0;
                }
            }
        }

    }
}
