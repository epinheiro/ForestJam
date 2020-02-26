using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

/// <summary>
/// TODO LIST
/// - Problema de concorrência
/// -- Mesmos agentes indo atrás do mesmo recurso
/// -- Agentes perdendo referência do que querem fazer
/// -- Agentes morrendo espontaneamente de fome
/// -- Problemas no ciclo de decisão?
/// -- Problemas na "IA" que decide aonde o agente vai procurar comida?
/// -- Alterar colisor de proximidade (mudando o scale)
/// </summary>

public class AnimalScript : MonoBehaviour
{
    public FoodData foodData;

    // Game meta-data
    public GameObject motherNatureRef;
    float timeFromLastUpdate;
    readonly float updateFrequency = 1;

    // Subject meta-data
    readonly float energyLimit = 100;
    float energy;
    NavMeshAgent navAgent;

    // Awareness meta-data
    GameObject PerceptionSphere;
    List<GameObject> foodList;
    readonly float SEARCH_RADIUS = 20;
    readonly float MIN_INTERACTION_PROXIMITY = 3;
    bool isSearchingFood = false;
    GameObject currentFoodPosition;


    void Awake()
    {
        timeFromLastUpdate = 0;

        PerceptionSphere = this.transform.GetChild(0).gameObject;
        foodList = new List<GameObject>();

        energy = 50;

        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Food searching
        SearchFood();
        if (currentFoodPosition != null) EatIfCan();

        // Mate searching

        // The galloping death race
        UpdateInternalState();
        CheckDeathConditions();
    }

    void EatIfCan() {
        float distance = Vector3.Distance(currentFoodPosition.transform.position, this.transform.position);

        if (distance < MIN_INTERACTION_PROXIMITY) {
            int nutrition = foodData.nutrition;
            foodList.Remove(currentFoodPosition);
            navAgent.destination = Vector3.zero;
            Destroy(currentFoodPosition.gameObject);
            energy = Mathf.Min(energy + nutrition, energyLimit);        
            currentFoodPosition = null;
            isSearchingFood = false;
            Debug.Log("Eating ("+energy+")");
            ChangeScale(energy);
        }
    }

    void ChangeScale(float factor) {
        Vector3 scale = this.transform.localScale;
        this.transform.localScale = Vector3.one * Mathf.Lerp(0.5F, 4.0F, factor/100);
    }

    void SearchFood() {
        if (foodList.Count != 0) {
            // Go to nearest food
            currentFoodPosition = GetNearestFood();
            if (currentFoodPosition != null) {
                navAgent.destination = currentFoodPosition.transform.position;
            }
        } else {
            currentFoodPosition = null;
            if (!isSearchingFood) {
                // Search food
                navAgent.destination = GetARandomSearchLocation();
                isSearchingFood = true;
            } else {
                // Check if arrived
                if (IsGoalCloseEnough()) {
                    isSearchingFood = false;
                }
            }
        }
    }

    GameObject GetNearestFood() {
        GameObject nearestFood = null;
        float minDistance = 10000;
        
        foreach (GameObject food in foodList) {
            if (food != null) {
                float dist = Vector3.Distance(food.transform.position, this.transform.position);

                if (minDistance > dist) {
                    minDistance = dist;
                    nearestFood = food;
                }
            } else {
                Debug.Log("Delete fooed");
                nearestFood = null;
                minDistance = 10000;
            }
        }

        return nearestFood;
    }

    bool IsGoalCloseEnough() {
        float dist = Vector3.Distance(navAgent.destination, transform.position);
        if (dist < MIN_INTERACTION_PROXIMITY) return true;
        else return false;
    }

    Vector3 GetARandomSearchLocation() {
        float terrainDimension = motherNatureRef.GetComponent<MotherNatureScript>().DIMENSION_CONST;
        float xAnimalPos = this.transform.position.x;
        float zAnimalPos = this.transform.position.z;

        //Debug.Log("X " + (xAnimalPos - SEARCH_RADIUS).ToString() + " " + (xAnimalPos + SEARCH_RADIUS).ToString());
        //Debug.Log("Z " + (zAnimalPos - SEARCH_RADIUS).ToString() + " " + (zAnimalPos + SEARCH_RADIUS).ToString());

        float xMin = Mathf.Max(xAnimalPos - SEARCH_RADIUS, -terrainDimension);
        float xMax = Mathf.Min(xAnimalPos + SEARCH_RADIUS, terrainDimension);
        float zMin = Mathf.Max(zAnimalPos - SEARCH_RADIUS, -terrainDimension);
        float zMax = Mathf.Min(zAnimalPos + SEARCH_RADIUS, terrainDimension);

        float xAxis = UnityEngine.Random.Range(xMin, xMax);
        float zAxis = UnityEngine.Random.Range(zMin, zMax);

        //Debug.Log(xAxis + ", " + zAxis);

        return new Vector3(xAxis, 0, zAxis);
    }

    // Life checkers
    void UpdateInternalState() {
        if (timeFromLastUpdate > 1 / updateFrequency) {
            Debug.Log("Energy (" + energy + ")");
            energy--;
            timeFromLastUpdate = 0;
            ChangeScale(energy);
        } else {
            timeFromLastUpdate += Time.deltaTime;
        }
    }

    void CheckDeathConditions() {
        if (energy < 0) {
            Destroy(this.gameObject);
        }
    }

    // Events
    public void NewFoodFound(GameObject foodReference) {
        if (!foodList.Contains(foodReference)) { // TODO check necessity
            foodList.Add(foodReference);
        }
    }
}
