using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Chunk {

    public Material cubeMaterial;
    public Block[,,] chunkData;
    public GameObject chunk;
    public enum ChunkStatus {DRAW,DONE,KEEP};
    public ChunkStatus status;

    List<Vector3> Verts = new List<Vector3>();
    List<Vector3> Norms = new List<Vector3>();
    List<Vector2> UVs = new List<Vector2>();
    List<int> Tris = new List<int>();

    void BuildChunk()
    {

        //Stopwatch stopWatch = new Stopwatch();
        //stopWatch.Start();


        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];

        //Create blocks
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    int worldX = (int)(x + chunk.transform.position.x);
                    int worldY = (int)(y + chunk.transform.position.y);
                    int worldZ = (int)(z + chunk.transform.position.z);

                    
                    if (worldY <= 0)
                        chunkData[x, y, z] = new Block(Block.BlockType.BEDROCK, pos,
                            chunk.gameObject, this);
                    else if (Utils.fBM3(worldX, worldY, worldZ,3,0.1f) < 0.42f)
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos,
                            chunk.gameObject, this);


                    else if (worldY <= Utils.GenerateStoneHeight(worldX, worldZ))
                    {
                        if (Utils.fBM3(worldX, worldY, worldZ, 2, 0.01f) < 0.36f && worldY < 40)
                            chunkData[x, y, z] = new Block(Block.BlockType.DIAMOND, pos,
                                chunk.gameObject, this);
                        else if (worldY < 20 && Utils.fBM3(worldX, worldY, worldZ, 3, 0.03f) < 0.41f)
                            chunkData[x, y, z] = new Block(Block.BlockType.REDSTONE, pos,
                                chunk.gameObject, this);
                        else
                            chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos,
                                chunk.gameObject, this);
                    } 
                    else if (worldY == Utils.GenerateHeight(worldX, worldZ))
                        chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos,
                            chunk.gameObject, this);
                    else if (worldY < Utils.GenerateHeight(worldX, worldZ))
                        chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos,
                            chunk.gameObject, this);
                    else
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos,
                            chunk.gameObject, this);
                    status = ChunkStatus.DRAW;
                }
        //stopWatch.Stop();


        //UnityEngine.Debug.Log("Chunk Built: " + chunk.name + " Time: " + stopWatch.Elapsed.Milliseconds);
    }

    public void DrawChunk() {
        //Stopwatch stopWatch = new Stopwatch();
        //stopWatch.Start();

        Verts.Clear();
        Norms.Clear();
        UVs.Clear();
        Tris.Clear();


        for (int z = 0; z < World.chunkSize; z++)
            for(int y = 0; y< World.chunkSize; y++)
                for(int x = 0; x< World.chunkSize; x++)
                {
                    chunkData[x,y,z].Draw(Verts, Norms, UVs, Tris);
                }
        Mesh mesh = new Mesh();
        mesh.name = "ScriptedMesh";

        mesh.vertices = Verts.ToArray();
        mesh.normals = Norms.ToArray();
        mesh.uv = UVs.ToArray();
        mesh.triangles = Tris.ToArray();

        mesh.RecalculateBounds();

        MeshFilter meshFilter = (MeshFilter)chunk.gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer renderer = chunk.gameObject.AddComponent<MeshRenderer>();
        renderer.material = cubeMaterial;

        MeshCollider collider = chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        collider.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;
        status = ChunkStatus.DONE;
        //stopWatch.Stop();

        //UnityEngine.Debug.Log("Chunk Rendered: " + chunk.name + " Time: " + stopWatch.Elapsed.Milliseconds);
    }

    // Use this for initialization
    public Chunk (Vector3 position, Material c)
    {
        chunk = new GameObject(World.BuildChunkName(position));
        chunk.transform.position = position;
        cubeMaterial = c;
        BuildChunk();
    }

    void CombineQuads()
    {
        //1. Combine all children meshes
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter)chunk.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        //3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
        MeshRenderer renderer = chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = cubeMaterial;

        //5. Delete all uncombined children
        foreach (Transform quad in chunk.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }


	
	
}
