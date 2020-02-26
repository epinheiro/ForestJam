using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionScript : MonoBehaviour
{
    GameObject agent;

    void Awake() {
        agent = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckIfNewFood(GameObject go) {
        if (go.tag == "Food") {
            agent.GetComponent<AnimalScript>().NewFoodFound(go);
        }
    }


    void OnTriggerEnter(Collider other) {
        CheckIfNewFood(other.gameObject);
    }
}
