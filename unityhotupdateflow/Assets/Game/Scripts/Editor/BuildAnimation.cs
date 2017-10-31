using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Animations;
using UnityEngine.EventSystems;

public class BuildAnimation : Editor 
{

    //生成出的Prefab的路径
	private static string PrefabPath = "Assets/Game/Prefabs/Fish/fish";
	//生成出的AnimationController的路径
	private static string AnimationControllerPath = "Assets/Game/Animation/Fish/Animation";
	//生成出的Animation的路径
	private static string AnimationPath = "Assets/Game/Animation/Fish/Animation";
    //美术给的原始图片路径
	private static string ImagePath = Application.dataPath + "/Game/UIs/NewFish/game_fish _new";
    // 冰冻图片路径
    private static string IceImagePath = "Assets\\Game\\UIs\\NewFish\\game_bgs\\fishdeskui\\skill\\newfishgame_desk_iceEffect.png";
    /// <summary>
    /// 鱼正常移动帧频
    /// </summary>
    public static int FishMoveFPS = 50;
    /// <summary>
    /// 鱼正常移动转换为死亡动画 帧频
    /// </summary>
    public static int FishMoveToDeadFPS = 50;
    /// <summary>
    /// 鱼死亡动画 帧频
    /// </summary>
    public static int FishDeadFPS = 30;

    private static string FishMove = "move";
    private static string FishDead = "dead";
    private static string FishStop = "stop";
    private static string LabelMove = "";
    private static string LabelDead = "sw";
    private static string LabelStop = "stop";

    //[MenuItem("Game Editor/Build Animaiton")]
	static void BuildAniamtion() 
	{
        DirectoryInfo raw = new DirectoryInfo(ImagePath);
        if(raw.GetDirectories().Length <= 0)
            return;
		foreach (DirectoryInfo dictorys in raw.GetDirectories()) 
		{
		    //if (dictorys.GetDirectories().Length > 0)
		    {
                Dictionary<string ,AnimationClip> clips = new Dictionary<string, AnimationClip>();
                //查找所有图片，因为我找的测试动画是.jpg 
                FileInfo[] images = dictorys.GetFiles("*.png");

                string[] labels = { LabelMove, LabelDead , LabelStop };
		        string[] clipNames = {FishMove, FishDead , FishStop };
                List<List<FileInfo>> files = ClassifyFile(images, labels);

                //图片生成出一个动画文件
                for (int i = 0; i < files.Count; i++)
		        {
                    clips[clipNames[i]] = BuildAnimationClip(clipNames[i], dictorys.Name, files[i].ToArray());
                }

                //把所有的动画文件生成在一个AnimationController里
                UnityEditor.Animations.AnimatorController controller = BuildAnimationController(clips, dictorys.Name, clipNames);
                //最后生成程序用的Prefab文件

                List<FileInfo> a = files[0];

                foreach (var item in a)
                {
                    Debug.Log(item);
                }
                FileInfo b = a[0];
     


                BuildPrefab(dictorys, controller, files[0][0]);
            }
		}	
	}

