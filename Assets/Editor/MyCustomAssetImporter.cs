using UnityEditor;
using UnityEngine;
using UnityEditor.AssetImporters;

[ScriptedImporter(1, "mycustomasset")]
public class MyCustomAssetImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        // 获取选定的构建目标
        BuildTarget selectedTarget = ctx.selectedBuildTarget;

        // 日志记录选定的构建目标
        Debug.Log("选定的构建目标: " + selectedTarget);

        // 在这里可以根据选定的目标添加自定义导入逻辑
        // 例如，你可能希望对Android和iOS的资产进行不同的处理
    }
}