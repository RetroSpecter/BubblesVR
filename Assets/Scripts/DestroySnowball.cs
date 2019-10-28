using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySnowball : MonoBehaviour
{
    public GameObject wreckedVersion;
    public AudioClip snowBallFallAudio;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().playOnAwake = false;
        GetComponent<AudioSource>().clip = snowBallFallAudio;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            GetComponent<AudioSource>().Play();
            Destroy(gameObject);
            Instantiate(wreckedVersion, transform.position, Quaternion.identity);
        }


    }
}
