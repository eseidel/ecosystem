﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Min(1)]
    public int mapSize;
    public Transform ground;
    public GameObject foodPrefab;
    public GameObject preyPrefab;
    public GameObject predatorPrefab;

    [Min(0)]
    public int initialPrey;
    public int initialPredators;

    [Range(0, 1)]
    public float initialFoodDensity = 0.02f;
    [Range(0, 1)]
    public float maxFoodDensity = 0.1f;

    public float foodPerSecond = 1; // per second;

    public Evolvable initialPreyTraits;
    public Evolvable initialPredatorTraits;

    int foodLimit;
    float nextFoodSpawn;

    void SpawnPreyAtRandomLocation()
    {
        Vector3 location = new Vector3(Random.value * mapSize - .5f * mapSize, 0, Random.value * mapSize - .5f * mapSize);
        SpawnPrey(location, initialPreyTraits);
    }

    void SpawnPrey(Vector3 location, Evolvable traits)
    {
        var go = Instantiate<GameObject>(preyPrefab, location, Quaternion.identity);
        go.transform.parent = transform;
        var animal = go.GetComponent<Animal>();
        animal.traits = traits;
        animal.OnReproduce += PreyReproduced;
    }

    void SpawnPredatorAtRandomLocation()
    {
        Vector3 location = new Vector3(Random.value * mapSize - .5f * mapSize, 0, Random.value * mapSize - .5f * mapSize);
        SpawnPredator(location, initialPredatorTraits);
    }

    void SpawnPredator(Vector3 location, Evolvable traits)
    {
        var go = Instantiate<GameObject>(predatorPrefab, location, Quaternion.identity);
        go.transform.parent = transform;
        var animal = go.GetComponent<Animal>();
        animal.traits = traits;
        animal.OnReproduce += PredatorReproduced;
    }

    void SpawnFoodAtRandomLocation()
    {
        Vector3 location = new Vector3(Random.value * mapSize - .5f * mapSize, 0, Random.value * mapSize - .5f * mapSize);
        var food = Instantiate<GameObject>(foodPrefab, location, Quaternion.identity);
        food.transform.parent = transform;
    }

    void PredatorReproduced(GameObject parent1, GameObject parent2)
    {
        Animal animal1 = parent1.GetComponent<Animal>();
        Animal animal2 = parent2.GetComponent<Animal>();
        SpawnPredator(parent1.transform.position, animal1.traits.MergeWith(animal2.traits));
    }

    void PreyReproduced(GameObject parent1, GameObject parent2)
    {
        Animal animal1 = parent1.GetComponent<Animal>();
        Animal animal2 = parent2.GetComponent<Animal>();
        SpawnPrey(parent1.transform.position, animal1.traits.MergeWith(animal2.traits));
    }

    int CountFromDensity(float density)
    {
        return Mathf.RoundToInt(density * mapSize * mapSize);
    }

    void Start()
    {
        ground.localScale = new Vector3(mapSize, 1, mapSize);
        int initialFood = CountFromDensity(initialFoodDensity);
        for (int i = 0; i < initialFood; i++)
        {
            SpawnFoodAtRandomLocation();
        }
        foodLimit = CountFromDensity(maxFoodDensity);

        for (int i = 0; i < initialPrey; i++)
        {
            SpawnPreyAtRandomLocation();
        }

        for (int i = 0; i < initialPredators; i++)
        {
            SpawnPredatorAtRandomLocation();
        }
    }

    int CountLivePlants()
    {
        return GameObject.FindGameObjectsWithTag("Plant").Length;
    }

    void Update()
    {
        if (nextFoodSpawn < Time.time && CountLivePlants() < foodLimit)
        {
            SpawnFoodAtRandomLocation();
            float timeBetweenSpawns = 1f / foodPerSecond;
            nextFoodSpawn = Time.time + 0.5f * timeBetweenSpawns + Random.value * timeBetweenSpawns;
        }
    }
}
