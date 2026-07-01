
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

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
}

public enum MatchStatus { Upcoming, Live, Completed }

public class News
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
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

public class LeagueService
{
    private readonly List<Team> _teams;
    private readonly List<Match> _matches;
    private readonly List<News> _news;
    private readonly List<TopScorer> _topScorers;
    private readonly List<TeamStats> _teamStats;

    public LeagueService()
    {
        _teams = InitializeTeams();
        _matches = InitializeMatches();
        _news = InitializeNews();
        _topScorers = InitializeTopScorers();
        _teamStats = InitializeTeamStats();
    }

    public List<Team> GetAllTeams() => _teams;
    public List<Match> GetAllMatches() => _matches.OrderBy(m => m.Date).ToList();
    public List<Match> GetCompletedMatches() => _matches.Where(m => m.Status == MatchStatus.Completed).OrderByDescending(m => m.Date).ToList();
    public List<TopScorer> GetTopScorers() => _topScorers.OrderByDescending(t => t.Goals).ToList();
    public List<TeamStats> GetTeamStats() => _teamStats;

    public List<Standing> GetStandings()
    {
        var standings = new List<Standing>();
        foreach (var team in _teams)
        {
            var teamMatches = _matches.Where(m => (m.HomeTeam.Id == team.Id || m.AwayTeam.Id == team.Id) && m.Status == MatchStatus.Completed).ToList();
            int won = 0, drawn = 0, lost = 0, goalsFor = 0, goalsAgainst = 0;
            foreach (var match in teamMatches)
            {
                bool isHome = match.HomeTeam.Id == team.Id;
                int teamScore = isHome ? match.HomeScore ?? 0 : match.AwayScore ?? 0;
                int oppScore = isHome ? match.AwayScore ?? 0 : match.HomeScore ?? 0;
                goalsFor += teamScore;
                goalsAgainst += oppScore;
                if (teamScore > oppScore) won++;
                else if (teamScore == oppScore) drawn++;
                else lost++;
            }
            standings.Add(new Standing
            {
                Team = team, Played = teamMatches.Count, Won = won, Drawn = drawn, Lost = lost,
                GoalsFor = goalsFor, GoalsAgainst = goalsAgainst
            });
        }
        return standings.OrderByDescending(s => s.Points).ThenByDescending(s => s.GoalDifference).ThenByDescending(s => s.GoalsFor)
            .Select((s, i) => { s.Rank = i + 1; return s; }).ToList();
    }

