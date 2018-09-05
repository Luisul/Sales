namespace Sales.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common.Models;
    using Newtonsoft.Json;
    using Plugin.Connectivity;

    public class ApiService
    {
        public async Task<Response> CheckConecction()
        { //valida que el dispositivo tenga el wifi activo 
            if (!CrossConnectivity.Current.IsConnected)
            {
                return new Response
                {
                    IsSucces = false,
                    Message = "Please turn on your internet settings"
                };
            }

            var isRecheable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isRecheable)
            {
                return new Response
                {
                    IsSucces = false,
                    Message = "No internet connecction"
                };
            }

            return new Response
            {
                IsSucces = true
            };
        }


        public async Task<Response> GetList<T>(string urlBase, string prefix, string controller)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = $"{prefix}{controller}";
                var response = await client.GetAsync(url);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucces = false,
                        Message = answer
                    };
                }

                
                var list = JsonConvert.DeserializeObject<List<T>>(answer);
                return new Response
                {
                    IsSucces = true,
                    Result = list
                };

            }
            catch (Exception ex)
            {

                return new Response
                {
                    IsSucces = false,
                    Message = ex.Message
                };
            }
        }
    }
}
