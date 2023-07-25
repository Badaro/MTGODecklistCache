# MTGODecklistCache
This repository contains a cache in JSON format of tournaments posted on the [MTGO Website](https://www.mtgo.com/en/mtgo/decklists) as well as a few other sources.

* Tournaments -> Tournament repository, organized by website and date
* Tournaments-Archive -> Tournament repository archive (for sources no longer being updated or maintained), organized by website and date

Each JSON file contains a tournament object, an array of decks, plus standings and bracket information when appropriate. Check [this folder](https://github.com/Badaro/MTGODecklistCache/tree/master/MTGODecklistCache.Tools/MTGODecklistCache.Updater.Model) to see exactly what these entities contain.

To use this data, clone the repository and run `git pull` periodically to get the latest tournaments. MTGO data is automatically updated daily around 17:00 UTC, Manatraders data at the same time on the last day of the month. Melee.gg data is manually selected and usually updated on Sundays and Mondays.

Tools used to generate this data are available [here](https://github.com/Badaro/MTGODecklistCache.Tools).