    private static List<List<FileInfo>> ClassifyFile(FileInfo[] images,string[] labels)
    {
        List<List<FileInfo>> listFileInfo =new List<List<FileInfo>>();
        for (int i = 0; i < labels.Length; i++)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            listFileInfo.Add(fileInfos);
            foreach (var image in images)
            {
                if (labels[i] == "")
                {
                    int num = -1;
                    for (int j = 1; j < labels.Length; j++)
                    {
                        if (image.Name.IndexOf(labels[j]) > -1)
                            num = image.Name.IndexOf(labels[j]);
                            break;
                    }
                    if (num == -1)
                        fileInfos.Add(image);
                }
                else
                {
                    if (image.Name.IndexOf(labels[i]) > -1)
                    {
                        fileInfos.Add(image);
                    }
                }
            }
        }
        return listFileInfo;
    }

    private static AnimationClip BuildAnimationClip(string animationName, string fileName,FileInfo[] images)
    {
        if (images.Length <= 0)
            return null;
        AnimationClip clip = new AnimationClip();
        //AnimationUtility.SetAnimationType(clip,ModelImporterAnimationType.Generic);
        EditorCurveBinding curveBinding = new EditorCurveBinding();
        curveBinding.type = typeof(SpriteRenderer);
        curveBinding.path = "";
        curveBinding.propertyName = "m_Sprite";
        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[images.Length];
        //动画长度是按秒为单位，1/10就表示1秒切10张图片，根据项目的情况可以自己调节
        float frameTime = 1 / 12f;
        for (int i = 0; i < images.Length; i++)
        {
            string path = DataPathToAssetPath(images[i].FullName);

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            keyFrames[i] = new ObjectReferenceKeyframe();
            keyFrames[i].time = frameTime * i;
            keyFrames[i].value = sprite;
        }
        //动画帧率，30比较合适
        clip.frameRate = 12;

        //有些动画我希望天生它就动画循环
        //if (animationName.IndexOf(LabelDead) == -1)
        {
            //设置idle文件为循环动画
            SerializedObject serializedClip = new SerializedObject(clip);
            AnimationClipSettings clipSettings = new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings"));
            clipSettings.loopTime = true;
            serializedClip.ApplyModifiedProperties();
        }

        System.IO.Directory.CreateDirectory(AnimationPath + "/" + fileName);
        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);
        AssetDatabase.CreateAsset(clip, AnimationPath + "/" + fileName + "/" + animationName + ".anim");
        AssetDatabase.SaveAssets();
        return clip;
    }

	static UnityEditor.Animations.AnimatorController BuildAnimationController(Dictionary<string, AnimationClip> clips, string name , string[] clipNames)
	{
        UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(AnimationControllerPath +"/"+name+".controller");
		UnityEditor.Animations.AnimatorControllerLayer layer = animatorController.layers[0];
		AnimatorStateMachine rootStateMachine = layer.stateMachine;

        //animatorController.AddParameter("IsDieBool", UnityEngine.AnimatorControllerParameterType.Bool);
        //animatorController.AddParameter("IseExitBool", UnityEngine.AnimatorControllerParameterType.Bool);

        // Add States
        for (int i = 0; i < clipNames.Length; i++)
	    {
            AnimatorState stateSwimming = rootStateMachine.AddState(clipNames[i]);
            int num = (clips[clipNames[i]] == null && i==1) ? 0 : i;
            stateSwimming.motion = clips[clipNames[num]];
	        stateSwimming.speed = (i == 1) ? 4f : 1f;
	    }
        //AnimatorStateTransition anyStateTransition = rootStateMachine.AddAnyStateTransition(stateDie);
        //anyStateTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 1, "IsDieBool");
        //anyStateTransition.duration = 0;
        
        //AnimatorStateTransition exitTransition = stateDie.AddExitTransition();
        //exitTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 10, "IseExitBool");
        //exitTransition.duration = 0;

        AssetDatabase.SaveAssets();
		return animatorController;
	}

    static void BuildPrefab(DirectoryInfo dictorys,UnityEditor.Animations.AnimatorController animatorCountorller, FileInfo imageInfo)
	{
  
		//生成Prefab 添加一张预览用的Sprite
		//FileInfo images  = dictorys.GetFiles("*.png")[0];
		GameObject go = new GameObject();
		go.name = dictorys.Name;
		SpriteRenderer spriteRender =go.AddComponent<SpriteRenderer>();
		spriteRender.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(DataPathToAssetPath(imageInfo.FullName));
		Animator animator = go.AddComponent<Animator>();
		animator.runtimeAnimatorController = animatorCountorller;
        //go.tag = "fish";
        GameObject ice = new GameObject("newfishgame_desk_iceEffect");
        ice.AddComponent<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>(IceImagePath);
        ice.transform.parent = go.transform;
        PrefabUtility.CreatePrefab(PrefabPath+"/"+go.name+".prefab",go);
		DestroyImmediate(go);
	}


	static string DataPathToAssetPath(string path)
	{
		if (Application.platform == RuntimePlatform.WindowsEditor)
			return path.Substring(path.IndexOf("Assets\\"));
		else
			return path.Substring(path.IndexOf("Assets/"));
	}

	class AnimationClipSettings
	{
		SerializedProperty m_Property;
		
		private SerializedProperty Get (string property) { return m_Property.FindPropertyRelative(property); }
		
		public AnimationClipSettings(SerializedProperty prop) { m_Property = prop; }
		
		public float startTime   { get { return Get("m_StartTime").floatValue; } set { Get("m_StartTime").floatValue = value; } }
		public float stopTime	{ get { return Get("m_StopTime").floatValue; }  set { Get("m_StopTime").floatValue = value; } }
		public float orientationOffsetY { get { return Get("m_OrientationOffsetY").floatValue; } set { Get("m_OrientationOffsetY").floatValue = value; } }
		public float level { get { return Get("m_Level").floatValue; } set { Get("m_Level").floatValue = value; } }
		public float cycleOffset { get { return Get("m_CycleOffset").floatValue; } set { Get("m_CycleOffset").floatValue = value; } }
		
		public bool loopTime { get { return Get("m_LoopTime").boolValue; } set { Get("m_LoopTime").boolValue = value; } }
		public bool loopBlend { get { return Get("m_LoopBlend").boolValue; } set { Get("m_LoopBlend").boolValue = value; } }
		public bool loopBlendOrientation { get { return Get("m_LoopBlendOrientation").boolValue; } set { Get("m_LoopBlendOrientation").boolValue = value; } }
		public bool loopBlendPositionY { get { return Get("m_LoopBlendPositionY").boolValue; } set { Get("m_LoopBlendPositionY").boolValue = value; } }
		public bool loopBlendPositionXZ { get { return Get("m_LoopBlendPositionXZ").boolValue; } set { Get("m_LoopBlendPositionXZ").boolValue = value; } }
		public bool keepOriginalOrientation { get { return Get("m_KeepOriginalOrientation").boolValue; } set { Get("m_KeepOriginalOrientation").boolValue = value; } }
		public bool keepOriginalPositionY { get { return Get("m_KeepOriginalPositionY").boolValue; } set { Get("m_KeepOriginalPositionY").boolValue = value; } }
		public bool keepOriginalPositionXZ { get { return Get("m_KeepOriginalPositionXZ").boolValue; } set { Get("m_KeepOriginalPositionXZ").boolValue = value; } }
		public bool heightFromFeet { get { return Get("m_HeightFromFeet").boolValue; } set { Get("m_HeightFromFeet").boolValue = value; } }
		public bool mirror { get { return Get("m_Mirror").boolValue; } set { Get("m_Mirror").boolValue = value; } }
	}

}
