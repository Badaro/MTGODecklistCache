# MTGODecklistCache
This repository contains a cache in JSON format of tournaments posted on the [MTGO Website](https://www.mtgo.com/en/mtgo/decklists) as well as a few other sources.

* Tournaments -> Tournament repository, organized by website and date
* Tournaments-Archive -> Tournament repository archive (for sources no longer being updated or maintained), organized by website and date

Each JSON file contains a tournament object, an array of decks, plus standings and bracket information when appropriate. Check [this folder](https://github.com/Badaro/MTGODecklistCache/tree/master/Tools/MTGODecklistCache.Updater.Model) to see exactly what these entities contain.

To use this data, clone the repository and run `git pull` periodically to get the latest tournaments. MTGO data is automatically updated daily around 17:00 UTC.

There's also a folder containg the code I'm using to generate this data:

* Tools -> Tool to update and validate the repository

You do *not* need to run this tool. It's there just as a reference for others who could be interested in building their own parser.