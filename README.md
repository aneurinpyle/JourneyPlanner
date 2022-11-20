# JourneyPlanner
A simple command line client for a simplified Journey Planner for specific Tube stations in London

## Before you start

To use this application, you will need the register for a developer API key from the [TfL API Developer Portal](https://api-portal.tfl.gov.uk/).

## How to build the code

To build this code, you should do the following:

- Checkout the `main` branch from Git using GitHub Desktop or through Visual Studio 2022.
- Open `JourneyPlannerChallenge` solution file using Visual Studio 2022.
- Once the solution has finished loading, right-click on the `JourneyPlannerClient` project and click `Publish`.
- Choose where to publish the output and click `Publish`.
- This will build and publish the code, creating an executable.

Alternatively, you can also build the application by clicking the `Build` tab then clicking `Build solution`. This will build the application only.

Before running the code, make sure that you open the `appsettings.json` file and add the value for the `TflApiKey`, mentioned in the previous step.

## How to run the output

To run the code, open `cmd.exe` Command Prompt window and change the directory to the location of your published application, in the previous step, e.g. `cd C:\folder`. Enter the executable name and press `Enter`.

You will then see the application run, with a welcome message and instructions. Enter a station name for `start station`, `destination station`. If you wish, enter a `via` station and/or an `excluding` station, but these are optional.

If you enter a station that is not an Underground station, an error will appear, stating that the station entered is not a valid station.

If you enter a station that is not part of this challenge, an error will appear, stating a similar message. 

If the API key or API url is incorrect or invalid, or if the API key used has made too many requests, a warning will appear, asking you to check the values entered in the `appsettings.json` against the [TfL API Developer Portal](https://api-portal.tfl.gov.uk/).

You can also run the application without publishing it using Visual Studio. When the solution file is open, right-click on `JourneyPlannerClient` project and click `Set as startup project` (it may already be set up as the startup project). You should then see the project name in the toolbar at the top, with a green play button. Click this button, and the application will build and run at the same time. This will also allow you to see the Exit codes once the application has run successfully or unsuccessfully.

## How to run the tests

To run the tests written, ensure that the solution is open within Visual Studio.

Then, click the `Test` tab and click `Test Explorer`, or press `Ctrl+E, T` as a keyboard shortcut. This will open a new window with the Test Explorer. To run all tests, click the `Run All Tests In View` button or press `Ctrl+R, V`.

The application will build and then run all tests, with the test results split into `Passed`, `Failed` and `Not run`.

## Assumptions

- The `excluding` option is not part of the Unified API, so the journey options are not exluding that station. So, if a user states a journey from `Embankment` to `Blackfriars` but excluding `Temple`, the API does not know of this part of the query, and will end up returning no results. 
- That the `tube` is the only mode for the journey query. 

## Other relevant information

- Noticed that the main link to the [TfL APIs](https://api.tfl.gov.uk/) refers to using both an `app_id` and an `api_key`. Yet the [TfL API Developer Portal](https://api-portal.tfl.gov.uk/) mentions only using an `app_key`, which is effectively an API key. The documentation on the [TfL APIs](https://api.tfl.gov.uk/) website is likely to be out of date.
- Noticed that the API returns `429` or `TooManyRequests` HTTP Status Code when the AppKey is invalid. Should return HTTP Status Code `401` or `Unauthorized`.
- The `tube` mode query for the journey planner still returns a walking route, even though `walking` is not specfied as a mode. The results for walking are removed.
