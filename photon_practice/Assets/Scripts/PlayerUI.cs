using UnityEngine;
using UnityEngine.UI;

namespace Com.SJTU.XYCsGame
{
    public class PlayerUI : MonoBehaviour
    {
        #region Private Fields

        [Tooltip("UI Text to display Player's Name")]
        [SerializeField]
        private Text playerNameText;

        [Tooltip("UI Slider to display Player's Health")]
        [SerializeField]
        private Slider playerHealthSlider;

        // to acquire player's name(target.photonView.Owner.NickName) and health(target.health)
        private PlayerManager target;

        // to make the ui follow the target player
        float characterControllerHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup _canvasGroup;
        Vector3 targetPosition;

        #endregion

        #region Public Fields

        [Tooltip("Pixel offset from the player target")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            // Parenting To UI Canvas
            // Why going brute force and find the Canvas this way? Because when scenes are going to be loaded and unloaded, so is our Prefab, and the Canvas will be everytime different. To avoid more complex code structure, we'll go for the quickest way. However it's really not recommended to use "Find", because this is a slow operation.
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false); // static method GameObject.Find(string) also used in MyHeadRotationCtrl.cs line 79 in "oculus_practice"

            // Following The Target Player
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }

        void Update()
        {
            // Reflect the Player health
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = target.health;
            }

            // destroy playerUI instance manually when the player has left
            // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network (Photon wouldn't destroy playerUI instance at the same time??)
            if (target == null)
            {
                Destroy(this.gameObject); // compare with "DontDestroyOnLoad(this.gameObject)" in PlayerManager.cs
                return;
            }
        }

        // Following The Target Player
        void LateUpdate()
        {
            // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
            if (targetRenderer != null)
            {
                // modify the UI transparency according to the visibility of the player target
                this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }

            // #Critical
            // Follow the Target GameObject on screen. The trick to match a 2D position with a 3D position is to use the WorldToScreenPoint function of a camera.
            // Notice how we setup the offset in several steps: first we get the actual position of the target, then we add the characterControllerHeight, and finally, after we've deduced the screen position of the top of the Player, we add the screen offset.
            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }

        #endregion

        #region Public Methods

        public void SetTarget(PlayerManager _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayerManager target for PlayerUI.SetTarget.", this);
                return; // necessary??
            }

            // We need to think ahead here, we'll be looking up for the health regularly, so it make sense to cache a reference(property 'target') of the PlayerManager for efficiency.
            // Cache references for efficiency
            target = _target;

            if (playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName; // PhotonNetwork.NickName??
            }

            // Following The Target Player
            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponent<Renderer>();
            CharacterController characterController = _target.GetComponent<CharacterController>();

            // Get data from the Player that won't change during the lifetime of this Component
            if (characterController != null)
            {
                characterControllerHeight = characterController.height; // 2f
            }
        }

        #endregion
    }
}