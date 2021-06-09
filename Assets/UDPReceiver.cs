using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class UDPReceiver : MonoBehaviour
{ 
    static int port = 64039;
    static UdpClient uDPClient = null;
    static Thread receiverThread = null;


    public static Dictionary<int, float[]> bodyPartDict = new Dictionary<int, float[]>();

    // Start is called before the first frame update
    static UDPReceiver()
    {
        Debug.Log("Started receiver");
        uDPClient = new UdpClient(port);
        receiverThread = new Thread(new ThreadStart(ReceiveText));
        receiverThread.Start();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    static void ReceiveText() 
    {
        Debug.Log("Created Receiverthread");
        UdpClient udpClient = new UdpClient(port);
        Debug.Log("Create UDPClient");
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        float[] xValMean =  new float[3] {0,0,0,};
        float[] yValMean =  new float[3] {0,0,0,};
        float[] zValMean =  new float[3] {0,0,0,};


        while(true) 
        {
            for(int i = 0; i < 3; i++)
            {
                try 
                {
                    byte [] rawData = udpClient.Receive(ref ip);
                    string text = Encoding.UTF8.GetString(rawData);
                    Debug.Log("Received data string: "+ text);
                    string[] splitStringTemp = text.Split(';');
                    int[] updated = new int[33];

                    foreach (var bodypart in splitStringTemp)
                    {
                        string[] valueSplit = bodypart.Split(',');
                        try
                        {
                            int bodypartIndex = Int32.Parse(valueSplit[0]);
                        
                            xValMean[i] = float.Parse(valueSplit[1], CultureInfo.InvariantCulture);
                            yValMean[i] = -float.Parse(valueSplit[2], CultureInfo.InvariantCulture);
                            zValMean[i] = float.Parse(valueSplit[3], CultureInfo.InvariantCulture);

                            if (!bodyPartDict.ContainsKey(bodypartIndex))
                            {
                                bodyPartDict.Add(bodypartIndex, new float[] {xValMean.Average(), yValMean.Average(), zValMean.Average()});
                            }
                            else
                            {
                                bodyPartDict[bodypartIndex] = new float[] {xValMean.Average(), yValMean.Average(), zValMean.Average()};
                            }
                            updated[bodypartIndex] = 5;
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e.Message);
                        }                
                    }

                    for(int j = 0; j < 33; j++)
                    {
                        if(updated[j] == 0)
                        {
                            bodyPartDict.Remove(j);
                        }
                        updated[j]--;
                    }

                    

                    Debug.Log("Updated bodypartdict");
                } catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
    }
}
