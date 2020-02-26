using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherNatureScript : MonoBehaviour
{
    // Meta-game data
    [Range(1, 10)] public float timeScale = 1; // TODO to use the time scale

    // Terrain data
    public GameObject terrain;
    Transform terrainTransform;
    readonly public float DIMENSION_CONST = 40;

    // Food data
    public GameObject foodPrefab;
    [Range(1, 10)] public float foodDropRatePerSecond = 1;
    float timeFromLastFoodDropped;

    // Animal data
    public GameObject animalPrefab;
    [Range(0, 50)] public int startingAnimals = 10;

    void Awake() {
        Time.timeScale = 1;  
        terrainTransform = terrain.transform;
        timeFromLastFoodDropped = 0;
        GenerateRandomInitialAnimals();
    }

    
    void GenerateRandomInitialAnimals() {
        for(int i = 0; i<startingAnimals; i++) {
            float xAxis = Random.Range(-DIMENSION_CONST, DIMENSION_CONST);
            float zAxis = Random.Range(-DIMENSION_CONST, DIMENSION_CONST);
            Instantiate(
                animalPrefab,
                new Vector3(xAxis, 0, zAxis),
                Quaternion.identity
            );
        }
    }

    void CreateRandomNewFood() {
        float xAxis = Random.Range(-DIMENSION_CONST, DIMENSION_CONST);
        float zAxis = Random.Range(-DIMENSION_CONST, DIMENSION_CONST);

        Instantiate(
            foodPrefab, 
            new Vector3(xAxis, 0, zAxis), 
            Quaternion.identity
        );
    }

    void ProcessFoodGeneration() {
        if (timeFromLastFoodDropped > 1 / foodDropRatePerSecond) {
            CreateRandomNewFood();
            timeFromLastFoodDropped = 0;
        } else {
            timeFromLastFoodDropped = timeFromLastFoodDropped + Time.deltaTime;
        }
    }

    void UpdateTimeScale(float newTimeScale) {
        if (Time.timeScale != newTimeScale) {
            Time.timeScale = newTimeScale;
        }
    }




    void Update()
    {
        ProcessFoodGeneration();
        UpdateTimeScale(timeScale);
    }
}