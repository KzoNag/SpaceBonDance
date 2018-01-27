using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClipPlayerDelegate
{
    // クラップしたかどうか
    bool IsClap();

    // ポーズがヒットしたかどうか
    bool IsHit();

    // ノードのセットアップ
    void SetupNode(NodeDetail node);

    // ノードの結果処理
    void OnNodeResult(bool success, NodeDetail node);

    // フレーズの結果処理
    void OnPhraseResult(bool success, NodeDetail pharaseLastNode);

    // クリップの結果処理
    void OnClipResult(bool success);
}

public class DummyClipPlayerDelegate : IClipPlayerDelegate
{
    public bool IsClap()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public bool IsHit()
    {
        return Input.GetKeyDown(KeyCode.Return);
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
        Debug.Log("OnPhraseResult:" + success.ToString() + ", " + pharaseLastNode.Text);
    }

    public void OnClipResult(bool success)
    {
        Debug.Log("OnClipResult:" + success.ToString());
    }
}
