using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.SJTU.PhotonVRKeys
{
    public class UserPrefab : MonoBehaviour
    {
        public static GameObject userPrefabInstance;

        private void Start()
        {
            userPrefabInstance = this.gameObject;
        }
    }
}