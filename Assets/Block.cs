using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

    enum Cubeside { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };
    public enum BlockType { GRASS, DIRT, STONE, DIAMOND, BEDROCK, REDSTONE, AIR };

    public bool isSolid;

    BlockType bType;

    Material cubeMaterial;
    GameObject parent;
    Vector3 position;
    Chunk owner;

    int bx;
    int by;
    int bz;

    Vector2[,] blockUVs =
    {
        /*GRASS TOP*/   {new Vector2( 0.125f, 0.375f), new Vector2(0.1875f, 0.375f),
                            new Vector2(0.125f, 0.4375f), new Vector2(0.1875f, 0.4375f)},
        /*GRASS SIDE*/   {new Vector2( 0.1875f, 0.9375f), new Vector2(0.25f, 0.9375f),
                            new Vector2(0.1875f,1.0f), new Vector2(0.25f, 1.0f)},
        /*DIRT*/   {new Vector2( 0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f),
                            new Vector2(0.125f,1.0f), new Vector2(0.1875f, 1.0f)},
        /*STONE*/   {new Vector2( 0, 0.875f), new Vector2(0.0625f, 0.875f),
                            new Vector2(0,0.9375f), new Vector2(0.0625f, 0.9375f)},
        /*DIAMOND*/   {new Vector2( 0.125f, 0.75f), new Vector2(0.1875f, 0.75f),
                            new Vector2(0.125f,0.8125f), new Vector2(0.1875f, 0.8125f)},
        /*BEDROCK*/   {new Vector2( 0.25f, 0.4375f), new Vector2(0.3125f, 0.4375f),
                            new Vector2(0.25f,0.5f), new Vector2(0.3125f, 0.5f)},
        /*REDSTONE*/   {new Vector2( 0.1875f, 0.75f), new Vector2(0.25f, 0.75f),
                            new Vector2(0.1875f,0.8125f), new Vector2(0.25f, 0.8125f)}
    };

    public Block(BlockType b, Vector3 pos, GameObject p, Chunk c)
    {
        bType = b;
        parent = p;
        position = pos;
        owner = c;
        if (bType == BlockType.AIR)
            isSolid = false;
        else
            isSolid = true;
        bx = (int)pos.x;
        by = (int)pos.y;
        bz = (int)pos.z;
    }

    void CreateQuad(Cubeside side, List<Vector3> v, List<Vector3> n, List<Vector2> u, List<int> t)
    {
        //Mesh mesh = new Mesh();
        //mesh.name = "ScriptedMesh" + side.ToString();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];

        //all possible UVs
        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        if (bType == BlockType.GRASS && side == Cubeside.TOP)
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }
        else if (bType == BlockType.GRASS && side == Cubeside.BOTTOM)
        {
            uv00 = blockUVs[(int)(BlockType.DIRT + 1), 0];
            uv10 = blockUVs[(int)(BlockType.DIRT + 1), 1];
            uv01 = blockUVs[(int)(BlockType.DIRT + 1), 2];
            uv11 = blockUVs[(int)(BlockType.DIRT + 1), 3];
        }
        else
        {
            uv00 = blockUVs[(int)(bType + 1), 0];
            uv10 = blockUVs[(int)(bType + 1), 1];
            uv01 = blockUVs[(int)(bType + 1), 2];
            uv11 = blockUVs[(int)(bType + 1), 3];
        }


        //all possible vertices 
        Vector3 p0 = World.allVertices[bx, by, bz + 1];
        Vector3 p1 = World.allVertices[bx + 1, by, bz + 1];
        Vector3 p2 = World.allVertices[bx + 1, by, bz];
        Vector3 p3 = World.allVertices[bx, by, bz];
        Vector3 p4 = World.allVertices[bx, by + 1, bz + 1];
        Vector3 p5 = World.allVertices[bx + 1, by + 1, bz + 1];
        Vector3 p6 = World.allVertices[bx + 1, by + 1, bz];
        Vector3 p7 = World.allVertices[bx, by+1, bz];

        int trioffset = 0;

        switch (side)
        {
            case Cubeside.BOTTOM:

                trioffset = v.Count;
                v.Add(p0); v.Add(p1); v.Add(p2); v.Add(p3);
                n.Add(World.allNormals[(int)World.NDIR.DOWN]);
                n.Add(World.allNormals[(int)World.NDIR.DOWN]);
                n.Add(World.allNormals[(int)World.NDIR.DOWN]);
                n.Add(World.allNormals[(int)World.NDIR.DOWN]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                break;
            case Cubeside.TOP:
                trioffset = v.Count;
                v.Add(p7); v.Add(p6); v.Add(p5); v.Add(p4);
                n.Add(World.allNormals[(int)World.NDIR.UP]);
                n.Add(World.allNormals[(int)World.NDIR.UP]);
                n.Add(World.allNormals[(int)World.NDIR.UP]);
                n.Add(World.allNormals[(int)World.NDIR.UP]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);
                break;
            case Cubeside.LEFT:
                trioffset = v.Count;
                v.Add(p7); v.Add(p4); v.Add(p0); v.Add(p3);
                n.Add(World.allNormals[(int)World.NDIR.LEFT]);
                n.Add(World.allNormals[(int)World.NDIR.LEFT]);
                n.Add(World.allNormals[(int)World.NDIR.LEFT]);
                n.Add(World.allNormals[(int)World.NDIR.LEFT]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);
                break;
            case Cubeside.RIGHT:
                trioffset = v.Count;
                v.Add(p5); v.Add(p6); v.Add(p2); v.Add(p1);
                n.Add(World.allNormals[(int)World.NDIR.RIGHT]);
                n.Add(World.allNormals[(int)World.NDIR.RIGHT]);
                n.Add(World.allNormals[(int)World.NDIR.RIGHT]);
                n.Add(World.allNormals[(int)World.NDIR.RIGHT]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                break;
            case Cubeside.FRONT:
                trioffset = v.Count;
                v.Add(p4); v.Add(p5); v.Add(p1); v.Add(p0);
                n.Add(World.allNormals[(int)World.NDIR.FRONT]);
                n.Add(World.allNormals[(int)World.NDIR.FRONT]);
                n.Add(World.allNormals[(int)World.NDIR.FRONT]);
                n.Add(World.allNormals[(int)World.NDIR.FRONT]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);
                break;
            case Cubeside.BACK:
                trioffset = v.Count;
                v.Add(p6); v.Add(p7); v.Add(p3); v.Add(p2);
                n.Add(World.allNormals[(int)World.NDIR.BACK]);
                n.Add(World.allNormals[(int)World.NDIR.BACK]);
                n.Add(World.allNormals[(int)World.NDIR.BACK]);
                n.Add(World.allNormals[(int)World.NDIR.BACK]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);
                break;
        }


    }

    public bool  HasSolidNeighbour(int x, int y, int z)
    {
        Block[,,] blocks;

        if (x < 0 || x >= World.chunkSize ||
            y < 0 || y >= World.chunkSize ||
            z < 0 || z >= World.chunkSize)
        { // block in neighbouring chunk
            Vector3 neighbourChunkPos = this.parent.transform.position + new Vector3((x - (int)position.x) * World.chunkSize,
                                                                                 (y - (int)position.y) * World.chunkSize,
                                                                                 (z - (int)position.z) * World.chunkSize);
            string nName = World.BuildChunkName(neighbourChunkPos);

            x = ConvertBlockIndexToLocal(x);
            y = ConvertBlockIndexToLocal(y);
            z = ConvertBlockIndexToLocal(z);

            Chunk nChunk;

            if (World.chunks.TryGetValue(nName, out nChunk))
            {
                blocks = nChunk.chunkData;
            }
            else
                return false;

        }
        else
            blocks = owner.chunkData;


        try
        {
            return blocks[x, y, z].isSolid;
        }
        catch (System.IndexOutOfRangeException ex) { }

        return false;

    }

    private int ConvertBlockIndexToLocal(int i)
    {
        if (i == -1)
            i = World.chunkSize - 1;
        else if (i == World.chunkSize)
            i = 0;
        return i;
    }

    public void Draw(List<Vector3> v,List<Vector3> n, List<Vector2> u, List<int> t)
    {
        if (bType == BlockType.AIR) return;

        if(!HasSolidNeighbour((int)position.x, (int)position.y, (int)position.z + 1))
            CreateQuad(Cubeside.FRONT,v,n,u,t);
        if (!HasSolidNeighbour((int)position.x, (int)position.y, (int)position.z - 1))
            CreateQuad(Cubeside.BACK, v, n, u, t);
        if (!HasSolidNeighbour((int)position.x, (int)position.y+1, (int)position.z))
            CreateQuad(Cubeside.TOP, v, n, u, t);
        if (!HasSolidNeighbour((int)position.x, (int)position.y-1, (int)position.z))
            CreateQuad(Cubeside.BOTTOM, v, n, u, t);
        if (!HasSolidNeighbour((int)position.x-1, (int)position.y, (int)position.z))
            CreateQuad(Cubeside.LEFT, v, n, u, t);
        if (!HasSolidNeighbour((int)position.x+1, (int)position.y, (int)position.z))
            CreateQuad(Cubeside.RIGHT, v, n, u, t);
    }
    
}
