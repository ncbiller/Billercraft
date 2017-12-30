using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Realtime.Messaging.Internal;

public class World : MonoBehaviour {

    public GameObject player;

    public Material textureAtlas;
    public static Vector3[,,] allVertices = new Vector3[18, 18, 18];
    public static Vector3[] allNormals = new Vector3[6];
    public enum NDIR { UP, DOWN, LEFT, RIGHT, FRONT, BACK }



    //public static int columnHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 3;
    public static int radius = 6;
    public static ConcurrentDictionary<string, Chunk> chunks;

    //bool building = false;
    bool firstbuild = true;

    CoroutineQueue queue;
    public static uint maxCoroutines = 1000;

    public Vector3 lastbuildPos;

    public static string BuildChunkName(Vector3 position)
    {
        return (int)position.x + "_" + (int)position.y + "_" + (int)position.z;
    }

    void BuildChunkAt(int x, int y, int z)
    {
        Vector3 chunkPosition = new Vector3(x * chunkSize,
                                            y * chunkSize, 
                                            z * chunkSize);
        string n = BuildChunkName(chunkPosition);
        Chunk c;

        if(!chunks.TryGetValue(n, out c))
        {
            c = new Chunk(chunkPosition, textureAtlas);
            c.chunk.transform.parent = this.transform;
            chunks.TryAdd(c.chunk.name, c);
        }



    }

    IEnumerator BuildRecursiveWorld(int x, int y, int z, int rad)
    {
        rad--;

        if (rad <= 0) yield break;

        BuildChunkAt(x, y, z - 1);
        queue.Run(BuildRecursiveWorld(x, y, z - 1, rad));

        yield return null;

        BuildChunkAt(x, y, z + 1);
        queue.Run(BuildRecursiveWorld(x, y, z + 1, rad));

        yield return null;

        BuildChunkAt(x, y + 1, z);
        queue.Run(BuildRecursiveWorld(x, y + 1, z, rad));

        yield return null;

        BuildChunkAt(x, y - 1, z);
        queue.Run(BuildRecursiveWorld(x, y - 1, z, rad));

        yield return null;

        BuildChunkAt(x + 1, y, z);
        queue.Run(BuildRecursiveWorld(x + 1, y , z, rad));
        yield return null;

        BuildChunkAt(x - 1, y, z);
        queue.Run(BuildRecursiveWorld(x - 1, y, z, rad));

        yield return null;
    }

    IEnumerator DrawChunks()
    {
        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW)
            {
                c.Value.DrawChunk();
                c.Value.status = Chunk.ChunkStatus.KEEP;
            }

            yield return null;
        }
    }


    public void BuildNearPlayer()
    {
        StopCoroutine("BuildRecursiveWorld");
        queue.Run(BuildRecursiveWorld((int)(player.transform.position.x / chunkSize),
    (int)(player.transform.position.y / chunkSize),
    (int)(player.transform.position.z / chunkSize), radius));
    }




    void Start()
    {
        //generate all vertices
        for (int z = 0; z <= chunkSize; z++)
            for (int y = 0; y <= chunkSize; y++)
                for (int x = 0; x <= chunkSize; x++)
                {
                    allVertices[x, y, z] = new Vector3(x, y, z);
                }

        allNormals[(int)NDIR.UP] = Vector3.up;
        allNormals[(int)NDIR.DOWN] = Vector3.down;
        allNormals[(int)NDIR.LEFT] = Vector3.left;
        allNormals[(int)NDIR.RIGHT] = Vector3.right;
        allNormals[(int)NDIR.FRONT] = Vector3.forward;
        allNormals[(int)NDIR.BACK] = Vector3.back;


        Vector3 ppos = player.transform.position;
        player.transform.position = new Vector3(ppos.x,
                                                Utils.GenerateHeight(ppos.x, ppos.z) + 1,
                                                ppos.z);

        lastbuildPos = player.transform.position;

        player.SetActive(false);

        firstbuild = true;

        chunks = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;

        queue = new CoroutineQueue(maxCoroutines, StartCoroutine);

        BuildChunkAt((int)(player.transform.position.x / chunkSize),
            (int)(player.transform.position.y / chunkSize),
            (int)(player.transform.position.z / chunkSize));

        queue.Run(DrawChunks());

        queue.Run(BuildRecursiveWorld((int)(player.transform.position.x / chunkSize),
            (int)(player.transform.position.y / chunkSize),
            (int)(player.transform.position.z / chunkSize), radius));
         
    } 
	
	// Update is called once per frame
	void Update () {

        Vector3 movement = lastbuildPos - player.transform.position;

        if(movement.magnitude > chunkSize)
        {
            lastbuildPos = player.transform.position;
            BuildNearPlayer();
        }


        if (!player.activeSelf)
        {
            player.SetActive(true);
            firstbuild = false;
        }

        queue.Run(DrawChunks());
    }
}
