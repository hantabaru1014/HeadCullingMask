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
        public override string Version => "1.1.0";
        public override string Link => "https://github.com/hantabaru1014/HeadCullingMask";

        public static readonly string TARGET_COMMENT_TEXT = "net.hantabaru1014.HeadCullingMask.TargetSlot";
        public static readonly byte USE_LAYER = 10;

        private static readonly FieldInfo forceLayerFieldInfo = AccessTools.Field(typeof(SlotConnector), "forceLayer");

        private static Dictionary<RefID, Slot> avatars;

        public override void OnEngineInit()
        {
            avatars = new Dictionary<RefID, Slot>();

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

        [HarmonyPatch(typeof(ComponentBase<FrooxEngine.Component>), "OnStart")]
        class ComponentBase_OnStart_Patch
        {
            static void Postfix(FrooxEngine.Component __instance)
            {
                if (!(__instance is AvatarObjectSlot)) return;
                if ((__instance.Slot?.ActiveUser?.IsLocalUser ?? false) && (__instance.Slot.Name?.StartsWith("User") ?? false))
                {
                    AvatarObjectSlot avatarObjSlot = __instance as AvatarObjectSlot;
                    avatarObjSlot.Equipped.OnTargetChange += Equipped_OnTargetChange;
                    __instance.Disposing += Worker_Disposing;
                    Msg($"Found AvatarObjectSlot. RefID: {avatarObjSlot.ReferenceID}");
                }
            }

            private static void Worker_Disposing(Worker obj)
            {
                avatars.Remove(obj.ReferenceID);
                Msg($"Dispose AvatarObjectSlot. RefID: {obj.ReferenceID}");
            }

            private static void Equipped_OnTargetChange(SyncRef<IAvatarObject> avatarObj)
            {
                if (avatarObj?.Target?.Node != BodyNode.Root) return;
                if (avatars.TryGetValue(avatarObj.Worker.ReferenceID, out var oldAvatar))
                {
                    if (!(oldAvatar is null))
                    {
                        Msg($"Avatar Dequip : {oldAvatar.Name}");
                        UpdateLayer(oldAvatar, 0, false);
                        avatars[avatarObj.Worker.ReferenceID] = null;
                    }
                }
                if (avatarObj.State == ReferenceState.Available)
                {
                    Msg($"Avatar Equip : {avatarObj.Target.Slot.Name}");
                    UpdateLayer(avatarObj.Target.Slot, USE_LAYER, true);
                    avatars[avatarObj.Worker.ReferenceID] = avatarObj.Target.Slot;
                }
            }
        }

        [HarmonyPatch(typeof(HeadOutput), "Awake")]
        class HeadOutput_Awake_Patch
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
        class CameraPortal_RenderReflection_Patch
        {
            static void Prefix(UnityEngine.Camera reflectionCamera)
            {
                reflectionCamera.cullingMask |= (1 << USE_LAYER);
            }
        }

        private static void UpdateLayer(Slot avatarRootSlot, byte layer, bool excludeDisabled)
        {
            List<Comment> list = Pool.BorrowList<Comment>();
            avatarRootSlot.GetComponentsInChildren<Comment>(list, (Comment c) => (!excludeDisabled || c.Enabled) && c.Text.Value == TARGET_COMMENT_TEXT, false, false);
            foreach (var comment in list)
            {
                forceLayerFieldInfo.SetValue((SlotConnector)comment.Slot.Connector, layer);
                Msg($"SetLayer : {comment.Slot.Name} to {(int)layer}");
            }
        }
    }
}