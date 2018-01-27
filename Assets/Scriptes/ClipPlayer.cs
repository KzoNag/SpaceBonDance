using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipPlayer : MonoBehaviour 
{
	private bool _isPlay;

	private ClipDetail _clipData;
	private NodeDetail _currentNode;
	private int _nodeCount;
	private float _time;

    private IClipPlayerDelegate dlg;

	// Use this for initialization
	void Start () 
	{
		_isPlay = false;
		_nodeCount = 0;
		_time = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (_isPlay) {

			_currentNode = _clipData.nodeDatas [_nodeCount];
			float ta = ((((_currentNode.Measure) - 1) * _clipData.Bpm) / 15);
			float tb = ((_currentNode.Beat - 1) * 240) / (_clipData.BeatCount * _clipData.Bpm);
			float nextNodeTime = ta + tb;

			//Debug.Log (ta + " + " + tb + " = " + nextNodeTime);

			_time += Time.deltaTime;

			if (nextNodeTime <= (int)_time) {

				// 次ノード
				_nodeCount++;

                // 曲の終わり
				if (_nodeCount >= _clipData.nodeDatas.Count) {
					_isPlay = false;

                    dlg.OnClipResult(true);
				}
                else
                {
                    var newNode = _clipData.nodeDatas[_nodeCount];
                    dlg.SetupNode(newNode);
                }
            }
		}
	}

	public void Play(ClipDetail data, IClipPlayerDelegate clipDelegate)
	{
        this.dlg = clipDelegate;

		// audio準備
		AudioSource audio = this.GetComponent<AudioSource> ();
		audio.clip = Resources.Load("Sound/wav/" + data.ClipName) as AudioClip;

		// node準備
		data.CreateNode ();
		_clipData = data;

		// 楽曲再生
		audio.Play ();

		_isPlay = true;
	}
}



