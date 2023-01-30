using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class IMotions : MonoBehaviour
{
    private float timer = 0;
    public float period = 1; // seconds
    public string hostname = "127.0.0.1";
    public int port = 8089;

    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > period) {
            timer -= period;
            SendTime();
            SendMarker();
        }
    }

    private void SendTime() 
    {
        string value1 = Convert.ToString(DateTime.Now.Millisecond);
        string value2 = Convert.ToString(DateTime.Now.Second);

        // construct a UDP string with the above signals
        // The prefix "E" lets IMOTIONS know that this is a line graph type of input.
        string UDPstring = "E;1;GenericInput;1;0.0;;;GenericInput;" + value1 + ";" + value2 + "\r\n";
        SendUDPPacket(hostname, port, UDPstring, 1);
    }

    private void SendMarker()
    {
        int currentSecond = DateTime.Now.Second;

        // construct a UDP string with the above signals
        // The prefix "M" lets IMOTIONS know that this is marker event.
        string DiscreteTextEvent = "M;2;;;"+currentSecond+" Second;Marker Text with second counter "+currentSecond+";D;\r\n";
        SendUDPPacket(hostname, port, DiscreteTextEvent, 1);
    }

    // <summary>
    // Sends a sepcified number of UDP packets to a host or IP Address.
    // </summary>
    // <param name="hostNameOrAddress">The host name or an IP Address to which the UDP packets will be sent.</param>
    // <param name="destinationPort">The destination port to which the UDP packets will be sent.</param>
    // <param name="data">The data to send in the UDP packet.</param>
    // <param name="count">The number of UDP packets to send.</param>
    // Thanks Ole Braunbaek Jensen for this function!
    private void SendUDPPacket(string hostNameOrAddress, int destinationPort, string data, int count)
    {
        // Validate the destination port number
        if (destinationPort < 1 || destinationPort > 65535)
        {
            Debug.LogError("Destination port must be between 1 and 65,535.");
            return;
        }

        // Resolve the host name to an IP Address
        IPAddress[] ipAddresses = Dns.GetHostAddresses(hostNameOrAddress);
        if (ipAddresses.Length == 0)
        {
            Debug.LogError($"Host name '{hostNameOrAddress}' could not be resolved.");
            return;
        }

        // Use the first IP Address in the list
        IPAddress destination = ipAddresses[0];
        IPEndPoint endPoint = new IPEndPoint(destination, destinationPort);
        byte[] buffer = Encoding.ASCII.GetBytes(data);

        // Send the packets
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        for (int i = 0; i < count; i++)
        {
            Debug.Log($"Sending: {data}");
            socket.SendTo(buffer, endPoint);
        }
        socket.Close();
    }
}
