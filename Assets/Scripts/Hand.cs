using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages animation and the side of a hand
public class Hand : MonoBehaviour
{
    public Controllers controllerSide = Controllers.Left;
    public static string OculusInputPrefix = "Oculus_CrossPlatform_";
    public Animator anim;

    void Update()
    {
        string side = GetControllerPrefix(controllerSide);
        anim.SetFloat("Index", Input.GetAxis(OculusInputPrefix + side + "IndexTrigger"));
        anim.SetFloat("Finger", Input.GetAxis(OculusInputPrefix + side + "HandTrigger"));

        float thumbAmount = 0;

        if (controllerSide == Controllers.Left) {
            thumbAmount += Input.GetButton(OculusInputPrefix + "Button1") ? 0.5f : 0;
            thumbAmount += Input.GetButton(OculusInputPrefix + "Button2") ? 0.5f : 0;
        } else {
            thumbAmount += Input.GetButton(OculusInputPrefix + "Button3") ? 0.5f : 0;
            thumbAmount += Input.GetButton(OculusInputPrefix + "Button4") ? 0.5f : 0;
        }

        anim.SetFloat("Thumb", thumbAmount);
    }

    string GetControllerPrefix(Controllers c) {
        return c == Controllers.Left ? "Secondary" : "Primary";
    }
}

public enum Controllers { Left, Right };