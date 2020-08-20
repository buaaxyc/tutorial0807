using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.SJTU.XYCsGame
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private Fields

        [SerializeField]
        private float directionDampTime = 0.25f;

        private Animator animator;

        #endregion

        #region MonoBehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();

            // You should always write code as if it's going to be used by someone else :) It's tedious but worth it in the long run.
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Animation Input Control
            // why having then to enforce PhotonNetwork.IsConnected == true in our if statement? eh eh :) because during development, we may want to test this prefab without being connected. 
            // with this additional expression, we will allow input to be used if we are not connected. It's a very simple trick and will greatly improve your workflow during development.
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            if (!animator)
            {
                return;
            }

            // deal with Jumping
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // only allow jumping if we are running.
            if (stateInfo.IsName("Base Layer.Run")) // We simply ask if the current active state of the Animator is Run. We must append Base Layer because the Run state is in the Base Layer.
            {
                // When using trigger parameter
                if (Input.GetButtonDown("Fire2")) // Where is 'Fire2' defined(including the 'alt' key and the right mouse button)??
                {
                    animator.SetTrigger("Jump");
                }
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Since our game does not allow going backward, we make sure that v is less than 0. If the user is pressing the 'down arrow' or 's' key (default setting for the Verticalaxis), we do not allow this and force the value to 0.
            if (v < 0)
            {
                v = 0;
            }

            // The Animation Parameters(e.g. Speed, Direction, Hi, Jump) can be found in the Animator Controller 'Kyle Robot' - Parameters. Also refer to https://docs.unity3d.com/Manual/AnimationParameters.html
            // You'll also notice that we've squared both inputs. Why? So that it's always a positive absolute value as well as adding some easing.
            animator.SetFloat("Speed", h * h + v * v);
            //animator.SetFloat("Speed", Mathf.Abs(h) + Mathf.Abs(v));

            // Damping time makes sense: it's how long it will take to reach the desired value, but deltaTime? It essentially lets you write code that is frame rate independent since Update() is dependent on the frame rate, we need to counter this by using the deltaTime. 
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }

        #endregion
    }
}