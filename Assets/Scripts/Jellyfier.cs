using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Jellyfier : MonoBehaviour
{
    public float bounceSpeed;
    public float fallForce;
    public float stiffness;

    private MeshFilter meshFilter;
    private Mesh mesh;

    JellyVertex[] jellyVertices;
    Vector3[] currentMeshVertices;
    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        GetVerticies();
    }


    private void GetVerticies()
    {
        jellyVertices = new JellyVertex[mesh.vertices.Length];
        currentMeshVertices = new Vector3[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            jellyVertices[i] = new JellyVertex(i, mesh.vertices[i], mesh.vertices[i], Vector3.zero);
            currentMeshVertices[i] = mesh.vertices[i];
        }
    }

    private void Update()
    {
        UpdateVertices();
    }


    private void UpdateVertices() {
        for (int i = 0; i < jellyVertices.Length; i++) {
            jellyVertices[i].UpdateVelocity(bounceSpeed);
            jellyVertices[i].Settle(stiffness);

            jellyVertices[i].currentVertexPosition += jellyVertices[i].currentVelocity * Time.deltaTime;
            currentMeshVertices[i] = jellyVertices[i].currentVertexPosition;
        }

        mesh.vertices = currentMeshVertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    public void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] collisionPoints = collision.contacts;
        source.pitch = Random.Range(0.98f, 1.14f);
        source.Play();
        for (int i = 0; i < collisionPoints.Length; i++) {
            Vector3 inputPoint = collisionPoints[i].point + (collisionPoints[i].point * .1f);
            ApplyPressureToPoint(inputPoint, fallForce);
        }
    }

    public void ApplyPressureToPoint(Vector3 point, float pressure) {
        for (int i = 0; i < jellyVertices.Length; i++) {
            jellyVertices[i].ApplyPressuretoVertex(transform, point, pressure);
        }
    }
}

class JellyVertex {
    public int verticeIndex;
    public Vector3 initialVertexPosition;
    public Vector3 currentVertexPosition;

    public Vector3 currentVelocity;

    public JellyVertex(int verticeIndex, Vector3 initialVertexPosition, Vector3 currentVertexPosition, Vector3 currentVelocity) {
        this.verticeIndex = verticeIndex;
        this.initialVertexPosition = initialVertexPosition;
        this.currentVertexPosition = currentVertexPosition;
        this.currentVelocity = currentVelocity;
    }

    public Vector3 GetCurrentDisplacement() {
        return currentVertexPosition - initialVertexPosition;
    }

    public void UpdateVelocity(float bounceSpeed) {
        currentVelocity = currentVelocity - GetCurrentDisplacement() * bounceSpeed * Time.deltaTime;
    }

    public void Settle(float stiffness) {
        currentVelocity *= 1f - stiffness * Time.deltaTime;
    }

    public void ApplyPressuretoVertex(Transform transform, Vector3 position, float pressure) {
        Vector3 distanceVerticePoint = currentVertexPosition - transform.InverseTransformPoint(position);
        float adaptedPressure = pressure / (1f + distanceVerticePoint.sqrMagnitude);
        float velocity = adaptedPressure * Time.deltaTime;
        currentVelocity += distanceVerticePoint.normalized * velocity;

    }
}
