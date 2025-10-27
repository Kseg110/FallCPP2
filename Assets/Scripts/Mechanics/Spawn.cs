using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject[] gameObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObjects.Length == 0)
        {
            Debug.LogWarning("No game objects assigned to spawn.");
            return;
        }

        int RandNum = Random.Range(0, gameObjects.Length);
        if (gameObjects[RandNum] == null)
        {
            Debug.LogWarning("Selected game object is null.");
            return;
        }

        Instantiate(gameObjects[RandNum], transform.position, Quaternion.identity);
    }
}