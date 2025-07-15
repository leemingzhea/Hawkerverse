using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitCrate : MonoBehaviour
{
    public GameObject fruitPrefab;
    public int maxFruitCount = 5;
    public Transform spawnPoint;

    private List<GameObject> spawnedFruits = new List<GameObject>();
    private Camera playerCamera;
    public float interactionDistance = 3f;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsPlayerLookingAtCrate())
            {
                SpawnFruit();
            }
        }

        // Clean up any destroyed or null fruit objects from the list
        spawnedFruits.RemoveAll(fruit => fruit == null);
    }

    bool IsPlayerLookingAtCrate()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            return hit.collider != null && hit.collider.gameObject == this.gameObject;
        }
        return false;
    }

    void SpawnFruit()
    {
        if (spawnedFruits.Count >= maxFruitCount)
        {
            Debug.Log("Fruit limit reached for this crate.");
            return;
        }

        GameObject fruit = Instantiate(fruitPrefab, spawnPoint.position, spawnPoint.rotation);
        fruit.name = fruitPrefab.name; // Ensure name like "apple", not "apple (Clone)"

        spawnedFruits.Add(fruit);
    }
}