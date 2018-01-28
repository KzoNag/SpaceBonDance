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
		float deleteTime = clipdata.GetTime(Measure, Beat + BeatCount, this);

        return (deleteTime < time);
	}


	public float LimitTime01(float time, ClipDetail clipdata)
	{
		float deleteTime = clipdata.GetTime (Measure, Beat + BeatCount, this);
		float startTime = clipdata.GetTime (Measure, Beat, this);

		return (time - startTime) / (deleteTime - startTime);
	}
}


public enum NodeType
{
	Pose,		// ポーズ
	Clap		// クラップ
}