using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.Common;
namespace weightmod.src
{
    [HarmonyPatch]
    public class harmPatch
    {
        public static void Postfix_GetHeldItemInfo(Vintagestory.API.Common.CollectibleObject __instance, ItemSlot inSlot,
                                                                                                         StringBuilder dsc,
                                                                                                         IWorldAccessor world,
                                                                                                         bool withDebugInfo)
        {
            ItemStack itemstack = inSlot.Itemstack;
            if (itemstack.ItemAttributes != null && itemstack.ItemAttributes["weightmod"].Exists)
            {             
                float tmp = itemstack.ItemAttributes["weightmod"].AsFloat();
                if (tmp <= 0) return;
                dsc.Append("Weight: ").Append(itemstack.ItemAttributes["weightmod"].AsFloat().ToString()).Append("\n");
            }else if(itemstack.ItemAttributes != null && itemstack.ItemAttributes["weightbonusbags"].Exists)
            {
                float tmp = itemstack.ItemAttributes["weightbonusbags"].AsFloat();
                if (tmp <= 0) return;
                dsc.Append("Bonus weight: ").Append(itemstack.ItemAttributes["weightbonusbags"].AsFloat().ToString()).Append("\n");
            }
           
            return;
        }
        public static bool Prefix_ApplicableInAir(Vintagestory.GameContent.EntityInAir __instance, Entity entity, EntityPos pos, EntityControls controls)
        {
            if (!(entity is EntityPlayer))
            {
                return true;
            }
            if(!(entity.World.Side == EnumAppSide.Client))
            {
                return true;
            }
            EntityBehaviorWeightable beBeh = entity.GetBehavior<EntityBehaviorWeightable>();
            if (beBeh != null)
            {
                //EntityBehaviorControlledPhysics
                if (beBeh.isOverloaded())
                {
                    return false;
                }
            }
            return true;
        }
        public static bool Prefix_ApplicableInLiquid(Vintagestory.GameContent.EntityInLiquid __instance, Entity entity, EntityPos pos, EntityControls controls)
        {
            if (!(entity is EntityPlayer))
            {
                return true;
            }
            if (!(entity.World.Side == EnumAppSide.Client))
            {
                return true;
            }
            EntityBehaviorWeightable beBeh = entity.GetBehavior<EntityBehaviorWeightable>();
            if (beBeh != null)
            {
                //EntityBehaviorControlledPhysics
                if (beBeh.isOverloaded())
                {
                    return false;
                }
            }
            return true;
        }
        public static bool Prefix_ApplicableOnGround(Vintagestory.GameContent.EntityOnGround __instance, Entity entity, EntityPos pos, EntityControls controls)
        {
            if (!(entity is EntityPlayer))
            {
                return true;
            }
            if (!(entity.World.Side == EnumAppSide.Client))
            {
                return true;
            }
            EntityBehaviorWeightable beBeh = entity.GetBehavior<EntityBehaviorWeightable>();
            if (beBeh != null)
            {
                //EntityBehaviorControlledPhysics
                if (beBeh.isOverloaded())
                {
                    return false;
                }
            }
            return true;
        }      
    }
}
