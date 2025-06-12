using UnityEngine;

namespace Akila.FPSFramework.Animation
{
    public class MoveAnimationModifier : ProceduralAnimationModifier
    {
        public Vector3 position;
        public Vector3 rotation;

        public Vector3 defaultPosition;
        public Vector3 defaultRotation;

        public Vector3 offset = new Vector3(0,0,0.1f);


        protected void Update()
        {
            targetPosition = Vector3.Lerp(defaultPosition, position + offset, targetAnimation.progress);
            targetRotation = Vector3.Lerp(defaultRotation, rotation, targetAnimation.progress);
        }
    }
}