using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Diagnostics;
using System.IO;
using System;
using UnityEditor.Build.Profile;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Callbacks;


[InitializeOnLoad]
public class VersionIncrementor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    public void OnPreprocessBuild(BuildReport report)
    {
        SetPlatformVar();
        UpdateVersion();
        CommitAndPushToGit(BuildProfile.GetActiveBuildProfile().name, PlayerSettings.bundleVersion);
    }

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string path)
    {
        UnityEngine.Debug.Log("Build completed");
        UploadToFTP();
    }

    private void SetPlatformVar()
    {
        // load txt from resources
        string platform = BuildProfile.GetActiveBuildProfile().name;
        string platformFilePath = Path.Combine(Application.streamingAssetsPath, "platform.txt");
        File.WriteAllText(platformFilePath, platform);
    }

    private static void UpdateVersion()
    {
        var activeProfile = BuildProfile.GetActiveBuildProfile();
        var currentBuildTarget = activeProfile.name;
        UnityEngine.Debug.Log("Current Build Target: " + currentBuildTarget);
        string versionFilePath = Path.Combine(Application.streamingAssetsPath, "version.json");
        if (File.Exists(versionFilePath))
        {
            string json = File.ReadAllText(versionFilePath);
            VersionConfig versionData = JsonUtility.FromJson<VersionConfig>(json);
            foreach (var platform in versionData.platforms)
            {
                if (platform.name == currentBuildTarget)
                {
                    string[] versionInfoParts = PlayerSettings.bundleVersion.Split('.');
                    if (versionInfoParts.Length == 3)
                    {
                        if (int.TryParse(versionInfoParts[2], out int patchVersion))
                        {
                            patchVersion++;
                            platform.latestVersion = $"{versionInfoParts[0]}.{versionInfoParts[1]}.{patchVersion}";
                            UnityEngine.Debug.Log($"Version updated to {platform.latestVersion}");
                        }
                        else
                        {
                            UnityEngine.Debug.LogError("Patch version is not a number.");
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("Version format is not correct. It should be like 1.0.0");
                    }

                    File.WriteAllText(versionFilePath, JsonUtility.ToJson(versionData));
                    break;
                }
            }


        }

        string[] versionParts = PlayerSettings.bundleVersion.Split('.');
        if (versionParts.Length == 3)
        {
            if (int.TryParse(versionParts[2], out int patchVersion))
            {
                patchVersion++;
                PlayerSettings.bundleVersion = $"{versionParts[0]}.{versionParts[1]}.{patchVersion}";
                UnityEngine.Debug.Log($"Version updated to {PlayerSettings.bundleVersion}");
            }
            else
            {
                UnityEngine.Debug.LogError("Patch version is not a number.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Version format is not correct. It should be like 1.0.0");
        }
    }

    private static void CommitAndPushToGit(string platform, string versionParts)
    {
        RunGitCommand("git add .");
        RunGitCommand("git commit -m \"Auto commit from Unity Builder. \"");
        RunGitCommand("git tag -a " + platform + "v" + versionParts + " -m \"Auto tag from Unity Builder. \"");
        RunGitCommand("git push origin main");
        RunGitCommand("git push origin v" + versionParts);

        UnityEngine.Debug.Log("Git commit and push done");
    }

    private static void RunGitCommand(string command)
    {
        ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
        processStartInfo.WorkingDirectory = Application.dataPath;
        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.UseShellExecute = false;
        processStartInfo.CreateNoWindow = false;

        using (Process process = Process.Start(processStartInfo))
        {
            process.WaitForExit();
            UnityEngine.Debug.Log(process.StandardOutput.ReadToEnd());
        }
    }

    public class FTPaccount
    {
        public string host;
        public string username;
        public string password;
    }
    private static void UploadToFTP()
    {
        // 從 editor/account.json 獲取帳戶信息
        string accountFilePath = "Assets/Editor/FTPaccount.json";
        if (!File.Exists(accountFilePath))
        {
            UnityEngine.Debug.LogError("account.json 未找到");
            return;
        }

        string json = File.ReadAllText(accountFilePath);
        FTPaccount account = JsonUtility.FromJson<FTPaccount>(json) ?? new FTPaccount();
        string localFolderPath = "Builds/" + BuildProfile.GetActiveBuildProfile().name + "/";
        string ftpUrl = account.host + BuildProfile.GetActiveBuildProfile().name + "/";

        if (!Directory.Exists(localFolderPath))
        {
            UnityEngine.Debug.Log("本地文件夾不存在。");
            return;
        }

        // 開始遞歸上傳文件和文件夾
        UploadFilesRecursively(localFolderPath, ftpUrl, account);

        UnityEngine.Debug.Log("資源包已上傳到 FTP");
    }

    private static void UploadFilesRecursively(string localFolderPath, string ftpUrl, FTPaccount account)
    {
        // 上傳當前文件夾中的文件
        string[] files = Directory.GetFiles(localFolderPath);
        foreach (string localFilePath in files)
        {
            string fileName = Path.GetFileName(localFilePath);
            string fullFtpUrl = $"{ftpUrl}{fileName}";

            // 上傳文件
            UploadFile(fullFtpUrl, account.username, account.password, localFilePath);
        }

        // 遞歸上傳子文件夾中的文件
        string[] directories = Directory.GetDirectories(localFolderPath);
        foreach (string directory in directories)
        {
            // 獲取子文件夾名稱並創建對應的 FTP 文件夾 URL
            string directoryName = Path.GetFileName(directory);
            string newFtpUrl = $"{ftpUrl}{directoryName}/";

            // 遞歸上傳子文件夾中的文件
            UploadFilesRecursively(directory, newFtpUrl, account);
        }
    }
    static void UploadFile(string ftpUrl, string username, string password, string localFilePath)
    {
        FileInfo fileInfo = new FileInfo(localFilePath);
        if (!fileInfo.Exists)
        {
            UnityEngine.Debug.Log($"File does not exist: {localFilePath}");
            return;
        }
        string ftpDirectory = Path.GetDirectoryName(ftpUrl) + "\\";
        UnityEngine.Debug.Log($"ftpDirectory: {ftpDirectory}");
        CreateFtpDirectory(ftpDirectory, username, password);

        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.Credentials = new NetworkCredential(username, password);
        request.ContentLength = fileInfo.Length;

        using (FileStream fs = fileInfo.OpenRead())
        using (Stream requestStream = request.GetRequestStream())
        {
            fs.CopyTo(requestStream);
        }

        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
        {
            UnityEngine.Debug.Log($"Uploaded {fileInfo.Name}, status {response.StatusDescription}");
        }
    }


    private static void CreateFtpDirectory(string ftpDirectory, string username, string password)
    {

        // 確保 FTP URL 是有效的
        if (!Uri.IsWellFormedUriString(ftpDirectory, UriKind.Absolute))
        {
            UnityEngine.Debug.LogError("Invalid FTP URL: " + ftpDirectory);
            return;
        }
        // 使用 FtpWebRequest 創建 FTP 目錄
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpDirectory);
        request.Method = WebRequestMethods.Ftp.MakeDirectory;
        request.Credentials = new NetworkCredential(username, password);

        try
        {
            using (var response = (FtpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    UnityEngine.Debug.Log("Directory created: " + ftpDirectory);
                }
            }
        }
        catch (WebException ex)
        {
            // 550 錯誤代表目錄已存在，忽略此錯誤
            if (ex.Response is FtpWebResponse response && response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                UnityEngine.Debug.LogError("Failed to create directory: " + ex.Message);
            }
        }
    }
    [System.Serializable]
    public class VersionConfig
    {
        [System.Serializable]
        public class VersionInfo
        {
            public string name;
            public string latestVersion;
            public string downloadURL;
        }
        public List<VersionInfo> platforms;
    }
}