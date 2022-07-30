using NeosModLoader;
using HarmonyLib;
using FrooxEngine;
using FrooxEngine.CommonAvatar;
using System.Collections.Generic;
using BaseX;
using UnityNeos;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace HeadCullingMask
{
    public class HeadCullingMask : NeosMod
    {
        public override string Name => "HeadCullingMask";
        public override string Author => "hantabaru1014";
        public override string Version => "1.0.0";

        public static readonly string TARGET_COMMENT_TEXT = "net.hantabaru1014.HeadCullingMask.TargetSlot";
        public static readonly byte USE_LAYER = 10;

        private static readonly FieldInfo forceLayerFieldInfo = AccessTools.Field(typeof(SlotConnector), "forceLayer");

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.hantabaru1014.HeadCullingMask");
            harmony.PatchAll();

            var headOutputs = Resources.FindObjectsOfTypeAll<HeadOutput>()
                .Where(c => c.gameObject.hideFlags != HideFlags.NotEditable && c.gameObject.hideFlags != HideFlags.HideAndDontSave);
            foreach (var headOutput in headOutputs)
            {
                foreach (var cam in headOutput.cameras)
                {
                    cam.cullingMask &= ~(1 << USE_LAYER);
                    Msg($"Patch Camera cullingMask : {cam.name}");
                }
            }
        }

        [HarmonyPatch(typeof(AvatarRoot), nameof(AvatarRoot.Equip))]
        class AvatarRootEquipPatch
        {
            static void Postfix(AvatarRoot __instance)
            {
                Msg("Avatar Equip!");
                UpdateLayer(__instance.Slot, USE_LAYER, true);
            }
        }

        [HarmonyPatch(typeof(AvatarRoot), nameof(AvatarRoot.Dequip))]
        class AvatarRootDequipPatch
        {
            static void Postfix(AvatarRoot __instance)
            {
                Msg("Avatar Dequip!");
                UpdateLayer(__instance.Slot, 0, false);
            }
        }

        [HarmonyPatch(typeof(HeadOutput), "Awake")]
        class HeadOutputAwakePatch
        {
            static void Postfix(List<UnityEngine.Camera> ___cameras)
            {
                foreach (var cam in ___cameras)
                {
                    cam.cullingMask &= ~(1 << USE_LAYER);
                    Msg($"Patch Camera cullingMask : {cam.name}");
                }
            }
        }

        [HarmonyPatch(typeof(CameraPortal), "RenderReflection")]
        class CameraPortalRenderReflectionPatch
        {
            static void Prefix(UnityEngine.Camera reflectionCamera)
            {
                reflectionCamera.cullingMask |= (1 << USE_LAYER);
            }
        }

        private static void UpdateLayer(Slot avatarRootSlot, byte layer, bool excludeDisable)
        {
            List<Comment> list = Pool.BorrowList<Comment>();
            avatarRootSlot.GetComponentsInChildren<Comment>(list, (Comment c) => c.Text.Value == TARGET_COMMENT_TEXT, excludeDisable, false);
            foreach (var comment in list)
            {
                forceLayerFieldInfo.SetValue((SlotConnector)comment.Slot.Connector, layer);
                Msg($"SetLayer : {comment.Slot.Name} to {(int)layer}");
            }
        }
    }
}