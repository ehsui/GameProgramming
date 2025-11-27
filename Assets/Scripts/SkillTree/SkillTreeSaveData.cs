using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/SaveData")]
public class SkillTreeSaveData : ScriptableObject
{
    public List<NodeData> purchasedNodes = new List<NodeData>();
}
