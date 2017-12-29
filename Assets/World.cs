using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Realtime.Messaging.Internal;

public class World : MonoBehaviour {

    public GameObject player;
    public Material textureAtlas;

    public static int columnHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 1;
    public static int radius = 4;
    public static ConcurrentDictionary<string, Chunk> chunks;

    bool building = false;
    bool firstbuild = true;

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
        yield return null;
    }

    IEnumerator DrawChunks()
    {
        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW)
            {
                c.Value.DrawChunk();
            }

            yield return null;
        }
    }

    

    



    void Start () {
        Vector3 ppos = player.transform.position;
        player.transform.position = new Vector3(ppos.x,
                                                Utils.GenerateHeight(ppos.x,ppos.z) + 1,
                                                ppos.z);

        
        player.SetActive(false);

        firstbuild = true;

        chunks = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;

        BuildChunkAt((int)(player.transform.position.x / chunkSize),
            (int)(player.transform.position.y / chunkSize),
            (int)(player.transform.position.z / chunkSize));

        StartCoroutine(DrawChunks());

        StartCoroutine(BuildRecursiveWorld((int)(player.transform.position.x / chunkSize),
            (int)(player.transform.position.y / chunkSize),
            (int)(player.transform.position.z / chunkSize), radius));


    }
	
	// Update is called once per frame
	void Update () {
        if (!player.activeSelf)
        {
            player.SetActive(true);
            firstbuild = false;
        }
    }
}
