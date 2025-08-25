using MetroFramework;
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

        private string weatherIconsDownloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Weather_App",
            "Icons"
            );
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
            string sunrise = convertUnixToDT(double.Parse(weatherData.city.sunrise.ToString(), System.Globalization.CultureInfo.InvariantCulture)).ToString();
            string sunset = convertUnixToDT(double.Parse(weatherData.city.sunset.ToString(), System.Globalization.CultureInfo.InvariantCulture)).ToString();
            foreach (var wData in weatherData.list)
            {
                string windSpeed = wData.wind.speed.ToString();
                string windDirection = wData.wind.deg.ToString();
                string currentTemp = Math.Round(convertKelvinToCels(wData.main.temp),1).ToString();
                string tempMin = Math.Round(convertKelvinToCels(wData.main.temp_min), 1).ToString();
                string tempMax = Math.Round(convertKelvinToCels(wData.main.temp_max), 1).ToString();
                string wDescription = wData.weather[0].description;
                string clouds = wData.clouds.all.ToString();
                string humidity = wData.main.humidity.ToString();
                string weatherIcon = wData.weather[0].icon + "@2x.png";
                Image wIcon;
                byte[] wIconByte;

                if (!Directory.Exists(weatherIconsDownloadPath))
                {
                    Directory.CreateDirectory(weatherIconsDownloadPath);
                }
                if (File.Exists(weatherIconsDownloadPath + weatherIcon))
                {
                    wIcon = LoadImageWithoutLock(Path.Combine(weatherIconsDownloadPath, weatherIcon));
                    wIconByte = (byte[]) imageConverter.ConvertTo(wIcon,typeof(byte[])); 
                }
                else
                {
                    using (WebClient downloadC = new WebClient())
                    {
                        await Task.Run(() => downloadC.DownloadFileTaskAsync("https://openweathermap.org/img/wn/"
                            + weatherIcon, Path.Combine(weatherIconsDownloadPath, weatherIcon)));
                    }
                    wIcon = LoadImageWithoutLock(Path.Combine(weatherIconsDownloadPath, weatherIcon));
                    wIconByte = (byte[])imageConverter.ConvertTo(wIcon, typeof(byte[]));
                }

                DateTime time = Convert.ToDateTime(wData.dt_txt);
                Image windDirectionIcon = rotateImage(windDirectionArrow, float.Parse(windDirection));
                byte[] windDirectionIconByte = (byte[])imageConverter.ConvertTo(windDirectionIcon, typeof(byte[]));

                locationData.Rows.Add(new Object[] { currentTemp, tempMin, tempMax, clouds, humidity,
                wDescription, wIconByte, locationName, sunrise, sunset, windSpeed, time, time.DayOfWeek,
                time.Day, time.Month, time.Year, time.TimeOfDay, windDirection, windDirectionIconByte });

            }

            if (locationData.Rows.Count > 0)
            {
                locationData.DefaultView.Sort = "Date asc";
            }

            return locationData;
        }

        // Load image safely without locking the file
        private Image LoadImageWithoutLock(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return Image.FromStream(fs);
            }
        }

        private Image rotateImage(Image windDirectionArrow, float angle)
        {
            if (windDirectionArrow == null)
                throw new ArgumentNullException(nameof(windDirectionArrow));

            Bitmap rotatedBmp = new Bitmap(windDirectionArrow.Width, windDirectionArrow.Height);
            rotatedBmp.SetResolution(windDirectionArrow.HorizontalResolution, windDirectionArrow.VerticalResolution);
            using (Graphics g = Graphics.FromImage(rotatedBmp))
            {
                g.TranslateTransform(windDirectionArrow.Width / 2f, windDirectionArrow.Height / 2f);
                g.RotateTransform(angle);
                g.TranslateTransform(-windDirectionArrow.Width / 2f, -windDirectionArrow.Height / 2f);
                g.DrawImage(windDirectionArrow, new Point(0, 0));
            }
            return rotatedBmp;
        }

        private double convertKelvinToCels(double temp)
        {
            return temp - 273.15;
        }

        private DateTime convertUnixToDT(double unixTime)
        {
            long seconds = Convert.ToInt64(unixTime);
            DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(seconds);
            return dto.LocalDateTime;
        }
    }
}
