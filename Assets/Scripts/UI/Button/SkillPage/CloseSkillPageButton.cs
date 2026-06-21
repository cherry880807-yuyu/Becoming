using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseSkillPageButton : BaseButton
{
    [SerializeField] private GameObject skillPage;
    protected override void OnClick()
    {
        UIManager.Instance.HideUI(skillPage);
    }

}
