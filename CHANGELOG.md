2020-08-24

* Deck.Date can now be null if this information was not present on the MTGO website, previously it was doing an automated fallback to Tournament.Date.

2020-11-24

* Added Bracket property
* Added Standings property

2021-01-06

* BracketItem.WinningPlayer renamed to BracketItem.Player1
* BracketItem.LosingPlayer renamed to BracketItem.Player2

2021-03-01

* Modified folder structure to include website for the tournament
* The branch "before-folder-restructuring" has the data in the original format if you need it, though it'll no longer be updated

2022-11-10

* magic.wizards.com folder moved to Tournament-Archives

2022-11-15

* channelfireball.com folder moved to Tournament-Archives
* hunterburtonmemorialopen.com folder moved to Tournament-Archives
* labichetournaments folder moved to Tournament-Archives

2022-11-21

* Older data from starcitygames.com folder (not coming from MtgMelee) moved to Tournament-Archives

2022-11-22

* All MtgMelee tournaments updated with Rounds property
* All MtgMelee tournaments updated with more complete Standings property
