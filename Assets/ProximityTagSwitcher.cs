﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class ProximityTagSwitcher : MonoBehaviour
{
    public float minDistance = 0.2f;
    public float scaleMultiplyer = 2;
    private Vector3 initialScale;

    private Camera cam;
    private MeshRenderer mesh;
    private Rigidbody rigid;

    public int layer = 1;
    public float timer = 3f;

    private void Start() {
        cam = Camera.main;
        rigid = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
        initialScale = transform.localScale;
        TagSwitcher.swap += disappear;
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (disappearEnum == null)
        {
            this.transform.forward = Vector3.Normalize(cam.transform.position - transform.position);
            float xyScale = initialScale.x * Mathf.Lerp(1, scaleMultiplyer, 1 - (Vector3.Distance(cam.transform.position, transform.position) / minDistance));
            float zScale = initialScale.x * Mathf.Lerp(1, 0.0001f, 1 - (Vector3.Distance(cam.transform.position, transform.position) / 0.25f));
            transform.localScale = new Vector3(xyScale, xyScale, zScale);
            TagSwitcher.instance.updateVolumeMultiplyer(layer, Vector3.Distance(cam.transform.position, transform.position));
        }
    }
    

    public void SwapLayer() {
        if (mesh.enabled)
        {
            TagSwitcher.instance.SwapOcclusionLayer(layer);
            mesh.enabled = false;
            rigid.isKinematic = true;
            StartCoroutine(respawnTimer(timer));
        }
    }

    public void disappear() {
        if (disappearEnum == null)
        {
            disappearEnum = resize(3 + Random.Range(0.25f, 3f), 0.25f + Random.Range(0.05f, 0.5f));
            StartCoroutine(disappearEnum);
        }
    }

    IEnumerator disappearEnum;
    IEnumerator resize(float disappearTime, float resizeTime)
    {
        transform.localScale = Vector3.one * 0.001f;
        yield return new WaitForSeconds(disappearTime);

        float t = 0;
        while (t < resizeTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.one * 0.01f, initialScale, t/resizeTime);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localEulerAngles = initialScale;
        rigid.velocity = Random.insideUnitSphere * 0.5f;
        disappearEnum = null;
    }

    private void OnDestroy()
    {
        TagSwitcher.swap -= disappear;
    }

    IEnumerator respawnTimer(float seconds) {
        yield return new WaitForSeconds(seconds);
        mesh.enabled = true;
        rigid.isKinematic = false;
    }
}
