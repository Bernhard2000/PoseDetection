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

        float[][] xVals =  new float[31][];
        float[][] yVals =  new float[31][];
        float[][] zVals =  new float[31][];

        for(int i = 0; i < 31; i++)
        {
            xVals[i] = new float[5];
            yVals[i] = new float[5];
            zVals[i] = new float[5];
        }

        while(true) 
        {
            int[] updated = new int[33];
                try 
                {
                    byte [] rawData = udpClient.Receive(ref ip);
                    string text = Encoding.UTF8.GetString(rawData);
                    Debug.Log("Received data string: "+ text);
                    string[] splitStringTemp = text.Split(';');

                    foreach (var bodypart in splitStringTemp)
                    {
                        string[] valueSplit = bodypart.Split(',');
                        try
                        {
                            int nodeIndex = Int32.Parse(valueSplit[0]);

                            xVals[nodeIndex][4] = xVals[nodeIndex][3];
                            xVals[nodeIndex][3] = xVals[nodeIndex][2];
                            xVals[nodeIndex][2] = xVals[nodeIndex][1];
                            xVals[nodeIndex][1] = xVals[nodeIndex][0];
                        
                            xVals[nodeIndex][0] = float.Parse(valueSplit[1], CultureInfo.InvariantCulture);
                            yVals[nodeIndex][0] = -float.Parse(valueSplit[2], CultureInfo.InvariantCulture);
                            zVals[nodeIndex][0] = float.Parse(valueSplit[3], CultureInfo.InvariantCulture);

                            //glätten mit vorherigen Werten
                            float xValMean = (xVals[nodeIndex][0] * 50 + xVals[nodeIndex][1] * 25 + xVals[nodeIndex][2] * 13 + xVals[nodeIndex][3] * 8 +  xVals[nodeIndex][4] * 4) / 100;
                            float yValMean = (yVals[nodeIndex][0] * 50 + yVals[nodeIndex][1] * 25 + yVals[nodeIndex][2] * 13 + yVals[nodeIndex][3] * 8 +  yVals[nodeIndex][4] * 4) / 100;
                            float zValMean = (zVals[nodeIndex][0] * 50 + zVals[nodeIndex][1] * 25 + zVals[nodeIndex][2] * 13 + zVals[nodeIndex][3] * 8 +  zVals[nodeIndex][4] * 4) / 100;


                            if (!bodyPartDict.ContainsKey(nodeIndex))
                            {
                                bodyPartDict.Add(nodeIndex, new float[] {xValMean, yValMean, zValMean});
                            }
                            else
                            {
                                bodyPartDict[nodeIndex] = new float[] {xValMean, yValMean, zValMean};
                            }
                            updated[nodeIndex] = 5;
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e.Message + "\n" + "Stracktrace: " + e.StackTrace);
                        }                
                    }

                   
                    

                    Debug.Log("Updated bodypartdict");
                } catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            

             for(int j = 0; j < 33; j++)
                    {
                        if(updated[j] == 0)
                        {
                            bodyPartDict.Remove(j);
                        }
                        updated[j]--;
                    }

        }
    }
}
