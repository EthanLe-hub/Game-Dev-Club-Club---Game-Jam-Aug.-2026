using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CookDialogue", menuName = "Scriptable Objects/CookDialogue")]
public class CookDialogue : DialogueParent
{
    private void OnEnable()
    {

        talkLinesImposter = new[]
        {
            "Hey Captain! You hungry? You look... tired.",
            "Trust me, tonight's dinner will be to die for! Great great great!"
        };

        // Innocent Cook's clue templates ({name} = suspect, {deadCharacter} = last night's victim):
        talkClueTemplates = new[]
        {
            "I saw {name} sneakin' around the galley last night. Didn't look like a midnight snack run to me.",
            "{name} was actin' mighty strange at dinner, Captain. Barely touched my food.",
            "I heard {name} talkin' with {deadCharacter} right before lights out. Ain't that somethin'?",
            "{name} asked me weird questions about tonight's menu. Real weird."
        };

        talkLinesFallback = new[]
        {
            "Somethin' still feels off on this boat, Captain.",
            "I can't put my finger on it. Keep your eyes open, yeah?"
        };

        // inspect
        inspectLineImposter = "His smile is a little too wide, and it never reaches his eyes.";
        inspectLineInnocent = "He hums to himself while stirring a pot, same as always.";

        // ===== Shared / legacy content below =====

        day0 = new[]
        {
            "Hey, Captain!",
            "Are ya excited to try my farewell dinner tonight before we reach the port?",
            "Trust me! It'll be great! Great great great!"

        };

        deathDialogue = new Dictionary<string, string[]>[]
        {
            new Dictionary<string, string[]>() //Dictionary 0 is when the character is NOT the imposter
            {
                {"NavigationOfficer", new[] {
                    "Leah Jones is dead?!?!", "Nonono, we're gonna get lost down here!"
                } },
                {"Engineer", new[] {
                    "Ramona Lee is dead?!?!", "Crud! Her big appetite surely would've meant she'd eat a lot of my food tonight!", "I'm sad"
                } },
                {"Doctor", new[] {
                    "Walter Black is dead?!?!", "Shoot! I wanted his sophisticated opinion on my food I worked so hard on!"
                } },
                {"RichGuy", new[] {
                    "What happened to Dick (Richard) Moola?", "He was cool…", "I know most people didn't like him, but I considered him a friend.", "I'm sad."
                } },
                {"RichGirl", new[] {
                    "What happened to Mary Moola?", "She was cool…", "I know most people didn't like her, but I considered her a friend.", "I'm sad."
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
                    "Is he okay…?", "I mean uh… not that I mind TOO much.", "I don't feel things as deeply as I used to."
                } },
                {"RichGirl", new[] {
                    "Is she okay…?", "I mean uh… not that I mind TOO much.", "I don't feel things as deeply as I used to."
                } }
            }
        };
    }
}