    private List<Team> InitializeTeams() =>
    [
        new() { Id = 1, Name = "南京队", ShortName = "南京", City = "南京", Coach = "何塞（西班牙）", Tactics = "4231控球传控体系", Formation = "4-2-3-1", ColorPrimary = "#C8102E", ColorSecondary = "#FFFFFF", FoundedYear = 2025, HomeStadium = "南京奥体中心", Capacity = 61443, Description = "传统劲旅，前4场仅入2球且全部为点球",
            KeyPlayers = { new() { Id = 101, Name = "滕帅", Position = "前锋", JerseyNumber = 9, Age = 26 }, new() { Id = 102, Name = "司俊远", Position = "边锋", JerseyNumber = 11, Age = 23 }, new() { Id = 103, Name = "曹翰晨", Position = "中场", JerseyNumber = 8, Age = 25 } } },
        new() { Id = 2, Name = "苏州队", ShortName = "苏州", City = "苏州", Coach = "陈婉婷", Tactics = "433高位逼抢", Formation = "4-3-3", ColorPrimary = "#005BA9", ColorSecondary = "#FFFFFF", FoundedYear = 2025, HomeStadium = "苏州奥体中心", Capacity = 45000, Description = "年轻化的经济强队",
            KeyPlayers = { new() { Id = 201, Name = "吴宇帆", Position = "中锋", JerseyNumber = 9, Age = 25 }, new() { Id = 202, Name = "周鑫", Position = "左边锋", JerseyNumber = 11, Age = 23 }, new() { Id = 203, Name = "沈佳豪", Position = "后腰", JerseyNumber = 6, Age = 24 } } },
        new() { Id = 3, Name = "无锡队", ShortName = "无锡", City = "无锡", Coach = "唐京", Tactics = "451稳守反击", Formation = "4-5-1", ColorPrimary = "#2D3436", ColorSecondary = "#FDCB6E", FoundedYear = 2025, HomeStadium = "无锡体育中心", Capacity = 28000, Description = "目前唯一不败球队",
            KeyPlayers = { new() { Id = 301, Name = "谢志伟", Position = "中卫", JerseyNumber = 4, Age = 26 }, new() { Id = 302, Name = "卢则灵", Position = "前锋", JerseyNumber = 7, Age = 21 }, new() { Id = 303, Name = "蒋孟泽", Position = "中场", JerseyNumber = 10, Age = 24 } } },
        new() { Id = 4, Name = "常州队", ShortName = "常州", City = "常州", Coach = "殷铁生（顾问）、周斌", Tactics = "4141全攻阵型", Formation = "4-1-4-1", ColorPrimary = "#1E8449", ColorSecondary = "#FFFFFF", FoundedYear = 2025, HomeStadium = "常州奥体中心", Capacity = 36000, Description = "揭幕战3-0横扫南通",
            KeyPlayers = { new() { Id = 401, Name = "吉翔", Position = "右后卫", JerseyNumber = 24, Age = 34 }, new() { Id = 402, Name = "苗润东", Position = "中前卫", JerseyNumber = 14, Age = 25 }, new() { Id = 403, Name = "叶文杰", Position = "前锋", JerseyNumber = 18, Age = 20 } } },
        new() { Id = 5, Name = "南通队", ShortName = "南通", City = "南通", Coach = "葛勇", Tactics = "352三中卫", Formation = "3-5-2", ColorPrimary = "#2C3E50", ColorSecondary = "#E74C3C", FoundedYear = 2025, HomeStadium = "南通奥体中心", Capacity = 30000, Description = "上赛季亚军，本赛季进攻乏力",
            KeyPlayers = { new() { Id = 501, Name = "李贤成", Position = "中场", JerseyNumber = 8, Age = 24 }, new() { Id = 502, Name = "潘万权", Position = "前锋", JerseyNumber = 9, Age = 25 }, new() { Id = 503, Name = "赵东旭", Position = "门将", JerseyNumber = 1, Age = 27 } } },
        new() { Id = 6, Name = "徐州队", ShortName = "徐州", City = "徐州", Coach = "胡云峰", Tactics = "442菱形中场", Formation = "4-4-2", ColorPrimary = "#E67E22", ColorSecondary = "#FFFFFF", FoundedYear = 2025, HomeStadium = "徐州奥体中心", Capacity = 35000, Description = "苏北劲旅，第9轮主场被无锡绝杀",
            KeyPlayers = { new() { Id = 601, Name = "郑雪健", Position = "后腰", JerseyNumber = 6, Age = 26 }, new() { Id = 602, Name = "苗润东", Position = "前锋", JerseyNumber = 9, Age = 22 }, new() { Id = 603, Name = "刘洋", Position = "右前卫", JerseyNumber = 7, Age = 24 } } },
        new() { Id = 7, Name = "盐城队", ShortName = "盐城", City = "盐城", Coach = "德拉甘（塞尔维亚）", Tactics = "4231攻守平衡", Formation = "4-2-3-1", ColorPrimary = "#00A8FF", ColorSecondary = "#FFFFFF", FoundedYear = 2025, HomeStadium = "盐城体育中心", Capacity = 30000, Description = "五战全胜断层领跑",
            KeyPlayers = { new() { Id = 701, Name = "李明", Position = "中锋", JerseyNumber = 10, Age = 25 }, new() { Id = 702, Name = "赵鹏", Position = "后腰", JerseyNumber = 6, Age = 29 }, new() { Id = 703, Name = "孙浩", Position = "左后卫", JerseyNumber = 3, Age = 23 } } },
        new() { Id = 8, Name = "扬州队", ShortName = "扬州", City = "扬州", Coach = "曹睿", Tactics = "532防守反击", Formation = "5-3-2", ColorPrimary = "#8B4513", ColorSecondary = "#FFFFFF", FoundedYear = 2025, HomeStadium = "扬州体育公园", Capacity = 25000, Description = "积4分排名第11，保级严峻",
            KeyPlayers = { new() { Id = 801, Name = "王强", Position = "中卫", JerseyNumber = 5, Age = 30 }, new() { Id = 802, Name = "李洋", Position = "前锋", JerseyNumber = 11, Age = 24 }, new() { Id = 803, Name = "陈浩", Position = "中场", JerseyNumber = 14, Age = 26 } } },
        new() { Id = 9, Name = "镇江队", ShortName = "镇江", City = "镇江", Coach = "刘平豫", Tactics = "442平行站位", Formation = "4-4-2", ColorPrimary = "#9B59B6", ColorSecondary = "#FFFFFF", FoundedYear = 2025, HomeStadium = "镇江体育会展中心", Capacity = 28000, Description = "5战全败积0分垫底",
            KeyPlayers = { new() { Id = 901, Name = "周明", Position = "前腰", JerseyNumber = 10, Age = 25 }, new() { Id = 902, Name = "黄磊", Position = "中锋", JerseyNumber = 9, Age = 27 }, new() { Id = 903, Name = "陈明", Position = "右前卫", JerseyNumber = 7, Age = 23 } } },
        new() { Id = 10, Name = "泰州队", ShortName = "泰州", City = "泰州", Coach = "徐冀宁", Tactics = "343压迫式打法", Formation = "3-4-3", ColorPrimary = "#1ABC9C", ColorSecondary = "#FFFFFF", FoundedYear = 2025, HomeStadium = "泰州体育中心", Capacity = 30000, Description = "卫冕冠军，第9轮绝杀南通",
            KeyPlayers = { new() { Id = 1001, Name = "吴硕涛", Position = "前锋", JerseyNumber = 19, Age = 24 }, new() { Id = 1002, Name = "刘俊伯", Position = "边锋", JerseyNumber = 11, Age = 23 }, new() { Id = 1003, Name = "白雪松", Position = "中场", JerseyNumber = 8, Age = 25 } } },
        new() { Id = 11, Name = "淮安队", ShortName = "淮安", City = "淮安", Coach = "裴恩才", Tactics = "442防守反击", Formation = "4-4-2", ColorPrimary = "#F39C12", ColorSecondary = "#FFFFFF", FoundedYear = 2025, HomeStadium = "淮安奥体中心", Capacity = 35000, Description = "作风硬朗，2胜1平1负积7分",
            KeyPlayers = { new() { Id = 1101, Name = "何健", Position = "前锋", JerseyNumber = 9, Age = 28 }, new() { Id = 1102, Name = "杨帆", Position = "前锋", JerseyNumber = 15, Age = 25 }, new() { Id = 1103, Name = "林浩", Position = "后腰", JerseyNumber = 6, Age = 26 } } },
        new() { Id = 12, Name = "连云港队", ShortName = "连云港", City = "连云港", Coach = "魏新", Tactics = "541密集防守", Formation = "5-4-1", ColorPrimary = "#27AE60", ColorSecondary = "#FFFFFF", FoundedYear = 2025, HomeStadium = "连云港体育中心", Capacity = 28000, Description = "赛季未尝胜绩仅积2分",
            KeyPlayers = { new() { Id = 1201, Name = "刘强", Position = "门将", JerseyNumber = 1, Age = 27 }, new() { Id = 1202, Name = "张骋", Position = "前锋", JerseyNumber = 10, Age = 23 }, new() { Id = 1203, Name = "张明", Position = "中卫", JerseyNumber = 5, Age = 29 } } },
        new() { Id = 13, Name = "宿迁队", ShortName = "宿迁", City = "宿迁", Coach = "裴恩才", Tactics = "442快速反击", Formation = "4-4-2", ColorPrimary = "#E74C3C", ColorSecondary = "#FFFFFF", FoundedYear = 2025, HomeStadium = "宿迁奥体中心", Capacity = 32000, Description = "3胜2负积9分排名第3，第9轮3-0大胜镇江",
            KeyPlayers = { new() { Id = 1301, Name = "高驰", Position = "前锋", JerseyNumber = 9, Age = 26 }, new() { Id = 1302, Name = "殷国翔", Position = "中场", JerseyNumber = 10, Age = 24 }, new() { Id = 1303, Name = "陈力行", Position = "前锋", JerseyNumber = 11, Age = 22 } } },
    ];

