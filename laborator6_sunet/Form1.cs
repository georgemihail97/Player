using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laborator6_sunet
{
    public partial class Form1 : Form
    {
        int cnt = 0;
        int rot = 0;
        public Form1()
        {
            InitializeComponent();
            // Opreste rularea mp3-ului
            // axWindowsMediaPlayer1.settings.autoStart = false;
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            
            // Ruleaza continuu
            axWindowsMediaPlayer1.settings.setMode("loop", true);
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                // Asteapta o cuanta de timp si raporteaza progresul
                System.Threading.Thread.Sleep(50);
                worker.ReportProgress(cnt++);
            }
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (backgroundWorker1.CancellationPending)
            {

                backgroundWorker1.ReportProgress(0);
                return;
            }
            if (serialPort1.IsOpen)
                serialPort1.Close();
            serialPort1.Open();
            serialPort1.Write("r\r");
            string res = serialPort1.ReadLine();
            while (!res.Contains("<"))
                res += serialPort1.ReadLine();
            serialPort1.Close();
            // Exemplu de valori: 0.00 4999.12
            try
            {
                int value = int.Parse(res.Substring(res.IndexOf("=") + 1, res.IndexOf(".") - 1 - res.IndexOf("="))); // preia ce e intre egal si punct
                rot = Convert.ToInt32(value / 50);
                axWindowsMediaPlayer1.settings.volume = rot;
                textBox1.Text = rot.ToString();
                trackBar1.Value = rot;
            }
            catch (Exception ex)
            {

            }
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                textBox1.Text = "Anulat!";
            }
            else
            {
                textBox1.Text = "Complet!";
            }
        }
        void display_volume()
        {
            // Afiseaza valoarea curenta pentru volum
            textBox1.Text = axWindowsMediaPlayer1.settings.volume.ToString();
            trackBar1.Value = axWindowsMediaPlayer1.settings.volume;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Porneste operatia asincrona
            backgroundWorker1.RunWorkerAsync();
            // Porneste rularea mp3-ului
            axWindowsMediaPlayer1.Ctlcontrols.play();
            display_volume();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Creste volumul cu un procent
            axWindowsMediaPlayer1.settings.volume++;
            display_volume();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Scade volumul cu un procent
            axWindowsMediaPlayer1.settings.volume--;
            display_volume();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
