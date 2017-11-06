using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMain : MonoBehaviour {

	// Use this for initialization
	private void Awake()
	{
		HAResourceManager r = gameObject.AddComponent<HAResourceManager>();
		r.InitWithRootPath(PathManager.Instance.RuntimeAssetBundleReadPath(), "AssetBundles");

		HAResourceManager.self.GetSprite("uis_atlas.u3d", "uis_atlas");
	}

	public void OnClick()
	{
		HAResourceManager.self.LoadScene("skg.u3d", true, "6 SkeletonGraphic", package =>
		{
//			SceneManager.LoadScene("6 SkeletonGraphic");
		});
		
	}
}
