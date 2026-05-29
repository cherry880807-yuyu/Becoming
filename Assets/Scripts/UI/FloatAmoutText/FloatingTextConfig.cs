using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/FloatingTextConfig")]
public class FloatingTextConfig : ScriptableObject
{
    public float duration = 0.8f;
    public float floatHeight = 1.2f;
    public float normalFontSize = 4f;
    public Color normalColor = Color.white;
    public Color damageColor = new Color(1f, 0.4f, 0f);
    public Color healColor = new Color(0.2f, 1f, 0.4f);
}
