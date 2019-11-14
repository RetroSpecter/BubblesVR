using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    BubbleMovement currentBubble;
    public Controllers controllerSide;
    private static Camera cam;

    private void Start()
    {
        WorldLayerManager.swap += LetGoTeleport;
        cam = Camera.main;
    }

    private void Update()
    {
        if(currentBubble != null && !IsGripping()) {
            Release();
        }

        /*
        if (transform.position.magnitude < 0.1f) {
            Release();
        }
        */
    }

    //TODO: there is a bug where the hands are flipped from what they are supposed to be
    public bool IsGripping() {
        return (controllerSide == Controllers.Left  && (Input.GetButton(Hand.OculusInputPrefix + "Button1") || Input.GetButton(Hand.OculusInputPrefix + "Button2")
                || Input.GetAxis(Hand.OculusInputPrefix + "SecondaryIndexTrigger") > 0.1f || Input.GetAxis(Hand.OculusInputPrefix + "SecondaryHandTrigger") > 0.1f)) 
            || (controllerSide == Controllers.Right && (Input.GetButton(Hand.OculusInputPrefix + "Button3") || Input.GetButton(Hand.OculusInputPrefix + "Button4")
                || Input.GetAxis(Hand.OculusInputPrefix + "PrimaryIndexTrigger") > 0.1f || Input.GetAxis(Hand.OculusInputPrefix + "PrimaryHandTrigger") > 0.1f));
    }

    public void LetGoTeleport() {
        if(currentBubble != null)
            currentBubble.transform.position = new Vector3(1,1,0.5f);
        Release();
    }

    public void Release() {
        currentBubble?.Released();
        /*
        if (Vector3.Distance(currentBubble.transform.position, cam.transform.position + cam.transform.forward * 0.25f) < 0.5f) {
            StartCoroutine(currentBubble.lerpToPosition(cam.transform.forward * 0.25f, 0.25f));
        }
        */
        currentBubble = null;
    }

    private void OnTriggerStay(Collider other) {
        if (currentBubble == null && IsGripping()) {
            other.transform.GetComponent<BubbleMovement>()?.Grabbed(this.gameObject);
            currentBubble = other.transform.GetComponent<BubbleMovement>();
        }
    }

    private void OnDestroy()
    {
        WorldLayerManager.swap -= Release;
    }
}
