using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Incount Data", menuName ="Scriptable Object/IncountData")]
public class IncountData : ScriptableObject
{
    [SerializeField]
    public int incountCode = 0;
    public List<MsgData> incountSession;
}