    private List<Match> InitializeMatches()
    {
        var matches = new List<Match>();
        var teams = _teams;
        int matchId = 1;
        var completedMatches = new List<(int home, int away, int hs, int as_, int round, DateTime date)>
        {
            (4,5,3,0,1,new(2026,4,11)),(8,2,0,1,1,new(2026,4,11)),(3,9,3,1,1,new(2026,4,11)),(12,7,0,2,1,new(2026,4,11)),
            (13,1,2,0,2,new(2026,4,18)),(11,8,1,1,2,new(2026,4,18)),(6,10,3,0,2,new(2026,4,18)),
            (12,3,0,0,3,new(2026,4,25)),(5,6,0,0,3,new(2026,4,25)),(7,13,1,0,3,new(2026,4,25)),
            (1,4,2,1,4,new(2026,5,2)),(10,8,3,1,4,new(2026,5,2)),(2,11,0,1,4,new(2026,5,2)),(9,7,1,2,4,new(2026,5,2)),
            (3,10,3,1,5,new(2026,5,9)),(5,1,0,0,5,new(2026,5,9)),(6,13,1,2,5,new(2026,5,9)),
            (4,11,1,2,6,new(2026,5,16)),(2,12,2,2,6,new(2026,5,16)),(8,9,1,0,6,new(2026,5,16)),
            (11,7,0,1,7,new(2026,5,23)),(12,6,1,3,7,new(2026,5,23)),(13,5,1,2,7,new(2026,5,23)),
            (10,2,1,2,8,new(2026,5,30)),(7,8,1,0,8,new(2026,5,30)),(9,4,0,2,8,new(2026,5,30)),(3,1,0,2,8,new(2026,5,30)),
            (6,3,0,1,9,new(2026,6,13)),(13,9,3,0,9,new(2026,6,13)),(5,10,0,1,9,new(2026,6,13)),
        };
        foreach (var cm in completedMatches)
        {
            matches.Add(new Match
            {
                Id = matchId++, HomeTeam = teams.First(t => t.Id == cm.home), AwayTeam = teams.First(t => t.Id == cm.away),
                Date = cm.date, Time = "19:40", Stadium = teams.First(t => t.Id == cm.home).HomeStadium,
                HomeScore = cm.hs, AwayScore = cm.as_, Status = MatchStatus.Completed, Round = cm.round
            });
        }
        var upcoming = new List<(int h, int a, int r, DateTime d)> { (4,7,10,new(2026,6,20)),(8,12,10,new(2026,6,20)),(2,5,10,new(2026,6,20)),(1,11,10,new(2026,6,20)) };
        foreach (var um in upcoming)
        {
            matches.Add(new Match
            {
                Id = matchId++, HomeTeam = teams.First(t => t.Id == um.h), AwayTeam = teams.First(t => t.Id == um.a),
                Date = um.d, Time = "19:40", Stadium = teams.First(t => t.Id == um.h).HomeStadium,
                Status = MatchStatus.Upcoming, Round = um.r
            });
        }
        return matches;
    }

