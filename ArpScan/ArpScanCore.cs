using System.Runtime.InteropServices;

namespace ArpScan
{
    public class ArpScanCore
    {
        [DllImport("Iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int destIp, int srcIp, byte[] macAddr, ref int phyAddrLen);

        [DllImport("Ws2_32.dll")]
        private static extern int inet_addr(string ip);

        public static string GetRemoteMacAddress(string remoteIp)
        {
            int destIp = inet_addr(remoteIp);
            byte[] macAddr = new byte[6];
            int macAddrLen = macAddr.Length;

            int result = SendARP(destIp, 0, macAddr, ref macAddrLen);
            if (result != 0)
            {
                throw new InvalidOperationException("SendARP failed.");
            }

            return BitConverter.ToString(macAddr, 0, macAddrLen);
        }

        private static bool ArpCheck(string ip)
        {
            bool result = true;
            try
            {
                GetRemoteMacAddress(ip);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static int GetIpRange(string ip_start, string ip_end)
        {
            int ip_start_num = (int)IpStr2Uint(ip_start);
            int ip_end_num = (int)IpStr2Uint(ip_end);
            return ip_end_num - ip_start_num;
        }

        public static void Scan(string ip_start, string ip_end, Action<string, bool> callback)
        {
            if (ip_start != null && ip_end != null)
            {
                List<UInt32> successList = [];
                var list = GenerateIpList(ip_start, ip_end);
                uint finishCount = 0;
                foreach (var item in list)
                {
                    Task.Run(() =>
                    {
                        Console.WriteLine("Checking:" + item);
                        bool result = ArpCheck(item);
                        if (result)
                        {
                            successList.Add(IpStr2Uint(item));
                            callback(item, true);
                            Console.WriteLine("Success:" + item);
                        }
                        else
                        {
                            callback(item, false);
                            Console.WriteLine("Fail:" + item);
                        }
                        finishCount++;
                    });
                }
                while (finishCount < list.Count) ;
                successList.Sort();
                foreach (var item in successList)
                {
                    Console.WriteLine("Success:" + Uint2IpStr(item));
                }
            }
        }

        private static List<string> GenerateIpList(string ip_start, string ip_end)
        {
            List<string> result = new();
            uint ip_start_num = IpStr2Uint(ip_start);
            uint ip_end_num = IpStr2Uint(ip_end);
            for (uint i = ip_start_num; i <= ip_end_num; i++)
            {
                result.Add(Uint2IpStr(i));
            }
            return result;
        }

        public static UInt32 IpStr2Uint(string ip_str)
        {
            byte[] ip_byte_list = new byte[4];
            string[] ip_byte_str_list = ip_str.Split('.').Reverse().ToArray();
            if (ip_byte_str_list.Length != 4)
            {
                return UInt32.MaxValue;
            }
            for (int i = 0; i < ip_byte_str_list.Length; i++)
            {
                string? item = ip_byte_str_list[i];
                ip_byte_list[i] = Convert.ToByte(item);
            }
            return BitConverter.ToUInt32(ip_byte_list);
        }

        private static string Uint2IpStr(UInt32 ip_num)
        {
            byte[] ip_byte_list = BitConverter.GetBytes(ip_num).Reverse().ToArray();
            string ip_str = "";
            foreach (var item in ip_byte_list)
            {
                ip_str += item + ".";
            }
            ip_str = ip_str.Substring(0, ip_str.Length - 1);
            return ip_str;
        }
    }
}
