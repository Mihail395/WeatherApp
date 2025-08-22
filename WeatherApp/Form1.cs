using MetroFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeatherApp
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        private ImageConverter imageConverter;
        private Image windDirectionArrow;
        private Image sunset;
        private Image sunrise;
        private Image wind;
        private Image cloud;
        private Image termometer;
        String myCountry;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myCountry = "MK";
            cbMyCountry.Text = myCountry;

            windDirectionArrow = Properties.Resources.right_arrow;
            sunset = Properties.Resources.sunset;
            sunrise = Properties.Resources.sunrise;
            wind = Properties.Resources.windy;
            cloud = Properties.Resources.cloud;
            termometer = Properties.Resources.degre;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbCityName.Text.Length == 0) {
                    MetroFramework.MetroMessageBox.Show(this,"Please input a city name first!");
                }
                this.Text = "Showing the weather for " + tbCityName.Text;
                OpenWeatherAPI_Call(tbCityName.Text);
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "No weather data currerntly." + 
                    Environment.NewLine+ex.Message.ToString(), "Please try again later.");
            }
        }

        private async Task OpenWeatherAPI_Call(string cityName)
        {
           DataTable weatherData = new DataTable();
            weatherData = await API_Call(cityName);

            
        }

        private async Task<DataTable> API_Call(string cityName)
        {
            imageConverter = new ImageConverter();

            DataTable locationData = new DataTable();
            locationData.Columns.Add("Temp", typeof(string));
            locationData.Columns.Add("Temp Min", typeof(string));
            locationData.Columns.Add("Temp Max", typeof(string));
            locationData.Columns.Add("Clouds", typeof(string));
            locationData.Columns.Add("Humidity", typeof(string));
            locationData.Columns.Add("Weather Description", typeof(string));
            locationData.Columns.Add("Icon", typeof(byte[]));

            locationData.Columns.Add("Location Name", typeof(string));
            locationData.Columns.Add("Sunrise", typeof(string));
            locationData.Columns.Add("Sunset", typeof(string));
            locationData.Columns.Add("Wind Speed m/s", typeof(string));
            locationData.Columns.Add("Date", typeof(DateTime));
            locationData.Columns.Add("Day of week", typeof(string));
            locationData.Columns.Add("Day", typeof(string));
            locationData.Columns.Add("Month", typeof(string));
            locationData.Columns.Add("Year", typeof(string));
            locationData.Columns.Add("Time", typeof(string));
            locationData.Columns.Add("Direction", typeof(string));
            locationData.Columns.Add("Wind Direction", typeof(byte[]));

            var weatherData = await WeatherForecast.getApiData(cityName);
            string locationName = weatherData.city.name;
          
            return locationData;
        }
    }
}
