using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR; //needs to be UnityEngine.VR in Versions before 2017.2

public class MyHandGrabbing : MonoBehaviour
{

    //public string InputName;
    public string ThumbTouch;
    public string IndexFingerTouch;
    public string IndexFingerPress;
    public string MiddleFingerPress;
    public MyHandGrabbing OtherHandReference;
    public XRNode NodeType;
    public Vector3 ObjectGrabOffset;
    public float GrabDistance = 0.1f;
    public string GrabTag = "Grab";
    public float ThrowMultiplier = 1.5f;

    public Transform CurrentGrabObject
    {
        get { return _currentGrabObject; }
        set { _currentGrabObject = value; }
    }

    private Vector3 _lastFramePosition;
    private Transform _currentGrabObject;
    private bool _isGrabbing;

    //private string Js09 = "Js09", Js10 = "Js10", Js11 = "Js11", Js12 = "Js12", Js13 = "Js13", Js14 = "Js14", Js15 = "Js15", Js16 = "Js16";

    // Use this for initialization
    void Start()
    {
        _lastFramePosition = transform.position;

        XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);

        _currentGrabObject = null;

        _isGrabbing = false;

    }

    // Update is called once per frame
    void Update()
    {
        //update hand position and rotation
        transform.position = InputTracking.GetLocalPosition(NodeType);
        transform.rotation = InputTracking.GetLocalRotation(NodeType);


        //if we don't have an active object in hand, look if there is one in proximity
        if (_currentGrabObject == null)
        {
            //check for colliders in proximity
            Collider[] colliders = Physics.OverlapSphere(transform.position, GrabDistance);
            if (colliders.Length > 0)
            {
                //if there are colliders, take the first one if we press the grab button and it has the tag for grabbing
                if (Input.GetAxis(ThumbTouch) >= 0.01f && Input.GetAxis(IndexFingerTouch) >= 0.01f && Input.GetAxis(MiddleFingerPress) >= 0.01f && colliders[0].transform.CompareTag(GrabTag))
                {
                    //if we are already grabbing, return
                    if (_isGrabbing)
                    {
                        return;
                    }
                    _isGrabbing = true;

                    //set current object to the object we have picked up (set it as child)
                    colliders[0].transform.SetParent(transform);

                    //if there is no rigidbody to the grabbed object attached, add one
                    if (colliders[0].GetComponent<Rigidbody>() == null)
                    {
                        colliders[0].gameObject.AddComponent<Rigidbody>();
                    }

                    //set grab object to kinematic (disable physics)
                    colliders[0].GetComponent<Rigidbody>().isKinematic = true;


                    //save a reference to grab object
                    _currentGrabObject = colliders[0].transform;

                    //does other hand currently grab object? then release it!
                    if (OtherHandReference.CurrentGrabObject != null)
                    {
                        OtherHandReference.CurrentGrabObject = null;
                    }



                }
            }
        }
        else
        //we have object in hand, update its position with the current hand position (+defined offset from it)
        {

            //if we we release grab button, release current object
            if (Input.GetAxis(ThumbTouch) < 0.01f || Input.GetAxis(IndexFingerTouch) < 0.01f || Input.GetAxis(MiddleFingerPress) < 0.01f)
            {


                //set grab object to non-kinematic (enable physics)
                Rigidbody _objectRGB = _currentGrabObject.GetComponent<Rigidbody>();
                _objectRGB.isKinematic = false;
                _objectRGB.collisionDetectionMode = CollisionDetectionMode.Continuous;

                //calculate the hand's current velocity
                Vector3 CurrentVelocity = (transform.position - _lastFramePosition) / Time.deltaTime;

                //set the grabbed object's velocity to the current velocity of the hand
                _objectRGB.velocity = CurrentVelocity * ThrowMultiplier;

                //release the the object (unparent it)
                _currentGrabObject.SetParent(null);

                //release reference to object
                _currentGrabObject = null;
            }

        }

        //release grab ?
        if (Input.GetAxis(ThumbTouch) < 0.01f || Input.GetAxis(IndexFingerTouch) < 0.01f || Input.GetAxis(MiddleFingerPress) < 0.01f && _isGrabbing)
        {
            _isGrabbing = false;
        }

        //save the current position for calculation of velocity in next frame
        _lastFramePosition = transform.position;


    }
}
