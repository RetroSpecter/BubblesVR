using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private Rigidbody rigid;
    public float speed = 5;

    // Start is called before the first frame update
    void Start() {
        rigid = GetComponent<Rigidbody>();
        rigid.velocity = Random.insideUnitSphere * speed;
    }

    private void Update()
    {
        if (rigid.velocity.magnitude < speed / 2f)
        {
            rigid.velocity = rigid.velocity.normalized * speed;
        }
    }

    public void Grab(GameObject grabber) {
        rigid.isKinematic = true;
        transform.parent = grabber.transform;
        rigid.velocity = Vector3.zero;
    }

    public void LetGo() {
        rigid.isKinematic = false;
        transform.parent = null;
        rigid.velocity = Random.insideUnitSphere * speed;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.CompareTag("Hand")) {
            rigid.velocity = rigid.velocity.magnitude * -collision.relativeVelocity * 3;
        } else {
            rigid.velocity = rigid.velocity.magnitude * Vector3.Reflect(rigid.velocity, collision.GetContact(0).normal).normalized;
        }
    }
    /*
    public IEnumerator lerpToPosition(Vector3 position, float time) {
        
        float t = 0;
        while (t < time) {
            transform.position = Vector3.Lerp(transform.position, position, t/time);
            yield return null;
            t += Time.deltaTime;
        }
        
        //yield return null;
        //transform.position = position;
    }*/
}
