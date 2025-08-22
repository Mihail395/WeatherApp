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

        }
    }
}
