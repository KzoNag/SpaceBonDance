using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) { instance = FindObjectOfType<GameManager>(); }
            return instance;
        }
    }

    public ClipData[] clipDatas;

    private int stageIndex;

    public KinectInput kinectInput;

	// Use this for initialization
	IEnumerator Start ()
    {
        while(true)
        {
            bool clear = false;

            // タイトル
            yield return StartCoroutine(LoadScecneAsync("Title"));

            // タイトル終了監視？
            yield return new WaitUntil(() => kinectInput.IsClap());

            // ステージループ
            for (int i=0; i < clipDatas.Length; ++i)
            {
                var clipData = clipDatas[i];

                yield return StartCoroutine(LoadScecneAsync("Stage"));

                yield return null;

                // ステージコントローラー取得
                bool stageClear = false;

                // ステージ終了まで監視
                while(true)
                {
                    if(Input.GetKeyDown(KeyCode.S))
                    {
                        stageClear = true;
                        break;
                    }
                    else if(Input.GetKeyDown(KeyCode.F))
                    {
                        break;
                    }
                    yield return null;
                }

                // 失敗ならループ終了
                if(!stageClear)
                {
                    break;
                }
                // 最終ステージクリアならフラグ立てる
                else if(i == clipDatas.Length - 1)
                {
                    clear = true;
                }
            }

            // エンディング
            if(clear)
            {
                yield return StartCoroutine(LoadScecneAsync("End"));

                // エンディング終了監視？
                yield return new WaitUntil(() => kinectInput.IsClap());
            }

            yield return null;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator LoadScecneAsync(string sceneName)
    {
        // 暗転

        // Core以外をアンロード
        for(int i=0; i< SceneManager.sceneCount; ++i)
        {
            var scene = SceneManager.GetSceneAt(i);
            if(scene.name != "Core")
            {
               yield return SceneManager.UnloadSceneAsync(scene);
            }
        }

        // 指定シーンをロード
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // 明転

    }
}

[System.Serializable]
public class ClipData
{
    public string title;
}