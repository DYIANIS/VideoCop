using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoCop
{
    public partial class Form1 : Form
    {

        public TextProgressBar TextProgressBar1 = new TextProgressBar();
        public TextProgressBar TextProgressBar2 = new TextProgressBar();

        Stopwatch stopwatch = new Stopwatch();

        private static string file = "usersList.txt";
        Worker worker = new Worker();

        public Form1()
        {
            InitializeComponent();

            button1.Enabled = false;
            button3.Enabled = false;

            checkFile(file);
            List<String> usersList = getUsersList(file);
            foreach (string user in usersList)
            {
                comboBox1.Items.Add(user);
            }

            updateComboBox2();
            updateComboBox3();

            comboBox1.SelectedIndex = 0;

            if (IsAllSelected())
            {
                button1.Enabled = true;
            }

            TextProgressBar1.VisualMode = ProgressBarDisplayMode.NoText;

            TextProgressBar1.Minimum = 0;
            TextProgressBar1.Maximum = 100;
            TextProgressBar1.Value = 0;

            TextProgressBar1.Location = new Point(11, 462);
            TextProgressBar1.Size = new Size(529, 17);

            TextProgressBar2.VisualMode = ProgressBarDisplayMode.NoText;

            TextProgressBar2.Minimum = 0;
            TextProgressBar2.Maximum = 100;
            TextProgressBar2.Value = 0;

            TextProgressBar2.Location = new Point(11, 485);
            TextProgressBar2.Size = new Size(529, 23);

            this.Controls.Add(TextProgressBar1);
            this.Controls.Add(TextProgressBar2);

        }

        private bool IsAllSelected()
        {
            if (comboBox1.SelectedIndex == -1 | comboBox2.SelectedIndex == -1 | comboBox3.SelectedIndex == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private List<string> getUsersList(string file)
        {
            List<String> usersList = new List<String>();

            StreamReader sr = new StreamReader(file);
            while (!sr.EndOfStream)
            {
                usersList.Add(sr.ReadLine());
            }
            sr.Close();

            return usersList;
        }

        private void checkFile(string file)
        {
            FileInfo fi = new FileInfo(file);

            if (!fi.Exists)
            {
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine("Пользователь");
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAllSelected())
            {
                button1.Enabled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAllSelected())
            {
                button1.Enabled = true;
            }

            update_ListBox();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAllSelected())
            {
                button1.Enabled = true;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            TextProgressBar1.Value = 0;
            TextProgressBar1.VisualMode = ProgressBarDisplayMode.NoText;

            if (checkBox1.Checked == true)
            {
                dateTimePicker1.Enabled = false;
            }
            else
            {
                dateTimePicker1.Enabled = true;
            }

            update_ListBox();
            TextProgressBar1.Value = 100;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;

            stopwatch = new Stopwatch();
            stopwatch.Start();

            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            dateTimePicker1.Enabled = false;
            checkBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;

            worker = new Worker();
            worker.ComboBox1Text = comboBox1.Text;
            worker.ComboBox3Text = comboBox3.Text;
            worker.Form1 = this;

            listBox1.Enabled = false;

            backgroundWorker1.RunWorkerAsync();
        }

        private void update_ListBox()
        {
            string ComboBox2Text = comboBox2.Text;

            if (ComboBox2Text.Length < 4) return;

            string sDir = ComboBox2Text.Substring(ComboBox2Text.IndexOf(": ") - 1, 2);

            Сopier Сopier = new Сopier();
            Сopier.SetForm(this);
            Сopier.SearchFiles(sDir + "\\");
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            TextProgressBar1.Value = 0;
            TextProgressBar1.VisualMode = ProgressBarDisplayMode.NoText;
            update_ListBox();
            TextProgressBar1.Value = 100;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var value = e.ProgressPercentage;
            TextProgressBar1.Value = value;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button3.Enabled = false;
            button1.Enabled = true;

            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            MessageBox.Show("Копирование завершено!\n\n" + "Время копирования: " + timeSpan.Hours + "ч " + timeSpan.Minutes + "м " + timeSpan.Seconds + "c"); 

            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            dateTimePicker1.Enabled = true;
            checkBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;

            TextProgressBar1.Value = 0;
            TextProgressBar1.VisualMode = ProgressBarDisplayMode.NoText;

            TextProgressBar2.Value = 0;
            TextProgressBar2.VisualMode = ProgressBarDisplayMode.NoText;

            listBox1.Enabled = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            /*Invoke(new Action(() =>
            {
                worker.Work(this);
            }));*/
            
            worker.Work(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            updateComboBox2();
            updateComboBox3();
        }

        private void updateComboBox2()
        {
            comboBox2.Items.Clear();

            int index = 0;
            List<USBDiskInfo> usbDiskInfoList = USBDisk.getUSBDiskInfo();
            foreach (USBDiskInfo USBDiskInfo in usbDiskInfoList)
            {
                comboBox2.Items.Add(USBDiskInfo.DiskLabel + " ( " + USBDiskInfo.DiskLetter + " )");
                if (USBDiskInfo.DiskLabel.Contains("RECORDER"))
                {
                    comboBox2.SelectedIndex = index;
                }
                index++;
            }
        }

        private void updateComboBox3()
        {
            int index = 0;
            DriveInfo[] info = DriveInfo.GetDrives();
            foreach (var item in info)
            {
                if (!item.IsReady) continue;
                if (item.DriveType != DriveType.Removable)
                {
                    string DiskLabel = item.VolumeLabel;
                    if (DiskLabel.Length == 0)
                    {
                        DiskLabel = "Локальный диск";
                    }
                    comboBox3.Items.Add(DiskLabel + " ( " + item.Name.Replace("\\", "") + " )");

                    if (DiskLabel.Contains("Video"))
                    {
                        comboBox3.SelectedIndex = index;
                    }
                    index++;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        class Worker
        {
            public string ComboBox1Text { get; set; }
            public string ComboBox3Text { get; set; }
            public Form1 Form1 { get; set; }

            public void Work(Form1 form1)
            {
                Сopier Сopier = new Сopier();
                Сopier.SetForm(form1);
                Сopier.Copy(ComboBox3Text, ComboBox1Text);
            }
        }
    }
}
