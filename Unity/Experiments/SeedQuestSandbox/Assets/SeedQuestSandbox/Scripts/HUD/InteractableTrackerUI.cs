﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeedQuest.Interactables;

public class InteractableTrackerUI : MonoBehaviour
{
    public bool isFixedToCenterEdge = true;
    public Vector3 positionOffset = Vector3.zero;
    public float angleOffset = 25;
    public float wobbleStrength = 10;
    public float wobbleSpeed = 5;

    private Transform player;
    private new Camera camera;
    private RectTransform tracker;
    private RectTransform arrow;

    private float angle;
    private Vector3 screenPosition;
    private Vector3 unclampedScreenPosition;
    private bool isClampedTop = false;
    private bool isClampedBottom = false;
    private bool isClampedRight = false;
    private bool isClampedLeft = false;

    private Vector2 padding = new Vector2(150, 150);
    private Vector2 arrowOffset = new Vector2(75, 75);
    private float behindCameraOffset = 10000; 

    private float MidScreenX { get => camera.scaledPixelWidth / 2.0f; }
    private float MidScreenY { get => camera.scaledPixelHeight / 2.0f; }

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        tracker = GetComponentsInChildren<RectTransform>()[1];
        arrow = GetComponentsInChildren<RectTransform>()[5];
    }

    void Update() {
        if (InteractablePath.NextInteractable != null)
            SetPositionClamp();
            SetTrackerIconPosition();
            SetArrowIconPosition();
    }

    /// <summary> Sets the position clamp status for TrackerIcon i.e. when outside of the bounds of screen it sets which edges of the screen it is out of bounds </summary>
    private void SetPositionClamp() {
        isClampedRight = unclampedScreenPosition.x > camera.scaledPixelWidth - padding.x;
        isClampedLeft = unclampedScreenPosition.x < padding.x;
        isClampedTop = unclampedScreenPosition.y > camera.scaledPixelHeight - padding.y;
        isClampedBottom = unclampedScreenPosition.y < padding.y;
    }
                                               
    /// <summary> Set Tracker Position. Follows next interactable, unless offscreen then appears in direction of next interactable. </summary>
    private void SetTrackerIconPosition() {
        unclampedScreenPosition = camera.WorldToScreenPoint(InteractablePath.NextInteractable.transform.position);
        screenPosition = unclampedScreenPosition;

        Vector3 wobble = Vector3.zero;
        Vector3 _positionOffset = Vector3.zero;
        if(InBounds(screenPosition) && screenPosition.z > 0) {
            wobble = wobbleStrength * Vector3.up * Mathf.Sin(wobbleSpeed * Time.time);
            _positionOffset = positionOffset;
        }
        else if(InBounds(screenPosition) && screenPosition.z < 0) {
            // Clamp TrackerIcon when Next Interactable when object is behind camera
            var x = Mathf.Clamp(screenPosition.x + behindCameraOffset, 0.0f + padding.x, camera.scaledPixelWidth - padding.x);
            var y = Mathf.Clamp(screenPosition.y, 0.0f + padding.y, camera.scaledPixelHeight - padding.y);
            screenPosition = new Vector3(x, y, screenPosition.z);

            if (isFixedToCenterEdge) {
                screenPosition = new Vector3(camera.scaledPixelWidth - padding.x, MidScreenY, screenPosition.z);    
            }
        }
        else {
            // Clamp TrackerIcon when Next Interactable if off screen
            var x = Mathf.Clamp(screenPosition.x, 0.0f + padding.x, camera.scaledPixelWidth - padding.x);
            var y = Mathf.Clamp(screenPosition.y, 0.0f + padding.y, camera.scaledPixelHeight - padding.y);
            screenPosition = new Vector3(x, y, screenPosition.z);

            // Clamp TrackerIcon to Center Edge of Screen based on position off screen
            if(isFixedToCenterEdge) {
                if (screenPosition.z < 0)
                    screenPosition = new Vector3(camera.scaledPixelWidth - padding.x, MidScreenY, screenPosition.z);    
                else if (isClampedRight)
                    screenPosition = new Vector3(camera.scaledPixelWidth - padding.x, MidScreenY, screenPosition.z);    
                else if (isClampedLeft)
                    screenPosition = new Vector3(padding.x, MidScreenY, screenPosition.z);    
                else if (isClampedTop && !isClampedLeft && !isClampedRight)
                    screenPosition = new Vector3(MidScreenX, camera.scaledPixelHeight - padding.y, screenPosition.z);
                else if(isClampedBottom && !isClampedLeft && !isClampedRight)
                    screenPosition = new Vector3(MidScreenX, padding.y, screenPosition.z);
            }
        }

        // Set TrackerIcon Postion
        Vector3 camOffset = new Vector3(MidScreenX, MidScreenY, 0.0f);
        tracker.localPosition = screenPosition - camOffset + _positionOffset + wobble;    
    }

    /// <summary> Set ArrowIcon over player point in the direction of the Next interactable </summary>
    private void SetArrowIconPositionOnPlayer() {
        Vector3 playerPosition = camera.WorldToScreenPoint(player.position);
        Vector2 v1 = new Vector2(MidScreenX, MidScreenY);
        Vector2 v2 = new Vector2(unclampedScreenPosition.x, unclampedScreenPosition.y);
        angle = Vector2.SignedAngle(playerPosition, v2-v1);
        arrow.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, angle + angleOffset));
    }

    /// <summary> Sets arrow icon when Next interactable is off screen. Arrow faces the Next interactable. </summary>
    private void SetArrowIconPosition() {
        // Show Arrow if Iteractable is out of bounds of screen
        if (InBounds(unclampedScreenPosition) && unclampedScreenPosition.z > 0)
            arrow.gameObject.SetActive(false);
        else
            arrow.gameObject.SetActive(true);

        // Set ArrowIcon facing direction
        Vector3 playerPosition = camera.WorldToScreenPoint(player.position);
        Vector2 v1 = new Vector2(MidScreenX, MidScreenY);
        Vector2 v2 = new Vector2(unclampedScreenPosition.x, unclampedScreenPosition.y);
        angle = Vector2.SignedAngle(playerPosition, v2 - v1);
        arrow.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, angle + angleOffset));

        // Set ArrowIcon facing direction if UI is fixedToEdge
        if(isFixedToCenterEdge) {
            if (unclampedScreenPosition.z < 0)
                angle = 0;
            else if (isClampedRight)
                angle = 0;
            else if (isClampedLeft)
                angle = 180;
            else if (isClampedTop)
                angle = 90;
            else if (isClampedBottom)
                angle = -90;
            arrow.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, angle));
        }

        // Set ArrowIcon position offset from TrackerIcon position
        Vector3 clampOffset = Vector3.zero;
        if (isClampedRight)
            clampOffset = new Vector3(arrowOffset.x, 0, 0);
        else if (isClampedLeft)
            clampOffset = new Vector3(-arrowOffset.x, 0, 0);
        else if (isClampedTop)
            clampOffset = new Vector3(0, arrowOffset.y, 0);
        else if (isClampedBottom)
            clampOffset = new Vector3(0, -arrowOffset.y, 0);

        // Set ArrowIcon position
        Vector3 camOffset = new Vector3(MidScreenX, MidScreenY, 0.0f);
        arrow.localPosition = screenPosition - camOffset + clampOffset;
    }

    /// <summary> Checks if screen position is in Camera frame bounds </summary>
    private bool InBounds(Vector3 pos) {
        float x0 = 0;
        float x1 = camera.scaledPixelWidth;
        float y0 = 0; 
        float y1 = camera.scaledPixelHeight;
        return x0 <= pos.x && pos.x <= x1 && y0 <= pos.y && pos.y <= y1;
    }
}