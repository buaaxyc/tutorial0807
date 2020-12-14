using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using VRKeys;

namespace Com.SJTU.PhotonVRKeys
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        public static GameManager Instance;

        [Tooltip("The prefab to use for representing the user")]
        public GameObject VRKeys;
        public GameObject Avatar;

        #endregion

        #region Private Fields

        private GameObject vrkeys_instance;
        private GameObject avatar_instance;

        #endregion

        #region MonoBehaviour CallBacks

        void Start()
        {
            Instance = this;

            if (!VRKeys || !Avatar)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> userPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                Debug.LogFormat("We are Instantiating LocalUser");
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                vrkeys_instance = PhotonNetwork.Instantiate(this.VRKeys.name, new Vector3(-0.53f, -1.945f, 1.04f), Quaternion.identity, 0);
                avatar_instance = PhotonNetwork.Instantiate(this.Avatar.name, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, 0);
                //PhotonNetwork.Instantiate(this.VRKeys.name, new Vector3(10f, 0f, 0f), Quaternion.identity, 0);
                //vrkeys_instance.name += " of " + vrkeys_instance.GetComponent<DemoScene>().photonView.Owner.NickName;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                LeaveRoom();
            }
        }

        #endregion

        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            Destroy(vrkeys_instance);
            Destroy(avatar_instance);
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            GameObject[] userPrefabs = GameObject.FindGameObjectsWithTag("Player");
            if (userPrefabs != null && userPrefabs.Length > 0)
            {
                //Debug.Log("userPrefabs.Length = " + userPrefabs.Length);

                foreach (GameObject userPrefab in userPrefabs)
                {
                    if (userPrefab.GetComponent<DemoScene>().photonView.IsMine)
                    {
                        // Network user, receive data
                        userPrefab.GetComponent<DemoScene>().setIsUpdating(true); // seems that userPrefabs[0] always isMine??
                    }
                }
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            Debug.Log("GameManager: LeaveRoom() was called");
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}