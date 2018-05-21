using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class MyFishLua01 : MonoBehaviour
{
    private const string fileDir = "My/Fish/Lua";
    private const string filePath = "MyFishLua01.lua.txt";

    private LuaEnv luaEnv;

    private static Dictionary<string, GameObject> gameObjectDic;
    private static Queue<AssetBundle> AssetBundleQueue;

    private void Awake()
    {
        gameObjectDic = new Dictionary<string, GameObject>();
        AssetBundleQueue = new Queue<AssetBundle>();
        luaEnv = new LuaEnv();
        luaEnv.DoString("require 'xlua.util';");
        luaEnv.AddLoader(MyLoader);
        luaEnv.DoString("require 'MyFishLua01';");
        luaEnv.DoString("HotFix(true);");

    }

    private void Update()
    {
        while(AssetBundleQueue.Count>0)
        {
            var ab = AssetBundleQueue.Dequeue();
            ab.Unload(false);
        }
    }

    private byte[] MyLoader(ref string filepath)
    {
        string _path = string.Format("{0}/{1}/{2}"
            , Application.dataPath, fileDir, filePath);
        string text = File.ReadAllText(_path);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
        return bytes;
    }

    private void OnDestroy()
    {
        luaEnv.DoString("HotFix(false)");
        luaEnv.Dispose();
    }

    [LuaCallCSharp]
    public static GameObject LoadResource(string resName, string filePath)
    {
        GameObject go = GetGameObject(resName);
        if (!go)
        {
            string path = string.Format("{0}/{1}"
                , Application.dataPath.Remove(Application.dataPath.Length-7,7), filePath);
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            go = ab.LoadAsset<GameObject>(resName);
            gameObjectDic.Add(resName, go);
            AssetBundleQueue.Enqueue(ab);
        }
        return go;
    }

    [LuaCallCSharp]
    public static GameObject GetGameObject(string resName)
    {
        GameObject go = null;
        gameObjectDic.TryGetValue(resName, out go);
        return go;
    }

}
