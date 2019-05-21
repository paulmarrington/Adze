Advertisements are the life-blood for many games. It is difficult to attract players if they have to pay upfront. There are many different formats with the three most common being:

* Banner - displays above or below game-play
* Interstitial - displays between game segments
* Reward - Player chooses to see an ad for gain

With an extensive range of networks to choose from:

* AdMob - Google-backed and the most popular
* Chartboost - specifically for games
* Appodeal - Aggregator of many of the others
* AdColony - Yet another contender for best network
* UnityAds - Simplicity to use, aimed at gamers
* and many more

Now you could use an aggregator and manually select the network. May networks provide aggregation. There are some problems with this:

* Dependency - If the aggregator is offline or shut down you will not have any advertisements served.
* Size - An aggregator loads libraries for all the networks it supports. It is a problem for Android with the 64k limit on entry points.
* Versioning - You have to rely on the version of a network API supplied by the aggregator.
* Complexity - The aggregator system to set up priorities on networks can be hard to program and painful to maintain.
* Reliability - Networks cannot always serve an advertisement. It is useful to have more than one source to cover this situation.
* Platforms - Different providers support different platforms, with Android and iOS being the most popular.

Adze provides a decoupled interface to the systems of your choice. If you have more than one source, you can choose different strategies for using them.

Adze also provides a rewarded video prefab that once reskinned can ease the process for you.

[Full Documentation Here](https://paulmarrington.github.io/Unity-Documentation/Adze/)
