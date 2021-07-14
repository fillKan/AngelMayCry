using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitBoxMaster_", menuName = "Scriptable Object/HitBoxSet Master")]
public class HitBoxSetMaster : ScriptableObject
{
    [SerializeField]
    private List<HitBoxSet> _HitBoxList;
    
    public List<HitBoxSet> HitBoxSetList
    {
        get => _HitBoxList.ToList();
    }
    public Dictionary<string, HitBoxSet> GetDictionary()
    {
        var dic = new Dictionary<string, HitBoxSet>();
        for (int i = 0; i < _HitBoxList.Count; i++)
        {
            dic.Add(_HitBoxList[i].ID, _HitBoxList[i]);
        }
        return dic;
    }
}
