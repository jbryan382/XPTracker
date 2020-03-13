using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

// Admittedly I stupidly created the CommandController under Models but why doesnt the DBSaveChangesAsync no work goo even
// with and unneeded using statement... I'll move later
using XPTracker.Models;
using Microsoft.Extensions.Options;

namespace XPTracker.Controllers
{
    public class Commands : ModuleBase<SocketCommandContext>
    {

        private DatabaseContext _context;

        public Commands(DatabaseContext context, IOptions<Settings> secrets)
        {
            _context = context;
        }

    // Baseline reply command to ping the bot and reply to the discord server
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync($"Ponged at {DateTime.Now}");
        }

        // Baseline query from the discord server for the bot to respond with required information
        [Command("echo")]
        public async Task EchoAsync(string input){
           await ReplyAsync(input);
        }

        [Command("TotalXP")]
        public async Task GetXP1Tacker()
        {
        // var runningTotal = _context.XPTracker.Select( s => s.TotalXP).First();
            
            // This still only gets the fist and without first I still have a List of XP. I want to aggregate them all.
            // var runningTotal = _context.SessionTracker.Select( s => s.SocialInteractionXP + s.ExplorationXP + s.CombatXP).First();

        // An Attempt
            // var runningTotal = _context.SessionTracker.Select( s => s.SocialInteractionXP + s.ExplorationXP + s.CombatXP);
            // runningTotal.Aggregate(0, (total, session) => total + session);
    
        // Similar attempt with out Select
            // runningTotal.Aggregate(0, (total, session) => total + session.SocialInteractionXP + session.ExplorationXP + session.CombatXP);
            // var runningTotal = _context.SessionTracker.Aggregate(0, (total, session) => total + session.SocialInteractionXP + session.ExplorationXP + session.CombatXP);
            var runningTotal = _context.SessionTracker.Select(s => s.SocialInteractionXP + s.CombatXP + s.ExplorationXP).ToList().Sum();
            var totalLevel = _context.LevelTracker.Where(w => w.MinXP <= runningTotal && w.MaxXP > runningTotal).Select(s => s.Level).ToList().First();
            // I also want to add an XP to lvl comparison to output the groups amount of xp.
            // Tempted to use a switch statement to take into the account of XP points to level or maybe db model maybe idk

            await ReplyAsync($"The groups total experience is: {runningTotal} points and are Level {totalLevel}.");
        }
        

