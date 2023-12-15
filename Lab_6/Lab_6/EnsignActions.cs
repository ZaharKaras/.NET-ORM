using Lab_6.Entities;
using Lab_6.KarasEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6
{
    public class EnsignActions
    {
        private OnePieceContext _context;
        private Ensign _ensign;
        public EnsignActions(OnePieceContext context, Ensign ensign)
        {
            _context = context;
            _ensign = ensign;
        }

        public void ViewCommands()
        {
            Console.WriteLine("1: View Pirates");
            Console.WriteLine("2: View Missions");
            Console.WriteLine("3: View News");
            Console.WriteLine("4: Participate in the mission");
            Console.WriteLine("5: Exit");
        }

        public void ViewPiratesInformation()
        {
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
            var pirates = _context.PiratesSet.Select(p => new Pirates { Pirate_Id = p.Pirate_Id, Name = p.Name}).ToList();
            var missions = _context.MissionsSet.Select(m => new Missions {Mission_Id = m.Mission_Id, Name = m.Name, Summary = m.Summary, Pirates_Id = m.Pirates_Id }).ToList();

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
            Console.Clear();

            Console.WriteLine("Choose Mission id:");
            var missionId = int.Parse(Console.ReadLine()!);

            Console.Clear();

            Console.WriteLine("You can request promotion! (y/n)");
            var chosen = Console.ReadLine()!.ToLower();
            if (chosen == "y")
                RequestPromotion();
        }

        public async void RequestPromotion()
        {
            Console.WriteLine("You are promoted! Now, you are marine");
            await _context.EnsignSet.DeleteAsync(_ensign.Name);
            var marines = _context.MarinesSet.Select(n => new Marines { Marine_Id = n.Marine_Id}).ToList();
            await _context.MarinesSet.AddAsync(new Marines { Marine_Id = marines.Last().Marine_Id + 1, Name = _ensign.Name, Rank_Id = 1, Ships = 100 });
            Console.WriteLine("You need restart app!");
        }

        
    }
}
