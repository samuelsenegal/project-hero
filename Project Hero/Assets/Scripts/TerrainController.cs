using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 0, 400);

    [SerializeField] public GameObject[] terrainList;

    private bool generating = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GenerateTerrain(transform.root.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(transform.root.gameObject);
    }

    void GenerateTerrain(Vector3 position)
    {
        if (generating) { return; }
        Instantiate(terrainList[0], position + offset, Quaternion.identity);
        StartCoroutine(SectionCooldown());
    }

    IEnumerator SectionCooldown()
    {
        generating = true;
        yield return new WaitForSeconds(1.0f);
        generating = false;
    }

}