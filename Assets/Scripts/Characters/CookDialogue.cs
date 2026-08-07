using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CookDialogue", menuName = "Scriptable Objects/CookDialogue")]
public class CookDialogue : DialogueParent
{
    private void OnEnable()
    {
        day1 = new Dictionary<Section, string[]>()
        {
            {Section.Intro, new[] {
                "What's up?"
            } },
            {Section.Q1, new[] {
                "This is a multistage answer.", "Wait what was I saying?"
            } },
            {Section.Q2, new[] {
                "This is a singlestage answer. What else do you want me to say?"
            } },
            {Section.Q3, new[] {
                "This is a singlestage answer. What else do you want me to say?"
            } }
        };

        questions = new[,]
        {
            { //Day1
                "Test Question 1",
                "Test Question 2",
                "Test Question 3"
            }
        };
    }
}
