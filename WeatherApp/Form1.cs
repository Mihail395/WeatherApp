using MetroFramework;
using MetroFramework.Controls;
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
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json;

namespace WeatherApp
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        // Variables and file paths

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
        private readonly string appStateFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Weather_App",
            "lastWeather.json"
            );
        private string lastWeatherJson;
        public Form1()
        {
            InitializeComponent();
            this.Shown += Form1_Shown;
        }
        // Loading previous app state if it exists
        private async void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(appStateFile)) return;

                string savedJson = File.ReadAllText(appStateFile);
                if (string.IsNullOrWhiteSpace(savedJson)) return;

                var root = JsonConvert.DeserializeObject<WeatherForecast.Rootobject>(savedJson);
                if (root == null) return;

                
                tbCityName.Text = root.city.name;
                this.Text = "Showing the weather for " + root.city.name;

                
                var dt = await BuildDataTableFromRootAsync(root);

                
                PopulateUI(dt);
            }
            catch
            {
                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myCountry = "MK";
           

            windDirectionArrow = Properties.Resources.right_arrow;
            sunset = Properties.Resources.sunset;
            sunrise = Properties.Resources.sunrise;
            wind = Properties.Resources.windy;
            cloud = Properties.Resources.cloud;
            termometer = Properties.Resources.degre;
        }
        // Search button
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbCityName.Text.Length == 0) {
                    MetroFramework.MetroMessageBox.Show(this,"Please input a city name first!");
                    return;
                }
                this.Text = "Showing the weather for " + tbCityName.Text.ToString();
                this.Refresh();
                await OpenWeatherAPI_Call(tbCityName.Text);

                if (!string.IsNullOrEmpty(lastWeatherJson))
                {
                    string folder = Path.GetDirectoryName(appStateFile);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    File.WriteAllText(appStateFile, lastWeatherJson);
                }
                this.Refresh();

            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "No weather data currerntly." + 
                    Environment.NewLine+ex.Message.ToString(), "Please try again later.");
            }
        }
        // Create the data table when data is recieved from the API and fill the UI
        private async Task OpenWeatherAPI_Call(string cityName)
        {
            var weatherData = await API_Call(cityName);
            PopulateUI(weatherData);
        }

        // helper methods
        private void addTile(MetroPanel metroPanel1, MetroTile tile)
        {
            var nTiles = metroPanel1.Controls.OfType<MetroTile>().Count();
            tile.Location = new Point(nTiles * 220, 0);
            metroPanel1.Controls.Add(tile);
        }

        private void fillTile(MetroTile tile, DataRow item)
        {
            PictureBox weatherIcon = new PictureBox();
            weatherIcon.Size = new Size(70, 70);
            weatherIcon.Location = new Point(0, 0);
            weatherIcon.Image = (Image)imageConverter.ConvertFrom(item["Icon"]);
            weatherIcon.SizeMode = PictureBoxSizeMode.Zoom;
            weatherIcon.BackColor = Color.Transparent;
            tile.Controls.Add(weatherIcon);

            MetroLabel weatherDesc = new MetroLabel();
            weatherDesc.Text = item["Weather Description"].ToString();
            weatherDesc.Location = new Point(70, 30);
            weatherDesc.AutoSize = true;
            tile.Controls.Add(weatherDesc);

            PictureBox tempIcon = new PictureBox();
            tempIcon.Size = new Size(20, 20);
            tempIcon.Location = new Point(10, 70);
            tempIcon.Image = termometer;
            tempIcon.SizeMode = PictureBoxSizeMode.Zoom;
            tempIcon.BackColor = Color.Transparent;
            tile.Controls.Add(tempIcon);

            MetroLabel tempTxt = new MetroLabel();
            tempTxt.Text = item["Temp"].ToString()+ " °C";
            tempTxt.Location = new Point(40, 70);
            tempTxt.AutoSize = true;
            tile.Controls.Add(tempTxt);

            PictureBox windIcon = new PictureBox();
            windIcon.Size = new Size(20, 20);
            windIcon.Location = new Point(10, 95);
            windIcon.Image = wind;
            windIcon.SizeMode = PictureBoxSizeMode.Zoom;
            windIcon.BackColor = Color.Transparent;
            tile.Controls.Add(windIcon);

            MetroLabel windTxt = new MetroLabel();
            windTxt.Text = item["Wind Speed m/s"].ToString()+ " m/s";
            windTxt.Location = new Point(40, 95);
            windTxt.AutoSize = true;
            tile.Controls.Add(windTxt);

            PictureBox cloudsIcon = new PictureBox();
            cloudsIcon.Size = new Size(20, 20);
            cloudsIcon.Location = new Point(10, 120);
            cloudsIcon.Image = cloud;
            cloudsIcon.SizeMode = PictureBoxSizeMode.Zoom;
            cloudsIcon.BackColor = Color.Transparent;
            tile.Controls.Add(cloudsIcon);

            MetroLabel cloudsTxt = new MetroLabel();
            cloudsTxt.Text = item["Clouds"].ToString() + " %";
            cloudsTxt.Location = new Point(40, 120);
            cloudsTxt.AutoSize = true;
            tile.Controls.Add(cloudsTxt);

            PictureBox sunriseIcon = new PictureBox();
            sunriseIcon.Size = new Size(20, 20);
            sunriseIcon.Location = new Point(10, 145);
            sunriseIcon.Image = sunrise;
            sunriseIcon.SizeMode = PictureBoxSizeMode.Zoom;
            sunriseIcon.BackColor = Color.Transparent;
            tile.Controls.Add(sunriseIcon);

            MetroLabel sunriseTxt = new MetroLabel();
            sunriseTxt.Text = Convert.ToDateTime(item["Sunrise"].ToString()).ToShortTimeString();
            sunriseTxt.Location = new Point(40, 145);
            sunriseTxt.AutoSize = true;
            tile.Controls.Add(sunriseTxt);

            PictureBox sunsetIcon = new PictureBox();
            sunsetIcon.Size = new Size(20, 20);
            sunsetIcon.Location = new Point(110, 145);
            sunsetIcon.Image = sunset;
            sunsetIcon.SizeMode = PictureBoxSizeMode.Zoom;
            sunsetIcon.BackColor = Color.Transparent;
            tile.Controls.Add(sunsetIcon);

            MetroLabel sunsetTxt = new MetroLabel();
            sunsetTxt.Text = Convert.ToDateTime(item["Sunset"].ToString()).ToShortTimeString();
            sunsetTxt.Location = new Point(140, 145);
            sunsetTxt.AutoSize = true;
            tile.Controls.Add(sunsetTxt);

            PictureBox windDirIcon = new PictureBox();
            windDirIcon.Size = new Size(40, 40);
            windDirIcon.Location = new Point(140, 90);
            windDirIcon.Image = (Image)imageConverter.ConvertFrom(item["Wind Direction"]);
            windDirIcon.SizeMode = PictureBoxSizeMode.Zoom;
            windDirIcon.BackColor = Color.Transparent;
            tile.Controls.Add(windDirIcon);

            MetroLabel windDirTxt = new MetroLabel();
            windDirTxt.Text = "Wind Direction";
            windDirTxt.Location = new Point(110, 70);
            windDirTxt.AutoSize = true;
            tile.Controls.Add(windDirTxt);
        }
        

        private MetroTile createNewTile(string v)
        {
            MetroTile tile = new MetroTile();
            tile.Text = v;
            tile.Style = MetroColorStyle.Blue;
            tile.TileTextFontSize = MetroTileTextSize.Tall;
            tile.UseStyleColors = true;
            tile.Theme = MetroThemeStyle.Dark;
            tile.Tag = v + "_Tag";
            tile.Size = new Size(210, 210);
            return tile;

        }
        // API call logic and how the data is stored
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

            // API call made in the separate class WeatherForecast
            var weatherData = await WeatherForecast.getApiData(cityName);
            lastWeatherJson = JsonConvert.SerializeObject(weatherData);
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
        // helper
        private void UnloadTiles(MetroPanel metroPanel1)
        {
            var tiles = metroPanel1.Controls.OfType<MetroTile>().ToArray();
            for (int i = tiles.Count() - 1; i >= 0; i--)
            {
                metroPanel1.Controls.Remove(tiles[i]);
                tiles[i].Dispose();
            }
            metroPanel1.AutoScrollPosition = new Point(0, 0);
            metroPanel1.HorizontalScroll.Value = 0;
        }

        // Load image safely without locking the file , error that image is being used if done normally!
        private Image LoadImageWithoutLock(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return Image.FromStream(fs);
            }
        }
        // helper methods
        private Image rotateImage(Image windDirectionArrow, float angle)
        {
            if (windDirectionArrow == null)
                throw new ArgumentNullException(nameof(windDirectionArrow));

            float adjustedAngle = angle - 90f;
            adjustedAngle += 180f;

            Bitmap rotatedBmp = new Bitmap(windDirectionArrow.Width, windDirectionArrow.Height);
            rotatedBmp.SetResolution(windDirectionArrow.HorizontalResolution, windDirectionArrow.VerticalResolution);
            using (Graphics g = Graphics.FromImage(rotatedBmp))
            {
                g.TranslateTransform(windDirectionArrow.Width / 2f, windDirectionArrow.Height / 2f);
                g.RotateTransform(adjustedAngle);
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

        private async Task<DataTable> BuildDataTableFromRootAsync(WeatherForecast.Rootobject weatherData)
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

            string locationName = weatherData.city.name;
            string sunrise = convertUnixToDT(weatherData.city.sunrise).ToString();
            string sunset = convertUnixToDT(weatherData.city.sunset).ToString();

            
            if (!Directory.Exists(weatherIconsDownloadPath))
                Directory.CreateDirectory(weatherIconsDownloadPath);

            foreach (var wData in weatherData.list)
            {
                string windSpeed = wData.wind.speed.ToString();
                string windDirection = wData.wind.deg.ToString();
                string currentTemp = Math.Round(convertKelvinToCels(wData.main.temp), 1).ToString();
                string tempMin = Math.Round(convertKelvinToCels(wData.main.temp_min), 1).ToString();
                string tempMax = Math.Round(convertKelvinToCels(wData.main.temp_max), 1).ToString();
                string wDescription = wData.weather[0].description;
                string clouds = wData.clouds.all.ToString();
                string humidity = wData.main.humidity.ToString();
                string weatherIcon = wData.weather[0].icon + "@2x.png";

                byte[] wIconByte = null;
                string iconPath = Path.Combine(weatherIconsDownloadPath, weatherIcon);

                
                if (File.Exists(iconPath))
                {
                    wIconByte = await Task.Run(() => File.ReadAllBytes(iconPath));
                }

                DateTime time = Convert.ToDateTime(wData.dt_txt);
                Image windDirectionIcon = rotateImage(windDirectionArrow, float.Parse(windDirection));
                byte[] windDirectionIconByte = (byte[])imageConverter.ConvertTo(windDirectionIcon, typeof(byte[]));

                locationData.Rows.Add(new Object[] {
            currentTemp, tempMin, tempMax, clouds, humidity,
            wDescription, wIconByte, locationName, sunrise, sunset,
            windSpeed, time, time.DayOfWeek, time.Day, time.Month,
            time.Year, time.TimeOfDay, windDirection, windDirectionIconByte
        });

                windDirectionIcon.Dispose();
            }

            if (locationData.Rows.Count > 0)
            {
                locationData.DefaultView.Sort = "Date asc";
            }

            return locationData;
        }

        private void PopulateUI(DataTable weatherData)
        {
            chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
            this.chart1.Series.Clear();
            this.chart1.Titles.Clear(); 

            Title title = new Title();
            title.Font = new Font("Arial", 16, FontStyle.Bold);
            title.Text = "Temperature in the next 5 days from " + DateTime.Now.DayOfWeek.ToString() +
                         " to " + DateTime.Now.AddDays(5).DayOfWeek.ToString()+ " (°C)";
            chart1.Titles.Add(title);

            Series seriesWeatherData = this.chart1.Series.Add("°C");
            seriesWeatherData.ChartType = SeriesChartType.Line;
            seriesWeatherData.Color = Color.Green;
            seriesWeatherData.BorderWidth = 3;
            seriesWeatherData.IsVisibleInLegend = false;

            UnloadTiles(metroPanel1);
            foreach (DataRow item in weatherData.Rows)
            {
                seriesWeatherData.Points.AddXY(item["Day of week"].ToString() + " " +
                    item["Time"].ToString(), Convert.ToDouble(item["Temp"]));
                MetroTile tile = createNewTile(item["Day of week"].ToString() + " " +
                    item["Time"].ToString());
                fillTile(tile, item);
                addTile(metroPanel1, tile);
            }
        }


        private void metroPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
