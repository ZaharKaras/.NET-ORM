using Lab_6.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6
{
    internal class AdminActions
    {
        private OnePieceContext _context;
        private Marines _marine;

        public AdminActions(OnePieceContext context, Marines marine)
        {
            _context = context;
            _marine = marine;
        }

        public void ViewCommands()
        {
            Console.WriteLine("1: View Pirates");
            Console.WriteLine("2: View Missions");
            Console.WriteLine("3: View News");
            Console.WriteLine("4: Create a Mission");
            Console.WriteLine("5: Delete a Mission");
            Console.WriteLine("6: Create News");
            Console.WriteLine("7: Edit Pirates Information");
            Console.WriteLine("8: Edit Missions Information");
            Console.WriteLine("9: Edit News Information");
            Console.WriteLine("10: Exit");
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


        public async void CreateMission()
        {
            Console.WriteLine("Enter mission name:");
            var name = Console.ReadLine();
            Console.WriteLine("Enter mission info:");
            var info = Console.ReadLine();
            Console.WriteLine("Enter pirate id:");
            var id = int.Parse(Console.ReadLine()!);

            var missions = _context.MissionsSet.Select(m => new Missions { Mission_Id = m.Mission_Id}).ToList();

            await _context.MissionsSet.AddAsync(new Missions { Mission_Id = missions.Last().Mission_Id + 1, Name = name, Summary = info, Pirates_Id = id });
            Console.WriteLine("Mission was added");
        }

        public async void DeleteMission()
        {
            Console.WriteLine("Enter mission name");
            var name = Console.ReadLine()!;

            await _context.MissionsSet.DeleteAsync(name);
            Console.WriteLine("Mission was deleted");
        }

        public async Task CreateNewsAsync()
        {
            Console.Clear();

            var newsList = _context.NewsSet.Select(n => new News { News_Id = n.News_Id }).ToList();
            Console.WriteLine("Write New:");
            var message = Console.ReadLine();
            await _context.NewsSet.AddAsync(new News { News_Id = newsList.Last().News_Id + 1, Message = message, Author_Id = _marine.Marine_Id, Publish_Date = DateTime.Now });
            Console.WriteLine("New is added!");
            Console.ReadLine();
        }

        
        public async void EditPiratesInformation()
        {
            Console.WriteLine("Choose pirate id:");
            var id = int.Parse(Console.ReadLine()!);

            Console.WriteLine("Enter new info:");
            var info = Console.ReadLine();


            var pirates = _context.PiratesSet.Select(n => new Pirates { Pirate_Id = n.Pirate_Id, Name = n.Name, Info = n.Info, Reward = n.Reward, Devil_Fruit_Id = n.Devil_Fruit_Id, Rank_Id = n.Rank_Id }).ToList();
            // Logic to reward a pirate by updating their reward amount in the database
            var pirate = pirates.FirstOrDefault(p => p.Equals(id));

            pirate.Info = info;

            await _context.PiratesSet.UpdateAsync(pirate, id);
        }

        public void EditMissionsInformation()
        {
            // Logic to edit missions information in the database
        }

        public void EditNewsInformation()
        {
            // Logic to edit news information in the database
        }
    }
}
