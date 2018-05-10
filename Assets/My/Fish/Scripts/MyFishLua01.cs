using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MyFishLua01 : MonoBehaviour
{
    private const string fileDir = "My/Fish/Lua";
    private const string filePath = "MyFishLua01.lua.txt";

    private XLua.LuaEnv luaEnv;


    private void Awake()
    {
        luaEnv = new XLua.LuaEnv();
        luaEnv.AddLoader(MyLoader);
        luaEnv.DoString("require 'MyFishLua01'");
        luaEnv.DoString("HotFix();");
    }

    private byte[] MyLoader(ref string filepath)
    {
        string _path = string.Format("{0}/{1}/{2}"
            , Application.dataPath, fileDir, filePath);
        string text  = File.ReadAllText(_path);
        byte[] bytes= System.Text.Encoding.UTF8.GetBytes(text);
        return bytes;
    }

    private void OnDestroy()
    {
        luaEnv.DoString("Dispose()");
        luaEnv.Dispose();
    }
}
