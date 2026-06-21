namespace Jiangsu_City_League_Blazor_ASP.NET.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Coach { get; set; } = string.Empty;
    public string Tactics { get; set; } = string.Empty;
    public string Formation { get; set; } = string.Empty;
    public string ColorPrimary { get; set; } = "#000000";
    public string ColorSecondary { get; set; } = "#ffffff";
    public string LogoUrl { get; set; } = string.Empty;
    public int FoundedYear { get; set; }
    public string HomeStadium { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public List<Player> KeyPlayers { get; set; } = [];
    public string Description { get; set; } = string.Empty;
}

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int JerseyNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Nationality { get; set; } = "中国";
}

public class Standing
{
    public int Rank { get; set; }
    public Team Team { get; set; } = null!;
    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points => Won * 3 + Drawn;
}

public class Match
{
    public int Id { get; set; }
    public Team HomeTeam { get; set; } = null!;
    public Team AwayTeam { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public string Stadium { get; set; } = string.Empty;
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public MatchStatus Status { get; set; }
    public int Round { get; set; }
    public string Highlights { get; set; } = string.Empty;
}

public enum MatchStatus
{
    Upcoming,
    Live,
    Completed
}

public class News
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public class TeamStats
{
    public Team Team { get; set; } = null!;
    public int GoalsScored { get; set; }
    public int GoalsConceded { get; set; }
    public int CleanSheets { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public double PossessionRate { get; set; }
    public int ShotsOnTarget { get; set; }
}

public class TopScorer
{
    public Player Player { get; set; } = null!;
    public Team Team { get; set; } = null!;
    public int Goals { get; set; }
    public int Assists { get; set; }
}