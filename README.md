ChemStation Remote Dashboard
============================

A client-server system that is currently allowing laboratory technicians at CSU Fresno to monitor their HPLC remotely.

ChemStationClientService
------------------------
This contains the C#.NET Desktop Service application that extracts the ChemStation status by acting as a client and connecting to ChemStation via DDE. It then sends the status to the web server.

Server
------
This contains the web application that manages the ChemStation statuses. The server-side is built primarily as a RESTful API that sends and receives JSON using Flask and Python. The client-side browser portion is built with AngularJS, d3.js and Bootstrap. The browser app lets a lab tech monitor the current status of ChemStation, as well as to explore past statuses and view some status statistics over the last week.