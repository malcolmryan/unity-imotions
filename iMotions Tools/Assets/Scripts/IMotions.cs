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
    public string hostname = "127.0.0.1";
    public int port = 8089;

    public string eventID = "Unity";    
    public int version = 1;
    public string instance = "Default";
    public string sampleID = "Sample";    
    public string[] sensors = {"Milliseconds", "Seconds"};
    private Dictionary<string, string> sensorValues;

    private string naString = "na";

    public void OnEnable()
    {
        sensorValues = new Dictionary<string, string>();
        // initialise all sensors to NA
        for (int i = 0; i < sensors.Length; i++) 
        {
            sensorValues[sensors[i]] = naString;
        }
    }

    public void SetSensor(string sensor, string value)
    {
        if (!sensorValues.ContainsKey(sensor)) {
            throw new ArgumentException($"Sensor {sensor} is not defined.", "sensor");
        }
        sensorValues[sensor] = value;
    }

    public void SetSensor(string sensor, int value)
    {
        SetSensor(sensor, Convert.ToString(value));
    }

    public void SetSensor(string sensor, float value)
    {
        SetSensor(sensor, Convert.ToString(value));
    }

    public void ResetSensor(string sensor) 
    {
        if (!sensorValues.ContainsKey(sensor)) {
            throw new ArgumentException($"Sensor {sensor} is not defined.", "sensor");
        }
        sensorValues[sensor] = naString;
    }

    public void SendSensors() 
    {
        // construct a UDP string with the above signals
        // The prefix "E" lets IMOTIONS know that this is a line graph type of input.

        // 1: Type = 'E' - Sensor Event
        // 2: Version = 1 - Version of the event string format
        // 3: Source = sensor group (must match EventSource ID in XML)
        // 4: Source Version = sensor version
        // 5: Instance = which instance of this sensor (if there are two identical sensors) - This doesn't appear to be set
        // 6: Timestamp (ms) = timestamp for data (should match iMotions timestamp)
        // 7: Media Time = media timerstamp for data (value unclear)
        // 8: Sample Type = should match Sample ID in XML
        // 9... data fields   

        StringBuilder sb = new StringBuilder($"E;1;{eventID};{version};{instance};;;{sampleID}");

        // append values
        for (int i = 0; i < sensors.Length; i++) {
            sb.Append(';');
            sb.Append(sensorValues[sensors[i]]);
        }
        sb.Append("\r\n");

        SendUDPPacket(hostname, port, sb.ToString(), 1);
    }

    public void SendStartMarker(string description)
    {
        SendMarker('S', description);
    }

    public void SendEndMarker(string description)
    {
        SendMarker('E', description);
    }

    public void SendNextMarker(string description)
    {
        SendMarker('N', description);
    }

    public void SendDiscreteMarker(string description)
    {
        SendMarker('D', description);
    }

    private void SendMarker(char type, string description)
    {
        // Marker version 2
        // 1: Type = 'M' - Marker Event
        // 2: Version = 2
        // 3: Elapsed time (optional)
        // 4: Media time (optional)
        // 5: Short text description
        // 6: Long test description
        // 7: Marker type - 'D' = Discrete, 'S' = Start segment, 'E' = End segment, 'N' = Next segment (auto end/start)
        // 8: Scence Type (optional) - 'V' = Video, 'I' = Image

        // construct a UDP string with the above signals
        // The prefix "M" lets IMOTIONS know that this is marker event.
        string DiscreteTextEvent = $"M;2;;;{description};{description};{type};\r\n";
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
