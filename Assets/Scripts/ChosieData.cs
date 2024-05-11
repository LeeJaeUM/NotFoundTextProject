using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Choise Data", menuName = "Scriptable Object/ChoiseData")]
public class ChoiseData : ScriptableObject
{
    [SerializeField]
    public int choiseCount = 0;
    public List<MsgData> choiseSelects;
}
