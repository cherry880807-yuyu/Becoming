using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData : MonoBehaviour
{

}

//成就類型
public struct AttackThreeTimesEvent
{
    public int attackCount;
}



public struct MutationUnlockedEvent
{
    public MutationDataSO mutation;
}