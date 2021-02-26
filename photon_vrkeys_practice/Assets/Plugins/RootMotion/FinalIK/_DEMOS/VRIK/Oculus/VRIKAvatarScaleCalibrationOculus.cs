using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

namespace RootMotion.Demos
{
    // Simple avatar scale calibration.
    public class VRIKAvatarScaleCalibrationOculus : MonoBehaviour
    {
        public VRIK ik;
        public float scaleMlp = 1f;

        private void Start()
        {
            if (!ik.solver.spine.headTarget)
                ik.solver.spine.headTarget = GameObject.Find("Head Target").transform;
            if (!ik.solver.leftArm.target)
                ik.solver.leftArm.target = GameObject.Find("Left Hand Target").transform;
            if (!ik.solver.rightArm.target)
                ik.solver.rightArm.target = GameObject.Find("Right Hand Target").transform;
        }

        private void LateUpdate()
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                // Compare the height of the head target to the height of the head bone, multiply scale by that value.
                float sizeF = (ik.solver.spine.headTarget.position.y - ik.references.root.position.y) / (ik.references.head.position.y - ik.references.root.position.y);
                ik.references.root.localScale *= sizeF * scaleMlp;
            }
        }

    }
}
