using System.IO;
using System.Net;
using System;
using UnityEditor.Build.Profile;
using UnityEngine;
using System.Linq;

namespace FTP_Manager
{

    public class FTP_Account
    {
        public string host;
        public string username;
        public string password;
    }

    public class FTP_Controller
    {
        public static void UploadDirectory(string localFolderPath, string ftpUrl, string username, string password)
        {
            Debug.Log($"Uploading Folder: {Path.GetFileName(localFolderPath)}\n {localFolderPath}\n To FTP server:\n {ftpUrl}");

            // Upload all files in the directory
            foreach (string filePath in Directory.GetFiles(localFolderPath))
            {
                UploadFile(filePath, ftpUrl, username, password);
            }

            // Recurse into subdirectories
            foreach (string directoryPath in Directory.GetDirectories(localFolderPath))
            {
                string newFtpUrl = $"{ftpUrl}{Path.GetFileName(directoryPath)}/";

                // Create directory on FTP server
                CreateFtpDirectory(newFtpUrl, username, password);

                // Recursively upload the subdirectory
                UploadDirectory(directoryPath, newFtpUrl, username, password);
            }
        }

        public static void UploadFile(string filePath, string ftpUrl, string username, string password)
        {
            string fileName = Path.GetFileName(filePath);
            string uploadUrl = $"{ftpUrl}{fileName}";

            Debug.Log($"Uploading file: {fileName}\n {filePath}\n To FTP server:\n {uploadUrl}");

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(username, password);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (Stream requestStream = request.GetRequestStream())
            {
                fileStream.CopyTo(requestStream);
            }

            Debug.Log($"Uploaded: {fileName}");
        }

        public static void CreateFtpDirectory(string ftpUrl, string username, string password)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(username, password);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Debug.Log($"Directory created: {ftpUrl}");
                }
            }
            catch (WebException ex)
            {
                // Handle the case where the directory already exists
                if (ex.Response is FtpWebResponse ftpResponse && ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    Debug.Log($"Directory already exists: {ftpUrl.Split('/').Last()}\n {ftpUrl}");
                }
                else
                {
                    throw;
                }
            }
        }
    }
}