using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EngineerDialogue", menuName = "Scriptable Objects/EngineerDialogue")]
public class EngineerDialogue : DialogueParent
{
    private void OnEnable()
    {

        talkLinesImposter = new[]
        {
            "I can't fix any of your problems.",
            "I can only fix your gears, but even then, I really do not want to right now."
        };

        // Innocent Engineer's clue templates ({name} = suspect, {deadCharacter} = last night's victim):
        talkClueTemplates = new[]
        {
            "Yo {name} was up to something funky. Hope they won't be tearing down this sub.",
            "{name} started to break out in dance in our quarters. It was really weird, but perhaps I should've danced with them just for the vibes.",
            "I heard {name} having a nice chit-chat {deadCharacter} right before lights out. Not sure what they talked about, though.",
            "{name} was playing with their food at dinner time. I would've gladly eaten their leftovers if they didn't want it!"
        };

        talkLinesFallback = new[]
        {
            "I know I wasn't that useful for most of this journey, but if anything goes wrong, I'll do my best to help you.",
            "I promise you that, Captain."
        };

        // inspect
        inspectLineImposter = "A crewmate who has not done much does raise some suspicion, especially with her unhinged comments about feasting lately.";
        inspectLineInnocent = "She has tried her best to be useful, that is all that matters.";

        // ===== Shared / legacy content below =====

        day0 = new[]
        {
            "Yo. What's up, Cap?",
            "Thanks for the cool tour to the sunken Devil's Shell ship.",
            "Sorry I couldn't do more. I feel pretty bad about needing to fix your sub's gears only once this whole trip.",
            "Felt pretty useless, but it is what it is."
        };

        deathDialogue = new Dictionary<string, string[]>[]
        {
            new Dictionary<string, string[]>() //Dictionary 0 is when the character is NOT the imposter
            {
                {"NavigationOfficer", new[] {
                    "Oh yo yo yo what happened here?", 
                    "Nav Officer is dead?", 
                    "Is this where I have to step up and finally do something useful?"
                } },
                {"Cook", new[] {
                    "Nooo bro, the cook!", 
                    "I was looking forward to another all-you-can-eat buffet tonight if we are to be stuck onboard!", 
                    "Wait, did he already finish cooking it?"
                } },
                {"Doctor", new[] {
                    "Ooh… Walter was the only one who was ever concerned about my health.", 
                    "Does this mean… I can freely eat without him breathing down my back?"
                } },
                {"RichGuy", new[] {
                    "Richard barely worked his way up to get where he is.", 
                    "Guess this is a lesson for him that all glamour and no brain can get you killed."
                } },
                {"RichGirl", new[] {
                    "Mary barely worked her way up to get where she is.", 
                    "Guess this is a lesson for her that all glamour and no brain can get you killed."
                } }
            },
            new Dictionary<string, string[]>() //Dictionary 1 is when the character IS the imposter
            {
                {"NavigationOfficer", new[] {
                    "The navigation lassie is dead.", 
                    "What a shame.", 
                    "Don't ask me to take her place. Don't."
                } },
                {"Cook", new[] {
                    "What a twisted turn of events.", 
                    "Oh well, maybe I will feast on him instead."
                } },
                {"Doctor", new[] {
                    "I can feast… I can feast… I CAN FEAST ON ANYTHING! No doc to stop me!"
                } },
                {"RichGuy", new[] {
                    "What can I say?", 
                    "Richard was a sorry excuse of a person anyway."
                } },
                {"RichGirl", new[] {
                    "What can I say?", 
                    "Mary was a sorry excuse of a person anyway."
                } }
            }
        };
    }
}
