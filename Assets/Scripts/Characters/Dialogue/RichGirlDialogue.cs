using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RichGirlDialogue", menuName = "Scriptable Objects/RichGirlDialogue")]
public class RichGirlDialogue : DialogueParent
{
    private void OnEnable()
    {

        talkLinesImposter = new[]
        {
            "Captain... something weird is going on.",
            "I DEFINITELY know it has nothing to do with me though."
        };

        // Innocent Rich Guy's clue templates ({name} = suspect, {deadCharacter} = last night's victim):
        talkClueTemplates = new[]
        {
            "I saw {name} spending too long in the bathroom. I wonder why.",
            "I kind of enjoyed the dancing that {name} was doing in our quarters.",
            "{name} was arguing a lot with {deadCharacter}. Reminds me of my husband and I back then.",
            "{name} kept begging me for money. I said no."
        };

        talkLinesFallback = new[]
        {
            "I sure hope we get out of this submarine in one piece.",
            "It'd be tragic if we didn't."
        };

        // inspect
        inspectLineImposter = "Rich Girl has a little more heart than her husband... right?";
        inspectLineInnocent = "Compared to her husband, the Rich Girl seems to be a little more friendly towards the other crewmates and the Captain.";

        // ===== Shared / legacy content below =====

        day0 = new[]
        {
            "What is it?",
            "Oh, nevermind. Just ask my husband.",
            "Oh- careful not to interrupt his phone calls, though."
        };

        deathDialogue = new Dictionary<string, string[]>[]
        {
            new Dictionary<string, string[]>() //Dictionary 0 is when the character is NOT the imposter
            {
                {"NavigationOfficer", new[] {
                    "Leah was a sweet soul.", 
                    "I just wish she'd had the chance to find her confidence..."
                } },
                {"Cook", new[] {
                    "[sniffle] My Husband and I really loved Andy's work.", 
                    "A true culinary artist, he was.",
                    "Such a kind soul too, despite appearances."
                } },
                {"Engineer", new[] {
                    "We're still going home though, right?", 
                    "Right!?"
                } },
                {"Doctor", new[] {
                    "He always gave me a concerned look...", 
                    "He offered to sit and talk whenever I needed...",
                    "I wish I still could."
                } },
                {"RichGuy", new[] {
                    "...", 
                    "[She doesn't seem in the mood to talk.]"
                } }
            },
            new Dictionary<string, string[]>() //Dictionary 1 is when the character IS the imposter
            {
                {"NavigationOfficer", new[] {
                    "Oh she was ever so sweet...", 
                    "I just wish we could've had more time."
                } },
                {"Cook", new[] {
                    "His last meal was divine...",
                    "Such a shame though."
                } },
                {"Engineer", new[] {
                    "...",
                    "We're never going home now..."
                } },
                {"Doctor", new[] {
                    "I always caught him giving me a weird look...", 
                    "I feel safer now that he's gone."
                } },
                {"RichGuy", new[] {
                    "...", 
                    "I'm fine.",
                    "He never cared about me anyway."
                } }
            }
        };
    }
}
