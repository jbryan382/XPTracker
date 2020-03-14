# D&D 5e XP Tracker for Discord

This project is ready for beta testing during my groups session of DND.

This Discord Bot allows a D&D DM to track the experience and level of the party.
The tracker accounts for groups that award experience equally amongst the group thus
leveling up in unison.

The Commands accepted by the bot can be viewed by typing: !help

Displaying :

Enter ! and then your chosen command:
1 - TotalXP
2 - AddToTotalXP YourCombatExperienceInt YourExplorationExperienceInt YourSocialInteractionExperienceInt ''Short Description of the Session''
3 - CurrentSession
4 - LastSession
5 - SelectSession TypeTheSessionNumber
6 - SelectSessionFull TypeTheSessionNumber
7 - DeleteLastSession

This Project Utilizes:

- CORS Enabled
- Swagger
- Postgres & EF Core
- Ready for Docker Deployment
- Discord.Net

After Beta Testing I will deploy to heroku:

- [ ] create a web app on heroku, make sure to have the CLI downloaded, installed, logged in and be logged into the container via heroku.
- [ ] Update your `dockerfile` to use your `*.dll` file instead of `dotnet-sdg-template.dll`
- [ ] Update the deploy script:
  - [ ] change `sdg-template-image` to `your-project-name-image`
  - [ ] change `heroku-web-app` to your web app name on heroku
