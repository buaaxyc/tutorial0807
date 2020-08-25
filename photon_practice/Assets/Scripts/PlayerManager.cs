using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

namespace Com.SJTU.XYCsGame
{
    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Private Fields

        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;

        //True, when the user is firing
        bool IsFiring; // Uppercase??

        #endregion

        #region Public Fields

        // For easy debugging we made the Health float a public float so that it's easy to check its value while we are waiting for the UI to be built.
        [Tooltip("The current Health of our player")]
        public float health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;

        #endregion

        // Fire Input Synchronization 
        // 3 syncs in "Photon View": transform, animator, fire&health;
        // 3 ctrls: animation input, fire input, camera.
        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // In this method, we get a variable stream, this is what is going to be send over the network, and this call is our chance to read and write data.

            // We can only write when we are the local player (photonView.IsMine == true), else we read. Since the stream class has helpers to know what to do, we simply rely on stream.isWriting to know what is expected in the current instance case.
            if (stream.IsWriting)
            {
                // We own this player(photonView.IsMine == true): send the others our data
                stream.SendNext(IsFiring); // a very convenient method that hides away all the hard work of data serialization
                stream.SendNext(health);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
                this.health = (float)stream.ReceiveNext();

                //then do Update()
            }
        }

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // Hide the Beams
            if (beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }

            // Keep Track Of The Player Instance
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }

            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            //Camera Control
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                //  if photonView.IsMine is true, it means we need to follow this instance, and so we call _cameraWork.OnStartFollowing() which effectivly makes the camera follow that very instance in the scene.
                // All other player instances will have their photonView.IsMine set as false, and so their respective _cameraWork will do nothing.
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }

#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded; // '+' means register, '-' means delete(haven't seen this grammar before)??
#endif

            // instantiate PlayerUI prefab for the first time (this Player prefab is instantiated in Start() of GameManager.cs)
            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab); // (Object.)Instantiate() or PhotonNetwork.Instantiate() in GameManager.cs line43??

                // call SetTarget(PlayerManager) of PlayerUI.cs, send sup object 'PlayerManager' to the sub instance we've just created
                // Another way would have been to get the PlayerUI component from the instance, and then call SetTarget directly. It's generally recommended to use components directly, but it's also good to know you can achieve the same thing in various ways.
                // _uiGo.gameObject.GetComponent<PlayerUI>().SetTarget(this);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver); // We require a receiver, which means we will be alerted if the SetTarget did not find a component to respond to it.
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

            // check our health every 0.5s
            if (photonView.IsMine)
            {
                //Debug.LogWarning("test 1");

                InvokeRepeating("HealthCheckingTimer", 0f, 0.5f); // compare with gameObject.SendMessage(), both use method name to call
            }
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {
            if (photonView.IsMine)
            {
                // Fire Input Control
                ProcessInputs();

                // The below code will be run for multiple times since our health <= 0f and then log error in the console, so we use InvokeRepeating() in Start()/Awake() to check our health every 0.5s.
                /*if (health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }*/
            }

            // trigger Beams active state
            if (beams != null && IsFiring != beams.activeInHierarchy) // if IsFiring doesn't equal to beams' state, change beams' state.
            {
                beams.SetActive(IsFiring);
            }
        }

        /// <summary>
        /// MonoBehaviour method called when the Collider 'other' enters the trigger.
        /// Affect Health of the Player if the collider is a beam
        /// Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
        /// One could move the collider further away to prevent this or check if the beam belongs to the player.
        /// In contrast, in project "oculus_practice" - "ColorController.cs", we use the method OnCollisionEnter() & OnCollisionExit() to callback.
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            // we dont' do anything if we are not the local player.
            // it means every client needs to process every collision event, but not processed in the server??
            if (!photonView.IsMine)
            {
                return;
            }

            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }

            health -= 0.1f;
        }

        /// <summary>
        /// MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
        /// We're going to affect health while the beams are touching the player
        /// </summary>
        /// <param name="other">Other.</param>
        void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (!other.name.Contains("Beam"))
            {
                return;
            }

            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
            // we decrement the health using Time.deltaTime during TriggerStay for the speed of decrement not to be dependant on the frame rate.
            // This is an important concept that usually applies to animation, but here, we need this too, we want the health to decrease in a predictable way across all devices, it would not be fair that on a faster computer, your health decreases faster :) Time.deltaTime is here to guarantee consistency. 
            health -= 0.1f * Time.deltaTime;
        }

        // Manage Player Position When Outside The Arena
        // New version sequence: In Start(), register and then callback OnSceneLoaded() -> call CalledOnLevelWasLoaded() to manage Player Position -> In OnDisable(), delete OnSceneLoaded()
        // Old version sequence: callback OnLevelWasLoaded()(not available in v5.4 or newer) -> manage Player Position
#if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif

        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f)) // raycast downwards the current player's position to see if we hit anything,if not(i.e. we are not above the arena's ground) then do the following code to reposition
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            // instantiate PlayerUI prefab when a new level is loaded and the previous PlayerUI is being destroyed
            // Why the UI is destroyed(target==null) yet player remains?? Can we use "DontDestroyOnLoad(this.gameObject)" in Awake() of PlayerUI.cs as an alternate??
            // the below code can also be replaced by calling a private method to avoid code duplication
            GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

#if UNITY_5_4_OR_NEWER
        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
#endif

        #endregion

        #region Custom

        /// <summary>
        /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
        /// </summary>
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1")) // Where is 'Fire2' defined(including the left 'ctrl' key and the left mouse button)??
            {
                if (!IsFiring)
                {
                    IsFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (IsFiring)
                {
                    IsFiring = false;
                }
            }
        }

        void HealthCheckingTimer()
        {
            //Debug.LogWarning("test 2: " + Time.time);

            if (health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
        }

        #endregion

        #region Private Methods

#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif

        #endregion
    }
}