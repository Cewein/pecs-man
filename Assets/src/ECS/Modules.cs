using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public struct Edible : ECS.IComponent
    {
        bool active;
    }

    public struct FollowTarget : ECS.IComponent
    {
        GameObject target;
        float speed;
    }

    public struct TargetEdible : ECS.IComponent
    {
        GameObject target;
        float speed;
    }

    public struct Score : ECS.IComponent
    {
        int score;
    }
}