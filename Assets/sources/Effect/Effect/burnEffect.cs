using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class burnEffect : MonoBehaviour
{
    public float cutoffValue = 0.95f;
    public float cutoffMin = 0.5f;
    public Transform burnPrefab;
    public bool burning;
    public float burnRate = 0.005f;
    public Transform newBurnQuad;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (burning)
        {
            Renderer burnRenderer = newBurnQuad.GetComponent<MeshRenderer>();
            burnRenderer.material.SetFloat("_Cutoff", cutoffValue);

            if(burnRate < 1f)
            {
                burnRate += 0.001f * Time.deltaTime;
            }

            if (cutoffValue >= cutoffMin)
            {
                cutoffValue = cutoffMin;
            }
            else
            {
                cutoffValue = cutoffMin;
                burning = false;
            }

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Vector3 pos = contact.point;
        GetComponent<Rigidbody>().detectCollisions = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        newBurnQuad = Instantiate(burnPrefab, pos, Quaternion.Euler(90, 0, 0)) as Transform;
        burning = true;
        Destroy(this.gameObject);

    }
}
