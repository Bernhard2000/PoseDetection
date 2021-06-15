using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LimbController : MonoBehaviour
{
    GameObject [] limbs = new GameObject[33];
    Dictionary<int, Dictionary<int, GameObject>> connectors = new Dictionary<int, Dictionary<int,GameObject>>();

    // Start is called before the first frame update
    void Start()
    {
        /*for(int i = 0; i < limbs.Length; i++)
        {
            limbs[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < limbs.Length; i++)
        {
            try 
            {
                if(UDPReceiver.bodyPartDict.ContainsKey(i))
                {
                    if(limbs[i] == null)
                    {
                        limbs[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        limbs[i].transform.localScale = new Vector3(2,2,2);
                    }
                    

                    float xpos = (UDPReceiver.bodyPartDict[i][0]) * 40;
                    float ypos = (UDPReceiver.bodyPartDict[i][1]) * 40;
                    float zpos = (UDPReceiver.bodyPartDict[i][2]) * 40;

                    limbs[i].transform.position = new Vector3(xpos, ypos, zpos);
                    Debug.Log("Setting node: " + i + ":" + xpos + "," + ypos + "," + zpos);
                } else
                {
                    GameObject.Destroy(limbs[i]);
                    limbs[i] = null;
                }
            } catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }       
    }
}
