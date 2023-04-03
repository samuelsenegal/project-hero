using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    [SerializeField] public GameObject[] terrainList;
    private float offset = 100;

    private void OnCollisionEnter(Collision collision)
    {
        GenerateTerrain();
    }

    private void OnCollisionExit(Collision collision)
    {
        Destroy(this.gameObject);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    void GenerateTerrain()
    {

    }

}