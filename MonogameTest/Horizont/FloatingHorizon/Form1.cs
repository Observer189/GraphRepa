using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloatingHorizon
{
    public partial class Form1 : Form
    {
        HorizonDrawer horizonDrawer = null;
        List<functionType> functions = new List<functionType>();

        public Form1()
        {
            InitializeComponent();
            DoMyInitialization();
        }

        private void DoMyInitialization()
        {
            #region functions
            List<String> funcNames = new List<String>();
            funcNames.Add("Cos(Cos(z) - Sin(x));");
            funcNames.Add("Функция \"Вейвлета\"");
            funcNames.Add("Функция \"Лекция1\"");
            funcNames.Add("Функция \"Лекция2\"");
            funcNames.Add("Функция \"Лекция3\"");

            functions.Add(Functions.CosDelta);
            functions.Add(Functions.Wavelet);
            functions.Add(Functions.Lect);
            functions.Add(Functions.Lect1);
            functions.Add(Functions.Lect2);

            cmbBoxFunctions.Items.AddRange(funcNames.ToArray());
            cmbBoxFunctions.SelectedIndex = 0;
            #endregion


            #region horizontDrawer
            horizonDrawer = new HorizonDrawer(picBox.Width, picBox.Height);
            InitializeHorizonDrawer();
            #endregion

        }

        private void InitializeHorizonDrawer()
        {
            try
            {
                horizonDrawer.SetBoundsOnX(-5, 5);
                horizonDrawer.SetBoundsOnZ(-5, 5);
                horizonDrawer.SetXZsteps(0.05, 0.2);

                horizonDrawer.SetAngleX(trackBarX.Value);
                horizonDrawer.SetAngleY(trackBarY.Value);
                horizonDrawer.SetAngleZ(trackBarZ.Value);
            }
            catch (System.Exception)
            {
                MessageBox.Show("Неверные входные данные");
            }
            horizonDrawer.SetBackColor(Color.Black);
            horizonDrawer.SetMainColor(Color.Gray);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitializeHorizonDrawer();
            ReDraw();
        }

        private void trackBarX_ValueChanged(object sender, EventArgs e)
        {
            horizonDrawer.SetAngleX(trackBarX.Value);
            ReDraw();
        }

        private void trackBarY_ValueChanged(object sender, EventArgs e)
        {
            horizonDrawer.SetAngleY(trackBarY.Value);
            ReDraw();
        }

        private void trackBarZ_ValueChanged(object sender, EventArgs e)
        {
            horizonDrawer.SetAngleZ(trackBarZ.Value);
            ReDraw();
        }

        private void ReDraw()
        {
            horizonDrawer.Draw(picBox.CreateGraphics(), functions[cmbBoxFunctions.SelectedIndex]);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }


    public static class Functions
    {
        public static double CosDelta(double x, double z)
        {
            return Math.Cos(Math.Cos(z) - Math.Sin(x));
        }

        public static double Wavelet(double x, double z)
        {
            return Math.Cos(5 * x) * Math.Cos(5 * z) * Math.Exp(-(x * x + z * z));
        }

        public static double Lect(double x, double z)
        {
            double r = x * x + z * z + 1;
            return 5 * (Math.Cos(r) / r + 0.1);
        }

        public static double Lect1(double x, double z)
        {
            double r = x * x + z * z;
            return Math.Cos(r) / (r + 1);

        }
        public static double Lect2(double x, double z)
        {
            return Math.Sin(x) + Math.Cos(x);
        }


    }
}
