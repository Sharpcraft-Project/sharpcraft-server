using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPnP;

public class TestUPnP : MonoBehaviour 
{
    public int port;
    private NatUtility natUtility;
    void Start()
    {
        natUtility = new NatUtility();
        natUtility.Map(port, "PortForwarding test");
    }
}