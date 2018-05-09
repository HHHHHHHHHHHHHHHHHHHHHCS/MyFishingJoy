using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MyLuaTest01 : MonoBehaviour
{
    private const string fileDir = "My/Sanbox/Test01/Lua";
    private const string filePath = "MyLuaTest01.lua.txt";

    private XLua.LuaEnv luaEnv;


    private void Awake()
    {
        luaEnv = new XLua.LuaEnv();
        luaEnv.AddLoader(MyLoader);
        luaEnv.DoString("require 'MyLuaTest01'");
    }

    private byte[] MyLoader(ref string filepath)
    {
        string _path = string.Format("{0}/{1}/{2}"
            , Application.dataPath, fileDir, filePath);
        string text  = File.ReadAllText(_path);
        byte[] bytes= System.Text.Encoding.UTF8.GetBytes(text);
        return bytes;
    }
}
