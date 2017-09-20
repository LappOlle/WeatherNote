using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Weather_Note.Models.POCO
{
    public class OpenWeatherService
    {
        private OpenWeatherContainer.RootObject root;
        private int CityID;
        private string ApiKey;
        private string openWeatherRequestUrl;

        public OpenWeatherService(int cityID = 2686469,string apiKey = "9ecf1e65b722afe0b4e5cabe65a2e820")
        {
            CityID = cityID;
            ApiKey = apiKey;
            openWeatherRequestUrl = "http://api.openweathermap.org/data/2.5/forecast?id=" + CityID + "&APPID=" + ApiKey + "&units=metric";
        }
        private async Task SetupWeatherData()
        {       
                var http = new HttpClient();
                var response = await http.GetAsync(openWeatherRequestUrl);
                var result = await response.Content.ReadAsStringAsync();
                var serializer = new DataContractJsonSerializer(typeof(OpenWeatherContainer.RootObject));
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
                root = (OpenWeatherContainer.RootObject)serializer.ReadObject(ms);
        }

        public async Task<string> GetMaxTemp(DateTime actualDate)
        {
            var date = actualDate.ToString();

            if (root == null)
            {
                await SetupWeatherData();
            }

            foreach (var maxTemp in root.list)
            {
                var jsonDate = maxTemp.dt_txt.Substring(0, 10);
                var tempDate = date.Substring(0, 10);
                if (jsonDate.Equals(tempDate))
                {
                    var roundedTemp = Math.Round(maxTemp.main.temp_max);
                    return roundedTemp.ToString() + "°";
                }
            }
            return "?";
        }
    }
}