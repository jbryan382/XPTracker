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

namespace XPTracker.Models
{
    public class Commands : ModuleBase<SocketCommandContext>
    {

        private DatabaseContext _context;

        public Commands(DatabaseContext context)
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
            var runningTotal = _context.SessionTracker.Select( s => s.SocialInteractionXP + s.ExplorationXP + s.CombatXP).First();

        // An Attempt
            // var runningTotal = _context.SessionTracker.Select( s => s.SocialInteractionXP + s.ExplorationXP + s.CombatXP);
            // runningTotal.Aggregate(0, (total, session) => total + session);
    
        // Similar attempt with out Select
            // runningTotal.Aggregate(0, (total, session) => total + session.SocialInteractionXP + session.ExplorationXP + session.CombatXP);
            // var runningTotal = _context.SessionTracker.Aggregate(0, (total, session) => total + session.SocialInteractionXP + session.ExplorationXP + session.CombatXP);
            runningTotal.ToString();
            await ReplyAsync($"The groups total experience is: {runningTotal} points.");
        }

        [Command("AddToTotalXP")]
        public async Task AddXPTacker(int? combatXP, int? explorationXP, int? socialInteractionXP, string? sDescription)
        {
    // Issues with this try catch I am trying to provide feedback if the user forgets a parameter
            try
            {
        // Probaby best to calculate here and save the total to the db.
            var runningTotal = _context.XPTracker.Select( s => s.TotalXP).First();
            var cXP = _context.SessionTracker.Select(s => s.CombatXP).First();
            var eXP = _context.SessionTracker.Select(s => s.ExplorationXP).First();
            var sIXP = _context.SessionTracker.Select(s => s.SocialInteractionXP).First();

        // Once I have more than one entry... Also might fuck with each sessions 3 xp's due to making them total xp's of each instead of XP per session.
            // var runningTotal = _context.XPTracker.Select( s => s.TotalXP).Last();
            // var cXP = _context.SessionTracker.Select(s => s.CombatXP).Last();
            // var eXP = _context.SessionTracker.Select(s => s.ExplorationXP).Last();
            // var sIXP = _context.SessionTracker.Select(s => s.SocialInteractionXP).Last();


            runningTotal += cXP + eXP + sIXP;
        // Can I add a full new object?
            var newTotalXP = new XPTrackerModel {
                TotalXP = runningTotal,
                // TotalLevel = some shite,
                // Sessions = [
                // SessionDescription = sDescription,
                // CombatXP = cXP,
                // ExplorationXP = eXP,
                // SocialInteractionXP = sIXP,
                // ]
            };

            Console.WriteLine($"{runningTotal}");
            

        // Creating new SessionTrackerModel to store each sessions individual xp gains

            var newSessionXP = new SessionTrackerModel {
                SessionDescription = sDescription,
                CombatXP = cXP,
                ExplorationXP = eXP,
                SocialInteractionXP = sIXP,
            };

            Console.WriteLine($"{newSessionXP}");
            

        // Why doesn't this want to work I am just trying to save changes to my database

            // _context.XPTracker.SaveChangesAsync(newTotalXP);
            // _context.SessionTracker.SaveChangesAsync(newSessionXP);


            await ReplyAsync($"The groups total experience is: {runningTotal} points.");
            }
            catch (System.Exception)
            {
                
                await ReplyAsync("Please Enter: !AddToTotalXP YourCombatExperienceInt YourExplorationExperienceInt YourSocialInteractionExperienceInt ''Short Description of the Session''");
            }
        
        }

        [Command("LastSession")]
        public async Task LastSessionAsync()
        {
        var lastSession = _context.SessionTracker.Select(s => s.SessionDescription).First();
        var lastSessionId = _context.SessionTracker.Select(s => s.Id).First();
    // Once there are more sessions
        // var lastSession = _context.SessionTracker.Select(s => s.SessionDescription).Last();
        // var lastSessionId = _context.SessionTracker.Select(s => s.Id).Last();

        await ReplyAsync($"Session {lastSessionId}: {lastSession}");
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
            var maybeId = _context.SessionTracker.Select(s => s.Id).First();
        // Once there are more sessions
            // var maybeId = _context.SessionTracker.Select(s => s.Id).Last();
            await ReplyAsync($"I can't seem to find that session try: #{maybeId}");
        }
        }
        

        [Command("You can't hide from me")]
        public async Task DemotivatingYay() {
            await ReplyAsync("I am the Admin now!");   
        }
    }
}