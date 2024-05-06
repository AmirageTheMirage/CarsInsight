using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGround : MonoBehaviour
{
    public float yOffset;

    void Awake()
    {
        StartCoroutine(OneSec());
        
    }

    IEnumerator OneSec()
    {
        
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 200f, gameObject.transform.position.z);
        Vector3 PosRay = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 100f, gameObject.transform.position.z);
        Ray ray = new Ray(PosRay, Vector3.down);
        RaycastHit hit;
        yield return new WaitForSeconds(1f);
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            //Debug.Log("Hit object: " + hit.collider.gameObject.name + " Tag: *" + hit.collider.tag + "*");

            if (hit.collider.tag == "TerrainTag")
            {
                //Debug.Log("TerrainTag hit!");
                gameObject.transform.position = hit.point;
                gameObject.transform.position += Vector3.up * yOffset;
                //gameObject.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Random.Range(0f, 360f), transform.eulerAngles.z);
            }
            else
            {
                Debug.LogWarning("No TerrainTag (CampGeneration) hit " + hit.collider.gameObject.name + " Tag: " + hit.collider.tag);

                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("No hit detected.");
            Destroy(gameObject);
        }
        
    }
}
