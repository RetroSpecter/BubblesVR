using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Manages the different worlds and their visibility layers
 * TODO: this class is kinda shit now that we added layer switching. If we do expand on this concept, I need to rewrite this
 */
public class WorldLayerManager : MonoBehaviour
{
    public static WorldLayerManager instance;
    public delegate void worldSwap();
    public static worldSwap swap;

    public MaskLayer[] maskLayers;
    public ObjectLayerGroup[] objectLayers;
    private List<List<int>> layerGroups;
    public List<BubbleLayerGroup> bubbleLayers;

    // eventually phase this out
    public float[] volumeMultiplyers;
    public float distanceThreshold;
    private int currentAudioLayer;

    public GameObject bubblePrefab;
    private AudioSource source;
    public AudioClip warpSFX;

    int swapCounter = 0;
    int probabilityIndex = 0;
    public List<Vector2Int> numWorldProbabilities;

    private void Start()
    {
        instance = this;
        source = GetComponent<AudioSource>();
        layerGroups = new List<List<int>>(objectLayers.Length);
        volumeMultiplyers = new float[objectLayers.Length];
        for (int i = 0; i < objectLayers.Length; i++) {
            layerGroups.Add(new List<int>() { i });
        }

        InitAudioLayers();
    }

    private void Update()
    {
        UpdateAudioLayers();

        if (Input.GetKeyDown(KeyCode.Space)) {
            SwapWorldLayers(Random.Range(0, layerGroups.Count));
        }
    }

    public void randomizeWorlds(int numOfWorlds) {   
        List<List< int >> tempNewList= new List<List<int>>();
        tempNewList.Add(layerGroups[0]);
        for (int i = 1; i < numOfWorlds; i++) {
            tempNewList.Add(new List<int>());
        }
        layerGroups = tempNewList;

        List<int> worldLayers = new List<int>();
        for (int i = 1; i < objectLayers.Count(); i++)
        {
            // pretty inefficient, but fine for how many worlds we have
            if (!layerGroups[0].Contains(i)) {
                worldLayers.Add(i);
            }
        }
        worldLayers = worldLayers.OrderBy(x => Random.value).ToList();

        if (!layerGroups[0].Contains(0)) {
            numOfWorlds--;
        }

        for (int i = 0; i < worldLayers.Count; i++) {
            List<int> curLayer = layerGroups[1 + (i % (numOfWorlds - 1))];
            curLayer.Add(worldLayers[i]);
        }

        if (!layerGroups[0].Contains(0)) {
            layerGroups[layerGroups.Count - 1].Add(0);
        }

        for(int i = 0; i < layerGroups.Count; i++) {
            combineWorlds(layerGroups[i], i);
        }

        for (int i = 0; i < bubbleLayers.Count; i++) {
            changeBubbleVisibility(i, i < numOfWorlds);
        }
    }

    void changeBubbleVisibility(int layer, bool active) {
        foreach (GameObject bubble in bubbleLayers[layer].bubbles)
        {
            bubble.SetActive(active);
        }
    }

    public void combineWorlds(List<int> layers, int occludedLayer) {
        string firstLayer = maskLayers[occludedLayer].occuldedLayer;
        for (int i = 0; i < layers.Count; i++) {
            SwapObjectLayer(layers, firstLayer);
        }
    }

    // Uses a maskNumber to set the current visible world
    public void SwapWorldLayers(int maskNumber) {
        if (swap != null) {
            swap.Invoke();
        }
        SwapObjectLayer(layerGroups[maskNumber], maskLayers[0].occuldedLayer);
        SwapObjectLayer(layerGroups[0], maskLayers[maskNumber].occuldedLayer);

        swapCounter++;
        if (probabilityIndex < numWorldProbabilities.Count-1 && numWorldProbabilities[probabilityIndex].x < swapCounter) {         
            probabilityIndex++;    
        }
        //randomizeWorlds(numWorldProbabilities[probabilityIndex].y);    
        print(probabilityIndex);
        //randomizeWorlds(Random.Range(numWorldProbabilities[0].y, numWorldProbabilities[probabilityIndex].y));

        setPrimaryMask(maskNumber);

        source.clip = warpSFX;
        source.pitch = Random.Range(0.98f, 1.04f);
        source.Play();
    }

