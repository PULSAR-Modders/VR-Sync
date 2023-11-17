using PulsarModLoader;
using PulsarModLoader.CustomGUI;
using UnityEngine;

namespace VR_Sync
{
    public class Mod : PulsarMod
    {
        public override string HarmonyIdentifier() => "Mest.VR-Sync";
        public override string Name => "VR-Sync";
        public override string Author => "Mest";
        public override string Version => "0.0.2";
        public override string ShortDescription => "Syncs VR movements over the network.";
    }
    public class Config : ModSettingsMenu
    {
        public override void Draw()
        {
            if (Character.Objects == null || PLNetworkManager.Instance == null || PLNetworkManager.Instance.MyLocalPawn == null || !Character.Objects.ContainsKey(PLNetworkManager.Instance.MyLocalPawn)) return;
            Character character = Character.Objects[PLNetworkManager.Instance.MyLocalPawn];
            if (character == null) return;
            Vector3 Head = ChangePosition(character.Head.transform.position);
            Vector3 RightHand = ChangePosition(character.RightHand.transform.position);
            Vector3 LeftHand = ChangePosition(character.LeftHand.transform.position);
            Quaternion HeadRot = Quaternion.Euler(ChangePosition(character.Head.transform.rotation.eulerAngles));
            Quaternion RightHandRot = Quaternion.Euler(ChangePosition(character.RightHand.transform.rotation.eulerAngles));
            Quaternion LeftHandRot = Quaternion.Euler(ChangePosition(character.LeftHand.transform.rotation.eulerAngles));
            PLServer.Instance.SendUnreliableVRHandPosUpdateToOthers(PLNetworkManager.Instance.MyLocalPawn.CombatTargetID, LeftHand, RightHand, Head, LeftHandRot, RightHandRot, HeadRot);
        }
        public static Vector3 ChangePosition(Vector3 original)
        {
            Vector3 ret = new Vector3();
            GUILayout.BeginHorizontal();
            GUILayout.Label("x");
            ret.x = GUILayout.HorizontalSlider(original.x, original.x - 10, original.x + 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("y");
            ret.y = GUILayout.HorizontalSlider(original.y, original.y - 10, original.y + 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("z");
            ret.z = GUILayout.HorizontalSlider(original.z, original.z - 10, original.z + 10);
            GUILayout.EndHorizontal();
            return ret;
        }

        public override string Name() => "VR-Sync Dev Config";
    }
}
