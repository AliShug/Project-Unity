using UnityEngine;
using System.Collections;

public class PrefabSpawner : MonoBehaviour
{

    public GameObject mob;
    public bool rainbow = false;

    public void Spawn()
    {
        GameObject newObject = (GameObject)GameObject.Instantiate(
            mob,
            transform.position,
            transform.rotation);

        if (rainbow)
        {
            Material newMaterial = new Material(mob.GetComponent<Renderer>().sharedMaterial);
            newMaterial.color = Random.ColorHSV();
            newObject.GetComponent<Renderer>().material = newMaterial;
        }
    }
}
