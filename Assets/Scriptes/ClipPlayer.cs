using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipPlayer : MonoBehaviour 
{
	private bool _isPlay;

	private ClipDetail _clipData;
	private NodeDetail _nextNode;			// 準備中のノード
	private NodeDetail[] _currentNodes;		// 出現中のノード
	private int _nodeCount;
	private float _time;
	private bool _phrase;

	private IClipPlayerDelegate dlg;

	// Use this for initialization
	void Start () 
	{
		_isPlay = false;
		_nodeCount = 0;
		_time = 0;
		_phrase = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_isPlay) {
			_nextNode = _clipData.nodeDatas [_nodeCount];
			float ta = ((((_nextNode.Measure) - 1) * _clipData.Bpm) / 15);
			float tb = ((_nextNode.Beat - 1) * 240) / (_clipData.BeatCount * _clipData.Bpm);
			float nextNodeTime = ta + tb;

			_time += Time.deltaTime;

			//Debug.Log (ta + " + " + tb + " = " + nextNodeTime);

			// 時間経過でノードを配置
			if (nextNodeTime <= (int)_time) {
				// ノードを格納
				_currentNodes [_nodeCount] = _nextNode;

				_nextNode.isAlive = true;
				dlg.SetupNode (_nextNode);

				if (_nodeCount < _clipData.nodeDatas.Count - 1) {
					// 次ノード
					_nodeCount++;
				}

				Debug.Log ("" + _nodeCount);
			}

			// 出現中のノードを処理
			for (int i = 0; i < _clipData.nodeDatas.Count; i++) {
				if (_clipData.nodeDatas [i].isAlive) {
					if (_clipData.nodeDatas [i].Type == NodeType.Pose) {
						// 消滅するタイミングで結果を処理する
						if (_currentNodes [i].DeleteTime (_time, _clipData)) {
							// ノードの結果処理
							if (dlg.IsHit ()) {
								dlg.OnNodeResult (true, _currentNodes [i]);
							} else {
								dlg.OnNodeResult (false, _currentNodes [i]);
								_phrase = false;
							}

							if (_clipData.nodeDatas [i].Text != "-") {
								dlg.OnPhraseResult (_phrase, _currentNodes [i]);
								_phrase = true;
							}

							if (i == _clipData.nodeDatas.Count - 1) {
								dlg.OnClipResult (true);
								_isPlay = false;
							}
						}
					}

					if (_clipData.nodeDatas [i].Type == NodeType.Clap) {
						// 拍手が決まった！
						if (dlg.IsClap ()) {
							dlg.OnNodeResult (true, _currentNodes [i]);

							_clipData.nodeDatas [i].BeatCount = 0;		// 強制消滅

							if (_clipData.nodeDatas [i].Text != "-") {
								dlg.OnPhraseResult (_phrase, _currentNodes [i]);
								_phrase = true;
							}

							// 消滅
							if (_currentNodes [i].DeleteTime (_time, _clipData)) {
								if (i == _clipData.nodeDatas.Count - 1) {
									dlg.OnClipResult (true);
									_isPlay = false;
								}
							}

							return;
						}

						// 消滅
						if (_currentNodes [i].DeleteTime (_time, _clipData)) {

							dlg.OnNodeResult (false, _currentNodes [i]);
							_phrase = false;

							if (_clipData.nodeDatas [i].Text != "-") {
								dlg.OnPhraseResult (_phrase, _currentNodes [i]);
								_phrase = true;
							}

							if (i == _clipData.nodeDatas.Count - 1) {
								dlg.OnClipResult (true);
								_isPlay = false;
							}
						}
					}
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
		_currentNodes = new NodeDetail[_clipData.nodeDatas.Count];

		// 楽曲再生
		audio.Play ();

		_isPlay = true;
	}
}



