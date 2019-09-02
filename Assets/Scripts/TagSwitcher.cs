using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagSwitcher : MonoBehaviour
{
    public string unocculdedLayer;
    public MaskLayer[] maskLayers;
    public ObjectLayerGroup[] objectLayers;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            //SwapMaskLayer(objectLayers[0], maskLayers[2]);
            //SwapMaskLayer(objectLayers[2], maskLayers[0]);
            SwitchOcclusionLayer(objectLayers[2], unocculdedLayer);
            SwitchOcclusionLayer(objectLayers[3], maskLayers[2].occuldedLayer);
        }
    }

    public void SwapMaskLayer(ObjectLayerGroup olg, MaskLayer ml) {
        SwitchOcclusionLayer(olg, ml.occuldedLayer);
        SwitchFrameLayer(olg, ml.frameLayer);
    }

    public void SwitchOcclusionLayer(ObjectLayerGroup olg, string layer) {
        foreach (GameObject go in olg.layerObjects) {
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
    public string frameLayer;
}