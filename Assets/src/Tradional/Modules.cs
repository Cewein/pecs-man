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
    }

    public struct TargetEdible : ECS.IComponent
    {
        public GameObject target;
    }

    public struct Score : ECS.IComponent
    {
        public int score;
        public bool isDead;
        public int number;

        public Score(int number, int score, bool isDead)
        {
            this.number = number;
            this.score = score;
            this.isDead = isDead;
        }
    }
}