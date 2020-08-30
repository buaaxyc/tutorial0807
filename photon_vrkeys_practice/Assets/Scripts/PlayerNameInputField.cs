using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using TMPro;

namespace Com.SJTU.PhotonVRKeys
{
    /// <summary>
    /// Player name input field. Let the user input his name, will appear above the player in the game.
    /// Using PlayerPrefs to remember the value of the Name so that when the user open the game again, we can recover what was the name. It's a very handy and quite important feature to implement in many areas of your game for a great User Experience.
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants

        // Store the PlayerPref Key to avoid typos
        const string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        void Start()
        {
            string defaultName = string.Empty;
            TMP_InputField _inputField = this.GetComponent<TMP_InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName; // This is main point of this script, setting up the name of the player over the network.
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// We need to bind the InputField OnValueChange() to call SetPlayerName() so that every time the user is editing the InputField, we record it.
        /// We could do this only when the user is pressing play, this is up to you, however this is a bit more involving script wise, so let's keep it simple for the sake of clarity. It also means that no matter what the user will do, the input will be remembered, which is often the desired behavior.
        /// </summary>
        /// <param name="value"></param>
        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }

            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }
}