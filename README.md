# MTGODecklistCache
This repository contains a cache in JSON format of tournaments posted on the MTGO Website (https://magic.wizards.com/en/content/deck-lists-magic-online-products-game-info)

* Updater -> Tool to update the repository
* Validator -> Tool to check for errors in the repository
* Tournaments -> Tournament repository, organized by the date they were posted on the MTGO website

Each JSON file contains a tournament object, an array of decks, plus standings and bracket information when appropriate. Check the MTGODecklistParser (https://github.com/Badaro/MTGODecklistParser/tree/master/MTGODecklistParser/Model) repository to see exactly what these entities contain.
