![Alt text](DesignsAndPipeline/SpA.I.ceRocks-Splash.png?raw=true "Splash")

This is a Unity 6 match 3 PC game for Steam  that I am developing using as much AI as I possibly can.
You can download the free game from Steam: https://store.steampowered.com/app/3412990/SpAIceRocks/

I'm building a video series on the effort: https://www.youtube.com/watch?v=xodknbWptZo&list=PLFUWlfd8IyFU_aLpT_mx_n4ktZFoeqfrJ

I am using:
- Game logic generated from Cline (VSCode plugin) + Claude AI  & Unity Muse
- Muse under beta, but I'm not able to speak about it at the moment.
- Graphics from Midjourney (paid subscription) processed with Rembg
- Music from Udio
- Sound effects from https://www.optimizerai.xyz/ and https://freesound.org/ (non-AI)
- Font suggestions from a mixture of experts with ChatGPT's and Claude's suggestions winning out so far.
- Special effects from Unity Particle Pack (non-AI)

Thank you to all the awesome providers above!

I've also explored a couple other solutions, and I go to ChatGPT and Gemini when I want a different take on a problem.
This work is very much a work in progress.  Not all funcationality is nice or complete yet. 
-Chris

Character Progression:
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
