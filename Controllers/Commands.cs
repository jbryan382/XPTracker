using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using XPTracker.Models;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

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



        [Command("S")]
        public async Task ShowSessionAsyncSpecial(int sId)
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
        // Change back to spell after testing

        [Command("Spell")]
        public async Task DescribeSpellAsync(string spell)
        {
            // Create HttpClient request out to ex: http://www.dnd5eapi.co/api/spells/acid-arrow
            var sanitizedSpell = spell.ToLower();
            var url = $"http://www.dnd5eapi.co/api/spells/{sanitizedSpell}";
            var client = new HttpClient();
            var body = await client.GetAsync(url);
            var jsonDoc = JsonDocument.Parse(body.Content.ReadAsStringAsync().Result);
            var root = jsonDoc.RootElement;

            // Works with most spells, but a few are giving me an issue
            // I need to determine if its the 2000 character limit we ran into prior.

            // For each property pulled from root we need to confirm if the property exists
            var descExists = root.TryGetProperty("desc", out JsonElement desc);
            var fullDescList = new List<string>();
            for (var i = 0; i < desc.GetArrayLength(); i++)
            {
                fullDescList.Add(desc[i].GetString());
            }
            var fullDesc = string.Join(", ", fullDescList).Replace('-', ' ').Replace(".,", ".");

            var descHLExists = root.TryGetProperty("higher_level", out JsonElement descHL);
            var fullHLList = new List<string>();
            var fullHL = "";

            if (descHLExists == true)
            {
                for (var i = 0; i < descHL.GetArrayLength(); i++)
                {
                    fullHLList.Add(descHL[i].GetString());
                }
                fullHL = string.Join(", ", fullHLList).Replace('-', ' ').Replace(".,", ".");
            }
            else
            {
                fullHL = "Not Found";
            }

            var rangeExists = root.TryGetProperty("range", out JsonElement range);

            var compExists = root.TryGetProperty("components", out JsonElement comp);

            var fullComponentsList = new List<string>();
            for (var i = 0; i < comp.GetArrayLength(); i++)
            {
                fullComponentsList.Add(comp[i].GetString());
            }
            var fullComp = string.Join(", ", fullComponentsList).Replace('-', ' ').Replace(".,", ".");

            var materialExists = root.TryGetProperty("material", out JsonElement material);

            var ritualExists = root.TryGetProperty("ritual", out JsonElement ritual);

            var durationExists = root.TryGetProperty("duration", out JsonElement duration);

            var concentrationExists = root.TryGetProperty("concentration", out JsonElement concentration);

            var casting_timeExists = root.TryGetProperty("casting_time", out JsonElement casting_time);

            await ReplyAsync($"Description: {fullDesc}\nHigher Level: {fullHL}\nRange: {range}\nComponents: {fullComp}\nMaterials: {material}\nRitual: {ritual}\nDuration: {duration}\nConcentration: {concentration}\nCasting Time: {casting_time}");
        }

        [Command("Monster")]
        public async Task DescribeMonsterAsync(string monster)
        {
            // Create curl request out to ex: http://www.dnd5eapi.co/api/monsters/owl
            var sanitizedMonster = monster.ToLower();
            var url = $"http://www.dnd5eapi.co/api/monsters/{sanitizedMonster}";
            var client = new HttpClient();
            var body = await client.GetAsync(url);
            var jsonDoc = JsonDocument.Parse(body.Content.ReadAsStringAsync().Result);
            var root = jsonDoc.RootElement;
            var sizeExists = root.TryGetProperty("size", out JsonElement size);
            var typeExists = root.TryGetProperty("type", out JsonElement type);
            var alignmentExists = root.TryGetProperty("alignment", out JsonElement alignment);
            var armor_classExists = root.TryGetProperty("armor_class", out JsonElement armor_class);
            var hit_pointsExists = root.TryGetProperty("hit_points", out JsonElement hit_points);
            var hit_diceExists = root.TryGetProperty("hit_dice", out JsonElement hit_dice);
            // I need to split the array into each individual props
            var speedExists = root.TryGetProperty("speed", out JsonElement speed);
            // I need to split the array into each individual props
            var proficienciesExists = root.TryGetProperty("proficiencies", out JsonElement proficiencies);
            // I need to split the array into each individual props
            var sensesExists = root.TryGetProperty("senses", out JsonElement senses);
            var languagesExists = root.TryGetProperty("languages", out JsonElement languages);
            // I need to split the array into each individual props
            var special_abilitiesExists = root.TryGetProperty("special_abilities", out JsonElement special_abilities);
            // I need to split the array into each individual props
            var actionsExists = root.TryGetProperty("actions", out JsonElement actions);

            await ReplyAsync($"Size: {size}\nType: {type}\nAlignment: {alignment}\nArmor Class: {armor_class}\nHit Points: {hit_points}\nHit Dice: {hit_dice}\nSpeed: {speed}\nProficiencies: {proficiencies}\nSenses: {senses}\nLanguages: {languages}\nSpecial Abilities: {special_abilities}\nActions: {actions}");
        }

        [Command("Condition")]
        public async Task DescribeConditionAsync(string condition)
        {
            // Create curl request out to ex: http://www.dnd5eapi.co/api/conditions/blinded
            var sanitizedCondition = condition.ToLower();

            var url = $"http://www.dnd5eapi.co/api/conditions/{sanitizedCondition}";
            var client = new HttpClient();
            var body = await client.GetAsync(url);
            var jsonDoc = JsonDocument.Parse(body.Content.ReadAsStringAsync().Result);
            var root = jsonDoc.RootElement;
            // var name = root.GetProperty("name").GetString(); shouldn't need this since we query with the name
            var descExists = root.TryGetProperty("desc", out JsonElement desc);
            var fullDescList = new List<string>();
            for (var i = 0; i < desc.GetArrayLength(); i++)
            {
                fullDescList.Add(desc[i].GetString());
            }
            var fullDesc = string.Join(", ", fullDescList).Replace('-', ' ').Replace(".,", ".");
            await ReplyAsync($"{fullDesc}");
        }

        [Command("Equipment")]
        public async Task DescribeEquipmentAsync(string equipment)
        {
            // Create curl request out to ex: http://www.dnd5eapi.co/api/equipment/club
            var sanitizedEquipment = equipment.ToLower();

            var url = $"http://www.dnd5eapi.co/api/equipment/{sanitizedEquipment}";
            var client = new HttpClient();
            var body = await client.GetAsync(url);
            var jsonDoc = JsonDocument.Parse(body.Content.ReadAsStringAsync().Result);
            var root = jsonDoc.RootElement;
            // var name = root.GetProperty("name").GetString(); shouldn't need this since we query with the name
            var fullDamageList = new List<string>();
            var damage = root.GetProperty("damage");
            for (var i = 0; i < damage.GetArrayLength(); i++)
            {
                fullDamageList.Add(damage[i].GetString());
            }
            var fullDamage = string.Join(", ", fullDamageList).Replace('-', ' ').Replace(".,", ".");
            await ReplyAsync($"{fullDamage}");
        }

        [Command("Night")]
        public async Task Night()
        {
            await ReplyAsync($"Goodnight Everyone.");
        }

        [Command("69")]
        public async Task RespondToFunnyMemeAsync()
        {
            await ReplyAsync("noice");
        }

        [Command("fuck u")]
        public async Task ArgueToRudePerson()
        {
            await ReplyAsync("no, fuck u");
        }

        [Command("pet")]
        public async Task PetTheCat(string pet)
        {
            await ReplyAsync($"*virtually pets {pet}*");
        }
    }
}