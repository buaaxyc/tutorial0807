using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.SJTU.XYCsGame
{
    // Why do we make a prefab out of this? Because our game requirement implies several scenes for the same game, and so we'll need to reuse this Game Manager.
    // In Unity the best way to reuse GameObjects is to turn them into prefabs.
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        // Duplicated code for the same result is something that you should avoid at all costs. This will also be a good time to introduce a very handy programming concept, "Singleton".
        // Notice we've decorated the Instance variable with the [static] keyword, meaning that this variable is available without having to hold a pointer to an instance of GameManager, so you can simply do GameManager.Instance.xxx() from anywhere in your code. It's very practical indeed!
        public static GameManager Instance;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        #endregion

        #region MonoBehaviour CallBacks

        void Start()
        {
            // Singleton assignment
            Instance = this;

            // Instantiate the player
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);

                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    // Notice that we instantiate well above the floor (5 units above while the player is only 2 units high). This is one way amongst many other to prevent collisions when new players join the room, players could be already moving around the center of the arena, and so it avoids abrupt collisions. A "falling" player is also a nice and clean indication and introduction of a new entity in the game.
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        #endregion

        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                // When current client is Master Client, load level.
                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                // When current client is Master Client, load level.
                // What if Master Client disconnects, will PUN select other players randomly as the master??
                LoadArena(); // the Method will check again whether this is Master Client.
            }
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom(); // where to destroy/delete the player instance, by Photon automatically??
        }

        #endregion

        #region Private Methods

        void LoadArena()
        {
            // PhotonNetwork.LoadLevel() should only be called if we are the MasterClient. So we check first that we are the MasterClient using PhotonNetwork.IsMasterClient.
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");

                // Why not add "return;", will Debug.LogError() break off the process??
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

            // We use PhotonNetwork.LoadLevel() to load the level we want, we don't use Unity directly, because we want to rely on Photon to load this level on all connected clients in the room, since we've enabled PhotonNetwork.AutomaticallySyncScene for this Game.
            // This is a very effective technique known as "convention over configuration".
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        #endregion
    }
}