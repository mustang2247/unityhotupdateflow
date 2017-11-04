using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildAbs : MonoBehaviour
{
    /// <summary>
    /// 资源打包
    /// </summary>
    [MenuItem("Game Editor/Build AssetBunldes")]
    static void CreateAssetBunldesMain()
    {
        Caching.CleanCache();

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

        AssetBundleManifest abm = BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundles",
            BuildAssetBundleOptions.ChunkBasedCompression, target);


        //刷新编辑器
        AssetDatabase.Refresh();


        GetAllFilesMD5(AssetBundlesPath);
        
        //压缩文件
        
        
        //解压文件
    }
    
//    [MenuItem ("Game Editor/BuildZip")]
//    private static void BuildZip()
//    {
//        GoCompress(Application.streamingAssetsPath + "/AssetBundles", Application.streamingAssetsPath +"/HotFixResources"+ "/AssetBundles");
////        GoCompress(Application.streamingAssetsPath + "/Config", Application.streamingAssetsPath + "/HotFixResources"+ "/Config");
//    }
//    
//    public static void GoCompress(string SourceFile, string TartgetFile)
//    {
//        string Source = SourceFile;
//        string Tartget = TartgetFile+".zip";
//        Directory.CreateDirectory(Path.GetDirectoryName(Tartget));
//
//        DebugTools.Log("Tartget =" + Tartget);
//        FileStream output = new FileStream(TartgetFile, FileMode.Create);
//        SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
//
//        Compress(Source, output, coder);
//        output.Flush();
//        output.Close();
//
//    }
//    
//    public static void Compress(string source, FileStream output, SevenZip.Compression.LZMA.Encoder coder)
//    {
//        
//        
//        string[] filenames = Directory.GetFileSystemEntries(source);
//        foreach (string input in filenames)
//        {
//            if (Directory.Exists(input))
//            {
//                // 递归压缩子文件夹
//                Compress(input, output, coder);
//            }
//            else
//            {
//                using (FileStream inputFs = File.OpenRead(input))
//                {
//                    // Write the encoder properties
//                    coder.WriteCoderProperties(output);
//		
//                    // Write the decompressed file size.
//                    output.Write(BitConverter.GetBytes(input.Length), 0, 8);
//		
//                    // Encode the file.
//                    coder.Code(inputFs, output, input.Length, -1, null);
////                    output.Flush();
////                    output.Close();
//                    inputFs.Close();
//                }
//            }
//        }
//    }
    
    [MenuItem ("Game Editor/CompressFile")]
    static void CompressFile () 
    {
        //压缩文件
        CompressFileLZMA(Application.dataPath+"/1.jpg",Application.dataPath+"/2.zip");
        AssetDatabase.Refresh();
 
    }
    [MenuItem ("Game Editor/DecompressFile")]
    static void DecompressFile () 
    {
        //解压文件
        DecompressFileLZMA(Application.dataPath+"/2.zip",Application.dataPath+"/3.jpg");
        AssetDatabase.Refresh();
    }
    
    private static void CompressFileLZMA(string inFile, string outFile)
    {
        SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);
		
        // Write the encoder properties
        coder.WriteCoderProperties(output);
		
        // Write the decompressed file size.
        output.Write(BitConverter.GetBytes(input.Length), 0, 8);
		
        // Encode the file.
        coder.Code(input, output, input.Length, -1, null);
        output.Flush();
        output.Close();
        input.Close();
    }
	
    private static void DecompressFileLZMA(string inFile, string outFile)
    {
        SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);
		
        // Read the decoder properties
        byte[] properties = new byte[5];
        input.Read(properties, 0, 5);
		
        // Read in the decompress file size.
        byte [] fileLengthBytes = new byte[8];
        input.Read(fileLengthBytes, 0, 8);
        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);
 
        // Decompress the file.
        coder.SetDecoderProperties(properties);
        coder.Code(input, output, input.Length, fileLength, null);
        output.Flush();
        output.Close();
        input.Close();
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
            string extension = files[i]; //filePath.Substring(files[i].LastIndexOf("."));

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