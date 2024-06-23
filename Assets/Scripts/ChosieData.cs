using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Choise Data", menuName = "Scriptable Object/ChoiseData")]
public class ChoiseData : ScriptableObject
{
    [SerializeField]
    public int continuousIndex = 0;        //현재 속한 인카운트
    public int choiseCount = 0;         //선택지의 개수
    public string msg1 = string.Empty;  //중앙 텍스트1
    public string msg2 = string.Empty;  //중앙 텍스트2

    public bool[] isContinues = null; 

    public List<string> choiseList = new List<string>();    //선택지의 개수에 맞게 선택지 지문 추가
}
