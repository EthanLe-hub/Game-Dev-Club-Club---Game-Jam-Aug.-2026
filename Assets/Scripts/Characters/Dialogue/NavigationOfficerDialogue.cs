using UnityEngine; 
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NavigationOfficerDialogue", menuName = "Scriptable Objects/NavigationOfficerDialogue")]
public class NavigationOfficerDialogue : DialogueParent
{
    private void OnEnable()
    {

        talkLinesImposter = new[]
        {
            "Do you need me to navigate you through something?",
            "I may have this helmet on, but I can certainly navigate us to a giant rock."
        };

        // Innocent Navigation Officer's clue templates ({name} = suspect, {deadCharacter} = last night's victim):
        talkClueTemplates = new[]
        {
            "{name} was wandering the halls late at night. I think that person needs some proper navigation.",
            "{name} started to break out in dance in our quarters. I wasn't able to see their dance, though. And no, it's NOT because of my helmet.",
            "I heard {name} having a nice chit-chat {deadCharacter} right before lights out. Well... I actually couldn't see the lights going out.",
            "{name} was playing with their food at dinner time. Probably stabbing their meatballs or something?"
        };

        talkLinesFallback = new[]
        {
            "Based on my clearly amazing vision with my helmet, I see that you are just as puzzled as I am about something onboard, Captain.",
            "I hope we can find out what it is."
        };

        // inspect
        inspectLineImposter = "It's pretty hard to see her facial expressions. Not sure whether to trust her or not.";
        inspectLineInnocent = "Her helmet seems like it would block most of her vision, but she claims she can see everything just fine. Hopefully that is true.";

        // ===== Shared / legacy content below =====

        day0 = new[]
        {
            "I'm the Navigation Officer. I navigate through things.",
            "Don't worry! I can navigate just fine even with this helmet on 24/7.",
            "I was able to lead you all to and back from the sunken Devil's Shell ship in the deep trenches, did I not?"

        };

        deathDialogue = new Dictionary<string, string[]>[]
        {
            new Dictionary<string, string[]>() //Dictionary 0 is when the character is NOT the imposter
            {
                {"Cook", new[] {
                    "Uhh… who just died again?", 
                    "No, my helmet is certainly and most DEFINITELY not blocking my view.", 
                    "Oh shoot, the COOK!", 
                    "There goes the one person who can make us food… really REALLY good food."
                } },
                {"Engineer", new[] {
                    "Who died?", 
                    "I can't see- oh, it's the yellow gal.", 
                    "The ENGINEER?",
                    "Uhh… pray that I don't accidentally navigate us to a rock and damage the sub."
                } },
                {"Doctor", new[] {
                    "Ooh… the Doctor is dead, huh?", 
                    "He was a man of science… but I think you could tell based on his appearance already.",
                    "Me? I had to gather that info from the way he talked, most of the time."
                } },
                {"RichGuy", new[] {
                    "Richard is gone. He even went out with his shades still on.", 
                    "How does he see with those things on when it's already dark enough in here?"
                } },
                {"RichGirl", new[] {
                    "Should I be happy about Mary being gone?", 
                    "I mean, regardless, you cannot really tell if you tried looking at my face anyway."
                } }
            },
            new Dictionary<string, string[]>() //Dictionary 1 is when the character IS the imposter
            {
                {"Cook", new[] {
                    "The Cook is dead…", 
                    "We're all cooked…", 
                    "Who's next on the menu…?"
                } },
                {"Engineer", new[] {
                    "Wow, Engineer gal is dead.", 
                    "She wasn't that useful anyway. She only fixed our gears like once, at the very start of this adventure.", 
                    "Was she THAT useful?"
                } },
                {"Doctor", new[] {
                    "He was a good man, very good.", 
                    "Guess he couldn't science his way out of that one.",
                    "Well! He already yapped a bunch of info about the deep-sea parasite.",
                    "We don't need anything more."
                } },
                {"RichGuy", new[] {
                    "His shades should've gotten busted before he was killed.", 
                    "Then he would've witnessed the TRUE horror of his demise."
                } },
                {"RichGirl", new[] {
                    "Mary is gone. Who would've thought?", 
                    "A clumsy richie like that was bound to get killed anyway."
                } }
            }
        };
    }
}
