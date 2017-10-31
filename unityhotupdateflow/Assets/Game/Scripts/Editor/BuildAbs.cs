using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildAbs : MonoBehaviour {

	/// <summary>
    /// 资源打包
    /// </summary>
    [MenuItem("Game Editor/Build AssetBunldes")]
    static void CreateAssetBunldesMain()
    {
        Caching.ClearCache();
        
        BuildTarget target;
#if UNITY_STANDALONE_WIN
            target = BuildTarget.StandaloneWindows;
#elif UNITY_STANDALONE_OSX
            target = BuildTarget.StandaloneOSXIntel;
#elif UNITY_IPHONE
            target = BuildTarget.iOS;
#elif UNITY_ANDROID
        target = BuildTarget.Android;
#endif


        string AssetBundlesPath = Application.dataPath + "/StreamingAssets/AssetBundles";
        DebugTools.TestLog(AssetBundlesPath);
        if (!Directory.Exists(AssetBundlesPath))
        {
            Directory.CreateDirectory(AssetBundlesPath);
        }

        AssetBundleManifest abm = BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundles", BuildAssetBundleOptions.ChunkBasedCompression, target);
       
     
        //刷新编辑器
        AssetDatabase.Refresh ();


        GetAllFilesMD5(AssetBundlesPath);
    }

    /// <summary>
    /// 获取文件夹下所有文件的相对路径和MD5值
    /// </summary>
    static void GetAllFilesMD5(string resPath)
    {
        DebugTools.Log(resPath);
        // 获取Res文件夹下所有文件的相对路径和MD5值  
        string[] files = Directory.GetFiles(resPath, "*", SearchOption.AllDirectories);  
        StringBuilder versions = new StringBuilder();  
        for (int i = 0, len = files.Length; i < len; i++)  
        {  
            string filePath = files[i];
            string extension = files[i];//filePath.Substring(files[i].LastIndexOf("."));
            
            if (extension.IndexOf(".u3d") > 0)  
            {  
                DebugTools.Log(extension);
                string relativePath = filePath.Replace(resPath, "").Replace("\\", "/");  
                string md5 = MD5File(filePath);  
                versions.Append(relativePath).Append(",").Append(md5).Append("\n");  
            }  
        }  
// 生成配置文件  
        FileStream stream = new FileStream(resPath + "version.txt", FileMode.Create);  
        byte[] data = Encoding.UTF8.GetBytes(versions.ToString());  
        stream.Write(data, 0, data.Length);  
        stream.Flush();  
        stream.Close();  
    }
    
    /// <summary>
    /// 生成文件的MD5值
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string MD5File(string file)  
    {  
        try  
        {  
            FileStream fs = new FileStream(file, FileMode.Open);  
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();  
            byte[] retVal = md5.ComputeHash(fs);  
            fs.Close();  
            StringBuilder sb = new StringBuilder();  
            for (int i = 0; i < retVal.Length; i++)  
            {  
                sb.Append(retVal[i].ToString("x2"));  
            }  
            return sb.ToString();  
        }  
        catch (Exception ex)  
        {  
            throw new Exception("md5file() fail, error:" + ex.Message);  
        }  
    }  
}
