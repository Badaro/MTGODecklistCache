# MTGODecklistCache
This repository contains a cache in JSON format of tournaments posted on the MTGO Website (https://magic.wizards.com/en/content/deck-lists-magic-online-products-game-info) as well as a few other sources.

* Tools -> Tool to update and validate the repository
* Tournaments -> Tournament repository, organized by website and date

Each JSON file contains a tournament object, an array of decks, plus standings and bracket information when appropriate. Check the Tools/MTGODecklistCache.Updater.Model to see exactly what these entities contain.
