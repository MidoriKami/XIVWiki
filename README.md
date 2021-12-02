# XIV Wiki
Extremely simple plugin, only adds a single command **/wiki**
  
### Why is this useful?
If you are in a instance, and the boss or a monster does something unexpected, and you want to know what the mechanics are, you can simply **/wiki [instancename]** and it will pull up the wiki article in a web browser.

### You will get better results with more precise search terms

The search returns the first url that even partially matches your search.

For example, "Turn 1" will return The Binding Coil of Bahamut Turn 1, even though "The Second Coil of Bahamut - Turn 1" and "The Final Coil of Bahamut - Turn 1" are valid results.

However, for many instances shorthand does work, especially if the shothand is unique.

For example, prae should always return The Praetorium.

### This tool only includes Dungeons, Raids, and Trials
This tool works by referencing pre-generated json files generated with my [XIV Parsing Tool](https://github.com/MidoriKami/XIVWikiParser).

If you would like to reach out and help me develop a better method for lookup please dont hesitate to post an issue.
