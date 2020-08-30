using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;
using TMPro;

namespace Com.SJTU.PhotonVRKeys
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        public TMP_Text _textInput;
        public TMP_Text _placeholder;
        public TMP_Text _validationNotice;
        public TMP_Text _infoNotice;
        public TMP_Text _successNotice;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {

        }

        void Start()
        {

        }

        void Update()
        {

        }

        #endregion

        #region Custom

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this user: send the others our data
                stream.SendNext(_textInput.text);
                stream.SendNext(_placeholder.text);
                stream.SendNext(_validationNotice.text);
                stream.SendNext(_infoNotice.text);
                stream.SendNext(_successNotice.text);
            }
            else
            {
                // Network user, receive data
                _textInput.text = (string)stream.ReceiveNext();
                _placeholder.text = (string)stream.ReceiveNext();
                _validationNotice.text = (string)stream.ReceiveNext();
                _infoNotice.text = (string)stream.ReceiveNext();
                _successNotice.text = (string)stream.ReceiveNext();
            }
        }

        #endregion
    }
}