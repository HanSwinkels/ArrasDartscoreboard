# Arras - Dartscoreboard
A dartscoreboard application for Android, Windows, iOS and as PWA, sharing a single code base in order to improve maintainability. The solution is setup using a Onion architecture, consisting of a Business, Data, Common and the Front-end layer. 

### Common layer
This layer can be accessed by every project in the solution. It contains enums and classes that are used throughout the application.

### Data layer
This layer is only accessible through the Business layer. It has the responsibility to communicate with the database via the EntityFramework, both for storing as for retrieving data.

### Business layer
This layer communicates with the Data layer and is accessible by the front-end (all the seperate applications). This in order to abstract from the layers underneath and simplify the logic for the front-end layer.
