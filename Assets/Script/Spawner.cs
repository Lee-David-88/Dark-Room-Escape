using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Wave[] waves;
    public Enemy enemy;

    public GameObject item;

    LivingEntity playerEntity;
    Transform PlayerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    int itemRemainingToSpawn;
    int itemRemainingAlive;

    float nextEnemySpawnTime;
    float nextItemSpawnTime;

    MapGenerator map;

    private void Start() {
        playerEntity = FindObjectOfType<Player>();
        PlayerT = playerEntity.transform;

        map = FindObjectOfType<MapGenerator>();
        nextWave();
    }
    private void Update() {
        if (enemiesRemainingToSpawn > 0 && Time.time > nextEnemySpawnTime) {
            enemiesRemainingToSpawn--;
            nextEnemySpawnTime = Time.time + currentWave.timeBtwEnemySpawns;
            StartCoroutine(SpawnEnemy());
        }
        
    }

    void SpawnItem(int numberOfItems) {
        for (int i = 0; i < numberOfItems; i++) {
            Transform randomTile = map.GetRandomOpenTile();
            Instantiate(item, randomTile.position + Vector3.up, Quaternion.identity);
        }
    }

    void SpawnExit() {
        Transform randomTile = map.GetRandomOpenTile();
        Instantiate(item, randomTile.position + Vector3.up, Quaternion.identity);
    }

    IEnumerator SpawnEnemy() {
        float spawnDelay = 1f;
        float tileFlashSpeed = 4f;

        Transform randomTile = map.GetRandomOpenTile();
        Material tileMat = randomTile.GetComponent<Renderer>().material;
        Color initialColour = Color.white;
        Color flashColour = Color.red;
        float spawnTimer = 0;

        while(spawnTimer < spawnDelay) {

            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnEnemy = Instantiate(enemy, randomTile.position + Vector3.up, Quaternion.identity);
        spawnEnemy.OnDeath += OnEnemyDeath;
    }

    void OnEnemyDeath() {
        enemiesRemainingAlive--;

        if (enemiesRemainingAlive == 0) {
            nextWave();
        }
    }

    void randomPlayerPosition() {
        PlayerT.position = map.GetRandomOpenTile().position + Vector3.up * 3;
    }

    void nextWave() {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length) {
            currentWave = waves[currentWaveNumber - 1];
        }
        enemiesRemainingToSpawn = currentWave.enemyCount;
        enemiesRemainingAlive = enemiesRemainingToSpawn;

        itemRemainingToSpawn = currentWave.itemCount;
        itemRemainingAlive = itemRemainingToSpawn;

        randomPlayerPosition();
        SpawnItem(3);
    }

    [System.Serializable]
    public class Wave {
        public int enemyCount;
        public int itemCount;
        public float timeBtwEnemySpawns;
        public float timeBtwItemSpawns;
    }
}
