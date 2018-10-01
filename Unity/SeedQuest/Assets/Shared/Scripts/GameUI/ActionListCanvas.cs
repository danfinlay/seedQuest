﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionListCanvas : MonoBehaviour {

    /// <summary> List of current UI ActionItems </summary>
    public List<ActionItem> actionItemList = new List<ActionItem>();

	void Update () {
        if(GameManager.State == GameState.GameStart) {
            ClearActionList();
        }
        else if(GameManager.State == GameState.Rehearsal) {
            CreateRehersalActionList();
            UpdateRehersalActionList();
        }
        else if(GameManager.State == GameState.Recall){
            UpdateRecallActionList();
        }
	}

    /// <summary> Creates an action list for rehersal mode </summary>
    private void CreateRehersalActionList() {
        if (actionItemList.Count > 0)
            return;

        int count = PathManager.Path.Length;
        for (int i = 0; i < count; i++)
        {
            string name = PathManager.Path[i].Name;
            string action = PathManager.Path[i].RehersalActionName;
            GameObject item = CreateActionItem(i, name + ": " + action);
            actionItemList.Add(item.GetComponent<ActionItem>());
        }
    }

    /// <summary> Clears the action list </summary>
    private void ClearActionList() {
        actionItemList.Clear();
    }

    /// <summary> Creates an action list item </summary>
    private GameObject CreateActionItem(int index, string text) {
        GameObject item = new GameObject();
        item = Instantiate(item, transform);
        item.name = "Action Item " + index;

        ActionItem actionItem = item.AddComponent<ActionItem>();
        actionItem.SetItem(index, text, GameManager.GameUI.uncheckedBox);
        return item;
    }

    /// <summary> Updates the action list for rehersal mode based on the interactable log </summary>
    private void UpdateRehersalActionList() {
        int count = InteractableManager.Log.Length;
        for (int i = 0; i < count; i++)
            actionItemList[i].image.sprite = GameManager.GameUI.checkedBox;
    }

    /// <summary> Updates the action list for recall mode - Creates new items as interactable log grows </summary>
    private void UpdateRecallActionList()
    {
        int listCount = actionItemList.Count;
        int logCount = InteractableManager.Log.Length;
        for (int i = listCount; listCount < logCount; i++)
        {
            string name = InteractableManager.Log.interactableLog[i].Name;
            int actionID = InteractableManager.Log.actionLog[i];
            string action = InteractableManager.Log.interactableLog[i].stateData.states[actionID].actionName;
            GameObject item = CreateActionItem(i, name + ": " + action);
            item.GetComponent<ActionItem>().image.sprite = GameManager.GameUI.checkedBox;
            actionItemList.Add(item.GetComponent<ActionItem>());
        }

    }
}
