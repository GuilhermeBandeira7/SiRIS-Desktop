using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SiRISApp.Services
{
    public class AppExecutionService
    {

        #region SINGLETON

        private static AppExecutionService? instance;
        public static AppExecutionService Instance
        {
            get
            {
                if (instance == null)
                    instance = new AppExecutionService();

                return instance;
            }
        }

        private AppExecutionService()
        {

        }


        #endregion


        MsoTriState ofalse = MsoTriState.msoFalse;
        MsoTriState otrue = MsoTriState.msoTrue;

        Process videoProcess = new();
        Process imageProcess = new();
        Process pptProcess = new();
        Process pdfProcess = new();
        Process wordProcess = new();
        Process excelProcess = new();




        const int SW_RESTORE = 9;

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);

        public bool BringProcessToFront(Process process)
        {
            try
            {
                IntPtr handle = process.MainWindowHandle;
                if (IsIconic(handle))
                {
                    ShowWindow(handle, SW_RESTORE);
                }

                SetForegroundWindow(handle);
                return true;
            }
            catch
            {
                return false;
            }
        }



        public void ExecuteFile(string path)
        {
            string extension = path.Split("\\").Last().Split(".").Last();
            if (extension == "mkv" || extension == "mp4" || extension == "avi")
                ExecuteVideo(path);
            else if (extension == "pptx" || extension == "ppt")
                ExecutePowerPoint(path);
            else if (extension == "docx" || extension == "doc")
                ExecuteWord(path);
            else if (extension == "xlsx" || extension == "xls")
                ExecuteExcel(path);
            else if (extension == "pdf")
                ExecutePdf(path);
            else if (extension == "jpeg" || extension == "jpg" || extension == "png" || extension == "bnp")
                ExecuteImage(path);
        }

        public void ExecuteListOfFiles(List<string> files)
        {
            if (CheckExtension(files))
            {
                List<string> extensions = GetExtensions(files);
                string extension = extensions.First();
                if (extension == "mkv" || extension == "mp4" || extension == "avi" || extension == "mp3")
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        foreach (string file in files)
                        {
                            videoProcess = ExecuteProcess(file);
                            videoProcess.WaitForExit();
                        }

                    });
                }
            }
        }

        public void OpenProcess(string processName)
        {
          
            if (processName == "Video")
            {
                if (!BringProcessToFront(pptProcess))
                    ExecuteFile("template\\template.mp4");

            }
            else if (processName == "PowerPoint")
            {
                if (!BringProcessToFront(pptProcess))
                    ExecuteFile("template\\template.pptx");
            }

            else if (processName == "Word")
            {
                if (!BringProcessToFront(wordProcess))
                    ExecuteFile("template\\template.docx");
            }

            else if (processName == "Excel")
            {
                if (!BringProcessToFront(excelProcess))
                    ExecuteFile("template\\template.xlsx");
            }

            else if (processName == "PDF")
            {
                if (!BringProcessToFront(pdfProcess))
                    ExecuteFile("template\\template.pdf");
            }

            else if (processName == "Fotos")
            {
                if (!BringProcessToFront(imageProcess))
                    ExecuteFile("template\\template.jpeg");
            }

            else if (processName == "SiRIS")
            {
                BringProcessToFront(Process.GetCurrentProcess());
            }

        }

        private bool CheckExtension(List<string> files)
        {
            List<string> extensions = GetExtensions(files);
            string extension = extensions.First();
            return !extensions.Any(e => e != extension);
        }

        private List<string> GetExtensions(List<string> files)
        {
            List<string> filesName = files.Select(f => f.Split("\\").Last()).ToList();
            List<string> extensions = filesName.Select(f => f.Split(".").Last()).ToList();
            return extensions;
        }

        private Process ExecuteProcess(string path)
        {
            Process process = new();
            process.StartInfo.FileName = path;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            return process;
        }

        private void ExecutePowerPoint(string path)
        {
            pptProcess = ExecuteProcess(path);


            /* Microsoft.Office.Interop.PowerPoint.Application pptApp = new Microsoft.Office.Interop.PowerPoint.Application();
             pptApp.Visible = otrue;
             pptApp.Activate();
             Microsoft.Office.Interop.PowerPoint.Presentations ps = pptApp.Presentations;
             Microsoft.Office.Interop.PowerPoint.Presentation p = ps.Open(path, ofalse, ofalse, otrue);
             p.SlideShowSettings.Run();*/
        }


        private void ExecuteWord(string path)
        {
            wordProcess = ExecuteProcess(path);
            /*
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true;
            wordApp.Activate();
            Document document = wordApp.Documents.Open(path);*/
        }


        private void ExecuteExcel(string path)
        {
            excelProcess = ExecuteProcess(path);
            /* Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
             excelApp.Visible = true;
             Workbook wb = excelApp.Workbooks.Open(path);*/
        }



        private void ExecutePdf(string path)
        {
            pdfProcess = ExecuteProcess(path);
        }



        private void ExecuteImage(string path)
        {
            imageProcess = ExecuteProcess(path);
        }

        private void ExecuteVideo(string path)
        {
            videoProcess = ExecuteProcess(path);
        }

    }
}
