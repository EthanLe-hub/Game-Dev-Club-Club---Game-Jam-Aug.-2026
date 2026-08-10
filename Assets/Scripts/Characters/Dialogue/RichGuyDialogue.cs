using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RichGuyDialogue", menuName = "Scriptable Objects/RichGuyDialogue")]
public class RichGuyDialogue : DialogueParent
{
    private void OnEnable()
    {

        talkLinesImposter = new[]
        {
            "Get out of my face, Captain.",
            "Do I really look like a deep-sea parasite to you?"
        };

        // Innocent Rich Guy's clue templates ({name} = suspect, {deadCharacter} = last night's victim):
        talkClueTemplates = new[]
        {
            "If you must know, {name} spent too long in the bathroom.",
            "I know nothing about {name} dancing in our quarters.",
            "{name} really thought arguing with {deadCharacter} was a good idea. Who cares that {deadCharacter} is dead, though.",
            "{name} kept begging me for money. I said no."
        };

        talkLinesFallback = new[]
        {
            "This was the worst trip I ever had, Captain.",
            "I could have done something more productive in that time."
        };

        // inspect
        inspectLineImposter = "Rich Guy is usually a jerk, but now he is a lot more of a jerk than usual. Stress or suspicious?";
        inspectLineInnocent = "Rich Guy is a jerk, he usually does not want to respect anyone, not even his wife.";

        // ===== Shared / legacy content below =====

        day0 = new[]
        {
            "Hold on- What?",
            "Is it something important, Captain?",
            "No? Then stop interrupting my call."
        };

        deathDialogue = new Dictionary<string, string[]>[]
        {
            new Dictionary<string, string[]>() //Dictionary 0 is when the character is NOT the imposter
            {
                {"NavigationOfficer", new[] {
                    "The Navigator is dead?", 
                    "This won't put me behind schedule, will it?"
                } },
                {"Cook", new[] {
                    "...", 
                    "Andy was a good fellow."
                } },
                {"Engineer", new[] {
                    "This thing isn't going to implode, is it?", 
                    "...",
                    "Good. I have a meeting next week I can't reschedule again."
                } },
                {"Doctor", new[] {
                    "I'll be honest.", 
                    "I never liked that weirdo.",
                    "But regardless, my condolences."
                } },
                {"RichGirl", new[] {
                    "...", 
                    "[He doesn't seem in the mood to talk.]"
                } }
            },
            new Dictionary<string, string[]>() //Dictionary 1 is when the character IS the imposter
            {
                {"NavigationOfficer", new[] {
                    "Hm.", 
                    "Sure hope you don't get lost, now."
                } },
                {"Cook", new[] {
                    "...",
                    "A good meal is hard to find."
                } },
                {"Engineer", new[] {
                    "You're certified in repairs, aren't you, Captain?"
                } },
                {"Doctor", new[] {
                    "A wack-job, really.", 
                    "He couldn't prevent what is coming, for us all."
                } },
                {"RichGirl", new[] {
                    "Tch-", 
                    "Good riddance.",
                    "I was planning to divorce her anyway."
                } }
            }
        };
    }
}
