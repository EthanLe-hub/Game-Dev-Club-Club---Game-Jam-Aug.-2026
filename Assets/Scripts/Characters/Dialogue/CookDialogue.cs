using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CookDialogue", menuName = "Scriptable Objects/CookDialogue")]
public class CookDialogue : DialogueParent
{
    private void OnEnable()
    {
        day0 = new[]
        {
            "Hey, Captain!",
            "Are ya excited to try my farewell dinner tonight before we reach the port?",
            "Trust me! It’ll be great! Great great great!"

        };

        day1 = new Dictionary<Section, string[]>[]
        {
            new Dictionary<Section, string[]>() //Dictionary 0 is when the character is NOT the imposter
            {
                {Section.Intro, new[] {
                    "What's up?"
                } },
                {Section.Q1, new[] {""} }, //Q1 is a placeholder. It will automatically be updated within code
                {Section.Q2, new[] {
                    "This is a singlestage answer. What else do you want me to say?"
                } },
                {Section.Q3, new[] {
                    "This is a multistage answer.", "Wait what was I saying?"
                } }
            },
            new Dictionary<Section, string[]>() //Dictionary 1 is when the character IS the imposter
            {
                {Section.Intro, new[] {
                    "What's up?"
                } },
                {Section.Q1, new[] {""} }, //Q1 is a placeholder. It will automatically be updated within code
                {Section.Q2, new[] {
                    "This is a singlestage IMPOSTER answer. What else do you want me to say?"
                } },
                {Section.Q3, new[] {
                    "This is a multistage IMPOSTER answer.", "Wait what was I saying? It wasn't suspicious, was it?"
                } }
            }
        };

        questions = new[,]
        {
            { //Day1
                "Death Question", //Q1 is a placeholder. It will automatically be updated within code
                "Test Question 2",
                "Test Question 3"
            }
        };

        deathDialogue = new Dictionary<string, string[]>[]
        {
            new Dictionary<string, string[]>() //Dictionary 0 is when the character is NOT the imposter
            {
                {"NavigationOfficer", new[] {
                    "Navigation Officer is dead?!?!", "Nonono, we’re gonna get lost down here!"
                } },
                {"Engineer", new[] {
                    "Engineer is dead?!?!", "Crud! Her big appetite surely would’ve meant she’d eat a lot of my food tonight!", "I’m sad"
                } },
                {"Doctor", new[] {
                    "Doctor is dead?!?!", "Shoot! I wanted his sophisticated opinion on my food I worked so hard on!"
                } },
                {"RichGuy", new[] {
                    "What happened to the Rich Guy?", "He was cool…", "I know most people didn’t like him, but I considered him a friend.", "I’m sad."
                } },
                {"RichGirl", new[] {
                    "What happened to the Rich Girl?", "She was cool…", "I know most people didn’t like her, but I considered her a friend.", "I’m sad."
                } }
            },
            new Dictionary<string, string[]>() //Dictionary 1 is when the character IS the imposter
            {
                {"NavigationOfficer", new[] {
                    "Are we gonna get lost?", "…"
                } },
                {"Engineer", new[] {
                    "Is our submarine going to break down?", "…"
                } },
                {"Doctor", new[] {
                    "Ah ah ah ah, staying alive, staying alive~~", "…"
                } },
                {"RichGuy", new[] {
                    "Is he okay…?", "I mean uh… not that I mind TOO much.", "I don’t feel things as deeply as I used to."
                } },
                {"RichGirl", new[] {
                    "Is she okay…?", "I mean uh… not that I mind TOO much.", "I don’t feel things as deeply as I used to."
                } }
            }
        };
    }
}
