using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using System.IO;
using System.Net;
using System.Net.Sockets;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            // See IMOTIONS API Programmer's guide page 28 and forth.
            // https://help.imotions.com/hc/en-us/articles/203045581-iMotions-API-Programming-Guide
            //
            // 1. Save this XML to file.
            // < EventSource Id = "GenericInput" Version = "1" Name = "GenericInput" >
            //      < Sample Id = "GenericInput" Name = "GenericInput" >
            //          < Field Id = "Value1" Range = "Variable" ></ Field >
            //          < Field Id = "Value2" Range = "Variable" ></ Field >
            //      </ Sample >
            //  </ EventSource >
            // 2. Load the file into IMOTIONS
            // 3. Enable API
            // 4. set receive method to UDP (non TCP)
            //
            // NOTE:
            // Range parameter is optional, however if the data should be visualised (live and post) then Range="Fixed" or Range="Variable" is required.
            // Range can be qualified with an expected max and min.
            //
            // NOTE:
            // value1 and value2 are recognised by their order in the semicolon separated list of values below: following the same order as in the XML above.

            int lastshownsecond = -1;

            while (true)
            {
                //
                // PART 1: SEND TIME SIGNAL TO IMOTIONS
                //

                //generate some random time signal data, two channels
                string value1 = Convert.ToString(DateTime.Now.Millisecond);
                string value2 = Convert.ToString(DateTime.Now.Second);

                // construct a UDP string with the above signals
                // The prefix "E" lets IMOTIONS know that this is a line graph type of input.
                string UDPstring = "E;1;GenericInput;1;0.0;;;GenericInput;" + value1 + ";" + value2 + "\r\n";
                SendUDPPacket("127.0.0.1", 8089, UDPstring, 1);

                //
                // PART 2: SEND MARKER TEXT AT DISCRETE TIME INTERVALS
                //
                if (lastshownsecond != DateTime.Now.Second)
                {
                    int currentSecond = DateTime.Now.Second;

                    // construct a UDP string with the above signals
                    // The prefix "M" lets IMOTIONS know that this is marker event.
                    string DiscreteTextEvent = "M;2;;;"+currentSecond+" Second;Marker Text with second counter "+currentSecond+";D;\r\n";
                    SendUDPPacket("127.0.0.1", 8089, DiscreteTextEvent, 1);

                    lastshownsecond = currentSecond;
                    Console.WriteLine("API Marker sent to IMOTIONS " + DiscreteTextEvent);
                }

                // take a little break before generating a new sample
                // 10 milliseconds = 100hz signal
                System.Threading.Thread.Sleep(10);
            }


        }



        // <summary>
        // Sends a sepcified number of UDP packets to a host or IP Address.
        // </summary>
        // <param name="hostNameOrAddress">The host name or an IP Address to which the UDP packets will be sent.</param>
        // <param name="destinationPort">The destination port to which the UDP packets will be sent.</param>
        // <param name="data">The data to send in the UDP packet.</param>
        // <param name="count">The number of UDP packets to send.</param>
        // Thanks Ole Braunbaek Jensen for this function!
        static void SendUDPPacket(string hostNameOrAddress, int destinationPort, string data, int count)
        {
            // Validate the destination port number
            if (destinationPort < 1 || destinationPort > 65535)
                throw new ArgumentOutOfRangeException("destinationPort", "Parameter destinationPort must be between 1 and 65,535.");

            // Resolve the host name to an IP Address
            IPAddress[] ipAddresses = Dns.GetHostAddresses(hostNameOrAddress);
            if (ipAddresses.Length == 0)
                throw new ArgumentException("Host name or address could not be resolved.", "hostNameOrAddress");

            // Use the first IP Address in the list
            IPAddress destination = ipAddresses[0];
            IPEndPoint endPoint = new IPEndPoint(destination, destinationPort);
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            // Send the packets
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            for (int i = 0; i < count; i++)
                socket.SendTo(buffer, endPoint);
            socket.Close();
        }


    }
}
