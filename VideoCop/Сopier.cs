using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace VideoCop
{
    class Сopier
    {
        private Form1 Form1;

        //private BackgroundWorker BackgroundWorkerForm1 { get; set; }


        public void SearchFiles(string sDir)
        {
            Form1.listBox1.Items.Clear();

            List<string> fileList = GetVideoFiles(sDir);
            foreach (string file in fileList)
            {
                if (!file.Contains("System Volume Information"))
                {
                    if (Form1.checkBox1.Checked == false)
                    {
                        FileInfo fi = new FileInfo(file);
                        if (fi.LastWriteTime.Date == Form1.dateTimePicker1.Value.Date)
                        {
                            Form1.listBox1.Items.Add(file);
                            continue;
                        }
                    }
                    else 
                    {
                        Form1.listBox1.Items.Add(file);
                    }
                }
            }
        }

        public void Copy(string ComboBoxText, string user)
        {

            string outputDir = ComboBoxText.Substring(ComboBoxText.IndexOf(": ") - 1, 2) + "\\";

            if (Form1.checkBox1.Checked == false)
            {
                outputDir += Form1.dateTimePicker1.Value.Date.ToString("dd.MM.yyyy") + "\\" + user + "\\";
            }
            else
            {
                outputDir += DateTime.Now.ToString("dd.MM.yyyy") + "\\" + user + "\\";
            }

            Directory.CreateDirectory(@outputDir);
            int index = 1;
            foreach (string file in Form1.listBox1.Items)
            {
                if (Form1.backgroundWorker1.CancellationPending)
                {
                    return;
                }

                Form1.TextProgressBar1.Invoke((MethodInvoker)delegate {
                    Form1.TextProgressBar1.CustomText = System.IO.Path.GetFileName(file);
                    Form1.TextProgressBar1.VisualMode = ProgressBarDisplayMode.TextAndPercentage;
                });

                Form1.TextProgressBar2.Invoke((MethodInvoker)delegate {
                    Form1.TextProgressBar2.VisualMode = ProgressBarDisplayMode.CustomText;
                    Form1.TextProgressBar2.Maximum = Form1.listBox1.Items.Count;
                    Form1.TextProgressBar2.CustomText = "Файл: " + index + " из " + Form1.listBox1.Items.Count;
                    Form1.TextProgressBar2.Value = index;
                });

                index++;

                int countError = 0;
                while (countError < 3)
                {
                    try
                    {
                        BgwCopyFile_DoWork(file, outputDir + System.IO.Path.GetFileName(file));
                        break;
                    }
                    catch (Exception)
                    {
                        countError++;
                    }
                }
            }
        }

        private void BgwCopyFile_DoWork(string inputFileName, string outputFileName)
        {
            var fsize = new FileInfo(inputFileName).Length;

            if (fsize < 100)
            {
                Microsoft.VisualBasic.FileIO.FileSystem.MoveFile(inputFileName, outputFileName, true);
                return;
            }

            var bytesForPercent = fsize / 100;
            var buffer = new byte[bytesForPercent];


            using (var inputFs = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            using (var outputFs = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
            {
                int counter = 0;
                while (inputFs.Position < inputFs.Length)
                {
                    if (Form1.backgroundWorker1.CancellationPending)
                    {
                        return;
                    }

                    var wasRead = inputFs.Read(buffer, 0, (int)bytesForPercent);
                    outputFs.Write(buffer, 0, wasRead);
                    counter++;

                    if (counter <= Form1.TextProgressBar1.Maximum) 
                        Form1.backgroundWorker1.ReportProgress(counter);
                }
            }

            File.Delete(@inputFileName);

            //System.IO.File.Move(inputFileName, outputFileName);

            /*try
            {
                Microsoft.VisualBasic.FileIO.FileSystem.MoveFile(inputFileName, outputFileName, true);

            }*/
            /*try
            {
                Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory("F:\\DCIM", "V:\\10.01.2023\\Штуро Д.В", Microsoft.VisualBasic.FileIO.UIOption.AllDialogs);
                
            }*/
            /*catch (System.OperationCanceledException ex)
            {
                MessageBox.Show("Копирование отменено пользователем!\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка копирования!\n" + ex.Message);
            }*/


        }

        private List<string> GetVideoFiles(string sDir)
        {
            List<string> fileList = new List<string>();
            try
            {
                foreach (string file in Directory.EnumerateFiles(sDir, "*", SearchOption.AllDirectories))
                {
                    fileList.Add(file);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Ошибка поиска файлов \n" + ex.Message + "\n\nПроверьте правильность выбраного носителя!");
            }
            return fileList;
        }

        public void SetForm(Form1 form1)
        {
            this.Form1 = form1;
        }
    }
}
