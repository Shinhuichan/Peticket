using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObject : MonoBehaviour
{
    private void OnEnable()
    {
        AnimalLogic animal = FindObjectOfType<AnimalLogic>();

        if(animal != null)
        {
            animal.OnFeedSpawned(gameObject);
        }
    }
}
