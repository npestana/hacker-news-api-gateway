#Hacker News API
This API will consume the Hacker News API and return the latest 20 best stories.

## Overview
- [1 API Analysis](#1-api-analysis)
    - [1.1 Conclusions](#11-conclusions)
    - [1.2 Decisions](#12-decisions)

## 1 API Analysis
1. How many results the Best Stories URI returns?
    - After doing an http request to the API, it returned 200 results.

2. Do we know how the results are sorted?
    - Checking the [official api][1], I do not have a confirmation. After some API requests, confirmed that best ones are on top of the results. But since they are the best ones, they might change the same items order based on the algorithm that determines the best ones.

3. What type of implementation the HackerNews API has?
    - It is an API with the data stored in the Firebase service. Firebase is a realtime NoSQL database currently developed by Google. At the time of writing, there are no official way to use the realtime API with .NET Core / .NET 5.

4. Can I use the realtime updates offered by [Firebase][2]?
    - I could not find an [official Library][3] for .NET Core / .NET 5. An [answer][4] by an official Firebase developer (appears by the profile) on Stack Overflow, confirms that there is none available.

5. How frequently we are allowed to request data from the API?
    - It is not specified by the API documentation.

### 1.1 Conclusions
- I can not try to find which stories are new to the list and only get those after first request. Order might have changed and I can not trust the last one as starting point for the new.
- Stories have to be refreshed too to get their updates.
- Since we are not getting the latest objects (the best stories take some time to mature until they are marked as best), the realtime is not crucial. Besides that, the exercise specifies that the RESTful API should be used.

### 1.2 Decisions
- I am going to assume an update each minute to get the latest best stories list and each story details.
- The data is going to be cached locally on the implemented solution. Cache will be in two ways, memory and in a MongoDB instance.
    - This will allow the data to be cached and available when the application starts and the HackerNews API is not reachable. On start, data from MongoDB instance goes to the memory.
    - MongoDB is going to be used. Since the data in memory will be JSON objects ready to be returned to the clients, MongoDB native storage in JSON format seems appropriate.
- A service will be implemented to run continuously in the defined TimeSpan and request the data from HackerNews. After a successfully acquire of the information, it will update the memory and MongoDB caches.
- The exercise required .NET Core 2.2 that reached the end of life on 12/23/2019. Since the .NET 5 has been released, I choose this version because it is more recent and has support until [3 months after .NET 6 release (around February 2022)][5].

[1]: https://github.com/HackerNews/API
[2]: https://firebase.google.com/docs/database
[3]: https://firebase.google.com/docs/libraries/
[4]: https://stackoverflow.com/a/10729948
[5]: https://dotnet.microsoft.com/platform/support/policy/dotnet-core
