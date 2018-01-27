using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public CanvasGroup fadeGroup;
    private const float FadeTime = 2.0f;

    // Use this for initialization
    private void Awake()
    {
        // 黒状態から開始
        fadeGroup.alpha = 1.0f;
    }

    IEnumerator Start ()
    {
        while(true)
        {
            bool clear = false;

            // タイトル
            yield return StartCoroutine(LoadScecneAsync("Title"));

            // タイトル終了監視？
            yield return new WaitUntil(() => kinectInput.IsClap);

            // ステージループ
            for (int i=0; i < clipDatas.Length; ++i)
            {
                var clipData = clipDatas[i];

                // ステージタイトル
                yield return StartCoroutine(LoadScecneAsync("StageTitle", () => { SetupStageTitle(i + 1, clipData.title); }));
                yield return new WaitForSeconds(3);

                yield return StartCoroutine(LoadScecneAsync("Stage"));

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
                yield return new WaitUntil(() => kinectInput.IsClap);
            }

            yield return null;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SetupStageTitle(int stageIndex, string title)
    {
        var stageTitle = FindObjectOfType<StageTitle>();
        stageTitle.Setup(stageIndex, title);
    }

    private IEnumerator LoadScecneAsync(string sceneName, System.Action onLoad = null)
    {
        // 暗転
        yield return StartCoroutine(FadeOut());

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

        yield return null;

        // ロードイベント実行
        if(onLoad != null)
        {
            onLoad();
        }

        // 明転
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        return Fade(0.0f, FadeTime);
    }

    private IEnumerator FadeOut()
    {
        return Fade(1.0f, FadeTime);
    }

    private IEnumerator Fade(float to, float duration)
    {
        float alpha = fadeGroup.alpha;
        float startTime = Time.time;

        while(Mathf.Abs(to - fadeGroup.alpha) > 0.001f)
        {
            fadeGroup.alpha = Mathf.Lerp(alpha, to, (Time.time - startTime) / duration);

            yield return null;
        }

        fadeGroup.alpha = to;
    }
}

[System.Serializable]
public class ClipData
{
    public string title;
}