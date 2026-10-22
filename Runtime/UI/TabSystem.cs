using System;
using UnityEngine;
using UnityEngine.UI;
using MyTools.UnityEvents;

namespace MyTools.UI
{
    public class TabSystem : MonoBehaviour
    {
        // Editor variables
        public int defaultActiveIndex = 0;
        public TabPair[] tabs;
        public bool buttonsReadonly = false;
        public IntEvent OnTabChanged = new();

        // Runtime variables
        public int currentTabIndex { get; private set; }

        private void OnValidate()
        {
            if (tabs == null || tabs.Length == 0)
            {
                return;
            }
            defaultActiveIndex = Mathf.Clamp(defaultActiveIndex, 0, tabs.Length - 1);
        }

        private void Start()
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                var tab = tabs[i];
                var btn = tab.button;
                int idx = i;
                if (!buttonsReadonly)
                {
                    tab.button.onClick.AddListener(() => SwitchTab(idx));
                }
                if (i == defaultActiveIndex)
                {
                    tab.content.SetActive(true);
                    btn.interactable = false;
                    currentTabIndex = i;
                }
                else
                {
                    tab.content.SetActive(false);
                    btn.interactable = true;
                }
            }
        }

        public void SwitchTab(int index)
        {
            if (index < 0 || index >= tabs.Length)
            {
                Debug.LogError("Tab index out of range.");
                return;
            }
            for (int i = 0; i < tabs.Length; i++)
            {
                var tab = tabs[i];
                var btn = tab.button;
                if (i == index)
                {
                    tab.content.SetActive(true);
                    btn.interactable = false;
                    currentTabIndex = i;
                }
                else
                {
                    tab.content.SetActive(false);
                    btn.interactable = true;
                }
            }
            OnTabChanged.Invoke(index);
        }

        public void NextTab() => SwitchTab(currentTabIndex + 1);
        public void PreviousTab() => SwitchTab(currentTabIndex - 1);
    }

    /// <summary>
    /// Represents a pair of a button and its associated content.
    /// </summary>
    /// <remarks>This class is typically used to associate a UI button with a corresponding content panel or
    /// object.</remarks>
    [Serializable]
    public class TabPair
    {
        public Button button;
        public GameObject content;
    }
}