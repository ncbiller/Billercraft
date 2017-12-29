using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

    static int maxHeight = 150;
    static float smooth = 0.01f;
    static int octaves = 4;
    static float persistence = 0.5f;

    public static int GenerateStoneHeight(float x, float z)
    {
        float height = Map(0, maxHeight-5, 0, 1, fBM(x * smooth*2, z * smooth*2, octaves+3, persistence));
        return (int)height;
    }


    public static int GenerateHeight(float x, float z)
    {
        float height = Map(0, maxHeight, 0, 1, fBM(x * smooth, z * smooth, octaves, persistence));
        return (int)height;
    }

    public static float fBM3(float x, float y, float z, int octaves = 3, float sm = 0.01f, float persistence = 0.5f)
    {
        float XY = fBM(x * sm, y * sm, octaves, persistence);
        float YX = fBM(y * sm, x * sm, octaves, persistence);
        float XZ = fBM(x * sm, z * sm, octaves, persistence);

        float ZX = fBM(z * sm, x * sm, octaves, persistence);
        float ZY = fBM(z * sm, y * sm, octaves, persistence);
        float YZ = fBM(y * sm, z * sm, octaves, persistence);

        return (XY + YX + XZ + ZX + ZY + YZ) / 6.0f;

    }


    static float Map(float min, float max, float omin, float omax, float value)
    {
        return Mathf.Lerp(min, max, Mathf.InverseLerp(omin, omax, value));
    }

    static float fBM(float x, float z, int octaves, float persistence)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }

        return total / maxValue;
    }
}
