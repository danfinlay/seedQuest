﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//[ExecuteInEditMode] 
[RequireComponent(typeof(BoxCollider))]
public class Interactable : MonoBehaviour {

    public GameObject transformTarget = null;
    public InteractableStateData stateData = null;
    public InteractableID ID;

    [HideInInspector]
    public float interactDistance = 2.0f;
    private bool isOnHover = false;
    private GameObject actionSpot = null;

    // Use this for initialization
    void Start () {
        //InitInteractable();
	}
	
	void Update () {
        if(GameManager.State == GameState.Rehearsal || GameManager.State == GameState.Recall)
            if(actionSpot == null)
                InitInteractable();
        
        if(!PauseManager.isPaused && Camera.main != null && actionSpot != null) {
            BillboardInteractable();
            HoverOnInteractable();
            clickOnInteractable();
        }

        HighlightPathTarget();
	} 

    public void InitInteractable() {
        Vector3 positionOffset = Vector3.zero;
        if (stateData != null)
            positionOffset = stateData.labelPosOffset;
        Vector3 position = transform.position + positionOffset;
        Quaternion rotate = Quaternion.identity;
        actionSpot = Instantiate(InteractableManager.Instance.actionSpotIcon, position, rotate);
        //actionSpot = Instantiate(InteractableManager.Instance.actionSpotIcon, position, rotate, InteractableManager.Instance.transform);
        var text = actionSpot.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        if (stateData != null)
            text.text = stateData.interactableName;
        else
            text.text = "Error: Require StateData";
    } 

    public void BillboardInteractable() {
        Vector3 targetPosition = Camera.main.transform.position;
        Vector3 interactablePosition = actionSpot.transform.position;
        Vector3 lookAtDir = targetPosition - interactablePosition;
        lookAtDir.y = 0;

        Quaternion rotate = Quaternion.LookRotation(lookAtDir);
        actionSpot.transform.rotation = rotate;
    }

    private bool PlayerIsNear() {
        Vector3 playerPosition = PlayerManager.Position;
        float dist = (transform.position - playerPosition).magnitude;
        if (dist < interactDistance)
            return true;
        else
            return false;
    }

    public void HoverOnInteractable() {
        Camera c = Camera.main;
        RaycastHit hit;
        //Ray ray = new Ray(c.transform.position, c.transform.forward);
        Ray ray = c.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            bool hitThisInteractable = hit.transform.GetInstanceID() == transform.GetInstanceID();
            if (hitThisInteractable) {
              
                if (!isOnHover)
                    toggleHighlight(true);
                isOnHover = true;

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                    InteractableManager.showActions(this);
            }
            else {
                if (isOnHover)
                    toggleHighlight(false);
                isOnHover = false;
            }
        }
    }

    public void clickOnInteractable() {
        return;

        if (PauseManager.isPaused == true)
            return;

        Camera c = Camera.main;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = new Ray(c.transform.position, c.transform.forward);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                bool hitThisInteractable = hit.transform.GetInstanceID() == transform.GetInstanceID();
                if (hitThisInteractable) {
                    InteractableManager.showActions(this);
                }
            }
        }
    }

    public void toggleHighlight(bool highlight) {
        Renderer rend = transform.GetComponent<Renderer>();
        if (rend == null)
            return;

        Shader shaderDefault = Shader.Find("Standard");
        Shader shader = Shader.Find("Custom/Outline + Rim");

        Material[] materials = rend.materials;
        for (int i = 0; i < materials.Length; i++) {

            if (highlight)
                rend.materials[i].shader = shader;
            else
                rend.materials[i].shader = shaderDefault;
        }
    }

    public void HighlightPathTarget() {
        if (GameManager.State != GameState.Rehearsal)
            return;
        
        if (PathManager.PathTarget == this)
            toggleHighlight(true);
        else if(!isOnHover)
            toggleHighlight(false);
    }

    public void doAction(int actionIndex) {
        if(stateData == null) {
            Debug.Log(this.name + " Error: StateData is null");
            return;
        }
        else if(stateData.states.Count == 0) {
            Debug.Log(this.name + " Error: States is empty");
            return;
        }
        else {
            InteractableState state = stateData.states[actionIndex];
            state.enterState(this);
        }
    }

    public string getInteractableName()
    {
        if (stateData == null)
            return "Interactable Name";
        else
            return this.stateData.interactableName;
    }

    public string Name
    {
        get { return getInteractableName(); }
    }

    public string getStateName(int index)
    {
        if (stateData == null)
            return "Action #" + index;
        else
            return this.stateData.getStateName(index);
    }

    public string RehersalActionName
    {
        get { return getStateName(ID.actionID); }
    }

    public int getStateCount()
    {
        if (stateData == null)
            return 0;
        else
            return this.stateData.states.Count;
    }

} 