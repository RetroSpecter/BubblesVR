using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagSwitcher : MonoBehaviour
{
    public string unocculdedLayer;
    public MaskLayer[] maskLayers;
    public ObjectLayerGroup[] objectLayers;
    public static TagSwitcher instance;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SwapOcclusionLayer(2);
        }
    }

    public void SwapOcclusionLayer(int maskNumber) {
        SwapLayer(objectLayers[maskNumber], unocculdedLayer);
        SwapLayer(objectLayers[0], maskLayers[maskNumber].occuldedLayer);

        ObjectLayerGroup temp = objectLayers[0];
        objectLayers[0] = objectLayers[maskNumber];
        objectLayers[maskNumber] = temp;

    }

    public void SwapLayer(ObjectLayerGroup olg, string layer) {
        foreach (GameObject go in olg.layerObjects) {
            go.layer = LayerMask.NameToLayer(layer);
            if (olg.affectChildren)
            {
                foreach (Transform transform in go.GetComponentsInChildren<Transform>())
                {
                    
                    transform.gameObject.layer = LayerMask.NameToLayer(layer);
                    //transform.GetComponent<Light>()?.cullingMask = LayerMask.NameToLayer(layer);
                    print(transform.gameObject.layer);
                }
            }
        }
    }

    /*
    public void SwapMaskLayer(ObjectLayerGroup olg, MaskLayer ml)
    {
        SwitchOcclusionLayer(olg, ml.occuldedLayer);
        SwitchFrameLayer(olg, ml.frameLayer);
    }

    public void SwitchFrameLayer(ObjectLayerGroup olg, string layer) {
        foreach (GameObject go in olg.frameObjects)
        {
            go.layer = LayerMask.NameToLayer(layer);
            if (olg.affectChildren)
            {
                foreach (Transform transform in go.GetComponentsInChildren<Transform>())
                {
                    transform.gameObject.layer = LayerMask.NameToLayer(layer);
                }
            }
        }
    }
    */
}

[System.Serializable]
public struct ObjectLayerGroup {
    public string name;
    // objects that are included in a layer
    public GameObject[] layerObjects;

    // objects that are used as masks into the layer
    public GameObject[] frameObjects;
    public bool affectChildren;
}

[System.Serializable]
public struct MaskLayer {
    public string name;
    public string occuldedLayer;
    //public string frameLayer;
}