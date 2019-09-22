using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagSwitcher : MonoBehaviour
{
    public static TagSwitcher instance;
    public delegate void worldSwap();
    public static worldSwap swap;

    public string unocculdedLayer;
    public MaskLayer[] maskLayers;
    public ObjectLayerGroup[] objectLayers;

    public AudioLayer[] audioLayers;
    public float[] volumeMultiplyers;
    public float distanceThreshold;
    private int currentAudioLayer;

    private void Start()
    {
        instance = this;
        InitAudioLayers();
    }

    private void Update()
    {
        UpdateAudioLayers();
    }

    public void SwapOcclusionLayer(int maskNumber) {
        SwapLayer(objectLayers[maskNumber], maskLayers[0].occuldedLayer);
        SwapLayer(objectLayers[0], maskLayers[maskNumber].occuldedLayer);

        ObjectLayerGroup temp = objectLayers[0];
        objectLayers[0] = objectLayers[maskNumber];
        objectLayers[maskNumber] = temp;

        //swap the audio layers as well
        AudioLayer audioTemp = audioLayers[0];
        audioLayers[0] = audioLayers[maskNumber];
        audioLayers[maskNumber] = audioTemp;

        float VolLayer = volumeMultiplyers[0];
        volumeMultiplyers[0] = volumeMultiplyers[maskNumber];
        volumeMultiplyers[maskNumber] = VolLayer;
    }

    public void SwapLayer(ObjectLayerGroup olg, string layer) {
        if (swap != null) {
            swap.Invoke();
        }

        foreach (GameObject go in olg.layerObjects) {
            go.layer = LayerMask.NameToLayer(layer);
            if (olg.affectChildren)
            {
                foreach (Transform transform in go.GetComponentsInChildren<Transform>())
                {
                    transform.gameObject.layer = LayerMask.NameToLayer(layer);
                    if (transform.GetComponent<Light>() != null)
                    {
                        transform.GetComponent<Light>().cullingMask = LayerMask.GetMask(new string[]{"Default", layer});
                    }
                }
            }
        }
    }

    void InitAudioLayers() {
        volumeMultiplyers = new float[objectLayers.Length];
        audioLayers = new AudioLayer[objectLayers.Length];
        for (int i = 0; i < objectLayers.Length; i++)
        {
            List<AudioSource> sources = new List<AudioSource>();
            foreach (GameObject go in objectLayers[i].layerObjects) {
                if (go.GetComponentsInChildren<AudioSource>() != null)
                {
                    sources.AddRange(go.GetComponentsInChildren<AudioSource>());
                }
            }
            audioLayers[i] = new AudioLayer(sources.ToArray());
        }
    }

    public void updateVolumeMultiplyer(int layerNum, float distance) {
        float vol = distance / distanceThreshold;
        vol = Mathf.Clamp(vol, 0, 1);
        volumeMultiplyers[layerNum] = Mathf.Max(volumeMultiplyers[layerNum], Mathf.InverseLerp(0.25f, 0, vol));
    }

    private void UpdateAudioLayers() {
        float maxVolume = 0;
        for (int i = 0; i < audioLayers.Length; i++) {
            audioLayers[i].setVolume(volumeMultiplyers[i]);
            if (i != 0)
                maxVolume = Mathf.Max(maxVolume, volumeMultiplyers[i]);

            volumeMultiplyers[i] = 0;
        }

        audioLayers[0].setVolume(1 - maxVolume);
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
public struct AudioLayer {
    public AudioSource[] sources;
    public float[] initialVolumes;

    public AudioLayer(AudioSource[] sources) {
        this.sources = sources;
        initialVolumes = new float[this.sources.Length];
        for (int i = 0; i < sources.Length; i++)
        {
            initialVolumes[i] = sources[i].volume;
        }
    }

    public void setVolume(float volumeMultiplyer) {
        for (int i = 0; i < sources.Length; i++) {
            sources[i].volume = initialVolumes[i] * volumeMultiplyer;
        }
    }

}

[System.Serializable]
public struct ObjectLayerGroup {
    public string name;
    // objects that are included in a layer
    public GameObject[] layerObjects;

    // objects that are used as masks into the layer
    public bool affectChildren;
}

[System.Serializable]
public struct MaskLayer {
    public string name;
    public string occuldedLayer;
    //public string frameLayer;
}