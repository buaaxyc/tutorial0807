using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;
using TMPro;

using VRKeys;

namespace Com.SJTU.PhotonVRKeys
{
    public class HiddenRegion : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("The hidden region instance. Use this to know if the hidden region is represented in the Scene")]
        public static GameObject HiddenRegionInstance;

        /// <summary>
        /// Reference to the VRKeys keyboard.
        /// </summary>
        public Keyboard keyboard;
        public GameObject VRKeys;

        #endregion

        #region Private Fields

        private string inputText;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine)
            {
                HiddenRegion.HiddenRegionInstance = this.gameObject;
            }

            //this.gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        public void SetText(string txt)
        {
            inputText = txt;
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                //Debug.Log("'stream.SendNext(inputText)' in HiddenRegion.cs");
                stream.SendNext(inputText);
            }
            else
            {
                //inputText = (string)stream.ReceiveNext();
                if (photonView.IsMine)
                {
                    if (!keyboard.text.Equals(inputText))
                    {
                        //Debug.Log("'keyboard.SetText((string)stream.ReceiveNext())' in HiddenRegion.cs");
                        keyboard.SetText((string)stream.ReceiveNext());
                    }
                }
            }
        }

        #endregion
    }
}