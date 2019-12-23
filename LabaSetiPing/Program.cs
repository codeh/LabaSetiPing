using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace LabaSetiPing
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            GetMac();
            //100.93.81.167
            var firstIP = IPAddress.Parse("100.93.81.160"); //дрю
            var lastIP = IPAddress.Parse("100.93.81.170");
            //var firstIP = IPAddress.Parse("192.168.1.1"); //дома
            //var lastIP = IPAddress.Parse("192.168.1.128");
            //var firstIP = IPAddress.Parse("172.20.10.0"); //айфон
            //var lastIP = IPAddress.Parse("172.20.10.128");
            //var firstIP = IPAddress.Parse("193.233.144.0"); //угату
            //var lastIP = IPAddress.Parse("193.233.144.20");
            var ipList = IPList(firstIP, lastIP);
            info(firstIP,lastIP);
            foreach (var ip in ipList)
            {
                Connect(ip);
            }

            Console.ReadKey();
        }

        public static List<IPAddress> IPList(IPAddress first, IPAddress last)
        {
            List<IPAddress> list = new List<IPAddress>();
            var begin = first.GetAddressBytes();
            var end = last.GetAddressBytes();

            Array.Reverse(begin);
            Array.Reverse(end);

            var beginIPint = BitConverter.ToInt32(begin, 0);
            var endIPint = BitConverter.ToInt32(end, 0);
            for (; beginIPint <= endIPint; beginIPint++)
            {
                var IP = BitConverter.GetBytes(beginIPint);
                Array.Reverse(IP);
                list.Add(new IPAddress(IP));
            }

            return list;
        }

        public static void Connect(IPAddress ip)
        {
            var pinger = new Ping();
            try
            {
                var pingrep = pinger.Send(ip, 50);
                Console.Write(ip + " ");
                if (pingrep.Address != null)
                    Console.Write(Dns.GetHostEntry(ip).HostName + " ");
                else
                    Console.Write("Unknown ");

                Console.WriteLine(pingrep.Status.ToString());
            }
            catch
            {
            }
        }

        public static void GetMac() 
        {
            foreach (NetworkInterface n in NetworkInterface.GetAllNetworkInterfaces())
                if (n.OperationalStatus == OperationalStatus.Up)
                {
                    var physicalAddress = n.GetPhysicalAddress();
                    var addressBytes = physicalAddress.GetAddressBytes();
                    var mac = string.Join(":", addressBytes.Select(b => b.ToString("x")));
                    Console.WriteLine("MAC: " + mac);
                    break;
                }
        }

        public static void info(IPAddress beginIP, IPAddress endIP)

        {

            var begin = beginIP.GetAddressBytes();
            var end = endIP.GetAddressBytes();
            byte[] broadcast = new byte[4];
            byte[] ipadress = new byte[4];
            byte[] mask = new byte[4];
            bool edge = false;
            for (int i = 0; i < 4; i++)

            {

                for (byte b = 128; b >= 1; b /= 2)

                {

                    if (!edge && (begin[i] & b) == (end[i] & b))

                        mask[i] |= b;
                    else
                    {

                        edge = true;

                        mask[i] = (byte) (mask[i] & ~b);

                    }
                }

                ipadress[i] = (byte) (mask[i] & begin[i]);
                broadcast[i] = (byte) (~mask[i] | ipadress[i]);

            }
            Console.WriteLine("Сеть: " + new IPAddress(ipadress).ToString());
            Console.WriteLine("Маска: " + new IPAddress(mask).ToString());
            Console.WriteLine("Широковещательный адрес: " + new IPAddress(broadcast).ToString());

        }
    }
}