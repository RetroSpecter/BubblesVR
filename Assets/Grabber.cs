using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    Bubble currentBubble;
    public Controllers controllerSize;

    private void Update()
    {
        if(currentBubble != null && !isGripping()) {
            LetGo();
        }
    }

    public bool isGripping() {
        return (controllerSize == Controllers.Left  && (Input.GetButton(ControllerHand.OculusInputPrefix + "Button1") || Input.GetButton(ControllerHand.OculusInputPrefix + "Button2")
                || Input.GetAxis(ControllerHand.OculusInputPrefix + "SecondaryIndexTrigger") > 0.1f || Input.GetAxis(ControllerHand.OculusInputPrefix + "SecondaryHandTrigger") > 0.1f)) 
            || (controllerSize == Controllers.Right || Input.GetButton(ControllerHand.OculusInputPrefix + "Button3") || Input.GetButton(ControllerHand.OculusInputPrefix + "Button4")
                || Input.GetAxis(ControllerHand.OculusInputPrefix + "PrimaryIndexTrigger") > 0.1f || Input.GetAxis(ControllerHand.OculusInputPrefix + "PrimaryHandTrigger") > 0.1f);
    }

    public void LetGo() {
        currentBubble?.LetGo();
        currentBubble = null;
    }

    private void OnTriggerStay(Collider other) {
        if (currentBubble == null && isGripping()) {
            other.transform.GetComponent<Bubble>()?.Grab(this.gameObject);
            currentBubble = other.transform.GetComponent<Bubble>();
        }
    }
}
