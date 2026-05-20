using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Mutation Data")]
//成就及相關資料
public class MutationDataSO : ScriptableObject
{
    public string mutationID;
    public string mutationName;
    public Sprite icon;

    //成就條件
    public List<MutationCondition> conditions;
    //成就獎勵
    public List<MutationEffect> effects;
}