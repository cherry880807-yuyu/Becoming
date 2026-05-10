using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : BaseBrain
{
    public ActorData ActorData { get; private set; }

    private void Awake()
    {
        ActorData = BuildActorData();
    }

    protected virtual ActorData BuildActorData()
    {
        ActorData ActorData = new();


        return ActorData;
    }
}