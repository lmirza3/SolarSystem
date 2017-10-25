using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonH : MonoBehaviour {

    public bool p;
    public bool s;
    public bool o;
    public Renderer rend;
      

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "VRController")
        {
            if (p == true)
            {
                rend.material.color = Color.red;
            }
            else if (s == true)
            {
                rend.material.color = Color.blue;
            }
            else if (o == true)
            {
                rend.material.color = Color.red;
            }
        }

    }
    // Use this for initialization
    void Start () {

        rend = GetComponent<Renderer>();
        rend.enabled = true;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
