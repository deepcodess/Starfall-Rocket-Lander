using System.Collections.Generic;
using UnityEngine;

public class FallingStarManager : MonoBehaviour
{
    [SerializeField] private Transform fallingStarPrefab;
    [SerializeField] private List<Transform> spawnPositionList;


    private void Start()
    {
        FunctionPeriodic.Create(SpawnFallingStar, 1f);
    }

    private void SpawnFallingStar()
    {
        if (!RandomData.TestChance(2))
        {
            return;
        }

        Transform randomSpawnPosition = spawnPositionList.GetRandomElement();
        Transform fallingStarTransform = Instantiate(fallingStarPrefab, randomSpawnPosition.position, Quaternion.identity);
        Rigidbody2D fallingStarRigidbody2D = fallingStarTransform.GetComponent<Rigidbody2D>();
        fallingStarRigidbody2D.AddForce(new Vector2(Random.Range(-800, -300), 0));
        Destroy(fallingStarTransform.gameObject, 4f);
    }
}