    private void setPrimaryMask(int maskNumber) {
        List <int> temp = layerGroups[0];
        layerGroups[0] = layerGroups[maskNumber];
        layerGroups[maskNumber] = temp;
    }

    private void SwapObjectLayer(List<int> olgs, string layer)
    {
        foreach (int olg in olgs) {
            SwapObjectLayer(objectLayers[olg], layer);
        }
    }

    // gets all of the gameobjects in a objectlayergroup, and switches them to the new stencil layer
    private void SwapObjectLayer(ObjectLayerGroup olg, string layer) {
        foreach (GameObject go in olg.layerObjects) {
            go.layer = LayerMask.NameToLayer(layer);
            if (olg.affectChildren)
            {
                foreach (Transform transform in go.GetComponentsInChildren<Transform>())
                {
                    transform.gameObject.layer = LayerMask.NameToLayer(layer);
                    if (transform.GetComponent<Light>() != null) {
                        transform.GetComponent<Light>().cullingMask = LayerMask.GetMask(new string[]{"Default", layer});
                    }
                }
            }
        }
    }

    private void InitAudioLayers() {

        for (int i = 0; i < objectLayers.Length; i++) {
            List<AudioSource> sources = new List<AudioSource>();
            foreach (GameObject go in objectLayers[i].layerObjects)
            {
                if (go.GetComponentsInChildren<AudioSource>() != null)
                {
                    sources.AddRange(go.GetComponentsInChildren<AudioSource>());
                }
            }
            objectLayers[i].audioSources = new AudioLayer(sources.ToArray());
        }
    }

    //TODO: this is really bad
    public void UpdateVolumeMultiplyer(int layerNum, float distance) {
        
        float vol = distance / distanceThreshold;
        vol = Mathf.Clamp(vol, 0, 1);
        for (int i = 0; i < layerGroups.Count; i++) {
            if (layerGroups[i].Contains(layerNum)) {
                volumeMultiplyers[i] = Mathf.Max(volumeMultiplyers[i], Mathf.InverseLerp(0.25f, 0, vol));
                break;
            }
        }


        
    }

    public void fuseBubbles(BubbleLayer a, BubbleLayer b) {
        if (bubblesEnum == null) {
            bubblesEnum = fuseBubblesEnum(a, b);
            StartCoroutine(bubblesEnum);
        }
    }

    IEnumerator bubblesEnum;
    IEnumerator fuseBubblesEnum(BubbleLayer a, BubbleLayer b) {
        List<int> newLayerGroup = new List<int>();
        List<int> usedLayers = new List<int>();
        for (int i = 0; i < layerGroups.Count; i++) {
            List<int> layerGroup = layerGroups[i];
            if (layerGroup.Contains(a.layer) || layerGroup.Contains(b.layer)) {
                usedLayers.Add(i-1);
                newLayerGroup.AddRange(layerGroup);
            }
        }
        changeBubbleVisibility(usedLayers[1], false);
        layerGroups[usedLayers[0]] = newLayerGroup;
        layerGroups[usedLayers[1]] = newLayerGroup;
        combineWorlds(newLayerGroup, a.layer);

        b.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        bubblesEnum = null;
    }

    private void UpdateAudioLayers() {
        float maxVolume = 0;
        for (int i = 1; i < layerGroups.Count; i++) {
            List<int> curLayer = layerGroups[i];
            foreach (int audioLayer in curLayer) {
                objectLayers[audioLayer].setVolume(volumeMultiplyers[i]);
                maxVolume = Mathf.Max(maxVolume, volumeMultiplyers[i]);
                volumeMultiplyers[audioLayer] = 0;
            }
        }

        foreach (int audioLayer in layerGroups[0])
        {
            objectLayers[audioLayer].setVolume(1 - maxVolume);
        }        
    }

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
    public AudioLayer audioSources;

    // objects that are used as masks into the layer
    public bool affectChildren;

    public void setVolume(float volumeMultiplyer) {
        audioSources.setVolume(volumeMultiplyer);
    }
}

[System.Serializable]
public struct MaskLayer {
    public string name;
    public string occuldedLayer;
    //public string frameLayer;
}

[System.Serializable]
public struct BubbleLayerGroup {
    public List<GameObject> bubbles;
}