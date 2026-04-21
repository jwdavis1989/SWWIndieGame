using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BladeTrail : MonoBehaviour
{
    public Transform bladeTip;
    public Transform bladeHilt;
    public float trailLifespan = 0.5f; //Time until a segment despawns in seconds
    public float minDistance = 0.05f;  //Low = performance, high = smooth appearance
    
    [Header("Elemental Materials")]
    public Material[] elementalMaterials;
    private MeshRenderer meshRenderer;

    private Mesh bladeVFXMesh;
    private List<TrailPoint> trailPoints = new List<TrailPoint>();

    struct TrailPoint {
        public Vector3 bladeTipPos, bladeHiltPos;
        public float timeCreated;
    }

    void Start() { 
        bladeVFXMesh = new Mesh(); 
        GetComponent<MeshFilter>().mesh = bladeVFXMesh; 
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Awake()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    void LateUpdate() {
        // 1. Remove old points (The "Going Away" logic)
        trailPoints.RemoveAll(p => Time.time - p.timeCreated > trailLifespan);

        // 2. Add new points with Interpolation (The "Jagged" fix)
        if (trailPoints.Count == 0 || Vector3.Distance(bladeTip.position, trailPoints[0].bladeTipPos) > minDistance) {
            trailPoints.Insert(0, new TrailPoint { 
                bladeTipPos = bladeTip.position, 
                bladeHiltPos = bladeHilt.position, 
                timeCreated = Time.time 
            });
        }
        
        UpdateMesh();
    }

    void UpdateMesh() {
        bladeVFXMesh.Clear();
        if (trailPoints.Count < 2) return;

        Vector3[] vertices = new Vector3[trailPoints.Count * 2];
        Vector2[] uvs = new Vector2[trailPoints.Count * 2];
        Color[] colors = new Color[trailPoints.Count * 2]; //Used for trail fading
        int[] triangles = new int[(trailPoints.Count - 1) * 6];

        for (int i = 0; i < trailPoints.Count; i++) {
            vertices[i * 2] = transform.InverseTransformPoint(trailPoints[i].bladeHiltPos);
            vertices[i * 2 + 1] = transform.InverseTransformPoint(trailPoints[i].bladeTipPos);
            
            float agePercent = 1f - ((Time.time - trailPoints[i].timeCreated) / trailLifespan);
            colors[i * 2] = colors[i * 2 + 1] = new Color(1, 1, 1, agePercent);
            
            float xUV = (float)i / (trailPoints.Count - 1);
            uvs[i * 2] = new Vector2(xUV, 0);
            uvs[i * 2 + 1] = new Vector2(xUV, 1);

            if (i < trailPoints.Count - 1) {
                int t = i * 6, v = i * 2;
                triangles[t] = v; triangles[t + 1] = v + 1; triangles[t + 2] = v + 2;
                triangles[t + 3] = v + 2; triangles[t + 4] = v + 1; triangles[t + 5] = v + 3;
            }
        }
        bladeVFXMesh.vertices = vertices;
        bladeVFXMesh.uv = uvs;
        bladeVFXMesh.colors = colors;
        bladeVFXMesh.triangles = triangles;
    }

    public void SetElementalTrailMaterial(int highestElementStatIndex)
    {
        if (elementalMaterials != null && highestElementStatIndex >= 0 && highestElementStatIndex < elementalMaterials.Length)
        {
            meshRenderer.material = elementalMaterials[highestElementStatIndex];
        }
        else
        {
            Debug.LogWarning("Elemental trail material index out of range or empty.");
        }
    }

}
