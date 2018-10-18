using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoMesh : MonoBehaviour {

    MeshFilter mesh;
    public StoneController stonePrefab;
    public float minimumSeparation = 1;

    void Start()
    {
        mesh = GetComponentInChildren<MeshFilter>();

        IndexPairs[] edges = GetEdges();
        float closest = 1;
        foreach (IndexPairs pair in edges)
        {
            Vector3 a = mesh.mesh.vertices[pair.a];
            Vector3 b = mesh.mesh.vertices[pair.b];
            float d = (a - b).sqrMagnitude;
            if (d < closest) closest = d;
        }
        float separation = minimumSeparation / Mathf.Sqrt(closest);
        mesh.transform.localScale = Vector3.one * separation;

        StoneController[] stones = new StoneController[mesh.mesh.vertices.Length];
        for (int i = 0; i < mesh.mesh.vertices.Length; i++)
        {
            Vector3 v = mesh.mesh.vertices[i] * separation;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, mesh.mesh.normals[i]);
            StoneController stone = Instantiate(stonePrefab, v, rot, transform);
            stone.SetGameState(0);
            stones[i] = stone;
        }

        foreach (IndexPairs pair in edges)
        {
            StoneController a = stones[pair.a];
            StoneController b = stones[pair.b];
            a.AddNeighbor(b);
            b.AddNeighbor(a);
        }
    }
    private IndexPairs[] GetEdges()
    {
        IndexPairs[] edges = new IndexPairs[mesh.mesh.triangles.Length];
        for (int i = 0; i < mesh.mesh.triangles.Length; i += 3)
        {
            int index1 = mesh.mesh.triangles[i];
            int index2 = mesh.mesh.triangles[i + 1];
            int index3 = mesh.mesh.triangles[i + 2];

            edges[i] = new IndexPairs(index1, index2);
            edges[i + 1] = new IndexPairs(index1, index3);
            edges[i + 2] = new IndexPairs(index2, index3);
        }
        return edges;
    }

    struct IndexPairs
    {
        public int a { get; private set; }
        public int b { get; private set; }

        public IndexPairs(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
        public bool IsSameAs(int a, int b)
        {
            return (this.a == a && this.b == b) || (this.a == b && this.b == a);
        }
    }
    void MakeBuddies(StoneController stone1, StoneController stone2, StoneController stone3)
    {
        stone1.AddNeighbor(stone2);
        stone1.AddNeighbor(stone3);
        stone2.AddNeighbor(stone1);
        stone2.AddNeighbor(stone3);
        stone3.AddNeighbor(stone1);
        stone3.AddNeighbor(stone2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
