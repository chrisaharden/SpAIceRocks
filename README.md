This is a Unity 6 match 3 PC game for Steam  that I am developing using as much AI as I possibly can.

As of 10/28/2024, I have not started blogging/vlogging about it, but I will soon.
For now, I am using:
Claude APi with Cline VS Code plugin, so I can have Claude right in the editor.
Muse under beta, but I'm not able to speak about it at the moment.
Midjourney under the paid subscription for asset generation.
A python file that uses AI to remove the backgrounds from the Midjourney assets.
Udio.com for the background audio. 
Claude for the game design doc.

I've also explored a couple other solutions, and I go to ChatGPT and Gemini when I want a different take on a problem.
This work is very much a work in progress.  Not all funcationality is nice or complete yet. 
-Chris

Characater Progression:
```mermaid
flowchart TD
    Mine["Match/Mine Gems"]
    Credits["Earn Credits"]
    Tools["Buy Tools"]
    Ships["Buy Spaceships"]
    Planets["Access New Planets"]
    ExoticMinerals["Mine Exotic Minerals"]
    
    Mine -->|"Generates"| Credits
    Credits -->|"Purchase"| Tools
    Credits -->|"Purchase"| Ships
    Tools -->|"Speeds up"| Mine
    Ships -->|"Travel to"| Planets
    Planets -->|"Unlock"| ExoticMinerals
    ExoticMinerals -->|"More valuable"| Credits

    classDef primary fill:#4a9eff,stroke:#666,stroke-width:2px,color:white
    classDef secondary fill:#50C878,stroke:#666,stroke-width:2px,color:white
    
    class Mine,Credits primary
    class Tools,Ships,Planets,ExoticMinerals secondary
```
