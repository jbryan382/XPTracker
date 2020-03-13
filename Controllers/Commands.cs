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

        [Command("TotalXP")]
        public async Task TotalXPAsync()
        {
            // To be used once Aggregate is accepted by System.Linq
            // var runningTotal = _context.SessionTracker.Aggregate(0, (total, session) => total + session.SocialInteractionXP + session.ExplorationXP + session.CombatXP);

            // Sums up the total SocialInteractionXP, CombatXP, and ExplorationXP
            var runningTotal = _context.SessionTracker.Select(s => s.SocialInteractionXP + s.CombatXP + s.ExplorationXP).ToList().Sum();
            // Compaires the current running total of XP with a Level Model to determine the current level of the group's members.
            var totalLevel = _context.LevelTracker.Where(w => w.MinXP <= runningTotal && w.MaxXP > runningTotal).Select(s => s.Level).ToList().First();

            await ReplyAsync($"The groups total experience is: {runningTotal} points and are Level {totalLevel}.");
        }


        [Command("AddToTotalXP")]
        public async Task AddToTotalXPAsync(string combatXP, string explorationXP, string socialInteractionXP, string sDescription)
        {
            var cXPCheck = Int32.TryParse(combatXP, out int cXP);
            var eXPCheck = Int32.TryParse(explorationXP, out int eXP);
            var sIXPCheck = Int32.TryParse(socialInteractionXP, out int sIXP);
            if (cXPCheck || eXPCheck || sIXPCheck == true)
            {
                var runningTotal = _context.XPTracker.ToList().Last();
                // var newTotal = runningTotal.TotalXP + cXP + eXP + sIXP;
                runningTotal.TotalLevel = _context.LevelTracker.Where(w => w.MinXP <= runningTotal.TotalXP && w.MaxXP > runningTotal.TotalXP).Select(s => s.Level).ToList().First();


                var newTotalXP = new XPTrackerModel
                {
                    TotalXP = runningTotal.TotalXP + cXP + eXP + sIXP,
                    TotalLevel = runningTotal.TotalLevel,
                };

                await _context.XPTracker.AddAsync(newTotalXP);

                var lastSession = _context.SessionTracker.ToList().Last();

                // Creating newSessionTrackerModel to store each sessions individual xp gains

                var newSessionXP = new SessionTrackerModel
                {
                    SessionNumber = lastSession.SessionNumber + 1,
                    SessionDescription = sDescription,
                    CombatXP = cXP,
                    ExplorationXP = eXP,
                    SocialInteractionXP = sIXP,
                    XPId = newTotalXP.Id
                    // XPId = runningTotal.Id
                };

                await _context.SessionTracker.AddAsync(newSessionXP);

                await _context.SaveChangesAsync();

                await ReplyAsync($"The groups total experience is: {newTotalXP.TotalXP} points and are at Level {newTotalXP.TotalLevel}.");

            }

            // await ReplyAsync("Please Enter: !AddToTotalXP YourCombatExperienceInt YourExplorationExperienceInt YourSocialInteractionExperienceInt ''Short Description of the Session''");

        }

        [Command("CurrentSession")]
        public async Task CurrentSessionAsync()
        {
            var lastSession = _context.SessionTracker.ToList().Last();
            await ReplyAsync($"Session {lastSession.SessionNumber + 1}");
        }

        [Command("LastSession")]
        public async Task LastSessionAsync()
        {
            var lastSession = _context.SessionTracker.ToList().Last();
            await ReplyAsync($"Session {lastSession.SessionNumber}: {lastSession.SessionDescription}");
        }



        [Command("ShowSession")]
        public async Task ShowSessionAsync(int sId)
        {
            try
            {
                var selectedSession = _context.SessionTracker.Where(w => w.SessionNumber == sId).First();
                await ReplyAsync($"Session {sId} occurred {selectedSession.SessionDate} - Session Description: {selectedSession.SessionDescription}");
            }
            catch (System.Exception)
            {
                // var maybeId = _context.SessionTracker.Select(s => s.Id).First();
                // Once there are more sessions
                var maybeId = _context.SessionTracker.Select(s => s.SessionNumber).ToList().Last();
                await ReplyAsync($"I can't seem to find that session try: #{maybeId}");
            }
        }

        [Command("ShowSessionFull")]
        public async Task ShowSessionFullAsync(int sId)
        {
            try
            {
                var selectedSession = _context.SessionTracker.Where(w => w.SessionNumber == sId).First();
                await ReplyAsync($"Session {sId} took place on {selectedSession.SessionDate} - You Gained {selectedSession.CombatXP} Combat XP, {selectedSession.ExplorationXP} Exploration XP, {selectedSession.SocialInteractionXP} Social Interacton XP and what transpired: {selectedSession.SessionDescription}");
            }
            catch (System.Exception)
            {
                // var maybeId = _context.SessionTracker.Select(s => s.Id).First();
                // Once there are more sessions
                var maybeId = _context.SessionTracker.Select(s => s.SessionNumber).ToList().Last();
                await ReplyAsync($"I can't seem to find that session try: #{maybeId}");
            }
        }

        [Command("DeleteLastSession")]
        public async Task DeleteLastSessionAsync()
        {
            var lastSession = _context.SessionTracker.ToList().Last();
            var lastTotal = _context.XPTracker.ToList().Last();
            await ReplyAsync($"Session {lastSession.SessionNumber} has been removed");
            _context.SessionTracker.Remove(lastSession);
            _context.XPTracker.Remove(lastTotal);
            await _context.SaveChangesAsync();
        }

        [Command("help")]
        public async Task botHelp()
        {
            await ReplyAsync($"Enter ! and then your chosen command:\n1 - TotalXP \n2 - AddToTotalXP YourCombatExperienceInt YourExplorationExperienceInt YourSocialInteractionExperienceInt ''Short Description of the Session'' \n3 - CurrentSession \n4 - LastSession \n5 - SelectSession TypeTheSessionNumber \n6 - SelectSession TypeTheSessionNumber \n7 - DeleteLastSession");
        }

        [Command("Night")]
        public async Task Night()
        {
            await ReplyAsync($"Goodnight Everyone.");
        }

    }
}