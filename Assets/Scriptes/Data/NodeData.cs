using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeDetail
{
	[SerializeField]
	public TextAsset csvfiles;

	public int Measure;		// 出現小節
	public int Beat;		// 出現ビート
	public int BeatCount;	// 滞在時間
	public string Text;		// セリフ

	public NodeType Type;

	public bool isAlive = false;


	public bool DeleteTime (float time, ClipDetail clipdata)
	{
		bool delete = false;

		float ta = ((((Measure) - 1) * clipdata.Bpm) / 15);
		float tb = (((Beat + BeatCount) - 1) * 240) / (clipdata.BeatCount * clipdata.Bpm);
		float deleteTime = ta + tb;

		if (deleteTime < (int)time) {
			delete = true;
			isAlive = false;
		}

		return delete;
	}
}


public enum NodeType
{
	Pose,		// ポーズ
	Clap		// クラップ
}