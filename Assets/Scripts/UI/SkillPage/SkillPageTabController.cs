using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillPageTabController : MonoBehaviour
{
    [Serializable]
    private class TabPage
    {
        public SkillType tabType = SkillType.Passive;
        public GameObject page = null;
        public Button button = null;
        [NonSerialized] public UnityAction onClickAction;
    }

    [SerializeField] private SkillType defaultTab = SkillType.Passive;
    [SerializeField] private Color selectedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color unselectedColor = Color.white;
    [SerializeField] private List<TabPage> tabPages = new();
    //TODO 未來有紅點系統的話可以在這裡加一個欄位來紀錄紅點物件，然後在EventBus訂閱相關事件來控制紅點的顯示與隱藏
    //private readonly Dictionary<SkillType, TabPage> tabLookup = new();
    private SkillType currentTab;

    private void Awake()
    {
        //BuildLookup();
        RegisterButtons();
        Show(defaultTab);
    }

    private void OnDestroy()
    {
        UnRegisterButtons();
    }

    public void Show(SkillType tabType)
    {
        currentTab = tabType;

        foreach (var tabPage in tabPages)
        {
            bool isSelected = tabPage.tabType == currentTab;
            if (tabPage.page != null) tabPage.page.SetActive(isSelected);
            if (tabPage.button != null) tabPage.button.image.color = isSelected ? selectedColor : unselectedColor;
        }
    }

    private void RegisterButtons()
    {
        foreach (var tabPage in tabPages)
        {
            if (tabPage.button == null) continue;
            tabPage.onClickAction = () => Show(tabPage.tabType);
            tabPage.button.onClick.AddListener(tabPage.onClickAction);
        }
    }
    private void UnRegisterButtons()
    {
        foreach (var tabPage in tabPages)
        {
             if (tabPage.button != null && tabPage.onClickAction != null) tabPage.button.onClick.RemoveListener(tabPage.onClickAction);
        }
    }

    /* private void BuildLookup()
     {
         tabLookup.Clear();

         foreach (var tabPage in tabPages)
         {
             if (!tabLookup.ContainsKey(tabPage.tabType))
                 tabLookup.Add(tabPage.tabType, tabPage);
         }
     }*/
}
