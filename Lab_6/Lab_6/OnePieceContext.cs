using Lab_6.Entities;
using Lab_6.KarasEF;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6
{
    public class OnePieceContext : KarasContext
    {
        public OnePieceContext(NpgsqlConnection connection) : base(connection)
        {

        }

        public KarasSet<Haki> HakiSet { get; set; }
        public KarasSet<Pirates> PiratesSet { get; set; }
        public KarasSet<Devil_Fruits> DevilFruitSet { get; set; }
        public KarasSet<Types_Of_Devil_Fruits> TypeOfDFSet { get; set; }
        public KarasSet<Pirates_Haki> PirateHakiSet { get; set; }
        public KarasSet<Pirate_Ranks> PirateRankSet { get; set; }
        public KarasSet<News> NewsSet { get; set; }
        public KarasSet<Marines> MarinesSet { get; set; }
        public KarasSet<Ensign> EnsignSet { get; set; }
        public KarasSet<Missions> MissionsSet { get; set; }
        public KarasSet<Reports> ReportsSet { get; set; }
    }
}
