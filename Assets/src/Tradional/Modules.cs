using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public struct Edible : ECS.IComponent
    {
        public bool active;
    }

    public struct FollowTarget : ECS.IComponent
    {
        public GameObject target;
        public float speed;
    }

    public struct TargetEdible : ECS.IComponent
    {
        public GameObject target;
        public float speed;
    }

    public struct Score : ECS.IComponent
    {
        public int score;
    }
}