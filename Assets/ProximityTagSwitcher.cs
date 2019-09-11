using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class ProximityTagSwitcher : MonoBehaviour
{
    //public float minDistance = 0.55f;

    private Camera cam;
    private MeshRenderer mesh;
    private Rigidbody rigid;

    public int layer = 1;
    public float timer = 3f;

    private void Start() {
        cam = Camera.main;
        rigid = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
    }
    
    /*
    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(cam.transform.position, transform.position) < minDistance) {
            print(Vector3.Distance(cam.transform.position, transform.position));
            TagSwitcher.instance.SwapOcclusionLayer(layer);
            this.gameObject.SetActive(false);
        }
    }
    */

    public void SwapLayer() {
        if (mesh.enabled)
        {
            TagSwitcher.instance.SwapOcclusionLayer(layer);
            mesh.enabled = false;
            rigid.isKinematic = true;
            StartCoroutine(respawnTimer(timer));
        }
    }

    IEnumerator respawnTimer(float seconds) {
        yield return new WaitForSeconds(seconds);
        mesh.enabled = true;
        rigid.isKinematic = false;
    }
}
