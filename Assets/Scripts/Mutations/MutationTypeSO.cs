using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Mutation Type")]
public sealed class WeaponFamilySO : ScriptableObject
{
    public string typeId;
    public string displayName;
    public bool isUniversal;
}
