using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LodingScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PLAYER")
        {
            SceneManager.LoadScene("Loading");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
