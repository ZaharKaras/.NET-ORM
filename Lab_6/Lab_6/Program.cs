using Lab_6;
using Lab_6.Entities;
using Lab_6.KarasEF;
using Npgsql;
using System.Data;

await using var connection = new NpgsqlConnection(ConnestionString.connStr);
await connection.OpenAsync();

var onpContext = new OnePieceContext(connection);

var typesOfDevilFruits = onpContext.TypeOfDFSet.Select(d => new Types_Of_Devil_Fruits() {Type_Id = d.Type_Id, Name = d.Name }).ToList();
//var devilFruits = onpContext.DevilFruitSet.Select(d => new Devil_Fruits() { Fruit_Id = d.Fruit_Id , Name = d.Name}).ToList();
//var pirates = onpContext.PiratesSet.Select(p => new Pirates { Pirate_Id = p.Pirate_Id, Name = p.Name, Devil_Fruit_Id = p.Devil_Fruit_Id, Rank_Id = p.Rank_Id}).ToList();

//void CheckMissions()
//{
//    var result = from mission in missions
//                 join pirate in pirates on mission.Pirates_Id equals pirate.Pirate_Id into gj
//                 from subPirate in gj.DefaultIfEmpty()
//                 select new
//                 {
//                     MissionName = mission.Name,
//                     MissionSummary = mission.Summary,
//                     PirateName = subPirate != null ? subPirate.Name : "No Pirate Assigned"
//                 };

//    foreach (var item in result)
//    {
//        Console.WriteLine($"Mission: {item.MissionName}");
//        Console.WriteLine($"Summary: {item.MissionSummary}");
//        Console.WriteLine($"Assigned Pirate: {item.PirateName}");
//        Console.WriteLine();
//    }
//}
