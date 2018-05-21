using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class CreateAssetBundles 
{
    [MenuItem("AssetBundles/Build")]
    public static  void BuildAssetBundles()
    {
        string dir = "AssetBundles";
        if(!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None
            , BuildTarget.StandaloneWindows64);
    }
}
