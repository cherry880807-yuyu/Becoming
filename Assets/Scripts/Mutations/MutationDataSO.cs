using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum SkillType// Mutation 主分類
{
    Passive,
    Active,
    WeaponMembrane
}
public enum SubCategory // Mutation 副分類
{
    Movement,
    Jump,
    Survival,
    Combat,

    Charge,
    Lunge,
    Aerial,
    Enhance,
    Special
}



[CreateAssetMenu(menuName = "Mutation/Mutation Data")]
public class MutationDataSO : ScriptableObject
{
    public string mutationID;
    public string mutationName;
    public Sprite icon;
    public SkillType category;
    public SubCategory subCategory;
    public WeaponFamilySO mutationType;
    [Min(1)] public int selectionLimit = 1;
    [TextArea] public string lockedDescription;
    [TextArea] public string unlockedDescription;

    public List<MutationCondition> conditions;
    public List<MutationEffect> effects;

    public bool IsPassive => category == SkillType.Passive;
    public bool RequiresSelection => category != SkillType.Passive;


}
