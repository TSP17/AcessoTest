using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Models;
using AcessoTeste.ThiagoPereira.Web.Application.Response.Api;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Infra.Repository
{
    public class AcessoRepository : IAcessoRepository
    {
        private readonly IConfiguration _configuration;

        public AcessoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AcessoAcountResponse> GetAccountByNumber(string AccountNumber)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{_configuration.GetValue<string>("AcessoApi:Url")}{_configuration.GetValue<string>("AcessoApi:AccountRoute")}{AccountNumber}");

                string responseData = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<AcessoAcountResponse>(responseData);
            }
        }

        public async Task<bool> Transfer(AcessoTransferRequest request)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"{_configuration.GetValue<string>("AcessoApi:Url")}{_configuration.GetValue<string>("AcessoApi:AccountRoute")}", content);

                string responseData = await response.Content.ReadAsStringAsync();

                return string.IsNullOrEmpty(responseData);
            }
        }
    }
}
