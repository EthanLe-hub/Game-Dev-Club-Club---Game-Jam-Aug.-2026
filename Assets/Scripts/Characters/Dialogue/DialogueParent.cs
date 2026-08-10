using UnityEngine;
using System.Collections.Generic;

public abstract class DialogueParent : ScriptableObject
{

    // talking dialogue
    public string[] talkLinesImposter; // When this character IS the imposter.

    // Innocent: generic clue templates. Placeholders are filled in at runtime:
    //   {name}          -> suspect's display name (GameManager.GetClueSuspect: 70% the real imposter, else a random other living character)
    //   {deadCharacter} -> last night's victim's display name
    // Templates containing {deadCharacter} should only be picked on days where someone actually died.
    public string[] talkClueTemplates;

    public string[] talkLinesFallback; // Innocent, but no clue applies (imposter dead/locked up).

    // inspect ui (popup graphic comes from Character.inspectSprite; one line shows depending on imposter status)
    public string inspectLineImposter; // Shown when this character IS the imposter.
    public string inspectLineInnocent; // Shown when this character is NOT the imposter.

    // shared
    public string[] day0; // Intro day lines (played straight, no Inspect/Talk menu on Day 0).

    // reaction to someones death
    public Dictionary<string, string[]>[] deathDialogue;

    // OLD
    public enum Section { Intro, Q1, Q2, Q3 };

    public Dictionary<Section, string[]>[] day1;

    public string[,] questions;
}
