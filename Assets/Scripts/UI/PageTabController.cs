using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PageTabController : MonoBehaviour
{
    [Serializable]
    protected class PageTab
    {
        public string tabId = "Page";
        public GameObject page = null;
        public Button button = null;
        [NonSerialized] public UnityAction onClickAction;
    }

    [SerializeField] protected string defaultTabId = "Page";
    [SerializeField] protected Color selectedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] protected Color unselectedColor = Color.white;
    [SerializeField] protected List<PageTab> tabs = new();

    protected string CurrentTabId { get; private set; }

    protected virtual void Awake()
    {
        RegisterButtons();
        Show(defaultTabId);
    }

    protected virtual void OnDestroy()
    {
        UnregisterButtons();
    }

    public void Show(string tabId)
    {
        CurrentTabId = tabId;

        foreach (PageTab tab in tabs)
        {
            if (tab == null) continue;
            bool isSelected = tab.tabId == CurrentTabId;
            if (tab.page != null) tab.page.SetActive(isSelected);
            if (tab.button != null && tab.button.image != null) tab.button.image.color = isSelected ? selectedColor : unselectedColor;
        }
    }

    private void RegisterButtons()
    {
        foreach (PageTab tab in tabs)
        {
            if (tab == null || tab.button == null) continue;

            string tabId = tab.tabId;
            tab.onClickAction = () => Show(tabId);
            tab.button.onClick.AddListener(tab.onClickAction);
        }
    }

    private void UnregisterButtons()
    {
        foreach (PageTab tab in tabs)
        {
            if (tab == null || tab.button == null || tab.onClickAction == null) continue;
            tab.button.onClick.RemoveListener(tab.onClickAction);
            tab.onClickAction = null;
        }
    }
}
