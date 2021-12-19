using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WeatherDashboard.Pages
{
    public class AlertModel : PageModel
    {
        public Feature alert { get; set; }
        public async Task OnGetAsync(string AlertId)
        {
            if (AlertId == null || AlertId.Length == 0)
                Redirect("~/");
            
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.weather.gov/alerts/" + AlertId),
                Headers = {
                    { "Accept", "application/geo+json,text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                    { "Authority", "api.weather.gov"},
                    { "User-Agent", "hamrzysko%40gmail.com" },
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var myJObject = JsonConvert.DeserializeObject<Feature>(body);
                this.alert = (myJObject);
            }
        }
        
        public string extractUrl(string from)
        {
            Regex urlFinder = new Regex(@"^(http:\/\/www\.|https:\/\/www\.| http:\/\/| https:\/\/)?[a-z0 - 9]+([\-\.]{ 1}[a-z0 - 9]+)*\.[a-z]{ 2,5}(:[0 - 9]{1,5})?(\/.*)?$");
            string url = urlFinder.Match(from).Value;
            return urlFinder.Replace(from, "<a class ='text-dark' href='" + url + "'>" + url + "</a>");
        }
    }
}
