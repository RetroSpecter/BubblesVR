using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshGenerator : MonoBehaviour
{
    private void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        // create verts and uvs
        Vector3[] verts = new Vector3[24];
        Vector2[] uvs = new Vector2[24];

        // front face
        verts[0] = new Vector3(1.0f, 1.0f, 1.0f);      // UR
        verts[1] = new Vector3(-1.0f, 1.0f, 1.0f);       // UL
        verts[2] = new Vector3(1.0f, -1.0f, 1.0f);     // DR      
        verts[3] = new Vector3(-1.0f, -1.0f, 1.0f);      // DL

        // back face
        verts[4] = new Vector3(-1.0f, 1.0f, -1.0f);      // UL
        verts[5] = new Vector3(1.0f, 1.0f, -1.0f);       // UR
        verts[6] = new Vector3(-1.0f, -1.0f, -1.0f);     // DL      
        verts[7] = new Vector3(1.0f, -1.0f, -1.0f);      // DR

        // left face
        verts[8] = new Vector3(-1.0f, 1.0f, 1.0f);      // UF    
        verts[9] = new Vector3(-1.0f, 1.0f, -1.0f);     // UB
        verts[10] = new Vector3(-1.0f, -1.0f, 1.0f);    // DF          
        verts[11] = new Vector3(-1.0f, -1.0f, -1.0f);   // DB

        // right face
        verts[12] = new Vector3(1.0f, 1.0f, -1.0f);      // UB    
        verts[13] = new Vector3(1.0f, 1.0f, 1.0f);     // UF
        verts[14] = new Vector3(1.0f, -1.0f, -1.0f);     // DB          
        verts[15] = new Vector3(1.0f, -1.0f, 1.0f);    // DF

        // up face
        verts[16] = new Vector3(1.0f, -1.0f, 1.0f);      // RF    
        verts[17] = new Vector3(-1.0f, -1.0f, 1.0f);     // LF
        verts[18] = new Vector3(1.0f, -1.0f, -1.0f);     // RB          
        verts[19] = new Vector3(-1.0f, -1.0f, -1.0f);    // LB

        // down face
        verts[20] = new Vector3(1.0f, 1.0f, -1.0f);      // RB    
        verts[21] = new Vector3(-1.0f, 1.0f, -1.0f);     // LB
        verts[22] = new Vector3(1.0f, 1.0f, 1.0f);     // RF          
        verts[23] = new Vector3(-1.0f, 1.0f, 1.0f);    // LF

        for (int i = 0; i < 24; i += 4)
        {
            uvs[i] = new Vector2(0.0f, 0.0f);
            uvs[i + 1] = new Vector2(1.0f, 0.0f);
            uvs[i + 2] = new Vector2(0.0f, 1.0f);
            uvs[i + 3] = new Vector2(1.0f, 1.0f);
        }

        // assign verts and uvs
        mesh.vertices = verts;
        mesh.uv = uvs;

        // create and assign tris
        mesh.subMeshCount = 6;
        for (int i = 0; i < 6; ++i)
        {
            int startIndex = 4 * i;

            int[] tris = new int[6];
            tris[0] = startIndex + 0;
            tris[1] = startIndex + 1;
            tris[2] = startIndex + 2;
            tris[3] = startIndex + 1;
            tris[4] = startIndex + 3;
            tris[5] = startIndex + 2;

            mesh.SetTriangles(tris, i);
        }
    }
}
