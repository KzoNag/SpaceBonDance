using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private ClipData clipData;

    public TestClipPlayer player;

    public void Play(ClipData clipData)
    {
        this.clipData = clipData;

        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        State = StageState.StartEvent;

        yield return new WaitForSeconds(2);

        player.Play(this);

        State = StageState.Playing;
    }

    private IEnumerator Result(bool success)
    {
        State = StageState.Result;

        // 成否に応じたリザルト演出
        yield return new WaitForSeconds(3);

        IsSuccess = success;
        State = StageState.End;
    }

    #region IClipPlayerDelegate

    public bool IsClap()
    {
        return GameManager.Instance.kinectInput.IsClap;
    }

    public bool IsHit()
    {
        return GameManager.Instance.kinectInput.IsHit(0.5f, 0.5f, 50f / 512f);
    }

    public void SetupNode(NodeDetail node)
    {
        Debug.Log("SetupNode");
    }

    public void OnNodeResult(bool success, NodeDetail node)
    {
        Debug.Log("OnNodeResult:" + success.ToString());
    }

    public void OnPhraseResult(bool success, NodeDetail pharaseLastNode)
    {
        Debug.Log("OnPhraseResult:" + success.ToString() + ", " + pharaseLastNode.text);
    }

    public void OnClipResult(bool success)
    {
        Debug.Log("OnClipResult:" + success.ToString());

        StartCoroutine(Result(success));
    }

    #endregion
}
