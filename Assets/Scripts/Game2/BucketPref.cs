using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketPref : MonoBehaviour
{
    Color toPaint;
    public GameObject pelletPrefab;

    [HideInInspector]
    public float dir = 1f;
    float speed = 1.0f;
    float distance = 9f;

    float spawnTime = 0f;
    float cTimeToSpawn = 0f;

    private void Start()
    {
        toPaint = Game2Mng.Instance.colorChoice;
        speed = distance / Game2Mng.Instance.restTime;
        Destroy(gameObject, Game2Mng.Instance.restTime);
    }

    void Update()
    {
        float dist = Time.deltaTime * speed * dir;
        transform.Translate(Vector2.right*dist);
        cTimeToSpawn -= Time.deltaTime;
        if (cTimeToSpawn <= 0f)
        {
            GameObject pellet = Instantiate(pelletPrefab, transform.position, Quaternion.identity);
            pellet.GetComponentInChildren<SpriteRenderer>().color = toPaint;
            spawnTime += Random.Range(0, Time.deltaTime/20);
            cTimeToSpawn = spawnTime;
        }
    }
}
