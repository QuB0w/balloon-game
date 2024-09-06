using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SpawnManager : MonoBehaviour
{
    [Header("OBJECTS")]
    [SerializeField] private GameObject _coin;
    [SerializeField] private GameObject[] _obstacles;

    public float _timeToSpawnObstacles;
    public float _timeToSpawnCoins;

    private void Start()
    {
        StartCoroutine(spawnObstacles());
        StartCoroutine(spawnCoins());
    }

    private IEnumerator spawnCoins()
    {
        yield return new WaitForSeconds(_timeToSpawnCoins);
        var first = Random.Range(-6f, 6f);
        Instantiate(_coin, new Vector3(first, 15, 0), Quaternion.identity);
        StartCoroutine(spawnCoins());
    }

    private IEnumerator spawnObstacles()
    {
        int rnd = Random.Range(0, _obstacles.Length);
        switch (rnd)
        {
            case 0:
                var first = Random.Range(-4.7f, 4.7f);
                Instantiate(_obstacles[0], new Vector3(first, 13, 0), Quaternion.identity);
                break;
            case 1:
                var second = Random.Range(-3.2f, 3.2f);
                Instantiate(_obstacles[0], new Vector3(second, 13, 0), Quaternion.identity);
                break;
            case 2:
                var third = Random.Range(-5.8f, 5.8f);
                Instantiate(_obstacles[0], new Vector3(third, 13, 0), Quaternion.identity);
                break;
            case 3:
                var fourth = Random.Range(-3.4f, 3.4f);
                Instantiate(_obstacles[0], new Vector3(fourth, 13, 0), Quaternion.identity);
                break;
        }
        yield return new WaitForSeconds(_timeToSpawnObstacles);
        StartCoroutine(spawnObstacles());
    }
}
