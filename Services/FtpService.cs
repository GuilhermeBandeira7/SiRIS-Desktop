using Microsoft.AspNetCore.Authentication;
using SiRISApp.View.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiRISApp.Services
{
    public class FtpService
    {
        #region SINGLETON

        private FtpService()
        {

        }

        private static FtpService? instance;
        public static FtpService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new();
                    return instance;
                }

                return instance;
            }
        }

        #endregion

        string username = "";
        string password = "";
        string userRoot = "";
        string root = "";
        string user = AppSessionService.Instance.User.Registration;

        public FileManagement fileManagement;

        public void Init()
        {
            List<string> lines = File.ReadAllLines("ftp.txt").ToList();
            string ip = lines.First();
            username = lines[1];
            password = lines[2];
            root = $"ftp://{ip}:21/files/";
            if (!ListDirectory(root).Contains(AppSessionService.Instance.User.Registration))
                CreateDirectory($"{root}/{AppSessionService.Instance.User.Registration}");

            userRoot = $"{root}";
        }

        public void ShowFileManagement()
        { 

            fileManagement = new FileManagement();
            fileManagement.Show();
            /*Thread t = new Thread(() =>
            {
                fileManagement = new FileManagement();
                fileManagement.Show();
            });
            t.SetApartmentState(ApartmentState.STA);*/

            //t.Start();

        }

        #region FOLDERS

        public List<string> GetFolders(string folderPath)
        {
            return ListDirectory($"{userRoot}{folderPath}");
        }

        public void CreateFolder(string folderPath)
        {
            CreateDirectory($"{userRoot}/{folderPath}");
        }


        public void DeleteFolder(string folderPath)
        {
            DeleteDirectory($"{userRoot}/{folderPath}");
        }

        #endregion

        #region FILES

        public List<string> GetFiles(string filePath)
        {
            return ListData($"{userRoot}/{filePath}");
        }

        public void UploadFile(string folderPath, string filename)
        {
            UploadData($"{userRoot}{folderPath}/{filename.Split("\\").Last()}", filename);
        }

        public void DownloadFile(string folderPath, string filename)
        {
            DownloadData($"{userRoot}{folderPath}", $"{filename}\\{folderPath.Split("/").Last()}");
            fileManagement.Close();
        }

        public void DeleteFile(string folderPath)
        {
            DeleteData($"{userRoot}{folderPath}");
        }

        #endregion

        #region DIRECTORY

        private List<string> ListDirectory(string url)
        {
            try
            {
                List<string> folders = new List<string>();
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();


                while (responseStream.CanRead)
                {
                    byte[] buffer = new byte[1024];
                    int readedBytes = responseStream.Read(buffer, 0, buffer.Length);
                    byte[] responseMessage = buffer.Take(readedBytes).ToArray();
                    folders.AddRange(Encoding.UTF8.GetString(responseMessage).Split("\r\n").ToList());
                }

                if (url.Contains(user))
                    return folders.Where(x => x != string.Empty).Where(x => !x.Contains('.')).ToList();
                else
                    return folders.Where(x => x != string.Empty).Where(x => !x.Contains('.')).Where(x => x.Contains(user)).ToList();
            }
            catch
            {
                return new();
            }
        }

        private bool CreateDirectory(string url)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("Delete status: {0}", response.StatusDescription);


                return true;
            }
            catch
            {
                return false;
            }
        }


        private bool DeleteDirectory(string url)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("Delete status: {0}", response.StatusDescription);

                return true;
            }
            catch
            {
                return false;
            }
        }


        #endregion

        #region DATA

        private List<string> ListData(string url)
        {
            try
            {
                List<string> folders = new List<string>();
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();


                while (responseStream.CanRead)
                {
                    byte[] buffer = new byte[1024];
                    int readedBytes = responseStream.Read(buffer, 0, buffer.Length);
                    byte[] responseMessage = buffer.Take(readedBytes).ToArray();
                    folders.AddRange(Encoding.UTF8.GetString(responseMessage).Split("\r\n").ToList());
                }

                return folders.Where(x => x != string.Empty).Where(x => x.Contains('.')).ToList();
            }
            catch
            {
                return new();
            }
        }

        private bool UploadData(string url, string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential("mtw", "Senha@mtw");
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.ContentLength = fileInfo.Length;

                Stream requestStream = request.GetRequestStream();
                FileStream stream = fileInfo.OpenRead();
                const int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];
                int count = 0;
                int readBytes = 0;

                do
                {
                    readBytes = stream.Read(buffer, 0, bufferLength);
                    requestStream.Write(buffer, 0, readBytes);
                    count += readBytes;
                }
                while (readBytes != 0);

                requestStream.Close();
                stream.Close();
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("Delete status: {0}", response.StatusDescription);


                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool DownloadData(string url, string filename)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential("mtw", "Senha@mtw");
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("Delete status: {0}", response.StatusDescription);

                Stream responseStream = response.GetResponseStream();
                if (File.Exists(filename))
                    File.Delete(filename);


                FileStream fs = File.OpenWrite(filename);

                while (responseStream.CanRead)
                {
                    byte[] buffer = new byte[1024];
                    int readedBytes = responseStream.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, readedBytes);
                }

                fs.Close();
                response.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool DeleteData(string url)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("Delete status: {0}", response.StatusDescription);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
