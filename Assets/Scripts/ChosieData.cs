using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Choise Data", menuName = "Scriptable Object/ChoiseData")]
public class ChoiseData : ScriptableObject
{
    [SerializeField]
    public int incountIndex = 0;
    public int choiseCount = 0;
    public string msg1 = string.Empty;
    public string msg2 = string.Empty;

    public List<string> choiseList = new List<string>();
}
