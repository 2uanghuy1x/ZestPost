using System.Net;
using System.Net.Http;

namespace ZestPost.Base.Controller
{
    public class IpSyncController
    {
        public class IPSync
        {
            public bool Status { get; set; }
            public string Proxy { get; set; }
            public string Address { get; set; }
            public string IpV6 { get; set; }
            public string Country { get; set; }
            public string Lat { get; set; }
            public string Lon { get; set; }
            public string Timezone { get; set; }
            public string Id { get; set; }
            public string Host { get; set; }
            public string Port { get; set; }
        }

        public class IPInforSync
        {
            public string Ip { get; set; }
            public string IpPulic { get; set; }
            public string TypeProxy { get; set; }

            public IPInforSync(string Ip, string Ip_public, string Type_proxy)
            {
                this.Ip = Ip;
                IpPulic = Ip_public;
                TypeProxy = Type_proxy;
            }
        }

        public IPSync MethodCheckIP(string type_ip, string proxy = null, int time = 5)
        {
            IPSync ip_result = new IPSync { Status = false, Country = null, Lat = null, Lon = null, Timezone = null };
            ip_result = CheckIPMKT(type_ip, proxy, 10);
            return ip_result;
        }
        public IPSync MethodCheckIPBase(string type_ip, string proxy = null, int timeout = 5)
        {
            IPSync infor_ip = new IPSync();
            try
            {
                if (!string.IsNullOrEmpty(proxy))
                {
                    string[] lst_proxy = proxy.Split(':').ToArray();
                    string proxy_checking = null;
                    if (lst_proxy.Count() >= 2 && lst_proxy.Count() <= 4)
                    {
                        proxy_checking = lst_proxy[0];
                    }
                    else if (lst_proxy.Count() > 4)
                    {
                        //proxyV6
                        proxy_checking = proxy;
                    }

                    // với proxy V4 tĩnh private thì phải phải lấy ip public thì mới lấy thông tin IP được
                    IPSync ip_public_infor = IsInRange(proxy_checking) ? MethodGetPublicIP(proxy) : new IPSync { Address = proxy };
                    if (!string.IsNullOrEmpty(ip_public_infor.Address))
                    {
                        infor_ip = MethodLookupIPMKT(ip_public_infor.Address, timeout);
                    }
                }
                else
                {
                    infor_ip = MethodGetPublicIP(proxy);
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return infor_ip;
        }
        public IPSync CheckIPMKT(string type_ip, string proxy = null, int timeout = 5)
        {
            IPSync infor_ip = new IPSync();
            try
            {
                if (!string.IsNullOrEmpty(proxy))
                {
                    string[] lst_proxy = proxy.Split(':').ToArray();
                    string proxy_checking = null;
                    if (lst_proxy.Count() >= 2 && lst_proxy.Count() <= 4)
                    {
                        proxy_checking = lst_proxy[0];
                    }
                    else if (lst_proxy.Count() > 4)
                    {
                        //proxyV6
                        proxy_checking = proxy;
                    }

                    IPSync ip_public_infor = IsInRange(proxy_checking) ? MethodGetPublicIP(proxy) : new IPSync { Address = proxy };
                    if (!string.IsNullOrEmpty(ip_public_infor.Address))
                    {
                        infor_ip = MethodLookupIPMKT(ip_public_infor.Address, timeout);
                        if (string.IsNullOrEmpty(infor_ip.Address))
                        {
                            //retry
                            ip_public_infor = MethodGetPublicIP(proxy);
                            if (!string.IsNullOrEmpty(ip_public_infor.Address))
                            {
                                infor_ip = MethodLookupIPMKT(ip_public_infor.Address, timeout);
                            }
                        }
                    }
                }
                else
                {
                    infor_ip = MethodGetPublicIP(proxy);
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return infor_ip;
        }
        public IPSync MethodLookupIPMKT(string ip_public_infor_address = null, int timeout = 5)
        {
            IPSync infor_ip = new IPSync();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Thiết lập thời gian timeout
                    client.Timeout = TimeSpan.FromSeconds(timeout);

                    // Thiết lập header Authorization
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2NmE3NmVlYTA1NWVmOGYzZDYzMzU1ZTEiLCJpYXQiOjE3MjIyNDkxMjMsImV4cCI6ODY0MTcyMjE2MjcyM30.nKr1Y27WHj4UTCLkkwB1GumYSosnQSbmIlKtuabDqHw");

                    // Tạo URL request
                    string link = $"http://45.77.33.135:3001/api/lookup?ip={ip_public_infor_address}";

                    // Gửi request GET
                    HttpResponseMessage response = client.GetAsync(link).Result;

                    // Kiểm tra response thành công
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        // Parse JSON
                        JObject obj = JObject.Parse(jsonResponse);
                        infor_ip.Country = obj["country"]?.ToString();
                        infor_ip.Timezone = obj["timezone"]?.ToString();
                        infor_ip.Lat = obj["lat"]?.ToString();
                        infor_ip.Lon = obj["lon"]?.ToString();
                        infor_ip.Address = obj["address"]?.ToString();

                        // Kiểm tra IP là IPv6
                        if (obj["type"]?.ToString() == "IPv6")
                        {
                            infor_ip.IpV6 = obj["address"]?.ToString();
                        }
                    }
                    else
                    {
                        throw new Exception($"HTTP Request Failed. Status Code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
            }
            return infor_ip;
        }
        public bool IsInRange(string ipAddress)
        {
            bool is_private = false;
            try
            {
                //dải ip private
                IPAddress ip = IPAddress.Parse(ipAddress);
                is_private = IsInRange(ip, "10.0.0.0", "10.255.255.255") ||
                       IsInRange(ip, "172.16.0.0", "172.31.255.255") ||
                       IsInRange(ip, "192.168.0.0", "192.168.255.255");
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return is_private;
        }
        public bool IsInRange(IPAddress ip, string startIp, string endIp)
        {
            byte[] ipBytes = ip.GetAddressBytes();
            byte[] startBytes = IPAddress.Parse(startIp).GetAddressBytes();
            byte[] endBytes = IPAddress.Parse(endIp).GetAddressBytes();

            bool lowerBound = true, upperBound = true;

            for (int i = 0; i < ipBytes.Length && (lowerBound || upperBound); i++)
            {
                if (lowerBound && ipBytes[i] < startBytes[i] || upperBound && ipBytes[i] > endBytes[i])
                {
                    return false;
                }

                lowerBound &= ipBytes[i] == startBytes[i];
                upperBound &= ipBytes[i] == endBytes[i];
            }

            return true;
        }
        public IPSync MethodGetPublicIP(string proxy, int timeout = 5)
        {
            IPSync ip_public_infor = new IPSync();
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    // Kiểm tra và cấu hình proxy
                    if (!string.IsNullOrEmpty(proxy) && proxy.Contains(":"))
                    {
                        string[] lst_proxy = proxy.Split(':').ToArray();
                        if (lst_proxy.Length > 4)
                        {
                            ip_public_infor.Status = true;
                            ip_public_infor.Address = proxy;
                            goto lb_finish;
                        }
                        else
                        {
                            var webProxy = new WebProxy(lst_proxy[0], int.Parse(lst_proxy[1]));
                            if (lst_proxy.Length > 3)
                            {
                                webProxy.Credentials = new NetworkCredential(lst_proxy[2], lst_proxy[3]);
                            }
                            handler.Proxy = webProxy;
                        }
                    }

                    // Thiết lập HttpClient với proxy
                    using (HttpClient client = new HttpClient(handler))
                    {
                        // Thiết lập thời gian timeout
                        client.Timeout = TimeSpan.FromSeconds(timeout);

                        // Gửi request GET
                        string link = "https://ipscore.io/api/v1/score";
                        HttpResponseMessage response = client.GetAsync(link).Result;

                        // Kiểm tra trạng thái phản hồi
                        if (response.IsSuccessStatusCode)
                        {
                            string html = response.Content.ReadAsStringAsync().Result;

                            // Parse JSON
                            JObject obj = JObject.Parse(html);
                            if ((bool)obj["success"])
                            {
                                ip_public_infor.Status = true;
                                ip_public_infor.Country = obj["country"]?.ToString();
                                ip_public_infor.Lat = obj["latitude"]?.ToString();
                                ip_public_infor.Lon = obj["longitude"]?.ToString();
                                ip_public_infor.Timezone = obj["timezone"]?.ToString();
                                string ip_address = obj["ip_address"]?.ToString();
                                ip_public_infor.Address = !string.IsNullOrEmpty(proxy) && proxy.Contains(ip_address) ? proxy : ip_address;
                            }
                        }
                        else
                        {
                            throw new Exception($"HTTP Request Failed. Status Code: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
            }

        lb_finish:;
            return ip_public_infor;
        }
    }
}
