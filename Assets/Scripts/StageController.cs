using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public class NodeInfo
    {
        public NodeDetail node;
        public List<NodeCircle> circleList = new List<NodeCircle>();

        public bool IsAchive()
        {
            if (node.Type == NodeType.Clap)
            {
                return GameManager.Instance.kinectInput.IsClap;
            }
            else
            {
                foreach (var circle in circleList)
                {
                    var normalizedPosition = ToNormalizePosition(circle.Target.anchoredPosition);
                    if (!GameManager.Instance.kinectInput.IsHit(normalizedPosition.x, normalizedPosition.y, 50f / 512f))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }

    public StageState State { get; private set; }
    public bool IsSuccess { get; private set; }
    private ClipDetail clipData;

    public ClipPlayer player;
    //public TestClipPlayer player;

    public RectTransform targetPrefab;
    //private RectTransform[] currentTargets = new RectTransform[2];

    public NodeCircle nodePrefab;
    private List<NodeInfo> nodeList = new List<NodeInfo>();

    public RectTransform clapNodeTarget;
    public List<RectTransform> poseNodeTargetList;

    public Text startText;
    public Text resultText;
    public GameObject clapText;

    public AudioSource seSource;
    public AudioClip okClip;
    public AudioClip ngClip;

    public Gohst gohst;
    public Kanekorian kaneko;

    public BalloonText gohstBalloon;
    public BalloonText kanekoBalloon;

    private void Awake()
    {
        startText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);

        GameManager.Instance.kinectInput.TargetCanvas.worldCamera = Camera.main;
    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.kinectInput.TargetCanvas.worldCamera = null;
        }
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

    private void AddNode(NodeDetail node)
    {
        NodeInfo nodeInfo = new NodeInfo();

        nodeInfo.node = node;

        if(node.Type == NodeType.Pose)
        {
            var randomIndex = Enumerable.Range(0, poseNodeTargetList.Count).OrderBy(_ => System.Guid.NewGuid());

            for(int i=0; i<2; ++i)
            {
                var circle = Instantiate(nodePrefab);
                var target = poseNodeTargetList[randomIndex.ElementAt(i)];
                circle.Setup(target, node);
                nodeInfo.circleList.Add(circle);
            }
        }
        else
        {
            var circle = Instantiate(nodePrefab);
            var target = clapNodeTarget;
            circle.Setup(target, node);
            nodeInfo.circleList.Add(circle);
        }

        nodeList.Add(nodeInfo);
    }

    private void RemoveNode(NodeDetail node)
    {
        var nodeInfos = nodeList.Where(_info => _info.node == node);
        foreach(var info in nodeInfos)
        {
            foreach(var circle in info.circleList)
            {
                Destroy(circle.gameObject);
            }
            info.circleList.Clear();
        }
        nodeList.RemoveAll(_info => _info.node == node);
    }

    private bool IsHitAll(NodeDetail node)
    {
        foreach(var nodeInfo in nodeList.Where(_info => _info.node == node))
        {
            if (!nodeInfo.IsAchive()) { return false; }
        }

        return true;
    }

    private static Vector2 ToScreenPosition(Vector2 position)
    {
        return new Vector2(
            position.x * GameManager.Instance.kinectInput.ShiletteRootTransform.sizeDelta.x,
            -position.y * GameManager.Instance.kinectInput.ShiletteRootTransform.sizeDelta.y
            );
    }

    private static Vector2 ToNormalizePosition(Vector2 position)
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

    public bool IsHit(NodeDetail node)
    {
        return IsHitAll(node);
    }

    public void SetupNode(NodeDetail node)
    {
        AddNode(node);
    }

    public void UpdateNode(NodeDetail node, float rate)
    {
        for(int i=0; i < nodeList.Count; ++i)
        {
            if(nodeList[i].node == node)
            {
                for(int j=0; j < nodeList[i].circleList.Count; ++j)
                {
                    nodeList[i].circleList[j].Rate = rate;
                }
            }
        }
    }

    public void OnNodeResult(bool success, NodeDetail node)
    {
        Debug.Log("OnNodeResult:" + success.ToString());

        RemoveNode(node);

        var clip = success ? okClip : ngClip;
        seSource.PlayOneShot(clip);

        float distance = 0.1f;

        if (node.Type == NodeType.Clap)
        {
            gohst.Clap();
            if (success)
            {
                gohst.Move(distance);
            }
        }
        else
        {
            kaneko.Pose();
            if(success)
            {
                kaneko.Move(distance);
            }
        }
    }

    public void OnPhraseResult(bool success, NodeDetail pharaseLastNode)
    {
        Debug.Log("OnPhraseResult:" + success.ToString() + ", " + pharaseLastNode.Text);

        if (!success) { return; }

        if(pharaseLastNode.Type == NodeType.Clap)
        {
            gohstBalloon.Show(pharaseLastNode.Text);
        }
        else
        {
            kanekoBalloon.Show(pharaseLastNode.Text);
        }
    }

    public void OnClipResult(bool success)
    {
        Debug.Log("OnClipResult:" + success.ToString());

        StartCoroutine(Result(success));
    }

    #endregion
}
