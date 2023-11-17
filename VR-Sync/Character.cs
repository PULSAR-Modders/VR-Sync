using System.Collections.Generic;
using UnityEngine;

namespace VR_Sync
{
    internal class Character
    {
        public static Dictionary<PLPawn, Character> Objects;
        public GameObject Head;
        public GameObject RightHand;
        public GameObject LeftHand;
    }
}
