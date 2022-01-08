using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WeatherDashboard.Pages
{
    public class LocationModel : PageModel
    {
        public string searchString { get; set; }
        public city[] cities { get; set; }
        public void OnGetAsync(string Location)
        {
            if (Location == null)
            {
                return;
            }
            searchString = Location;
            cities = new city[28340];
            string[] lines = System.IO.File.ReadAllLines("./wwwroot/uscities/uscities.csv");
            int x = 0;
            foreach (string line in lines)
            {
                // don't add to list to be displayed if it doesn't contain search string
                if (line.Contains(searchString)) {
                    string[] columns = line.Split(',');
                    int i = 0;
                    cities[x] = new();
                    foreach (string column in columns)
                    {
                        if (i == 0)
                            cities[x].name = column;
                        if (i == 1)
                            cities[x].state = column;
                        if (i == 2)
                            cities[x].state_id = column;
                        if (i == 3)
                            cities[x].lat = column;
                        if (i == 4)
                            cities[x].lng = column;
                        i++;
                    }
                    x++;
                }
            }
        }

        public async Task OnPost()
        {
            if (Request.Form.Count == 0)
                Redirect("/Location");
            Redirect("/Index/"+Request.Form["city"][0]);
        }
    }

    public class city
    {
        public string name { get; set; }
        public string state { get; set; }
        public string state_id { get; set; }
        public string lat { get; set;}
        public string lng { get; set; }
    }
}
