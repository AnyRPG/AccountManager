# Update database tables

Open Powershell

`dotnet tool install --global dotnet-ef --version 6.*`

Replace the word Initial with a database commit message

`dotnet ef migrations add Initial`

Look at the new file in the Migrations folder in visual studio and check the code that will be run.  If it looks good, then run the update.

`dotnet ef database update`