        [Command("AddToTotalXP")]
        public async Task AddXPTacker(string combatXP, string explorationXP, string socialInteractionXP, string sDescription)
        {
            var cXPCheck = Int32.TryParse(combatXP, out int cXP);
            var eXPCheck = Int32.TryParse(explorationXP, out int eXP);
            var sIXPCheck = Int32.TryParse(socialInteractionXP, out int sIXP);
            if ( cXPCheck || eXPCheck || sIXPCheck == true)
            {
             var runningTotal = _context.XPTracker.First();
            // var runningTotal = _context.XPTracker.Last();


            runningTotal.TotalXP += cXP + eXP + sIXP;

            runningTotal.TotalLevel = _context.LevelTracker.Where(w => w.MinXP <= runningTotal.TotalXP && w.MaxXP > runningTotal.TotalXP).Select(s => s.Level).ToList().First();

            // if (runningTotal.TotalXP >= 0 && runningTotal.TotalXP < 300)
            // {
            //     runningTotal.TotalLevel = 1;
            // } else if (runningTotal.TotalXP >= 300 && runningTotal.TotalXP < 900)
            // {
            //     runningTotal.TotalLevel = 2;
            // } else if (runningTotal.TotalXP >= 900 && runningTotal.TotalXP < 2700)
            // {
            //     runningTotal.TotalLevel = 3;
            // } else if (runningTotal.TotalXP >= 2700 && runningTotal.TotalXP < 6500)
            // {
            //     runningTotal.TotalLevel = 4;
            // } else if (runningTotal.TotalXP >= 6500 && runningTotal.TotalXP < 14000)
            // {
            //     runningTotal.TotalLevel = 5;
            // } else if (runningTotal.TotalXP >= 14000 && runningTotal.TotalXP < 23000)
            // {
            //     runningTotal.TotalLevel = 6;
            // } else if (runningTotal.TotalXP >= 23000 && runningTotal.TotalXP < 34000)
            // {
            //     runningTotal.TotalLevel = 7;
            // } else if (runningTotal.TotalXP >= 34000 && runningTotal.TotalXP < 48000)
            // {
            //     runningTotal.TotalLevel = 8;
            // } else if (runningTotal.TotalXP >= 48000 && runningTotal.TotalXP < 64000)
            // {
            //     runningTotal.TotalLevel = 9;
            // } else if (runningTotal.TotalXP >= 64000 && runningTotal.TotalXP < 85000)
            // {
            //     runningTotal.TotalLevel = 10;
            // } else if (runningTotal.TotalXP >= 85000 && runningTotal.TotalXP < 100000)
            // {
            //     runningTotal.TotalLevel = 11;
            // } else if (runningTotal.TotalXP >= 100000 && runningTotal.TotalXP < 120000)
            // {
            //     runningTotal.TotalLevel = 12;
            // } else if (runningTotal.TotalXP >= 120000 && runningTotal.TotalXP < 140000)
            // {
            //     runningTotal.TotalLevel = 13;
            // } else if (runningTotal.TotalXP >= 140000 && runningTotal.TotalXP < 165000)
            // {
            //     runningTotal.TotalLevel = 14;
            // } else if (runningTotal.TotalXP >= 165000 && runningTotal.TotalXP < 195000)
            // {
            //     runningTotal.TotalLevel = 15;
            // } else if (runningTotal.TotalXP >= 195000 && runningTotal.TotalXP < 225000)
            // {
            //     runningTotal.TotalLevel = 16;
            // } else if (runningTotal.TotalXP >= 225000 && runningTotal.TotalXP < 265000)
            // {
            //     runningTotal.TotalLevel = 17;
            // } else if (runningTotal.TotalXP >= 265000 && runningTotal.TotalXP < 305000)
            // {
            //     runningTotal.TotalLevel = 18;
            // } else if (runningTotal.TotalXP >= 305000 && runningTotal.TotalXP < 355000)
            // {
            //     runningTotal.TotalLevel = 19;
            // } else if (runningTotal.TotalXP >= 355000)
            // {
            //     runningTotal.TotalLevel = 20;
            // }

        // Creating newSessionTrackerModel to store each sessions individual xp gains

            var newSessionXP = new SessionTrackerModel {
                SessionDescription = sDescription,
                CombatXP = cXP,
                ExplorationXP = eXP,
                SocialInteractionXP = sIXP,
                XPId = runningTotal.Id
            };

            _context.SessionTracker.Add(newSessionXP);

            await _context.SaveChangesAsync();

            await ReplyAsync($"The groups total experience is: {runningTotal.TotalXP} points and are at Level {runningTotal.TotalLevel}.");   

            }   

            // await ReplyAsync("Please Enter: !AddToTotalXP YourCombatExperienceInt YourExplorationExperienceInt YourSocialInteractionExperienceInt ''Short Description of the Session''");
        
        }

        [Command("LastSession")]
        public async Task LastSessionAsync()
        {
        var lastSession = _context.SessionTracker.ToList().Last();
        await ReplyAsync($"Session {lastSession.Id}: {lastSession.SessionDescription}");
        }
// Might not implement due to the nature of the primary keys in Postgres and if a session is deleted the SessionId's would no longer
// be associated with the proper session number. Needs fixing possibly working with GUID.
        [Command("DeleteLastSession")]
        public async Task DeleteLastSessionAsync()
        {
        var lastSession = _context.SessionTracker.ToList().Last();
        await ReplyAsync($"Session {lastSession.Id} has been removed");
        _context.SessionTracker.Remove(lastSession);
        await _context.SaveChangesAsync();
        }
        

        [Command("ShowSession")]
        public async Task ShowSessionAsync(int sId)
        {
        try
        {
            var selectedSession = _context.SessionTracker.Where(w => w.Id == sId).Select(s => s.SessionDescription).First();
            await ReplyAsync($"Session {sId}: {selectedSession}");
        }
        catch (System.Exception)
        {
            // var maybeId = _context.SessionTracker.Select(s => s.Id).First();
        // Once there are more sessions
            var maybeId = _context.SessionTracker.Select(s => s.Id).ToList().Last();
            await ReplyAsync($"I can't seem to find that session try: #{maybeId}");
        }
        }

        [Command("help")]
        public async Task botHelp()
        {
        await ReplyAsync($"Enter ! and then your chosen command:\n1 - TotalXP \n2 - AddToTotalXP YourCombatExperienceInt YourExplorationExperienceInt YourSocialInteractionExperienceInt ''Short Description of the Session'' \n3 - LastSession \n4 - SelectSession TypeTheSessionNumber\n5 - *Please Don't Use* DeleteLastSession");
        }
        
        [Command("You can't hide from me")]
        public async Task DemotivatingYay() {
            await ReplyAsync("I am the Admin now!");   
        }

        [Command("Night")]
        public async Task Night()
        {
        await ReplyAsync($"Goodnight Everyone.");
        }


        
    }
}