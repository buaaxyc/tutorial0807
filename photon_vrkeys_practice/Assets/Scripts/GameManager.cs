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
        public GameObject userPrefab;

        #endregion

        #region Private Fields

        private GameObject localInstance;

        #endregion

        #region MonoBehaviour CallBacks

        void Start()
        {
            Instance = this;

            if (userPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> userPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                Debug.LogFormat("We are Instantiating LocalUser");
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                localInstance = PhotonNetwork.Instantiate(this.userPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                //PhotonNetwork.Instantiate(this.VRKeys.name, new Vector3(10f, 0f, 0f), Quaternion.identity, 0);
                //localInstance.name += " of " + localInstance.GetComponent<DemoScene>().photonView.Owner.NickName;
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
            Destroy(localInstance);
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