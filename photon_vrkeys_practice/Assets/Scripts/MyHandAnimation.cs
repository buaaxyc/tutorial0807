using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyHandAnimation : MonoBehaviour
{

    private Animator _anim;
    private MyHandGrabbing _handGrab;

    // Use this for initialization
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _handGrab = GetComponent<MyHandGrabbing>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis(_handGrab.ThumbTouch) >= 0.01f && Input.GetAxis(_handGrab.IndexFingerTouch) < 0.01f && Input.GetAxis(_handGrab.MiddleFingerPress) < 0.01f)
        {
            _anim.SetBool("IsThumbTouching", true);
            _anim.SetBool("IsIndexTouching", false);
            _anim.SetBool("IsMiddlePressing", false);
            _anim.SetBool("IsGrabbing", false);
        }
        else if (Input.GetAxis(_handGrab.ThumbTouch) < 0.01f && Input.GetAxis(_handGrab.IndexFingerTouch) >= 0.01f && Input.GetAxis(_handGrab.MiddleFingerPress) < 0.01f)
        {
            _anim.SetBool("IsThumbTouching", false);
            _anim.SetBool("IsIndexTouching", true);
            _anim.SetBool("IsMiddlePressing", false);
            _anim.SetBool("IsGrabbing", false);
        }
        else if (Input.GetAxis(_handGrab.ThumbTouch) < 0.01f && Input.GetAxis(_handGrab.IndexFingerTouch) < 0.01f && Input.GetAxis(_handGrab.MiddleFingerPress) >= 0.01f)
        {
            _anim.SetBool("IsThumbTouching", false);
            _anim.SetBool("IsIndexTouching", false);
            _anim.SetBool("IsMiddlePressing", true);
            _anim.SetBool("IsGrabbing", false);
        }
        else if (Input.GetAxis(_handGrab.ThumbTouch) >= 0.01f && Input.GetAxis(_handGrab.IndexFingerTouch) >= 0.01f && Input.GetAxis(_handGrab.MiddleFingerPress) < 0.01f)
        {
            _anim.SetBool("IsThumbTouching", true);
            _anim.SetBool("IsIndexTouching", true);
            _anim.SetBool("IsMiddlePressing", false);
            _anim.SetBool("IsGrabbing", false);
        }
        else if (Input.GetAxis(_handGrab.ThumbTouch) >= 0.01f && Input.GetAxis(_handGrab.IndexFingerTouch) < 0.01f && Input.GetAxis(_handGrab.MiddleFingerPress) >= 0.01f)
        {
            _anim.SetBool("IsThumbTouching", true);
            _anim.SetBool("IsIndexTouching", false);
            _anim.SetBool("IsMiddlePressing", true);
            _anim.SetBool("IsGrabbing", false);
        }
        else if (Input.GetAxis(_handGrab.ThumbTouch) < 0.01f && Input.GetAxis(_handGrab.IndexFingerTouch) >= 0.01f && Input.GetAxis(_handGrab.MiddleFingerPress) >= 0.01f)
        {
            _anim.SetBool("IsThumbTouching", false);
            _anim.SetBool("IsIndexTouching", true);
            _anim.SetBool("IsMiddlePressing", true);
            _anim.SetBool("IsGrabbing", false);
        }
        //if we are pressing grab, set animator bool IsGrabbing to true
        else if (Input.GetAxis(_handGrab.ThumbTouch) >= 0.01f && Input.GetAxis(_handGrab.IndexFingerTouch) >= 0.01f && Input.GetAxis(_handGrab.MiddleFingerPress) >= 0.01f)
        {
            _anim.SetBool("IsThumbTouching", false);
            _anim.SetBool("IsIndexTouching", false);
            _anim.SetBool("IsMiddlePressing", false);
            _anim.SetBool("IsGrabbing", true);
        }
        else
        {
            _anim.SetBool("IsThumbTouching", false);
            _anim.SetBool("IsIndexTouching", false);
            _anim.SetBool("IsMiddlePressing", false);
            _anim.SetBool("IsGrabbing", false);
        }

    }
}
