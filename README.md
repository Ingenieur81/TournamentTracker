# TournamentTracker
## An application for tracking tournament results programmed with C# as part of a learning activity

Created with the help of the C# tutorial made by Tim Corey in his YouTube video.
<a href="http://www.youtube.com/watch?feature=player_embedded&v=wfWxdh-_k_4
" target="_blank"><img src="http://img.youtube.com/vi/wfWxdh-_k_4/0.jpg" 
alt="C# tutorial by Tim Corey" width="480" height="360" border="10" /></a>

## Purpose
This repository is my introduction to programming in C# using .net and Visual Studio.
And also an introduction to SQL and databases.
Below is the planning stage of the application as described by the tutorial.

## Application requirements:
1. Track games played and their outcome (who won).
2. Multiple competitors play in the tournament.
3. Creates a tournament plan (who plays in what order).
4. Schedule games.
5. A single loss eliminates a player.
6. The last player standing is the winner.

## Questions to get a better understanding of what the full requirements are
1. How many players will the tournament handle? Is it variable?
2. If a tournament has less than the full complement of players, how do we handle it?
3. Should the ordering of who plays each other be random or ordered by input order?
4. Should we schedule the game or are they just played whenever?
5. If the games are scheduled, how does the system know when to schedule games for?
6. If the games are played whenever, can a game from the second round be played before the first round is complete?
7. Does the system need to store the score of some kind or just who won?
8. What type of front-end should this system have (form, webpage, app, etc.)?
9. Where will the data be stored?
10. Will this system handle entry fees, prizes, or other payouts?
11. What type of reporting is needed?
12. Who can fill in the results of the game?
13. Are there varying levels of access?
14. Should this system contact users about upcoming games?
15. Is each player on their own or can teams use this tournament tracker?

## Questions review: simulated stakeholder meetings
1. The application should be able to handle a variable number of players in a tournament.
2. A tournament with less than the perfect number (a multiple of 2) should add in "byes". 
   Basically, certain randomly selected players get to skip the first round and act if they won.
3. The ordering of the tournament should be random.
4. The games should be played in whatever order and whenever the players want to play them.
5. They are not scheduled.
6. No, each round should be fully completed before the next round is displayed.
7. Storing a simple score would be nice. Just a number for each player. 
   That way, the tracker can be felxible enough to handle a checkers tournament or a basketball tournament.
8. The system should be a desktop system for now, but down the road we might want to turn it into an app or website.
9. Ideally, the data should be stored in a Microsoft SQL database but please put in an option to store to a text file instead.
10. Yes, the tournament should have the option of charging an entry fee.
    Prizes should also be an option, where the tournament administator chooses how much money to award a variable number of places.
	The total cash amount should not exceed the income from the tournament. 
	A percentage-based system would also be nice to specify.
11. A simple report specifying the outcome of the games per round as well as a report that specifies who won and how much they won.
    These can be just displayed on a form or they can be emailed to tournament competitors and the administrator.
12. Anyone using the application should be able to fill in the game scores.
13. No, the only method of varied access is if the competitiors are not allowed into the application and instead they do everything via email.
14. Yes, the system should email users that they are due to play in a round as well as who they are scheduled to play.
15. The tournament tracker should be able to handle the addition of other members. 
    All members should be treated as equals in that they all get tournament emails.
	Teams should also be able to name their team.

## Big Picture Design (application boundaries)
**Structure**: Windows Forms Application and Class Library
**Data**: SQL and/or Text File
**Users**: One at a time on one application

**Key concepts**:
- Email
- SQL
- Custom Events
- Error Handling
- Interfaces
- Random Ordering
- Texting (bonus, not part of the requirements)
 
## Mapping the data
**First pass**: Determining names

**Second pass**: Determining datatypes

| First pass Data names | Second pass Datatypes |
| --- | --- |
| **Team** | object |
| TeamMembers | list<Person> |
| TeamName | string |
|   |   |
| **Person** | object |
| FirstName | string |
| LastName | string |
| EmailAddress  | string |
| CellphoneNumber | string *(not a number as such because no mathematical operations are performed)* |
|   |   |
| **Tournament** | object |
| TournamentName | string |
| EntryFee | decimal |
| EnteredTeams | list<Team> |
| Prizes | list<Prize> |
| Rounds | list<list<MatchUp>> |
|   |   |
| **Prize** | object |
| PlaceNumber | int |
| PlaceName | string |
| PrizeAmount | decimal |
| PrizePercentage | double |
|   |   |
| **Matchup** | object |
| Entries | list<MatchupEntry> |
| Winner | Team |
| MatchupRound | int |
|   |   |
| **MatchupEntry** | object |
| TeamCompeting | Team |
| Score | double |
| ParentMatchup | Matchup |

## Database design
The database is designed as simple as possible for this application. Therefore, instead of using composite keys to create unique keys a surrogate key is used for each table to simplify the table design.

The picture shows the design as used in the application.

![alt text][DatabaseDesign]

[DatabaseDesign]: DatabaseLayout.png "Database Table Design"

## Saving Data
Saving data to either a text file or a database or both is a tricky part of building an application. How do you know where to save the data?
How can the code save to both types of storage without being "ugly". Although the ugly way can work, it will require a lot of maintenance and is error prone because it is used in all the places where we talk to our database.
An ugly solution in sudo code:

```cs
model

if (usingSQL == true)
{
  open database connection
  save data
  get back updated model
}

if (usingTextFile == true)
{
  open text file
  generate ID
  save the data
}
```

This requires a system which can be used by every form. This makes it very clear what happens.
The forms do not care where they save the data to, just that it is saved.

So a few questions we need to answer:
1. How do we get the connection information?
2. How do we connect to two different data sources to do the same task?

Answers:
1. Static class (global variables) for data source info, although this takes up memory
2. An interface (i.e. contract) for data source, to call the data source in the same way
