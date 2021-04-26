using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherMap
{

    public class PairVal
    {
      
        public DateTime Date { get; set; }
        public decimal value1 { get; set; }
        public decimal value2 { get; set; }

     
    }

    class Weather
    {
        private JObject _mainObj;

        public List<decimal> List = new List<decimal>();

        public List<PairVal> List2 = new List<PairVal>();

        public Weather(JObject jobj )
        { 
            _mainObj = jobj; 
        }
         

        public string GetPressures()
        {
            // https://docs.microsoft.com/ru-ru/dotnet/api/system.double?view=net-5.0

            List<int> pressures = new List<int>();
           
            JArray daily = (JArray) _mainObj["daily"];

            if(daily == null)return null;

            int z = 0;
            DateTime date = DateTime.Now;
            for (int i = 0; i < daily.Count - 2; i++)
            {
                double dt = double.Parse(daily[i]["dt"].ToString());
                date = ConvertUnix(dt);

                if (Int32.TryParse(daily[i]["pressure"].ToString(), NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out z))
                {
                    pressures.Add(z);
                }
            }

            if(pressures.Count==0)return null;

            return date.ToShortDateString()+" "+MaxPressure(pressures.ToArray()).ToString();
             
        }


        private int MaxPressure(int[] arr)
        {
            int temp = arr[0];
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] > temp) temp = arr[i];
            }
            return temp;
        }

        private DateTime ConvertUnix(double tms)
        {
            // First make a System.DateTime equivalent to the UNIX Epoch.
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

            // Add the number of seconds in UNIX timestamp to be converted.
            dateTime = dateTime.AddSeconds(tms);

            return dateTime;

        }



        public Dictionary<DateTime, decimal> GetMinTemperture()
        {
            List<decimal> list = new List<decimal>();

            List<PairVal> pair = new List<PairVal>();

            JArray daily = (JArray)_mainObj["daily"];

            if (daily == null) return null;

            decimal z = 0m ;
            bool success = true;
            for (int i = 0; i < daily.Count; i++)
            {
                PairVal p = new PairVal();
                double dt = double.Parse(daily[i]["dt"].ToString());
                DateTime date = ConvertUnix(dt);
                p.Date = date;

                if (Decimal.TryParse(daily[i]["temp"]["night"].ToString(), NumberStyles.AllowDecimalPoint, NumberFormatInfo.CurrentInfo, out z))
                {  
                    p.value1 = z;
                }
                else
                {
                    success = false;
                }

                if (Decimal.TryParse(daily[i]["temp"]["morn"].ToString(), NumberStyles.AllowDecimalPoint, NumberFormatInfo.CurrentInfo, out z))
                { 
                    p.value2 =z;
                }
                else
                {
                    success = false;
                }
                 

                if(success)pair.Add(p);

            }
             
            if (pair.Count == 0) return null;

             
            List2 = pair;

            Dictionary<DateTime, decimal> result = new Dictionary<DateTime, decimal>();

            decimal diff = decimal.MaxValue;
            DateTime temp = DateTime.Now;
            for (int i = 0; i < pair.Count - 1; i++)
            {
                for (int j = i + 1; j < pair.Count; j++)
                {
                    if (Math.Abs((pair[i].value1 - pair[j].value2)) < diff)
                    {
                        diff = Math.Abs((pair[i].value1 - pair[j].value2));
                        temp = pair[i].Date;
                    }
                }
            }

            result.Add(temp, diff);

            return result;
        }


 

     


    }


}
