/**
 * Copyright (c) 2017 The Campfire Union Inc - All Rights Reserved.
 *
 * Licensed under the MIT license. See LICENSE file in the project root for
 * full license information.
 *
 * Email:   info@campfireunion.com
 * Website: https://www.campfireunion.com
 */

using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using Photon.Pun;
using Com.SJTU.PhotonVRKeys;
using Photon.Realtime;

namespace VRKeys
{

    /// <summary>
    /// Example use of VRKeys keyboard.
    /// </summary>
    public class DemoScene : MonoBehaviourPunCallbacks, IPunObservable
    {

        /// <summary>
        /// Reference to the VRKeys keyboard.
        /// </summary>
        public Keyboard keyboard;

        //public GameObject canvasChild;
        //public GameObject keyboardChild;
        //public Key key;

        /// <summary>
        /// See the following for why this is so convoluted:
        /// http://referencesource.microsoft.com/#System.ComponentModel.DataAnnotations/DataAnnotations/EmailAddressAttribute.cs,54
        /// http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx/
        /// </summary>
        private Regex emailValidator = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", RegexOptions.IgnoreCase);

        private bool isUpdating = false;

        //private string receivedText;

        /// <summary>
        /// Show the keyboard with a custom input message. Attaching events dynamically,
        /// but you can also use the inspector.
        /// </summary>
        private void OnEnable()
        {
            // Automatically creating camera here to show how
            /*GameObject camera = new GameObject ("Main Camera");
			Camera cam = camera.AddComponent<Camera> ();
			cam.nearClipPlane = 0.1f;
			camera.AddComponent<AudioListener> ();*/

            // Improves event system performance
            Canvas canvas = keyboard.canvas.GetComponent<Canvas>();
            //canvas.worldCamera = cam;

            keyboard.Enable();
            /*if (!photonView.IsMine)
            {
                canvasChild.SetActive(false);
                keyboardChild.SetActive(false);
            }*/

            keyboard.SetPlaceholderMessage("Please enter your email address");

            keyboard.OnUpdate.AddListener(HandleUpdate);
            keyboard.OnSubmit.AddListener(HandleSubmit);
            keyboard.OnCancel.AddListener(HandleCancel);

            if (!photonView.IsMine)
            {
                canvas.enabled = false;

                Renderer[] renders = this.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer render in renders)
                {
                    render.enabled = false;
                }
            }
        }

        private void OnDisable()
        {
            keyboard.OnUpdate.RemoveListener(HandleUpdate);
            keyboard.OnSubmit.RemoveListener(HandleSubmit);
            keyboard.OnCancel.RemoveListener(HandleCancel);

            keyboard.Disable();
        }

        private void Start()
        {
            /*PhotonNetwork.SerializationRate = 2;
            PhotonNetwork.SendRate = 4;
            if (!photonView.IsMine)
            {
                keyboard.keysParent.gameObject.SetActive(false);
            }*/

            //key = this.GetComponentInChildren<Key>();
            //Debug.Log(key.name);
        }

        /// <summary>
        /// Press space to show/hide the keyboard.
        ///
        /// Press Q for Qwerty keyboard, D for Dvorak keyboard, and F for French keyboard.
        /// </summary>
        private void Update()
        {
            /*if (!photonView.IsMine)
            {
                Renderer[] renders = this.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer render in renders)
                {
                    render.enabled = false;
                }
            }*/

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (keyboard.disabled)
                {
                    Debug.Log("keyboard.Enable() in DemoScene.cs");
                    keyboard.Enable();
                }
                else
                {
                    keyboard.Disable();
                }
            }

            if (keyboard.disabled)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                keyboard.SetLayout(KeyboardLayout.Qwerty);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                keyboard.SetLayout(KeyboardLayout.French);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                keyboard.SetLayout(KeyboardLayout.Dvorak);
            }
        }

        /// <summary>
        /// Hide the validation message on update. Connect this to OnUpdate.
        /// </summary>
        public void HandleUpdate(string text)
        {
            keyboard.HideValidationMessage();

            isUpdating = true;

            //HiddenRegion.HiddenRegionInstance.GetComponent<HiddenRegion>().SetText(keyboard.text);
        }

        /*public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0} in DemoScene.cs", other.NickName); // not seen if you're the player connecting
            isUpdating = true;
        }*/

        /// <summary>
        /// Validate the email and simulate a form submission. Connect this to OnSubmit.
        /// </summary>
        public void HandleSubmit(string text)
        {
            keyboard.DisableInput();

            if (!ValidateEmail(text))
            {
                keyboard.ShowValidationMessage("Please enter a valid email address");
                keyboard.EnableInput();
                return;
            }

            StartCoroutine(SubmitEmail(text));
        }

        public void HandleCancel()
        {
            Debug.Log("Cancelled keyboard input!");
        }

        /// <summary>
        /// Pretend to submit the email before resetting.
        /// </summary>
        private IEnumerator SubmitEmail(string email)
        {
            keyboard.ShowInfoMessage("Sending lots of spam, please wait... ;)");

            yield return new WaitForSeconds(2f);

            keyboard.ShowSuccessMessage("Lots of spam sent to " + email);

            yield return new WaitForSeconds(2f);

            keyboard.HideSuccessMessage();
            keyboard.SetText("");
            keyboard.EnableInput();
        }

        private bool ValidateEmail(string text)
        {
            if (!emailValidator.IsMatch(text))
            {
                return false;
            }
            return true;
        }

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (isUpdating)
                {
                    //Debug.Log("'stream.SendNext(keyboard.text)' in DemoScene.cs by " + this.gameObject.name);
                    Debug.Log("'stream.SendNext(keyboard.text)' in DemoScene.cs");

                    //PhotonNetwork.SerializationRate = 2;
                    //PhotonNetwork.SendRate = 4;

                    // We own this user: send the others our data
                    stream.SendNext(keyboard.text);
                    //stream.SendNext(_placeholder.text);
                    //stream.SendNext(_validationNotice.text);
                    //stream.SendNext(_infoNotice.text);
                    //stream.SendNext(_successNotice.text);

                    isUpdating = false;
                }
            }
            else
            {
                Debug.Log("'stream.ReceiveNext());' in DemoScene.cs");
                string receivedText = (string)stream.ReceiveNext();

                GameObject[] userPrefabs = GameObject.FindGameObjectsWithTag("Player");
                if (userPrefabs != null && userPrefabs.Length > 0)
                {
                    foreach (GameObject userPrefab in userPrefabs)
                    {
                        if (userPrefab.GetComponent<DemoScene>().photonView.IsMine)
                        {
                            // Network user, receive data
                            userPrefab.GetComponentInChildren<Keyboard>().SetText(receivedText, true);
                        }
                    }
                }
            }
        }

        #endregion

        public void setIsUpdating(bool value)
        {
            isUpdating = value;
        }

    }
}