using System;
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
    public class IndexModel : PageModel
    {

        public coordinatesResponse coordsRes { get; set; }

        public async Task OnGetAsync()
        {
            await GetGrid(0, 0);
        }

        public async Task GetGrid(float lat, float lon)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.weather.gov/points/45.5152%2C-122.6784"),
                Headers = {
                    { "Accept", "application/geo+json,text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                    { "Authority", "api.weather.gov"},
                    { "User-Agent", "hamrzysko+at+gmail.com" }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var myJObject = JsonConvert.DeserializeObject<coordinatesResponse>(body);
                this.coordsRes = (myJObject);
            }

        }

        public async Task GetWeather()
        {

        }
    }

    // used to store /points/{latitude},{longitude} requests
    public class coordinatesResponse
    {
        public string[][] context { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public geometryRes geometry { get; set; }
        public propertiesSection properties { get; set; }
    }

    // used to store /gridpoints/{wfo}/{x},{y} requests
    public class gridResponse
    {
        public string id { get; set; }
        public string type { get; set; }
        public geometryRes geometry { get; set; }
        public propertiesGridPoints properties { get; set; }
    }

    public class propertiesGridPoints
    {
        public string id { get; set; }
        public string type { get; set; }
        public string updateTime { get; set; }
        public string validTimes { get; set; }
        public distanceOrBearing elevation { get; set; }
        public string forecastOffice { get; set; }
        public string gridId { get; set; }
        public string gridX { get; set; }
        public string gridY { get; set; }
        public data temperature { get; set; }
        public data dewpoint { get; set; }
        public data maxTemperature { get; set; }
        public data minTemperature { get; set; }
        public data relativeHumidity { get; set; }
        public data apparentTemperature { get; set; }
        public data heatIndex { get; set; }
        public data windChill { get; set; }
        public data skyCover { get; set; }
        public data windDirection { get; set; }
        public data windSpeed { get; set; }
        public data windGust { get; set; }
        public weatherDataList weather { get; set; }
        public hazardDataList hazards { get; set; }
        public data probabilityOfPrecipitation { get; set; }
        public data quantitativePrecipitation { get; set; }
        public data iceAccumulation { get; set; }
        public data snowfallAmount { get; set; }
        public data snowLevel { get; set; }
        public data ceilingHeight { get; set; }
        public data visibility { get; set; }
        public data transportWindSpeed { get; set; }
        public data transportWindDirection { get; set; }
        public data mixingHeight { get; set; }
        public data hainesIndex { get; set; }
        public data lightningActivityLevel { get; set; }
        public data twentyFootWindSpeed { get; set; }
        public data twentyFootWindDirection { get; set; }
        public data waveHeight { get; set; }
        public data wavePeriod { get; set; }
        public data waveDirection { get; set; }
        public data primarySwellHeight { get; set; }
        public data primarySwellDirection { get; set; }
        public data secondarySwellHeight { get; set; }
        public data secondarySwellDirection { get; set; }
        public data wavePeriod2 { get; set; }
        public data windWaveHeight { get; set; }
        public data dispersionIndex { get; set; }
        public data pressure { get; set; }
        public data probabilityOfTropicalStormWinds { get; set; }
        public data probabilityOfHurricaneWinds { get; set; }
        public data potentialOf15mphWinds { get; set; }
        public data potentialOf25mphWinds { get; set; }
        public data potentialOf35mphWinds { get; set; }
        public data potentialOf45mphWinds { get; set; }
        public data potentialOf20mphWindGusts { get; set; }
        public data potentialOf30mphWindGusts { get; set; }
        public data potentialOf40mphWindGusts { get; set; }
        public data potentialOf50mphWindGusts { get; set; }
        public data potentialOf60mphWindGusts { get; set; }
        public data grasslandFireDangerIndex { get; set; }
        public data probabilityOfThunder { get; set; }
        public data davisStabilityIndex { get; set; }
        public data atmosphericDispersionIndex { get; set; }
        public data lowVisibilityOccurrenceRiskIndex { get; set; }
        public data stability { get; set; }
        public data redFlagThreatIndex { get; set; }
    }

    public class weatherDataList
    {
        public weatherData values { get; set; }
    }

    public class weatherData
    {
        public string validTime { get; set; }
        public weatherDescription [] value { get; set; }

    }

    public class weatherDescription
    {
        public string coverage { get; set; } 
        public string weather { get; set; } 
        public string intensity { get; set; } 
        public distanceOrBearing visibility { get; set; } 
        public string [] attributes { get; set; }
    }

    public class hazardDataList
    {
        public hazardData values { get; set; }
    }

    public class hazardData
    {
        public string validTime { get; set; }
        public hazardDescription [] value { get; set; }    
    }

    public class hazardDescription
    {
        public string phenomenon { get; set; }
        public string significance { get; set; }
        public int event_number { get; set; }
    }

    public class data
    {
        public string uom { get; set; }
        public temperatureValue[] values { get; set; }
    }

    public class temperatureValue
    {
        public string validTime { get; set; }
        public float value { get; set; }
    }



    public class propertiesSection
    {
        public string id { get; set; }
        public string type { get; set; }
        public string cwa { get; set; }
        public string forecastOffice { get; set; }
        public string gridId { get; set; }
        public int gridX { get; set; }
        public int gridY { get; set; }
        public string forecast { get; set; }
        public string forecastHourly { get; set; }
        public string forecastGridData { get; set; }
        public string observationStations { get; set; }
        public relativeLocationSection relativeLocation { get; set; }
        public string forecastZone { get; set; }
        public string county { get; set; }
        public string fireWeatherZone { get; set; }
        public string timeZone { get; set; }
        public string radarStation { get; set; }
    }

    public class relativeLocationSection
    {
        public string type { get; set; }
        public geometryRes geometry { get; set; }
        public propertiesRes properties { get; set; }
    }

    public class geometryRes
    {
        public string type { get; set; }
        public float[] coordinates { get; set; }
    }

    public class propertiesRes
    {
        public string city { get; set; }
        public string state { get; set; }
        public distanceOrBearing distance { get; set; }
        public distanceOrBearing bearing { get; set; }
    }

    public class distanceOrBearing
    {
        public string unitCode { get; set; }
        public float value { get; set; }
    }

}
