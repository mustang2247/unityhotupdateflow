using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Manager
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public class ResLoaderManager : MonoBehaviour
    {
        private static Dictionary<string, AssetBundle[]> depedentAssetBundlesList =
            new Dictionary<string, AssetBundle[]>();

        private static Dictionary<string, AssetBundle> depedentAssetBundlesA = new Dictionary<string, AssetBundle>();

        private static Dictionary<string, bool> _dictionaryssetBundles = new Dictionary<string, bool>();


        private static Dictionary<string, AssetBundle> _allAssetBundleDirectory = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// 加载所有依赖资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="cbAction"></param>
        /// <returns></returns>
        //public static IEnumerator LoadAllDependencies(string assetBundleName, Action<bool> cbAction)
        //{
        //    bool isSuc = true;
        //    //assetbundle所在的路径
        //    string assetBundlePath = PathManager.Instance.StreamingAssetsPath + "AssetBundles/";
        //    //            DebugTools.Log("assetbundle所在的路径: " + assetBundlePath);
        //    //manifest文件所在路径
        //    string manifestPath = assetBundlePath + "AssetBundles";
        //    //            DebugTools.Log("manifest文件所在路径: " + manifestPath);

        //    //首先加载manifest文件
        //    WWW wwwManifest = WWW.LoadFromCacheOrDownload(manifestPath, 0);
        //    yield return wwwManifest;
        //    if (wwwManifest.error == null)
        //    {

        //        AssetBundle manifestBundle = wwwManifest.assetBundle;
        //        AssetBundleManifest manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
        //        manifestBundle.Unload(false);

        //        string[] assetBundles = manifest.GetAllAssetBundles();
        //        for (int i = 0; i < assetBundles.Length; i++)
        //        {
        //            //获取依赖文件列表
        //            string[] depedentAssetBundles = manifest.GetAllDependencies(assetBundles[i]);
        //            if (depedentAssetBundles != null && depedentAssetBundles.Length > 0)
        //            {
        //                AssetBundle[] abs = new AssetBundle[depedentAssetBundles.Length];
        //                for (int j = 0; j < depedentAssetBundles.Length; j++)
        //                {
        //                    //加载所有的依赖文件
        //                    WWW www = WWW.LoadFromCacheOrDownload(assetBundlePath + depedentAssetBundles[j], 0);
        //                    yield return www;
        //                    abs[j] = www.assetBundle;
        //                    depedentAssetBundlesA[depedentAssetBundles[j]] = www.assetBundle;
        //                    //DebugTools.Log(assetBundles[i] + "  :   " + depedentAssetBundles[j]);
        //                }

        //                depedentAssetBundlesList[assetBundles[i]] = abs;
        //            }
        //            else
        //            {
        //                depedentAssetBundlesList[assetBundles[i]] = null;
        //            }

        //        }
        //    }
        //    else
        //    {
        //        isSuc = false;
        //        Debug.Log(wwwManifest.error);
        //    }

        //    if (cbAction != null)
        //    {
        //        cbAction(isSuc);
        //    }
        //}
        /// <summary>
        /// 加载AssetBundles目录下指定资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="cbAction"></param>
        /// <returns></returns>
        //public static IEnumerator LoadDependencies(string assetBundleName, Action<Sprite[]> cbAction)
        //{
        //    if (_allAssetBundleDirectory.ContainsKey(assetBundleName))
        //    {
        //        DebugTools.LogError("！！资源重复加载 name:"+ assetBundleName);
        //    }
        //    bool isSuc = true;
        //    //assetbundle所在的路径
        //    string assetBundlePath = PathManager.Instance.StreamingAssetsPath + "AssetBundles/";
        //    //            DebugTools.Log("assetbundle所在的路径: " + assetBundlePath);
        //    //manifest文件所在路径
        //    string manifestPath = assetBundlePath + assetBundleName;
        //    //            DebugTools.Log("manifest文件所在路径: " + manifestPath);

        //    //首先加载manifest文件
        //    WWW wwwManifest = WWW.LoadFromCacheOrDownload(manifestPath, 0);
        //    yield return wwwManifest;
        //    Sprite[] sprites = null;
        //    if (wwwManifest.error == null)
        //    {

        //        AssetBundle manifestBundle = wwwManifest.assetBundle;
        //        //存储assetBundle资源
        //        _allAssetBundleDirectory[assetBundleName] = manifestBundle;
        //        string[] names = manifestBundle.GetAllAssetNames();
        //        // AssetBundleManifest manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
        //        //manifestBundle.Unload(false);
        //        sprites = new Sprite[names.Length];
        //        var index = 0;
        //        foreach(string s in names)
        //        {
        //            Sprite sprite = manifestBundle.LoadAsset<Sprite>(s);
        //            // Debug.Log("---index---" + index + ", " + sprite);
        //            sprites[index] = sprite;
        //            index++;

        //        }
        //        //_dictionaryAtlasSprites.Add(assetBundleName, sprites);
        //    }
        //    else
        //    {
        //        isSuc = false;
        //        Debug.Log(wwwManifest.error);
        //    }

        //    if (cbAction != null)
        //    {
        //        cbAction(sprites);
        //    }
        //}
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneAssetBundle">场景文件名称</param>
        /// <param name="sceneName">场景名称</param>
        /// <param name="cbAction">回掉</param>
        /// <returns></returns>
        public static IEnumerator LoadScenes(string sceneAssetBundle, string sceneName, Action<bool> cbAction)
        {
            bool isSuc = true;
            Resources.UnloadUnusedAssets();

            if (_dictionaryssetBundles.ContainsKey(sceneAssetBundle))
                //if (SceneManager.GetSceneByName(sceneAssetBundle).isLoaded)
            {
                //AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
                //Scene scene = SceneManager.GetSceneByNam  e(sceneName);
                //SceneManager.SetActiveScene(scene);
                SceneManager.LoadScene(sceneName);
                yield return null;
            }
            else
            {
                //场景AssetBundle路径
                //______________________________ 修改位置0________________________________________//
                string path;



                path = PathManager.Instance.RuntimeAssetbundlePath() + sceneAssetBundle;


                /*
                if (Application.platform == RuntimePlatform.Android)
                {
                    path = "file://" + Application.persistentDataPath + "/XJJBXAppData/" + "HotFixResources/" + "AssetBundles/" + sceneAssetBundle;
                }
                else if (Application.platform == RuntimePlatform.WindowsEditor|| Application.platform == RuntimePlatform.OSXEditor)
                {
                    path = PathManager.Instance.StreamingAssetsPath + "AssetBundles/" + sceneAssetBundle;
                }
                else
                {
                    path = "";
                }
                */


                WWW www = WWW.LoadFromCacheOrDownload(path, 0);
                yield return www;

                if (www.error == null)
                {
                    AssetBundle bundle = www.assetBundle; 
                    AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
                    yield return async;
                }
                else
                {
                    isSuc = false;
                }
                _dictionaryssetBundles[sceneAssetBundle] = isSuc;
            }
 
            if (cbAction != null)
            {
                cbAction(isSuc);
            }
        }

        /// <summary>
        /// 加载assets资源
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="cbAction"></param>
        /// <returns></returns>
        public static IEnumerator LoadFromFileAsyncByPrefab(string filePath, string assetBundleName, string prefabName,
            Action<Object[]> cbAction)
        {
            if (_allAssetBundleDirectory.ContainsKey(assetBundleName))
            {
                DebugTools.LogError("！！资源重复加载 name:" + assetBundleName);
            }


            //string assetBundlePath = PathManager.Instance.StreamingAssetsPath + "AssetBundles/";

            string assetBundlePath;
            /*
            //______________________________ 修改位置1________________________________________//
       
            if (Application.platform == RuntimePlatform.Android)
            {
                assetBundlePath = "file://" + Application.persistentDataPath + "/XJJBXAppData/" + "HotFixResources/" + "AssetBundles/";
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                assetBundlePath = PathManager.Instance.StreamingAssetsPath + "AssetBundles/";
            }
            else
            {
                assetBundlePath = "";
            }
            */

            assetBundlePath = PathManager.Instance.RuntimeAssetbundlePath();


            filePath = assetBundlePath + assetBundleName;

            //加载需要的文件
            WWW www = WWW.LoadFromCacheOrDownload(filePath, 0);
            yield return www;

            AssetBundle assetBundle = www.assetBundle;
            assetBundle.Unload(false);
            _allAssetBundleDirectory[assetBundleName] = assetBundle;
            var assetLoadRequest = assetBundle.LoadAllAssetsAsync();
//            var assetLoadRequest = assetBundle.LoadAssetAsync<GameObject>(prefabName);
            yield return assetLoadRequest;
          //  www.assetBundle.Unload(false);
            //            GameObject prefab = assetLoadRequest.asset as GameObject;
            //DebugTools.Log("LoadFromFileAsync  3");
          
            if (cbAction != null)
            {
                cbAction(assetLoadRequest.allAssets);
            }

            //assetBundle.Unload(false);
        }

        /// <summary>
        /// 加载assets资源(返回多个资源)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="cbAction"></param>
        /// <returns></returns>
        public static IEnumerator LoadFromFileAsyncByPrefabs(string filePath, string assetBundleName,
            Action<GameObject[]> cbAction)
        {
            if (_allAssetBundleDirectory.ContainsKey(assetBundleName))
            {
                DebugTools.LogError("！！资源重复加载 name:" + assetBundleName);
            }



            //______________________________ 修改位置2________________________________________//



            string assetBundlePath;
            /*
            if (Application.platform == RuntimePlatform.Android)
            {
                assetBundlePath = "file://" + Application.persistentDataPath + "/XJJBXAppData/" + "HotFixResources/" + "AssetBundles/";
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                assetBundlePath = PathManager.Instance.StreamingAssetsPath + "AssetBundles/";
            }
            else
            {
                assetBundlePath = "";
            }
            */
            assetBundlePath = PathManager.Instance.RuntimeAssetbundlePath();


            filePath = assetBundlePath + assetBundleName;
            //加载需要的文件
            WWW www = WWW.LoadFromCacheOrDownload(filePath, 0);
            yield return www;
            AssetBundle assetBundle = www.assetBundle;
            _allAssetBundleDirectory[assetBundleName] = assetBundle;

            Sprite[] allAssetsd = assetBundle.LoadAllAssets<Sprite>();
            GameObject[] allAssets = assetBundle.LoadAllAssets<GameObject>();
            yield return allAssets;
            if (cbAction != null)
            {
                cbAction(allAssets);
            }
            //_bundleForPrefabs.Unload(false);
        }

        /// <summary>
        /// 切换场景必须调用释放资源
        /// </summary>
        public static void ClearAssetBundleDic()
        {
            //if (_bundleForPrefabs)
            //{
            //    _bundleForPrefabs.Unload(true);
            //}
            foreach (KeyValuePair<string, AssetBundle> keyValuePair in _allAssetBundleDirectory)
            {
                keyValuePair.Value.Unload(true);
            }
            _allAssetBundleDirectory.Clear();
        }

        
        /// <summary>
        /// 加载text json xml使用
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="cbAction"></param>
        /// <param name="progressAction"></param>
        /// <returns></returns>
        public static IEnumerator LoadXMLFileAsyncByFilePath(string[] paths,
            Action<Dictionary<string, string>> cbAction, Action<int> progressAction = null)
        {
            Dictionary<string, string> ouTextAssets = new Dictionary<string, string>();
            for (int i = 0; i < paths.Length; i++)
            {
                WWW www = new WWW(PathManager.Instance.RuntimeConfigPath() + paths[i]);

                yield return www;

             

                ouTextAssets[paths[i]] = www.text;
            }

            if (cbAction != null)
            {
                cbAction(ouTextAssets);
            }
        }

        /// <summary>
        /// 加载声音
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cbAction"></param>
        /// <returns></returns>
        public static IEnumerator LoadSound(string path, Action<AudioClip> cbAction)
        {
            string soundPath = PathManager.Instance.AssetsDataPath + path;
            //            DebugTools.Log("###########  " + soundPath);
         
            WWW www = new WWW(soundPath);
            yield return www;
            //            DebugTools.Log("加载完成");

            if (cbAction != null)
            {
                cbAction(www.GetAudioClip(false,true));
            }
        }
    }
}