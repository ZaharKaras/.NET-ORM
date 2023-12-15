using Lab_6.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6
{
    public class MarineActions
    {
        private OnePieceContext _context;
        private Marines _marine;

        public MarineActions(OnePieceContext context, Marines marine)
        {
            _context = context;
            _marine = marine;
        }

        public void ViewCommands()
        {
            Console.WriteLine("1: View Pirates");
            Console.WriteLine("2: View Missions");
            Console.WriteLine("3: Participate in a Mission");
            Console.WriteLine("4: Write News");
            Console.WriteLine("5: Reward a Pirate");
            Console.WriteLine("6: Write a Report");
            Console.WriteLine("7: Exit");
        }

        public void ViewPiratesInformation()
        {
            Console.Clear();

            var devilFruits = _context.DevilFruitSet.Select(d => new Devil_Fruits() { Fruit_Id = d.Fruit_Id, Name = d.Name }).ToList();
            var pirates = _context.PiratesSet.Select(p => new Pirates { Pirate_Id = p.Pirate_Id, Name = p.Name, Devil_Fruit_Id = p.Devil_Fruit_Id, Rank_Id = p.Rank_Id }).ToList();
            var hakis = _context.HakiSet.Select(h => new Haki { Haki_Id = h.Haki_Id, Name = h.Name }).ToList();
            var piratesHaki = _context.PirateHakiSet.Select(ph => new Pirates_Haki { Haki_Id = ph.Haki_Id, Pirate_Id = ph.Pirate_Id }).ToList();
            var ranks = _context.PirateRankSet.Select(r => new Pirate_Ranks { Rank_Id = r.Rank_Id, Name = r.Name }).ToList();
            var news = _context.NewsSet.Select(n => new News { Author_Id = n.Author_Id, Message = n.Message }).ToList();

            var joinedList = from pirate in pirates
                             join devilFruit in devilFruits
                             on pirate.Devil_Fruit_Id equals devilFruit.Fruit_Id into dfGroup
                             from subDevilFruit in dfGroup.DefaultIfEmpty()
                             join pirateRank in ranks
                             on pirate.Rank_Id equals pirateRank.Rank_Id into prGroup
                             from subPirateRank in prGroup.DefaultIfEmpty()
                             join pirateHaki in piratesHaki
                             on pirate.Pirate_Id equals pirateHaki.Pirate_Id into phGroup
                             from subPirateHaki in phGroup.DefaultIfEmpty()
                             join haki in hakis
                             on subPirateHaki.Haki_Id equals haki.Haki_Id into hGroup
                             from subHaki in hGroup.DefaultIfEmpty()
                             group subHaki by new { pirate, subDevilFruit, subPirateRank } into g
                             select new
                             {
                                 PirateName = g.Key.pirate.Name,
                                 DevilFruit = g.Key.subDevilFruit != null ? g.Key.subDevilFruit.Name : "No Devil Fruit",
                                 PirateRank = g.Key.subPirateRank != null ? g.Key.subPirateRank.Name : "No Rank",
                                 Haki = string.Join(", ", g.Where(x => x != null).Select(x => x.Name))
                             };

            foreach (var item in joinedList)
            {
                Console.WriteLine($"Pirate Name: {item.PirateName}");
                Console.WriteLine($"Devil Fruit: {item.DevilFruit}");
                Console.WriteLine($"Pirate Rank: {item.PirateRank}");
                Console.WriteLine($"Haki: {item.Haki}");
                Console.WriteLine();
            }
        }

        public void ViewMissionsInformation()
        {
            Console.Clear();

            var pirates = _context.PiratesSet.Select(p => new Pirates { Pirate_Id = p.Pirate_Id, Name = p.Name }).ToList();
            var missions = _context.MissionsSet.Select(m => new Missions { Mission_Id = m.Mission_Id, Name = m.Name, Summary = m.Summary, Pirates_Id = m.Pirates_Id }).ToList();

            var result = from mission in missions
                         join pirate in pirates on mission.Pirates_Id equals pirate.Pirate_Id into gj
                         from subPirate in gj.DefaultIfEmpty()
                         select new
                         {
                             Id = mission.Mission_Id,
                             MissionName = mission.Name,
                             MissionSummary = mission.Summary,
                             PirateName = subPirate != null ? subPirate.Name : "No Pirate Assigned"
                         };

            foreach (var item in result)
            {
                Console.WriteLine($"Id: {item.Id}");
                Console.WriteLine($"Mission: {item.MissionName}");
                Console.WriteLine($"Summary: {item.MissionSummary}");
                Console.WriteLine($"Assigned Pirate: {item.PirateName}");
                Console.WriteLine();
            }
        }

        public void ParticipateInMission()
        {
            // Similar to the EnsignActions class
            // Allow the marine to participate in a mission
        }

        public async void WriteNews()
        {
            Console.Clear();

            var newsList = _context.NewsSet.Select(n => new News { News_Id = n.News_Id}).ToList();
            Console.WriteLine("Write New:");
            var message = Console.ReadLine();
            await _context.NewsSet.AddAsync(new News { News_Id = newsList.Last().News_Id + 1, Message = message, Author_Id = _marine.Marine_Id, Publish_Date = DateTime.Now });
            Console.WriteLine("New is added!");
            Console.ReadLine();
        }

        public async Task RewardAPirateAsync()
        {
            Console.Clear();

            Console.WriteLine("Enter Pirate ID to Reward:");
            var pirateId = int.Parse(Console.ReadLine()!);

            Console.WriteLine("Enter the Reward Amount:");
            var rewardAmount = int.Parse(Console.ReadLine()!);

            var pirates = _context.PiratesSet.Select(n => new Pirates { Pirate_Id = n.Pirate_Id, Name = n.Name, Info = n.Info, Reward = n.Reward, Devil_Fruit_Id = n.Devil_Fruit_Id, Rank_Id = n.Rank_Id }).ToList();
            // Logic to reward a pirate by updating their reward amount in the database
            var pirate = pirates.FirstOrDefault(p => p.Equals(pirateId));
            pirate.Reward = rewardAmount;

            await _context.PiratesSet.UpdateAsync(pirate, pirateId);
        }

        public async void WriteAReport()
        {
            Console.Clear();

            var reports = _context.ReportsSet.Select(r => new Reports { Report_Id = r.Report_Id }).ToList();
            var missions = _context.MissionsSet.Select(m => new Missions { Mission_Id = m.Mission_Id, Name = m.Name, Summary = m.Summary}).ToList();


            Console.WriteLine("Enter mission id:");
            var id = int.Parse(Console.ReadLine()!);

            Console.WriteLine("Write your report:");
            var reportMessage = Console.ReadLine();

            await _context.ReportsSet.AddAsync(new Reports { Report_Id = reports.Last().Report_Id + 1, Message = reportMessage , Author_Id = _marine.Marine_Id, Mission_Id = id, Publish_Date = DateTime.Now});

            Console.WriteLine("Report is added!");
            Console.ReadLine();
        }
    }

}
