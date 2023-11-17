using ExitGames.Client.Photon;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace VR_Sync
{
    [HarmonyPatch(typeof(PLCameraSystem), "UpdateVRControllersNow")]
    internal class UpdateVRControllersRPC
    {
        public static void Postfix(PLCameraSystem __instance)
        {
            if (PLNetworkManager.Instance.MyLocalPawn == null) return;
            PLServer.Instance.SendUnreliableVRHandPosUpdateToOthers(
                PLNetworkManager.Instance.MyLocalPawn.CombatTargetID,
                __instance.LeftHandBaseTransform.position,
                __instance.RightHandBaseTransform.position,
                __instance.CurrentSubSystem.LocalPawnCameras[0].transform.position,
                __instance.LeftHandBaseTransform.rotation,
                __instance.RightHandBaseTransform.rotation,
                __instance.CurrentSubSystem.LocalPawnCameras[0].transform.rotation
                );
        }
    }

    #region Setup Character Rigging
    [HarmonyPatch(typeof(PLServer), "Start")]
    class Start
    {
        static void Postfix()
        {
            Character.Objects = new Dictionary<PLPawn, Character>();
        }
    }
    [HarmonyPatch(typeof(PLPawn), "Start")]
    class CreatePawnPositions
    {
        static void Postfix(PLPawn __instance)
        {
            Character character = new Character();
            foreach (Transform transform in __instance.transform.GetComponentsInChildren<Transform>())
            {
                GameObject go = transform.gameObject;
                if (go.name == "HeadTransform") character.Head = go; 
                else if (go.name == "ItemRootRight") character.RightHand = go;
                else if (go.name == "PL_HumanCharacter_LeftHand") character.LeftHand = go;
            }
            Character.Objects.Add(__instance, character);
        }
    }
    [HarmonyPatch(typeof(PLPawn), "OnDestroy")]
    class RemovePawnPositions
    {
        static void Prefix(PLPawn __instance)
        {
            Character.Objects.Remove(__instance);
        }
    }
    #endregion

    [HarmonyPatch(typeof(PLPawn), "VRNetUpdate")]
    internal class UpdatePawnPositions
    {
        public static bool Prefix(PLPawn __instance, Vector3 left, Vector3 right, Vector3 head, Quaternion leftRot, Quaternion rightRot, Quaternion headRot, int serverMs)
        {
            if (!Character.Objects.ContainsKey(__instance)) return true;
            Character character = Character.Objects[__instance];
            if (character == null) return true;
            if (__instance.LastestVRUpdateServerMs < serverMs)
            {
                __instance.LastestVRUpdateServerMs = serverMs;
                __instance.NetVRLeftHandPos = left;
                character.LeftHand.transform.position = left;
                __instance.NetVRRightHandPos = right;
                character.RightHand.transform.position = right;
                __instance.NetVRHeadPos = head;
                character.Head.transform.position = head;
                __instance.NetVRHeadRot = headRot;
                character.Head.transform.rotation = headRot;
                __instance.NetVRLeftHandRot = leftRot;
                character.LeftHand.transform.rotation = leftRot;
                __instance.NetVRRightHandRot = rightRot;
                character.RightHand.transform.rotation = rightRot;
                __instance.LastVRNetUpdateTime = Time.time;
            }
            return false;
        }
    }
}
