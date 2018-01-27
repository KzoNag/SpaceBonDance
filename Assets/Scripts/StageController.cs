using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour, IClipPlayerDelegate
{
    public enum StageState
    {
        Unknown,
        StartEvent,
        Playing,
        Result,
        End,
    }

    public StageState State { get; private set; }
    public bool IsSuccess { get; private set; }
    private ClipDetail clipData;

    public ClipPlayer player;
    //public TestClipPlayer player;

    public RectTransform targetPrefab;
    private RectTransform[] currentTargets = new RectTransform[2];

    public Text startText;
    public Text resultText;
    public GameObject clapText;

    public AudioSource seSource;
    public AudioClip okClip;
    public AudioClip ngClip;

    public Gohst gohst;

    private void Awake()
    {
        startText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
        DestroyTarget();
    }

    private void OnDestroy()
    {
        DestroyTarget();
    }

    public void Play(ClipDetail clipData)
    {
        this.clipData = clipData;

        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        State = StageState.StartEvent;

        startText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        startText.gameObject.SetActive(false);
        player.Play(clipData, this);

        State = StageState.Playing;
    }

    private IEnumerator Result(bool success)
    {
        State = StageState.Result;

        // 成否に応じたリザルト演出
        resultText.gameObject.SetActive(true);
        resultText.text = success ? "SUCCESS!!" : "FAILED..";

        yield return new WaitForSeconds(3);

        IsSuccess = success;
        State = StageState.End;
    }

    private void StartNext(NodeDetail node)
    {
        DestroyTarget();

        if (node.Type == NodeType.Pose)
        {
            CreateTarget();
        }
        else
        {
            clapText.SetActive(true);

            gohst.Clap();
        }
    }

    private void DestroyTarget()
    {
        if(clapText != null)
        {
            clapText.SetActive(false);
        }

        if (currentTargets != null)
        {
            foreach (var target in currentTargets)
            {
                if (target != null)
                {
                    Destroy(target.gameObject);
                }
            }
        }
    }

    private void CreateTarget()
    {
        for (int i = 0; i < currentTargets.Length; ++i)
        {
            Vector2 position;

            position.x = Random.Range(0.1f, 0.9f);
            position.y = Random.Range(0.1f, 0.9f);

            currentTargets[i] = CreateTarget(position);
        }
    }

    private RectTransform CreateTarget(Vector2 position)
    {
        var target = Instantiate(targetPrefab, GameManager.Instance.kinectInput.ShiletteRootTransform);

        target.gameObject.SetActive(true);
        target.anchoredPosition = ToScreenPosition(position);

        return target;
    }

    private bool IsHitAll()
    {
        for (int i = 0; i < currentTargets.Length; ++i)
        {
            if (!IsHit(currentTargets[i])) { return false; }
        }

        return true;
    }

    private bool IsHit(RectTransform target)
    {
        var normalizedPosition = ToNormalizePosition(target.anchoredPosition);

        return GameManager.Instance.kinectInput.IsHit(normalizedPosition.x, normalizedPosition.y, 50f / 512f);
    }

    private Vector2 ToScreenPosition(Vector2 position)
    {
        return new Vector2(
            position.x * GameManager.Instance.kinectInput.ShiletteRootTransform.sizeDelta.x,
            -position.y * GameManager.Instance.kinectInput.ShiletteRootTransform.sizeDelta.y
            );
    }

    private Vector2 ToNormalizePosition(Vector2 position)
    {
        return new Vector2(
            position.x / GameManager.Instance.kinectInput.ShiletteRootTransform.sizeDelta.x,
            -position.y / GameManager.Instance.kinectInput.ShiletteRootTransform.sizeDelta.y
            );
    }

    #region IClipPlayerDelegate

    public bool IsClap()
    {
        return GameManager.Instance.kinectInput.IsClap;
    }

    public bool IsHit()
    {
        return IsHitAll();
    }

    public void SetupNode(NodeDetail node)
    {
        StartNext(node);
    }

    public void OnNodeResult(bool success, NodeDetail node)
    {
        Debug.Log("OnNodeResult:" + success.ToString());

        DestroyTarget();

        var clip = success ? okClip : ngClip;
        seSource.PlayOneShot(clip);

        if(success)
        {
            if (node.Type == NodeType.Clip)
            {
                gohst.Move(0.1f);
            }
        }
    }

    public void OnPhraseResult(bool success, NodeDetail pharaseLastNode)
    {
        Debug.Log("OnPhraseResult:" + success.ToString() + ", " + pharaseLastNode.Text);
    }

    public void OnClipResult(bool success)
    {
        Debug.Log("OnClipResult:" + success.ToString());

        StartCoroutine(Result(success));
    }

    #endregion
}
