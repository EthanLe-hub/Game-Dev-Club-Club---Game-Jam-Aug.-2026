using UnityEngine;
using System.Collections.Generic;

public abstract class DialogueParent : ScriptableObject
{
    public enum Section { Intro, Q1, Q2, Q3 };

    public Dictionary<Section, string[]> day1;

    public string[,] questions;
}
