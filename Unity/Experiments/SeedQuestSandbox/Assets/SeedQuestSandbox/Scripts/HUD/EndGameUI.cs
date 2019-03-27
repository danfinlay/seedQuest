﻿using UnityEngine;
using UnityEngine.SceneManagement;

using SeedQuest.Interactables;

public class EndGameUI : MonoBehaviour {

    static private EndGameUI instance = null;
    static private EndGameUI setInstance() { instance = HUDManager.Instance.GetComponentInChildren<EndGameUI>(true); return instance; }
    static public EndGameUI Instance { get { return instance == null ? setInstance() : instance; } }

    public string PrototypeSelectScene = "PrototypeSelect";
    public string RehearsalScene = "PrototypeSelect";
    public string RecallScene = "PrototypeSelect";

    /// <summary> Toggles On the EndGameUI </summary>
    static public void ToggleOn() {
        if (Instance.gameObject.activeSelf)
            return;
        
        Instance.gameObject.SetActive(true);
        var textList = Instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        textList[0].text = InteractablePathManager.SeedString;

        if (GameManager.Mode == GameMode.Rehearsal)
            textList[1].text = "Rehearsal Complete! \n Need more practice? Select Rehearsal mode. \n Ready to go? Select Recall";
        else
            textList[1].text = "Key is Recovered!";
    }

    /// <summary> Handles selecting PrototypeSelect Button </summary>
    public void PrototypeSelect() {
        InteractablePathManager.InitalizePathAndLog();
        InteractableManager.destroyInteractables();
        SceneManager.LoadScene(PrototypeSelectScene);
    }

    /// <summary> Handles selecting Recall Button </summary>
    public void Recall() {
        GameManager.Mode = GameMode.Recall;
        GameManager.State = GameState.Play;

        InteractablePathManager.InitalizePathAndLog();
        InteractableManager.destroyInteractables();
        SceneManager.LoadScene(RecallScene);
    }

    /// <summary> Handles selecting Rehearsal Button </summary>
    public void Rehearsal() {
        GameManager.Mode = GameMode.Rehearsal;
        GameManager.State = GameState.Play;

        InteractablePathManager.InitalizePathAndLog();
        InteractableManager.destroyInteractables();
        SceneManager.LoadScene(RehearsalScene);
    }
}