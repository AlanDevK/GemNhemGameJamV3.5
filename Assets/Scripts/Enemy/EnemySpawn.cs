using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] Transform[] enemySpawnPoints;
    [SerializeField] GameObject enemyPrefab;
    int numberOfEnemies = 1;
    float timer = 0;
    float timeBetweenSpawn = 2f;

    void Update()
    {
        timer+=Time.deltaTime;
    }
    public void SpawnEnemies()
    {
        if (timer >= timeBetweenSpawn)
        {
            foreach (Transform enemySpawnPoint in enemySpawnPoints)
            {
                for (int i = 0; i<numberOfEnemies; i++)
                {
                    Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity);
                }
            }
            numberOfEnemies++;
            timer = 0f;
        }
    }
}
