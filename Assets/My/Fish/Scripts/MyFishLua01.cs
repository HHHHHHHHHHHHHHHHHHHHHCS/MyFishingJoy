using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using XLua;

public class MyFishLua01 : MonoBehaviour
{
    private const string fileDir = "My/Fish/Lua";
    private const string filePath = "MyFishLua01.lua.txt";
    private const string webPath = @"http://127.0.0.1/MyFishingJoy/";
    private const string savePath = "Hotfix.lua.txt";

    private LuaEnv luaEnv;

    //本地的
    /*
    private static Dictionary<string, GameObject> gameObjectDic;
    private static Queue<AssetBundle> AssetBundleQueue;
    */
    private Dictionary<string, GameObject> gameObjectDic;
    private Queue<AssetBundle> AssetBundleQueue;

    //本地的
    /* private void Awake()
    {
        gameObjectDic = new Dictionary<string, GameObject>();
        AssetBundleQueue = new Queue<AssetBundle>();
        luaEnv = new LuaEnv();
        luaEnv.DoString("require 'xlua.util';");
        luaEnv.AddLoader(MyLoader);
        luaEnv.DoString("require 'MyFishLua01';");
        luaEnv.DoString("HotFix(true);");
    }*/


    private void Awake()
    {
        gameObjectDic = new Dictionary<string, GameObject>();
        AssetBundleQueue = new Queue<AssetBundle>();
        luaEnv = new LuaEnv();
        luaEnv.DoString("require 'xlua.util';");
        LoadLua();

    }

    //网络
    private void Start()
    {

    }

    //本地
    /*
    private void LateUpdate()
    {
        while (AssetBundleQueue.Count > 0)
        {
            var ab = AssetBundleQueue.Dequeue();
            ab.Unload(false);
        }
    }
    */

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

        while (AssetBundleQueue.Count > 0)
        {
            var ab = AssetBundleQueue.Dequeue();
            ab.Unload(false);
        }
    }

    // 本地的
    /* [LuaCallCSharp]
    public static GameObject LoadResource(string resName, string filePath)
    {
        GameObject go = GetGameObject(resName);
        if (!go)
        {
            string path = string.Format("{0}/{1}"
                , Application.dataPath.Remove(Application.dataPath.Length - 7, 7), filePath);
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
    }*/

    public void LoadLua()
    {
        StartCoroutine(LoadLuaCorotine());
    }

    private IEnumerator LoadLuaCorotine()
    {
        if(File.Exists(savePath))
        {
            luaEnv.DoString(File.ReadAllText(savePath));
        }
        else
        {
            UnityWebRequest uwr = UnityWebRequest.Get(webPath + "MyFishLua01.lua.txt");
            yield return uwr.SendWebRequest();
            luaEnv.DoString(uwr.downloadHandler.text);
            File.WriteAllText(savePath, uwr.downloadHandler.text);
        }

        //luaEnv.AddLoader((ref string filepath) =>
        //{
        //    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(uwr.downloadHandler.text);
        //    return bytes;
        //});
  
        //luaEnv.DoString("require 'MyFishLua01';");
        luaEnv.DoString("HotFix(true)");

    }

    //网络的
    [LuaCallCSharp]
    public GameObject LoadResource(string resName, string filePath)
    {
        GameObject go = GetGameObject(resName);
        if (!go)
        {
            StartCoroutine(LoadAB(resName, filePath));
        }
        return go;
    }

    [LuaCallCSharp]
    public GameObject GetGameObject(string resName)
    {
        GameObject go = null;
        gameObjectDic.TryGetValue(resName, out go);
        return go;
    }


    private IEnumerator LoadAB(string resName, string filePath)
    {
        UnityWebRequest uwr = UnityWebRequest.GetAssetBundle(webPath + filePath);
        yield return uwr.SendWebRequest();
        AssetBundle ab = (uwr.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
        var go = ab.LoadAsset<GameObject>(resName);
        gameObjectDic.Add(resName, go);
        AssetBundleQueue.Enqueue(ab);
    }
}