    private List<News> InitializeNews() => [];

    private List<TopScorer> InitializeTopScorers()
    {
        var t = _teams;
        return [
            new() { Player = t[6].KeyPlayers[0], Team = t[6], Goals = 5, Assists = 1 },
            new() { Player = t[12].KeyPlayers[0], Team = t[12], Goals = 4, Assists = 1 },
            new() { Player = t[9].KeyPlayers[0], Team = t[9], Goals = 3, Assists = 1 },
            new() { Player = t[2].KeyPlayers[1], Team = t[2], Goals = 3, Assists = 1 },
            new() { Player = t[1].KeyPlayers[0], Team = t[1], Goals = 2, Assists = 2 },
            new() { Player = t[5].KeyPlayers[0], Team = t[5], Goals = 2, Assists = 1 },
            new() { Player = t[12].KeyPlayers[1], Team = t[12], Goals = 2, Assists = 0 },
            new() { Player = t[3].KeyPlayers[2], Team = t[3], Goals = 2, Assists = 0 }
        ];
    }

    private List<TeamStats> InitializeTeamStats()
    {
        var t = _teams;
        return [
            new() { Team = t[0],  GoalsScored=4, GoalsConceded=3, CleanSheets=2, YellowCards=5,  RedCards=0, PossessionRate=52.3, ShotsOnTarget=18 },
            new() { Team = t[1],  GoalsScored=5, GoalsConceded=4, CleanSheets=1, YellowCards=6,  RedCards=0, PossessionRate=54.1, ShotsOnTarget=22 },
            new() { Team = t[2],  GoalsScored=7, GoalsConceded=4, CleanSheets=2, YellowCards=7,  RedCards=0, PossessionRate=48.6, ShotsOnTarget=24 },
            new() { Team = t[3],  GoalsScored=7, GoalsConceded=4, CleanSheets=1, YellowCards=8,  RedCards=0, PossessionRate=51.2, ShotsOnTarget=26 },
            new() { Team = t[4],  GoalsScored=2, GoalsConceded=5, CleanSheets=2, YellowCards=6,  RedCards=0, PossessionRate=44.3, ShotsOnTarget=14 },
            new() { Team = t[5],  GoalsScored=7, GoalsConceded=4, CleanSheets=1, YellowCards=9,  RedCards=0, PossessionRate=49.8, ShotsOnTarget=28 },
            new() { Team = t[6],  GoalsScored=7, GoalsConceded=1, CleanSheets=4, YellowCards=3,  RedCards=0, PossessionRate=58.2, ShotsOnTarget=32 },
            new() { Team = t[7],  GoalsScored=3, GoalsConceded=6, CleanSheets=1, YellowCards=7,  RedCards=0, PossessionRate=41.5, ShotsOnTarget=16 },
            new() { Team = t[8],  GoalsScored=2, GoalsConceded=11,CleanSheets=0, YellowCards=10, RedCards=0, PossessionRate=38.2, ShotsOnTarget=12 },
            new() { Team = t[9],  GoalsScored=6, GoalsConceded=9, CleanSheets=1, YellowCards=8,  RedCards=1, PossessionRate=47.1, ShotsOnTarget=24 },
            new() { Team = t[10], GoalsScored=4, GoalsConceded=3, CleanSheets=2, YellowCards=5,  RedCards=0, PossessionRate=45.6, ShotsOnTarget=20 },
            new() { Team = t[11], GoalsScored=3, GoalsConceded=7, CleanSheets=0, YellowCards=6,  RedCards=0, PossessionRate=39.8, ShotsOnTarget=14 },
            new() { Team = t[12], GoalsScored=8, GoalsConceded=4, CleanSheets=2, YellowCards=7,  RedCards=0, PossessionRate=50.3, ShotsOnTarget=26 },
        ];
    }
}

