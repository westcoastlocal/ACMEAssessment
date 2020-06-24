**<u>ACME Assessment Notes</u>**

The application is a .NET Core 3.1 Worker Service.

It can be converted to a Windows Service with a few lines of code

The database is an SQL Azure Database and has already been created and always access to all IP addresses

I used a the Dapper Micro ORM as the database is small (1 Table)

SendGrid is used to send the emails.

The application currently sends both Birthday and Work Anniversary messages

The startup project is the Assessment.Application.CorrespondenceService

Please change the ToEmailAddress value in the appsettings.json file in the above project.

The PollIntervalMinutes setting is how often the service checks for correspondence and is in minutes.





