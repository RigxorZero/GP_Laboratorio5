using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SokobanGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    public GameObject goalPrefab;
    public InputField seedInput;
    public int seed = 12345;

    private CustomRandom rand;

    private List<GameObject> spawnedLevel = new List<GameObject>();

    void Start()
    {
        rand = new CustomRandom(seed);
        GenerateLevel();
    }

    public void RegenerateLevel()
    {
        if (int.TryParse(seedInput.text, out int newSeed))
        {
            seed = newSeed;
        }
        DestroyLevel();
        GenerateLevel();
    }

    void GenerateLevel()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (rand.NextFloat() < 0.1f)
                {
                    GameObject obj = Instantiate(wallPrefab, new Vector3(x - (width / 2), 0, y - (height / 2)), Quaternion.identity);
                    spawnedLevel.Add(obj);
                }
            }
        }

        Vector3 playerPos = new Vector3(rand.Range(0, width) - (width / 2), 0, rand.Range(0, height) - (height / 2));
        GameObject player = Instantiate(playerPrefab, playerPos, Quaternion.identity);

        Vector3 goalPos = new Vector3(rand.Range(0, width) - (width / 2), 0, rand.Range(0, height) - (height / 2));
        GameObject goal = Instantiate(goalPrefab, goalPos, Quaternion.identity);

        spawnedLevel.Add(player);
        spawnedLevel.Add(goal);
    }

    void DestroyLevel()
    {
        foreach (var obj in spawnedLevel)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedLevel.Clear();
    }
}

public class CustomRandom
{
    private int seed;

    private const int a = 1664525;
    private const int c = 1013904223;
    private const int m = int.MaxValue;

    public CustomRandom(int seed)
    {
        this.seed = seed;
    }

    private int NextInt()
    {
        seed = (a * seed + c) % m;
        return seed;
    }

    public float NextFloat()
    {
        return (float)NextInt() / (float)m;
    }

    public int Range(int min, int max)
    {
        return min + Mathf.Abs(NextInt()) % (max - min);
    }
}