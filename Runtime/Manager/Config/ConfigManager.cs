using System;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using Framework.Assets;
using Framework.Asynchronous;
using UnityEngine;

public class ConfigManager : ManagerBase<ConfigManager, ConfigAttribute, string>
{
    private object[] _objects = new object[1];
    private MulAsyncResult asyncResult;
    private Action _loadedCb;
    public bool Loaded { get; private set; }
    
    public override void Start()
    {
        asyncResult = new MulAsyncResult();
        foreach (ClassData classData in GetAllClassDatas())
        {
            var path = (classData.Attribute as ConfigAttribute).Path;
            var method = classData.Type.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            var progress = Res.Default.LoadAssetAsync<TextAsset>(path);
            progress.Callbackable().OnCallback(result =>
            {
                lock (_objects)
                {
                    _objects[0] = result.Result.text;
                    try
                    {
                        method.Invoke(null, _objects);
                    }
                    catch (Exception)
                    {
                        Log.Error("加载",method.DeclaringType,"出错");
                        throw;
                    }
                }
            });
            asyncResult.AddAsyncResult(progress);
        }
        asyncResult.Callbackable().OnCallback(result =>
        {
            GameLoop.Ins.Delay(() =>
            {
                Loaded = true;
                _loadedCb?.Invoke();
            });
        });
    }

    public void AddLoadedCallback(Action action)
    {
        _loadedCb += action;
    }

#if UNITY_EDITOR
    public void EditorLoad()
    {
        List<Type> allTypes = new List<Type>();
        var ilrConfig = ConfigBase.Load<FrameworkRuntimeConfig>().ILRConfig;
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            var ilrAsm = AssemblyManager.GetAssembly(ilrConfig.DllName);
            allTypes.AddRange(ilrAsm.DefinedTypes);
        }
        foreach (var type in allTypes)
        {
            CheckType(type);
        }
    }
#endif
}