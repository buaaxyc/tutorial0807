using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSync0 : MonoBehaviour
{

    private GameObject ovrcamera;
    public GameObject avatarHead;
    public GameObject avatarBody;

    // Start is called before the first frame update
    void Start()
    {
        ovrcamera = GameObject.Find("CenterEyeAnchor");
    }

    // Update is called once per frame
    void Update()
    {
        avatarHead.transform.rotation = ovrcamera.transform.rotation;
        avatarBody.transform.position = ovrcamera.transform.position + new Vector3(0.0f, -0.66f, -0.05f);
    }
}
