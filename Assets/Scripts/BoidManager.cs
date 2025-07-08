using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance;

    [SerializeField] private float startDelay = 2f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float timeBetweenSpawns = 1f;
    [SerializeField] private int boidsPerWave = 10;
    [SerializeField] private GameObject boidPrefab;
    [SerializeField] private int poolDefaultCapacity = 50;
    [SerializeField] private int poolMaxSize = 100;

    private float _timeSinceLastSpawn;
    private int _currentBoidsPerWave = 1;

    public HashSet<Rigidbody2D> AllBoidRigidbodies { private set; get; } = new();

    private ObjectPool<GameObject> _boidPool;

    private void Awake()
    {
        Instance = this;
        _timeSinceLastSpawn = -startDelay;

        _boidPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(boidPrefab),
            actionOnGet: (obj) => {
                obj.transform.position = spawnPoint.position;
                obj.transform.rotation = Quaternion.identity;
                obj.SetActive(true);
            },
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: poolDefaultCapacity,
            maxSize: poolMaxSize
        );
    }

    private void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;

        if (_timeSinceLastSpawn > timeBetweenSpawns)
        {
            _timeSinceLastSpawn = 0f;
            Spawn();
        }
    }

    private void Spawn()
    {
        for (int i = 0; i < _currentBoidsPerWave; i++)
        {
            _boidPool.Get();
        }

        if (_currentBoidsPerWave == boidsPerWave) { return; }
        _currentBoidsPerWave++;
    }

    public void ReleaseBoid(GameObject boid)
    {
        _boidPool.Release(boid);
    }

    public void AddBoid(Boid boid)
    {
        AllBoidRigidbodies.Add(boid.GetComponent<Rigidbody2D>());
    }

    public void RemoveBoid(Boid boid)
    {
        AllBoidRigidbodies.Remove(boid.GetComponent<Rigidbody2D>());
    }
}