using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    Bubble currentBubble;
    public Controllers controllerSize;
    private static Camera cam;

    private void Start()
    {
        TagSwitcher.swap += LetGoTeleport;
        cam = Camera.main;
    }

    private void Update()
    {
        if(currentBubble != null && !isGripping()) {
            LetGo();
        }

        if (transform.position.magnitude < 0.1f) {
            LetGo();
        }
    }

    public bool isGripping() {
        return (controllerSize == Controllers.Left  && (Input.GetButton(ControllerHand.OculusInputPrefix + "Button1") || Input.GetButton(ControllerHand.OculusInputPrefix + "Button2")
                || Input.GetAxis(ControllerHand.OculusInputPrefix + "SecondaryIndexTrigger") > 0.1f || Input.GetAxis(ControllerHand.OculusInputPrefix + "SecondaryHandTrigger") > 0.1f)) 
            || (controllerSize == Controllers.Right && (Input.GetButton(ControllerHand.OculusInputPrefix + "Button3") || Input.GetButton(ControllerHand.OculusInputPrefix + "Button4")
                || Input.GetAxis(ControllerHand.OculusInputPrefix + "PrimaryIndexTrigger") > 0.1f || Input.GetAxis(ControllerHand.OculusInputPrefix + "PrimaryHandTrigger") > 0.1f));
    }

    public void LetGoTeleport() {
        if(currentBubble != null)
            currentBubble.transform.position = new Vector3(1,1,0.5f);
        LetGo();
    }

    public void LetGo() {
        currentBubble?.LetGo();
        /*
        if (Vector3.Distance(currentBubble.transform.position, cam.transform.position + cam.transform.forward * 0.25f) < 0.5f) {
            StartCoroutine(currentBubble.lerpToPosition(cam.transform.forward * 0.25f, 0.25f));
        }
        */
        currentBubble = null;
    }

    private void OnTriggerStay(Collider other) {
        if (currentBubble == null && isGripping()) {
            other.transform.GetComponent<Bubble>()?.Grab(this.gameObject);
            currentBubble = other.transform.GetComponent<Bubble>();
        }
    }

    private void OnDestroy()
    {
        TagSwitcher.swap -= LetGo;
    }
}
