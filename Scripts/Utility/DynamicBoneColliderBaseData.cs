﻿using UnityEngine;

namespace ReverieSDK
{
    public class DynamicBoneColliderBaseData : MonoBehaviour
    {
        public enum Direction
        {
            X, Y, Z
        }
        public Direction m_Direction = Direction.Y;
        public Vector3 m_Center = Vector3.zero;
        public enum Bound
        {
            Outside,
            Inside
        }
        public Bound m_Bound = Bound.Outside;
    }
}