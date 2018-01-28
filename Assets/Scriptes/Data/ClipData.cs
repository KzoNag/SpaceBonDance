using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ClipDetail
{
	public int Bpm;
	public int BeatCount;

	public string ClipName;
	public string Title;		// stage title

	public TextAsset csv;
	public List<NodeDetail> nodeDatas;

	public void CreateNode () 
	{
		string[,] nodeArray = CSVIO.Read ("/CSV/" + csv.name + ".csv");
		nodeDatas = new List<NodeDetail> ();

		for (int i = 1; i < nodeArray.GetLength(0); i++) {
			string[] array = new string[nodeArray.GetLength(0)];


			for (int j = 0; j < nodeArray.GetLength(1); j++) {
				array [j] = nodeArray [i, j];
			}

			this.SetNodeStatus (array);
		}
	}


	/// <summary>
	/// (index >>> .Type:0 .Measure:1 .Beat:2 .BeatCount:3 .Text:4)
	/// </summary>
	/// <param name="status">Status.</param>
	private void SetNodeStatus (string[] status) 
	{
		NodeDetail node = new NodeDetail ();
		node.Type = (NodeType)Enum.Parse (typeof(NodeType), (status [0]));
		node.Measure = int.Parse (status [1]);
		node.Beat = int.Parse (status [2]);
		node.BeatCount = int.Parse(status [3]);
		node.Text = status [4];

		nodeDatas.Add (node);
	}

	public float GetTime(int Measure, int Beat)
    {
        float ta = (((Measure - 1.0f) * Bpm) / 15.0f);
        float tb = ((Beat - 1) * 240.0f) / (BeatCount * Bpm);
        return ta + tb;
    }
}