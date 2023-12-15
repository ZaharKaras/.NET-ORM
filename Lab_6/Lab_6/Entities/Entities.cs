using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6.Entities
{
    public class Types_Of_Devil_Fruits
    {
        public int Type_Id { get; set; }
        public string Name { get; set; }
    }

    public class Devil_Fruits
    {
        public int Fruit_Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public int Type_Id { get; set; }

    }
    public class Haki
    {
        public int Haki_Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class Pirates
    {
        public int Pirate_Id { get; set; }
        public string? Name { get; set; }
        public string? Info { get; set; }
        public int Reward { get; set; }
        public int? Devil_Fruit_Id { get; set; }
        public int Rank_Id { get; set; }

     
    }
    public class Pirate_Ranks
    {
        public int Rank_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Pirates_Haki
    {
        public int Haki_Id { get; set; }
        public int Pirate_Id { get; set; }

    }

    public class Missions
    {
        public int Mission_Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public int Pirates_Id { get; set; }
    }

    public class Marine_Ranks
    {
        public int Rank_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Marines
    {
        public int Marine_Id { get; set; }
        public string Name { get; set; }
        public int Ships { get; set; }
        public int Rank_Id { get; set; }
    }

    public class Reports
    {
        public int Report_Id { get; set; }
        public string? Message { get; set; }
        public DateTime Publish_Date { get; set; }
        public int Author_Id { get; set; }
        public int Mission_Id { get; set; }
    }

    public class Ensign
    {
        public int Ensign_Id { get; set; }
        public string Name { get; set; }
        public int Captain_Id { get; set; }
    }

    public class News
    {
        public int News_Id { get; set; }
        public string? Message { get; set; }
        public DateTime? Publish_Date { get; set; }
        public int Author_Id { get; set; }
    }

}
