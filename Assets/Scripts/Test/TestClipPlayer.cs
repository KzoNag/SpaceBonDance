using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClipPlayer : MonoBehaviour
{
    private IClipPlayerDelegate dlg;

    private float time;
    private bool isPlaying;

    private NodeDetail currentNode;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!isPlaying) { return; }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            dlg.OnClipResult(true);
            isPlaying = false;
            return;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            dlg.OnClipResult(false);
            isPlaying = false;
            return;
        }

        // ノード成功
        if (currentNode != null && Input.GetKeyDown(KeyCode.S))
        {
            dlg.OnNodeResult(true, currentNode);

            if(Input.GetKey(KeyCode.P))
            {
                dlg.OnPhraseResult(true, currentNode);
            }

            currentNode = null;
        }

        // ノード失敗
        if(currentNode != null && Time.time - time > 2)
        {
            dlg.OnNodeResult(false, currentNode);

            if (Input.GetKey(KeyCode.P))
            {
                dlg.OnPhraseResult(false, currentNode);
            }

            currentNode = null;
        }

        // ノード生成
		if(Time.time - time > 3)
        {
            currentNode = new NodeDetail() { text = "セリフのテスト" };
            dlg.SetupNode(currentNode);
            time = Time.time;
        }
	}

    public void Play(IClipPlayerDelegate dlg)
    {
        this.dlg = dlg;
        time = Time.deltaTime;
        isPlaying = true;
    }
}
