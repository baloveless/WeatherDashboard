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
        public gridResponse gridRes { get; set; }
        public forecastResponse forecastRes { get; set; }
        public forecastResponse hourlyRes { get; set; }
        public ipLocation locationRes { get; set; }
        public forecastDisplay forecastFormatted { get; set; }
     

        public async Task OnGetAsync()
        {
            await GetLocation();
            await GetGrid(locationRes.lat, locationRes.lon);
            await GetForecast();
            await GetForecastHourly();
            await FormatForecast();
        }

        // use geolocation information to get local weather information
        public async Task GetGrid(float lat, float lon)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.weather.gov/points/"+lat+"%2C"+lon),
                Headers = {
                    { "Accept", "application/geo+json,text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                    { "Authority", "api.weather.gov"},
                    { "User-Agent", "hamrzysko%40gmail.com" }
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

        // get grid must be run before GetWeather can be used
        public async Task GetWeather()
        {
            if (coordsRes == null)
                return;
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(coordsRes.properties.forecastGridData),
                Headers = {
                    { "Accept", "application/geo+json,text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                    { "Authority", "api.weather.gov"},
                    { "User-Agent", "hamrzysko%40gmail.com" }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var myJObject = JsonConvert.DeserializeObject<gridResponse>(body);
                this.gridRes = (myJObject);
            }
        }

        // gets hourly forecast data based on coords res response
        public async Task GetForecastHourly()
        {
            if (coordsRes == null)
                return;
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(coordsRes.properties.forecastHourly + "?units=us"),
                Headers = {
                    { "Accept", "application/geo+json,text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                    { "Authority", "api.weather.gov"},
                    { "User-Agent", "hamrzysko%40gmail.com" },
                    { "forecast_temperature_qv", "true"},
                    { "forecast_wind_speed_qv", "true"},
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var myJObject = JsonConvert.DeserializeObject<forecastResponse>(body);
                this.hourlyRes = (myJObject);
            }
        }

        // gets hourly forecast data based on coords res response
        public async Task GetForecast()
        {
            if (coordsRes == null)
                return;
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(coordsRes.properties.forecast + "?units=us"),
                Headers = {
                    { "Accept", "application/geo+json,text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                    { "Authority", "api.weather.gov"},
                    { "User-Agent", "hamrzysko%40gmail.com" },
                    { "forecast_temperature_qv", "true"},
                    { "forecast_wind_speed_qv", "true"},
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var myJObject = JsonConvert.DeserializeObject<forecastResponse>(body);
                this.forecastRes = (myJObject);
            }
        }
        
        //formats forecast information for homepage 
        public async Task FormatForecast()
        {
            if (forecastRes == null)
                return;
            forecastFormatted = new forecastDisplay();
            int i = 0;
            forecastFormatted.days = new wholeDay[7];
            foreach(periodData period in forecastRes.properties.periods)
            {
                if (i % 2 == 0)
                {
                    forecastFormatted.days[i / 2] = new wholeDay();
                    forecastFormatted.days[i / 2].day = new periodData();
                    forecastFormatted.days[i / 2].day = period;
                }
                else {
                    forecastFormatted.days[i / 2].night = new periodData();
                    forecastFormatted.days[i / 2].night = period;
                }
                i++;
            }
        }



        // use geolocation API to find users location
        public async Task GetLocation()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.techniknews.net/ipgeo"),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var myJObject = JsonConvert.DeserializeObject<ipLocation>(body);
                this.locationRes = (myJObject);
            }
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
        public geometryMultiRes geometry { get; set; }
        public propertiesGridPoints properties { get; set; }
    }

    // used to store /gridpoints/{wfo}/{x},{y}/forecast/hourly
    public class forecastResponse
    {
        public string id { get; set; }
        public string type { get; set; }
        public geometryMultiRes geometry { get; set; }
        public forecastProperties properties { get; set; }
    }

    public class forecastDisplay
    {
        public wholeDay [] days { get; set; }
    }

    public class wholeDay
    {
        public periodData day { get; set; }
        public periodData night { get; set; }
    }

    public class ipLocation{
        public string status { get; set; }
        public string continent { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string regionName { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string timezone { get; set; }
        public string currency { get; set; }
        public string isp { get; set; }
        public string org { get; set; }
        public string As { get; set; }
        public string reverse { get; set; }
        public bool mobile { get; set; }
        public bool proxy { get; set; }
        public string ip { get; set; }
    }

    /*******************************************
     *          SHARED DATA CLASSES            *
     *******************************************/

    public class data
    {
        public string uom { get; set; }
        public temperatureValue[] values { get; set; }
    }

    public class temperatureValue
    {
        public string validTime { get; set; }
        public string value { get; set; }
    }

    public class distanceOrBearing
    {
        public string unitCode { get; set; }
        public string value { get; set; }
    }

   public class geometryRes
    {
        public string type { get; set; }
        public float[] coordinates { get; set; }
    }

    public class geometryMultiRes
    {
        public string type { get; set; }
        public float[][][] coordinates { get; set; }
    }



    /*******************************************
     *      COORDINATES RESPONSE DATA CLASSES  *
     *******************************************/
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

     public class propertiesRes
    {
        public string city { get; set; }
        public string state { get; set; }
        public distanceOrBearing distance { get; set; }
        public distanceOrBearing bearing { get; set; }
    }

    /*******************************************
     *      GRID RESPONSE DATA CLASSES         *
     *******************************************/

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
        public weatherData [] values { get; set; }
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
        public hazardData [] values { get; set; }
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

    /*******************************************
     *      FORECAST RESPONSE DATA CLASSES     * 
     *******************************************/

    public class forecastProperties
    {
        public string updated {get; set;}
        public string units {get; set;}
        public string forecastGenerator {get; set;}
        public string generatedAt{get; set;}
        public string updateTime{get; set;}
        public string validTimes{get; set;}
        public distanceOrBearing elevation {get; set;}
        public periodData [] periods {get; set;}
    }

    public class periodData
    {
        public int number { get; set; }
        public string name {get; set;}
        public string startTime{get; set;}
        public string endTime{get; set;}
        public bool isDayTime{get; set;}
        public int temperature{get; set;}
        public string temperatureUnit {get; set;}
        public string temperatureTrend{get; set;}
        public string windSpeed{get; set;}
        public string windDirection{get; set;}
        public string icon {get; set;}
        public string shortForecast {get; set;}
        public string detailedForecast {get; set;}
    }
    

}
