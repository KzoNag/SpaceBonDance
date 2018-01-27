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

	public void SetNodeType (int value)
	{
		Type = (NodeType)value;
	}
}


public enum NodeType
{
	Pose,		// ポーズ
	Clip		// クリップ
}