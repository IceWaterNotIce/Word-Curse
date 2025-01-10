using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Build.Profile;
using PlasticGui.WorkspaceWindow;


public class AssetBundleBuilder
{
    [MenuItem("Assets/Build AssetBundles")]
    public static void BuildAllAssetBundles()
    {
        // create directory if not exist
        if (!Directory.Exists("Assets/AssetBundles/" + BuildProfile.GetActiveBuildProfile().name))
        {
            Directory.CreateDirectory("Assets/AssetBundles/" + BuildProfile.GetActiveBuildProfile().name);
        }

        // filter current platform asset bundles
        string[] assetBundles = AssetDatabase.GetAllAssetBundleNames();
        List<string> filted_assetBundles = new List<string>();
        foreach (string assetBundle in assetBundles)
        {
            if (assetBundle.Contains(".all") || assetBundle.Contains(BuildProfile.GetActiveBuildProfile().name))
            {
                filted_assetBundles.Add(assetBundle);
            }
        }
        AssetBundleBuild[] buildMap = new AssetBundleBuild[filted_assetBundles.Count];

        for (int i = 0; i < filted_assetBundles.Count; i++)
        {
            buildMap[i].assetBundleName = filted_assetBundles[i];
            buildMap[i].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(filted_assetBundles[i]);
        }

        // build asset bundles
        BuildPipeline.BuildAssetBundles(
            "Assets/AssetBundles/" + BuildProfile.GetActiveBuildProfile().name,
            buildMap,
            BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget
        );

        // update version.json and push to git
        UpdateVersionJson();
        CommitAndPushToGit();
        UploadToFTP();
        //
    }


    private static void UpdateVersionJson()
    {
        // local version.json
        string versionFilePath = "Assets/AssetBundles/" + BuildProfile.GetActiveBuildProfile().name + "/version.json";
        if (!File.Exists(versionFilePath))
        {
            File.Create(versionFilePath).Close();
            File.WriteAllText(versionFilePath, JsonUtility.ToJson(new VersionConfig()));
        }
        string json = File.ReadAllText(versionFilePath);
        VersionConfig versionData = JsonUtility.FromJson<VersionConfig>(json) ?? new VersionConfig();

        // filter current platform asset bundles
        string[] assetBundles = AssetDatabase.GetAllAssetBundleNames();
        List<string> filted_assetBundles = new List<string>();
        foreach (string assetBundle in assetBundles)
        {
            if (assetBundle.Contains(".all") || assetBundle.Contains(BuildProfile.GetActiveBuildProfile().name))
            {
                filted_assetBundles.Add(assetBundle);
            }
        }

        // Add new bundle to versionData.bundles
        if (versionData.bundles == null)
        {
            versionData.bundles = new List<VersionConfig.AssetBundleInfo>();
        }
        foreach (var bundle in filted_assetBundles)
        {
            if (versionData.bundles.Find(b => b.name == bundle) == null)
            {
                versionData.bundles.Add(new VersionConfig.AssetBundleInfo
                {
                    name = bundle,
                    version = "1.0",
                    url = "https://raw.githubusercontent.com/IceWaterNotIce/WordCurse/main/Assets/AssetBundles/" + BuildProfile.GetActiveBuildProfile().name + "/" + bundle
                });
            }
        }

        // Remove deleted bundle from versionData.bundles
        foreach (var bundle in versionData.bundles)
        {
            if (filted_assetBundles.Find(b => b == bundle.name) == null)
            {
                versionData.bundles.Remove(bundle);
            }
        }

        // Update version number
        foreach (var bundle in versionData.bundles)
        {
            //version format is 1.0
            bundle.version = (float.Parse(bundle.version) + 0.1f).ToString();
        }



        File.WriteAllText(versionFilePath, JsonUtility.ToJson(versionData));
        UnityEngine.Debug.Log("Version.json updated");

    }


    private static void CommitAndPushToGit()
    {
        RunCommand("git add .");
        RunCommand("git commit -m \"Auto commit from Unity Asset Bundle Builder\"");
        RunCommand("git push origin main");

        UnityEngine.Debug.Log("Asset Bundles pushed to git");
    }

    public class FTPaccount
    {
        public string host;
        public string username;
        public string password;
    }
    private static void UploadToFTP()
    {
        // get account info from editor/account.json
        string accountFilePath = "Assets/Editor/FTPaccount.json";
        if (!File.Exists(accountFilePath))
        {
            UnityEngine.Debug.LogError("account.json not found");
            return;
        }
        string json = File.ReadAllText(accountFilePath);
        FTPaccount account = JsonUtility.FromJson<FTPaccount>(json) ?? new FTPaccount();

        UnityEngine.Debug.Log("cd Assets/AssetBundles/" + BuildProfile.GetActiveBuildProfile().name);
        UnityEngine.Debug.Log("ftp -i " + account.host);
        UnityEngine.Debug.Log(account.username);
        UnityEngine.Debug.Log(account.password);
        UnityEngine.Debug.Log("cd domains/" + account.host + "/public_html/games/WordCurse/AssetBundles/" + BuildProfile.GetActiveBuildProfile().name);
        UnityEngine.Debug.Log("mput *");
        
        RunCommand("cd Assets/AssetBundles/" + BuildProfile.GetActiveBuildProfile().name);
        RunCommand("ftp -i " + account.host);
        RunCommand(account.username);
        RunCommand(account.password);
        RunCommand("cd domains/" + account.host + "/public_html/games/WordCurse/AssetBundles/" + BuildProfile.GetActiveBuildProfile().name);
        RunCommand("mput *");
        RunCommand("bye");

      

    }


    private static void RunCommand(string command)
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




}