// ================= 开始校验 =================
var svc = new LeagueService();

Console.WriteLine("=== 1. 积分榜理论值（从32场已完赛自动计算）:");
Console.WriteLine("Rk 球队      赛 胜 平 负  进 失  净  积分");
foreach (var s in svc.GetStandings())
    Console.WriteLine($"{s.Rank,2}. {s.Team.ShortName,-3} {s.Played,2} {s.Won,2} {s.Drawn,2} {s.Lost,2}  {s.GoalsFor,2}-{s.GoalsAgainst,2}  {s.GoalDifference,3}  {s.Points,3}");

Console.WriteLine("\n=== 2. 已完赛场次统计:");
var cm = svc.GetCompletedMatches();
Console.WriteLine($"已完赛: {cm.Count} 场");
int totalGF = cm.Sum(m => (m.HomeScore??0) + cm.Sum(m => (m.AwayScore??0);
Console.WriteLine($"总进球（按比赛算: {totalGF}");

Console.WriteLine($"零封场次数（从比赛算）：{cm.Count(m => (m.HomeScore == 0 || m.AwayScore == 0)}");
var cs = cm.Count(m => m.HomeScore == 0) + cm.Count(m => m.AwayScore == 0);
Console.WriteLine($"按球队零封总次数：{cs}");

Console.WriteLine("\n=== 3. 球队统计 vs 积分榜交叉校验（每支球队单独核对:");
var standings = svc.GetStandings();
var stats = svc.GetTeamStats();
Console.WriteLine("球队       积分榜(进/失)  统计(进/失)   积分GoalsScored匹配 积分GoalsConceded匹配");
foreach (var s in standings)
{
    var ts = stats.First(x => x.Team.Id == s.Team.Id);
    bool g = s.GoalsFor == ts.GoalsScored;
    bool c = s.GoalsAgainst == ts.GoalsConceded;
    Console.WriteLine($"{s.Team.ShortName,-5}  {s.GoalsFor,2}/{s.GoalsAgainst,2}       {ts.GoalsScored,2}/{ts.GoalsConceded,2}        {(g?"✅":"❌ 不一致!"}  {(c?"✅":"❌ 不一致!"}");
}

Console.WriteLine("\n=== 4. TeamStats 总进球 vs 积分榜总进球:");
int standTotal = standings.Sum(s => s.GoalsFor);
int statsTotal = stats.Sum(s => s.GoalsScored);
int cmTotal = cm.Sum(m => (m.HomeScore ?? 0) + (m.AwayScore ?? 0));
Console.WriteLine($"积分榜所有队进球和: {standTotal}");
Console.WriteLine($"TeamStats总进球: {statsTotal}  {(standTotal == statsTotal ? "✅ 匹配" : "❌ 不匹配!"}");
Console.WriteLine($"按比赛算总进球: {cmTotal}  {(standTotal == cmTotal ? "✅ 匹配积分榜" : "❌ 不匹配积分榜!"}");

Console.WriteLine("\n=== 5. 球队零封场次校验:");
Console.WriteLine("球队       TeamStats零封  按比赛实际零封  匹配");
foreach (var s in standings)
{
    var ts = stats.First(x => x.Team.Id == s.Team.Id);
    var tid = s.Team.Id;
    int realClean = cm.Count(m =>
        (m.HomeTeam.Id == tid && m.AwayScore == 0) ||
        (m.AwayTeam.Id == tid && m.HomeScore == 0));
    bool ok = ts.CleanSheets == realClean;
    Console.WriteLine($"{s.Team.ShortName,-5}  {ts.CleanSheets,2}          {realClean,2}          {(ok?"✅":"❌ 不一致!")}");
}

Console.WriteLine("\n=== 6. 无锡是否真的不败：");
var wx = svc.GetAllTeams().First(t => t.Name == "无锡队");
var wxMatches = cm.Where(m => m.HomeTeam.Id == wx.Id || m.AwayTeam.Id == wx.Id).ToList();
Console.WriteLine($"无锡已完赛: {wxMatches.Count} 场");
foreach (var m in wxMatches.OrderBy(m => m.Date))
{
    bool isHome = m.HomeTeam.Id == wx.Id;
    int my = isHome ? (m.HomeScore ?? 0) : (m.AwayScore ?? 0);
    int opp = isHome ? (m.AwayScore ?? 0) : (m.HomeScore ?? 0);
    string res = my > opp ? "胜" : my == opp ? "平" : "负";
    Console.WriteLine($"  R{m.Round}: {m.HomeTeam.ShortName} {m.HomeScore}-{m.AwayScore} {m.AwayTeam.ShortName} ({res})");
}

Console.WriteLine("\n=== 7. 盐城是否真的5战全胜+进7失1:");
var yc = svc.GetAllTeams().First(t => t.Name == "盐城队");
var ycStand = standings.First(s => s.Team.Id == yc.Id);
Console.WriteLine($"赛{ycStand.Played} 胜{ycStand.Won} 平{ycStand.Drawn} 负{ycStand.Lost} 进{ycStand.GoalsFor} 失{ycStand.GoalsAgainst}");
Console.WriteLine($"实际赛: {svc.GetCompletedMatches().Where(m => m.HomeTeam.Id == yc.Id || m.AwayTeam.Id == yc.Id).Count()}");

Console.WriteLine("\n=== 8. 射手榜球员归属校验:");
foreach (var ts in svc.GetTopScorers())
{
    bool ok = ts.Team.KeyPlayers.Any(p => p.Id == ts.Player.Id);
    Console.WriteLine($"{ts.Player.Name,-5} {ts.Team.ShortName,-3} {ts.Goals,2}球 {ts.Assists}助  球员在队:{(ok?"✅":"❌ 不在KeyPlayers中!")}");
}

Console.WriteLine("\n=== 9. 红黄牌总数（球队攻防统计:");
Console.WriteLine($"总黄牌: {stats.Sum(s => s.YellowCards)}  总红牌: {stats.Sum(s => s.RedCards)}");
Console.WriteLine($"平均控球率: {stats.Average(s => s.PossessionRate):F1}%");

