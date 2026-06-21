using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSkillPageButton : BaseButton
{
    [SerializeField] private GameObject skillPage;
    protected override void OnClick()
    {
        UIManager.Instance.ShowUI(skillPage);
    }
}
