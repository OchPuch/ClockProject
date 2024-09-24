using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Clock
{
    public static class WebTimeProvider
    {
        private static readonly HttpClient HttpClient = new();
        private const string WorldTimeApiUrl = "http://worldtimeapi.org/api/ip";
        private const string NtpServer = "ntp1.ntp-servers.net";
        
        public static async Task<DateTime> GetTimeAsync()
        {
            try
            {
                Task<DateTime>[] timeRetrievingTasks = {
                    GetNetworkTimeAsync(),
                    GetWorldTimeApiDateTime()
                };
                
                var completedTask = await Task.WhenAny(timeRetrievingTasks);
                return completedTask.Result;
            }
            catch (Exception ex)
            {
                Debug.LogError("Error fetching time: " + ex.Message);
                return DateTime.Now;
            }
        }
        
        #region  WorldTimeApi

        private static async Task<DateTime> GetWorldTimeApiDateTime()
        {
            HttpResponseMessage response = await HttpClient.GetAsync(WorldTimeApiUrl);
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var timeData = JsonUtility.FromJson<WorldTimeResponse>(jsonResponse);
            DateTime dateTime = DateTime.Parse(timeData.datetime);
            return dateTime;
        }

        [Serializable]
        private class WorldTimeResponse
        {
            public string datetime;
        } 

        #endregion

        #region TimeFromNTP

        private static async Task<DateTime> GetNetworkTimeAsync()
        {
            const int port = 123;
            const int timeout = 3000;

            byte[] ntpData = new byte[48];
            ntpData[0] = 0x1B;

            IPAddress[] addresses = await Dns.GetHostAddressesAsync(NtpServer);
            IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], port);

            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.Connect(ipEndPoint);
                await udpClient.SendAsync(ntpData, ntpData.Length);

                udpClient.Client.ReceiveTimeout = timeout;

                UdpReceiveResult udpResult;
                try
                {
                    udpResult = await udpClient.ReceiveAsync();
                }
                catch (SocketException)
                {
                    throw new Exception("NTP запрос не удался.");
                }

                ntpData = udpResult.Buffer;
            }

            ulong intPart = BitConverter.ToUInt32(ntpData, 40);
            ulong fractPart = BitConverter.ToUInt32(ntpData, 44);

            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            DateTime networkDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }

        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                          ((x & 0x0000ff00) << 8) +
                          ((x & 0x00ff0000) >> 8) +
                          ((x & 0xff000000) >> 24));
        }

        #endregion
        
    }

    
}