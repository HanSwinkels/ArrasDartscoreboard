# Arras - Dartscoreboard
A dartscoreboard application for Android, Windows, iOS and as PWA, sharing a single code base in order to improve maintainability. The solution is setup using a Onion architecture, consisting of a Business, Data, Common and the Front-end layer. 

### Common layer
This layer can be accessed by every project in the solution. It contains enums and classes that are used throughout the application.

### Data layer
This layer is only accessible through the Business layer. It has the responsibility to communicate with the database via the EntityFramework, both for storing as for retrieving data.

### Business layer
This layer communicates with the Data layer and is accessible by the front-end (all the seperate applications). This in order to abstract from the layers underneath and simplify the logic for the front-end layer.

### Colorscheme
These colors are used throughout the different application in order to keep a consistent color palette

| Color         | Colorcode |
| ------------- | ------------- |
| ColorPrimary  | #F44336  |
| ColorPrimaryDark  | #D32F2F  |
| ColorAccent  | #448AFF  |
| ColorBackground  | #F1F5F8  |
| ColorWhite  | #FFFFFF  |
| ColorText  | #575757  |
| ColorSecondaryText  | #BDBDBD  |
| ColorOverlay  | #66000000  |
| ColorBottomBar  | #F3F3F3  |
