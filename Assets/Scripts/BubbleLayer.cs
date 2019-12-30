using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * manages how the bubble interacts with the stencil layers
*/
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class BubbleLayer : MonoBehaviour
{
    public float minDistance = 0.2f;
    public float scaleMultiplyer = 2;
    private Vector3 initialScale;

    private Camera cam;
    private MeshRenderer mesh;
    private Rigidbody rigid;

    public int layer = 1;
    public float timer = 3f;

    IEnumerator disappearEnum;

    private void Start() {
        cam = Camera.main;
        rigid = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
        initialScale = transform.localScale;
        WorldLayerManager.swap += hideBubble;
    }



    void Update()
    {
        if (disappearEnum == null)
        {
            this.transform.forward = Vector3.Normalize(cam.transform.position - transform.position);
            float xyScale = initialScale.x * Mathf.Lerp(1, scaleMultiplyer, 1 - (Vector3.Distance(cam.transform.position, transform.position) / minDistance));
            float zScale = initialScale.x * Mathf.Lerp(1, 0.0001f, 1 - (Vector3.Distance(cam.transform.position, transform.position) / 0.25f));
            transform.localScale = new Vector3(xyScale, xyScale, zScale);
            WorldLayerManager.instance.UpdateVolumeMultiplyer(layer, Vector3.Distance(cam.transform.position, transform.position));
        }
    }

    public void SwapLayer() {
        if (mesh.enabled)
        {
            WorldLayerManager.instance.SwapWorldLayers(layer);
            mesh.enabled = false;
            rigid.isKinematic = true;
            StartCoroutine(respawnTimer(timer));
        }
    }

    public void hideBubble() {
        if (disappearEnum == null)
        {
            disappearEnum = resize(3 + Random.Range(0.25f, 3f), 0.25f + Random.Range(0.05f, 0.5f));
            StartCoroutine(disappearEnum);
        }
    }

    private bool isGrabbed() {
        return rigid.isKinematic == true;
    }

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

    IEnumerator respawnTimer(float seconds) {
        yield return new WaitForSeconds(seconds);
        mesh.enabled = true;
        rigid.isKinematic = false;
    }

    private void OnDestroy()
    {
        WorldLayerManager.swap -= hideBubble;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (this.isGrabbed() && other.transform.GetComponent<BubbleLayer>() != null && other.transform.GetComponent<BubbleLayer>().isGrabbed()) {
            //WorldLayerManager.instance.fuseBubbles(this, other.transform.GetComponent<BubbleLayer>());
        }
    }
}
