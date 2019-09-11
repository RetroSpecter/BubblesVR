using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRaycast : MonoBehaviour
{
    Camera cam;

    [Range(0,1)]
    public float width, height;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        fullyBlocked()?.SwapLayer();
    }

    public ProximityTagSwitcher fullyBlocked() {
        GameObject blockingObject = null;

        for (int i = -1; i <= 2; i += 2)
        {
            for (int j = -1; j < 2; j += 2)
            {
                Vector2 corner = new Vector2(0.5f + width / 2 * i, 0.5f + height / 2 * j);
                // print(i + " " + j + ":" + corner);
                Ray ray = cam.ViewportPointToRay(corner);
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            }
        }


        for (int i = -1; i <= 2; i+=2) {
            for (int j = -1; j < 2; j+=2)
            {
                Vector2 corner = new Vector2(0.5f + width/2 * i, 0.5f + height / 2 * j);
                Ray ray = cam.ViewportPointToRay(corner);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {                   
                    if (i == -1 && j == -1) {
                        blockingObject = hit.transform.gameObject;
                    } else if (blockingObject != hit.transform.gameObject){
                        return null;
                    }
                } else {
                    return null;
                }
            }
        }
        return blockingObject.GetComponent<ProximityTagSwitcher>();
    }
}
