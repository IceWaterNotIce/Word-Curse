using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
public class BuildPostProcessor
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string path)
    {
        Debug.Log("Build completed for: " + target.ToString());
        Debug.Log("Build path: " + path);
        // 在這裡添加你想在構建後執行的代碼
    }
}