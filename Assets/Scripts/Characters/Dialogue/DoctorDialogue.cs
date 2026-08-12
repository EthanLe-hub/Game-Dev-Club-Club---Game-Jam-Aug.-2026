using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DoctorDialogue", menuName = "Scriptable Objects/DoctorDialogue")]
public class DoctorDialogue : DialogueParent
{
    private void OnEnable()
    {

        talkLinesImposter = new[]
        {
            "I have no knowledge about the deep-sea parasite.",
            "What are you talking about right now?"
        };

        // Innocent Doctor's clue templates ({name} = suspect, {deadCharacter} = last night's victim):
        talkClueTemplates = new[]
        {
            "The creature could be possessing anyone right now. {name} was meddling in the comms room, but I hope the creature is not possessing them.",
            "A little disco never hurts anyone, but the way {name} danced in our quarters looked like they were imitating tentacles.",
            "I heard {name} having a nice chit-chat {deadCharacter} right before lights out. I would've gone there to warn them that they could be talking to the deep-sea parasite.",
            "{name} was playing with their food at dinner time. Weird."
        };

        talkLinesFallback = new[]
        {
            "There is a deep-sea parasite onboard our submarine who can possess any crewmate at any time.",
            "What's worse is that they can kill their host and then jump to another crewmate onboard."
        };

        // inspect
        inspectLineImposter = "They say an apple a day keeps the doctor away, but the doctor has not been eating his fruits lately.";
        inspectLineInnocent = "The doctor has a lot of knowledge about medicine AND biology. He knows what's up.";

        // ===== Shared / legacy content below =====

        day0 = new[]
        {
            "Ah, salutations, captain.",
            "I am glad you and the other passengers enjoyed listening to my lecture on the sunken Devil's Shell ship.",
            "If there is any other knowledge you seek, I'd be happy to share."
        };

        deathDialogue = new Dictionary<string, string[]>[]
        {
            new Dictionary<string, string[]>() //Dictionary 0 is when the character is NOT the imposter
            {
                {"NavigationOfficer", new[] {
                    "Oh dear. Leah Jones appears to have been killed by some foreign creature.", 
                    "Precisely, the same creature that stowed away from Devil's Shell.", 
                    "It can jump hosts at any time too.",
                    "But who could it be possessing right now?"
                } },
                {"Cook", new[] {
                    "Andy O'Connor was killed by a foreign creature that stowed away from Devil's Shell.", 
                    "If it has jumped hosts, we could be in big trouble.",
                    "But who could it be possessing right now?"
                } },
                {"Engineer", new[] {
                    "The Devil himself…", 
                    "Not the actual Devil Devil from the underworld.",
                    "I mean the deep-sea parasite that lurks around Devil's Shell.",
                    "It's onboard somewhere, posing as one of us. Maybe even jumping hosts in the middle of the night."
                } },
                {"RichGuy", new[] {
                    "Yikes, I can't believe Richard is dead.", 
                    "Well, whatever killed him is still onboard.",
                    "I advise you find the poser fast before each of us gets killed.",
                    "And beware of the parasite hopping to a new person."
                } },
                {"RichGirl", new[] {
                    "Mary, huh?", 
                    "We gotta find this parasite fast.",
                    "If we don't, it'll kill all of us or change its host.",
                    "Interrogate every person, if you must."
                } }
            },
            new Dictionary<string, string[]>() //Dictionary 1 is when the character IS the imposter
            {
                {"NavigationOfficer", new[] {
                    "Do I know who or what killed Leah?", 
                    "Not really.", 
                    "I don't have the slightest hint."
                } },
                {"Cook", new[] {
                    "Do I know who or what killed Andy?",
                    "Eh.", 
                    "I don't know."
                } },
                {"Engineer", new[] {
                    "What Devil are you talking about?",
                    "There's no correlation between this parasite and the killer. Trust me."
                } },
                {"RichGuy", new[] {
                    "Yikes, I can't believe Richard is dead.", 
                    "Maybe he died from too much money.",
                    "Yeah. That must be it."
                } },
                {"RichGirl", new[] {
                    "Did she die because she was too arrogant?", 
                    "High arrogance like that was likely the cause of her death."
                } }
            }
        };
    }
}
