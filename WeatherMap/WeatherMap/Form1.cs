using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherMap
{
    public partial class Form1 : Form
    {
        //venore3812@yehudabx.com
        private const string _api = "c5a12aa2e8956ae6b436e591e11975b8";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        public async Task<JObject> GetData(Uri url)
        {
            try
            {
                var client = new WebClient();
                client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                client.Encoding = Encoding.UTF8;
                string data = await client.DownloadStringTaskAsync(url);

                dynamic json = JsonConvert.DeserializeObject<dynamic>(data);
                if (json is JArray) return null; 
                return (JObject)json;
            }
            catch (WebException ex)
            {
                string resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                MessageBox.Show(resp);
            }
            return null;
        }

        private async void buttonGetPressure_Click(object sender, EventArgs e)
        {
            try
            { 
                listBox1.Items.Clear();
                listBox1.Items.Add("Ожидайте...");

                string latitude = "52.0278"; // широта
                string longitude = "47.8007";// долгота
                string url = string.Format("https://api.openweathermap.org/data/2.5/onecall?lat={0}&lon={1}&exclude=minutely,hourly,current&appid={2}&lang=RU&units=metric", latitude, longitude, _api);

                JObject jobj = await GetData(new Uri(url));

                Weather obj = new Weather(jobj);
                 
                var list = obj.GetPressures();

                var result = (string.IsNullOrEmpty(list))?"Данные не верны":"Максимальное давление : "+list + " hPa";

                listBox1.Items.Clear();
                listBox1.Items.Add(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
        }

        private async void buttonTemperature_Click(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();
                listBox1.Items.Add("Ожидайте...");
                string latitude = "52.0278"; // широта
                string longitude = "47.8007";// долгота
                string url = string.Format("https://api.openweathermap.org/data/2.5/onecall?lat={0}&lon={1}&exclude=minutely,hourly,current&appid={2}&lang=RU&units=metric", latitude, longitude, _api);

                JObject jobj = await GetData(new Uri(url));

                Weather obj = new Weather(jobj);

                var list = obj.GetMinTemperture();

                listBox1.Items.Clear();
                listBox1.Items.Add("Минимальная разница: " + list.Keys.ElementAt(0).ToShortDateString() + " " + list.Values.ElementAt(0) + "°C");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
        }
    }
}
