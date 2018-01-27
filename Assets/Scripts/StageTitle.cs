using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageTitle : MonoBehaviour
{
    public Text stageCountText;
    public Text titleText;

    public void Setup(int stageCount, string title)
    {
        stageCountText.text = string.Format("ステージ {0}", stageCount);
        titleText.text = title;
    }
}
