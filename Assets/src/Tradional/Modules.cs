using System;
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
        public bool asBeenCalmDown;

        public FollowTarget(GameObject target, bool asBeenCalmDown)
        {
            this.target = target;
            this.asBeenCalmDown = asBeenCalmDown;
        }
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

    public struct Vendetta : ECS.IComponent
    {
        public bool wantToDoVandetta;
        public GameObject target;

        public Vendetta(bool wantToDoVandetta, GameObject target)
        {
            this.wantToDoVandetta = wantToDoVandetta;
            this.target = target;
        }
    }
}