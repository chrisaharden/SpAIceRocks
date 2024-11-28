using System;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PlanetConfig
{
    public bool isLocked;
    public int purchasePrice;
    public string info = "";
    public int planetNumber;
    public float ShipPosX;
    public bool ShipFlyOrJump; // True for fly, false for jump
    public Sprite BackgroundImage; 
    public Sprite CharacterSprite;  
    public string GetCurrentCharacterDialogue()
    {
        switch (planetNumber)
        {
            case 0:
                return "Once a thriving hub of intergalactic trade, was ravaged by a catastrophic asteroid impact, leaving behind a barren wasteland. The remnants of its civilization now scavenging for resources in the dark, toxic swamps. Its people cling to life in the shadows of their former glory.";
            case 1:
                return "A planet of contrasts, boasts luxurious cities floating above a toxic atmosphere, while its lower classes toil in the depths of its polluted underbelly. The upper echelons live in opulence, oblivious to the suffering below.";
            case 2:
                return "A world of twisted metal and scavenged technology, is home to a resilient people who've adapted to the harsh environment. They forge a living from the discarded remnants of a bygone era, their ingenuity a testament to their unyielding spirit.";
            case 3:
                return "A gas giant's moon, is a frozen tomb, where ancient ruins whisper secrets of a long-lost civilization. The few who dare to settle here eke out a living amidst the cryogenic storms, searching for answers in the frozen wasteland.";
            case 4:
                return "A terrestrial paradise, was once a haven for refugees. Now, its beauty is tainted by the looming threat of an unstable core, threatening to engulf the planet in flames. Its inhabitants live in the shadow of impending doom.";
            case 5:
                return "A planet of eternal storms, is home to a hardy people who've learned to harness the fury of the elements. Theirs is a world of constant struggle, where survival is a daily battle against the raging tempests.";
            default:
                return "Unknown planet";
        }
    }
}
