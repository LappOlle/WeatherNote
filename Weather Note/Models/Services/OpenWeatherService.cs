﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private OpenWeatherProxy.RootObject root; //RootObject for the Json Response from the weather API.
        private int CityID;
        private string ApiKey;
        private string openWeatherRequestUrl;

        /// <summary>
        /// It's optional to send cityID and apiKey, Default is örnsköldsvik and apiKey is mine.
        /// </summary>
        /// <param name="cityID">Send what cityID you want to check.</param>
        /// <param name="apiKey">Send what your ApiKey is.</param>
        public OpenWeatherService(int cityID = 2686469,string apiKey = "9ecf1e65b722afe0b4e5cabe65a2e820")
        {
            CityID = cityID;
            ApiKey = apiKey;
            openWeatherRequestUrl = "http://api.openweathermap.org/data/2.5/forecast?id=" + CityID + "&APPID=" + ApiKey + "&units=metric";
        }

        /*Method for connect and GET the JSON response and then parse it to the objects.
         I have made the classes with the json2csharp tool. Visit http://json2csharp.com/ for further information.*/
        private async Task SetupWeatherData()
        {       
                var http = new HttpClient();
                var response = await http.GetAsync(openWeatherRequestUrl);
                var result = await response.Content.ReadAsStringAsync();
                var serializer = new DataContractJsonSerializer(typeof(OpenWeatherProxy.RootObject));
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
                root = (OpenWeatherProxy.RootObject)serializer.ReadObject(ms);
        }

        /// <summary>
        /// Method to get the MaxTemp of the date you pass into it. If the date is outside the 
        /// openweatherapi span it will return "?". You have to pass a date from today and 4 days forward.
        /// </summary>
        /// <param name="actualDate">Send a datetime object.</param>
        /// <returns>Returns the Max temp of that day.</returns>
        public async Task<string> GetMaxTemp(DateTime actualDate)
        {
            var date = actualDate.ToString();

            if (root == null)//if root is null we do a get request and setup the json data.
            {
                await SetupWeatherData();
            }

            /*Iterate through every day in the json data, 
            and then return the max_temp from the item with same date as the passed DateTime Object.*/
            double highestMaxTemp = 0.0;
            foreach (var item in root.list)
            {
                var jsonDate = item.dt_txt.Substring(0, 10);//cut away the time from it.
                var passedModelDate = date.Substring(0, 10);//-||-
                if (jsonDate.Equals(passedModelDate))
                {
                    //Looking what the highest maxtemp is. it's 8 maxtemp for everyday.
                    if (highestMaxTemp < item.main.temp_max)
                    {
                        highestMaxTemp = item.main.temp_max;
                    }
                }
            }
            if(highestMaxTemp != 0)
            {
                //I round the temp_max to closest integer. it's a 2 decimal data before i round it.
                var maxTemp = Math.Round(highestMaxTemp);
                return maxTemp.ToString() + "° (" + highestMaxTemp + "°)";
            }
           else
            {
                return "?"; //If there is no data for the passed DateTime object it returns "?".
            }
        }
    }
}