using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Com.SJTU.XYCsGame
{
    /// <summary>
    /// Player name input field. Let the user input his name, will appear above the player in the game.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants

        // Store the PlayerPref Key to avoid typos
        const string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)  // Is this checking necessary while using the above statement "[RequireComponent(typeof(InputField))]"??
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);  // Where does the Object "PlayerPrefs" save in??
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// We need to bind the InputField OnValueChange() to call SetPlayerName() so that every time the user is editing the InputField, we record it.
        /// (We could do this only when the user is pressing play, this is up to you.)
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)  // Who calls this method??
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;

            // the value has been stored locally on the user device for later retrieval (the next time the user will open this game)
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }
}