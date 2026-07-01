
using System;
using System.Collections.Generic;
using System.Linq;

// ============ 顶级语句（必须在最前） ============
var svc = new LeagueService();

Console.WriteLine("========== 1. 积分榜（自动计算自32场已完赛） ==========");
var standings = svc.GetStandings();
Console.WriteLine($"{"Rk",2} {"球队",-4} {"赛",2} {"胜",2} {"平",2} {"负",2}  {"进",2}/{"失",2}  {"净",3}  {"积",3}");
foreach (var s in standings)
    Console.WriteLine($"{s.Rank,2}. {s.Team.ShortName,-4} {s.Played,2} {s.Won,2} {s.Drawn,2} {s.Lost,2}  {s.GoalsFor,2}/{s.GoalsAgainst,2}  {(s.GoalDifference>=0?"+":"")}{s.GoalDifference,2}  {s.Points,3}");

Console.WriteLine("\n========== 2. 核心数据一致性校验 ==========");
var cm = svc.GetCompletedMatches();
var stats = svc.GetTeamStats();

int mTotalGoals = cm.Sum(m => (m.HomeScore ?? 0) + (m.AwayScore ?? 0));
int sTotalGoals = standings.Sum(s => s.GoalsFor);
int tsTotalGoals = stats.Sum(s => s.GoalsScored);
Console.WriteLine($"[总进球] 比赛算: {mTotalGoals}  积分榜和: {sTotalGoals} TeamStats和: {tsTotalGoals}");
Console.WriteLine($"  -> {(mTotalGoals==sTotalGoals && sTotalGoals==tsTotalGoals ? "PASS 三者一致" : "FAIL 不一致!")}");

Console.Write("[进球一致] 每队积分榜进球 vs TeamStats进球: ");
bool gfOK = standings.All(s => s.GoalsFor == stats.First(x => x.Team.Id == s.Team.Id).GoalsScored);
bool gaOK = standings.All(s => s.GoalsAgainst == stats.First(x => x.Team.Id == s.Team.Id).GoalsConceded);
Console.WriteLine($"进:{(gfOK?"PASS":"FAIL")} 失:{(gaOK?"PASS":"FAIL")}");
if (!gfOK || !gaOK) foreach (var s in standings)
{
    var ts = stats.First(x => x.Team.Id == s.Team.Id);
    if (s.GoalsFor != ts.GoalsScored || s.GoalsAgainst != ts.GoalsConceded)
        Console.WriteLine($"   FAIL {s.Team.ShortName}: 积分({s.GoalsFor}/{s.GoalsAgainst}) != TeamStats({ts.GoalsScored}/{ts.GoalsConceded})");
}

Console.Write("[零封场次] 比赛实际 vs TeamStats: ");
bool csOK = true;
foreach (var s in standings)
{
    var ts = stats.First(x => x.Team.Id == s.Team.Id);
    int real = cm.Count(m => (m.HomeTeam.Id == s.Team.Id && m.AwayScore == 0) || (m.AwayTeam.Id == s.Team.Id && m.HomeScore == 0));
    if (ts.CleanSheets != real) { csOK = false; Console.WriteLine($"\n   FAIL {s.Team.ShortName}: TeamStats写{ts.CleanSheets}, 实际是{real}"); }
}
Console.WriteLine($"{(csOK ? "PASS 全部一致" : "")}");

Console.WriteLine("\n========== 3. 典型描述一致性 ==========");
Check("盐城5战全胜 进7失1", standings.First(s=>s.Team.Name=="盐城队"), s => s.Played==5 && s.Won==5 && s.Drawn==0 && s.Lost==0 && s.GoalsFor==7 && s.GoalsAgainst==1);
Check("无锡3胜1平1负积10分居次席", standings.First(s=>s.Team.Name=="无锡队"), s => s.Played==5 && s.Won==3 && s.Drawn==1 && s.Lost==1 && s.Points==10 && s.Rank==2);
Check("镇江5战全败积0",         standings.First(s=>s.Team.Name=="镇江队"), s => s.Played==5 && s.Lost==5 && s.Points==0);
Check("宿迁3胜2负积9分",       standings.First(s=>s.Team.Name=="宿迁队"), s => s.Won==3 && s.Lost==2 && s.Points==9);
Check("南通积5分",             standings.First(s=>s.Team.Name=="南通队"), s => s.Points==5);
Check("泰州积6分",             standings.First(s=>s.Team.Name=="泰州队"), s => s.Points==6);
Check("淮安2胜1平1负积7",      standings.First(s=>s.Team.Name=="淮安队"), s => s.Won==2 && s.Drawn==1 && s.Points==7);
Check("连云港积2分第12",        standings.First(s=>s.Team.Name=="连云港队"), s => s.Points==2);
Check("扬州积4分第11",          standings.First(s=>s.Team.Name=="扬州队"), s => s.Points==4);

void Check(string desc, Standing s, Func<Standing, bool> pred)
{
    bool ok = pred(s);
    Console.WriteLine($"  {(ok ? "PASS" : "FAIL")} {desc,-24} -> 实际:赛{s.Played} 胜{s.Won}平{s.Drawn}负{s.Lost} 积{s.Points} 进{s.GoalsFor}失{s.GoalsAgainst} (排{s.Rank})");
}

Console.WriteLine("\n========== 4. 射手榜 ==========");
var ts2 = svc.GetTopScorers();
int idx = 1;
foreach (var s in ts2)
{
    bool inTeam = s.Team.KeyPlayers.Any(p => p.Id == s.Player.Id);
    Console.WriteLine($"  {idx++}. {s.Player.Name,-4} ({s.Team.ShortName}) {s.Goals}球{s.Assists}助 -> 球员在KeyPlayers:{(inTeam?"PASS":"FAIL")}");
}
Console.WriteLine($"射手榜总进球: {ts2.Sum(x=>x.Goals)}  助攻: {ts2.Sum(x=>x.Assists)}");

Console.WriteLine("\n========== 5. 赛事数据总览 (Stats页) ==========");
var totalGoals = stats.Sum(s => s.GoalsScored);
var completedCount = cm.Count;
double avgG = completedCount > 0 ? (double)totalGoals / completedCount : 0;
int totalCS = stats.Sum(s => s.CleanSheets);
double csRate = completedCount > 0 ? (double)totalCS / (completedCount * 2) * 100 : 0;
double avgPos = stats.Average(s => s.PossessionRate);
int yc = stats.Sum(s => s.YellowCards);
int rc = stats.Sum(s => s.RedCards);
Console.WriteLine($"  已完赛: {completedCount} 场");
Console.WriteLine($"  总进球: {totalGoals}  场均: {avgG:F1} 球/场");
Console.WriteLine($"  零封(球队累计): {totalCS}  零封占比: {csRate:F1}%");
Console.WriteLine($"  平均控球率: {avgPos:F1}%");
Console.WriteLine($"  黄牌: {yc}  红牌: {rc}");

Console.WriteLine("\n========== 6. 各队完赛场次 ==========");
foreach (var s in standings.OrderByDescending(s => s.Played))
    Console.WriteLine($"  {s.Team.ShortName,-4} 赛{s.Played,2}场");

// ============ 类型定义（必须在顶级语句之后） ============
public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Coach { get; set; } = string.Empty;
    public List<Player> KeyPlayers { get; set; } = [];
}

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int JerseyNumber { get; set; }
    public int Age { get; set; }
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
    public Team HomeTeam { get; set; } = null!;
    public Team AwayTeam { get; set; } = null!;
    public DateTime Date { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public MatchStatus Status { get; set; }
    public int Round { get; set; }
}

public enum MatchStatus { Upcoming, Live, Completed }

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
    private readonly List<TopScorer> _topScorers;
    private readonly List<TeamStats> _teamStats;

    public LeagueService()
    {
        _teams = InitializeTeams();
        _matches = InitializeMatches();
        _topScorers = InitializeTopScorers();
        _teamStats = InitializeTeamStats();
    }

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
                goalsFor += teamScore; goalsAgainst += oppScore;
                if (teamScore > oppScore) won++;
                else if (teamScore == oppScore) drawn++;
                else lost++;
            }
            standings.Add(new Standing { Team = team, Played = teamMatches.Count, Won = won, Drawn = drawn, Lost = lost, GoalsFor = goalsFor, GoalsAgainst = goalsAgainst });
        }
        return standings.OrderByDescending(s => s.Points).ThenByDescending(s => s.GoalDifference).ThenByDescending(s => s.GoalsFor)
            .Select((s, i) => { s.Rank = i + 1; return s; }).ToList();
    }

    private List<Team> InitializeTeams() =>
    [
        new() { Id = 1, Name = "南京队", ShortName = "南京", Coach = "何塞", KeyPlayers = { new() { Id = 101, Name = "滕帅", Position = "前锋", JerseyNumber = 9 }, new() { Id = 102, Name = "司俊远", Position = "边锋", JerseyNumber = 11 }, new() { Id = 103, Name = "曹翰晨", Position = "中场", JerseyNumber = 8 } } },
        new() { Id = 2, Name = "苏州队", ShortName = "苏州", Coach = "陈婉婷", KeyPlayers = { new() { Id = 201, Name = "吴宇帆", Position = "中锋", JerseyNumber = 9 }, new() { Id = 202, Name = "周鑫", Position = "左边锋", JerseyNumber = 11 }, new() { Id = 203, Name = "沈佳豪", Position = "后腰", JerseyNumber = 6 } } },
        new() { Id = 3, Name = "无锡队", ShortName = "无锡", Coach = "唐京", KeyPlayers = { new() { Id = 301, Name = "谢志伟", Position = "中卫", JerseyNumber = 4 }, new() { Id = 302, Name = "卢则灵", Position = "前锋", JerseyNumber = 7 }, new() { Id = 303, Name = "蒋孟泽", Position = "中场", JerseyNumber = 10 } } },
        new() { Id = 4, Name = "常州队", ShortName = "常州", Coach = "殷铁生", KeyPlayers = { new() { Id = 401, Name = "吉翔", Position = "右后卫", JerseyNumber = 24 }, new() { Id = 402, Name = "苗润东", Position = "中前卫", JerseyNumber = 14 }, new() { Id = 403, Name = "叶文杰", Position = "前锋", JerseyNumber = 18 } } },
        new() { Id = 5, Name = "南通队", ShortName = "南通", Coach = "葛勇", KeyPlayers = { new() { Id = 501, Name = "李贤成", Position = "中场", JerseyNumber = 8 }, new() { Id = 502, Name = "潘万权", Position = "前锋", JerseyNumber = 9 }, new() { Id = 503, Name = "赵东旭", Position = "门将", JerseyNumber = 1 } } },
        new() { Id = 6, Name = "徐州队", ShortName = "徐州", Coach = "胡云峰", KeyPlayers = { new() { Id = 601, Name = "郑雪健", Position = "后腰", JerseyNumber = 6 }, new() { Id = 602, Name = "苗润东", Position = "前锋", JerseyNumber = 9 }, new() { Id = 603, Name = "刘洋", Position = "右前卫", JerseyNumber = 7 } } },
        new() { Id = 7, Name = "盐城队", ShortName = "盐城", Coach = "德拉甘", KeyPlayers = { new() { Id = 701, Name = "李明", Position = "中锋", JerseyNumber = 10 }, new() { Id = 702, Name = "赵鹏", Position = "后腰", JerseyNumber = 6 }, new() { Id = 703, Name = "孙浩", Position = "左后卫", JerseyNumber = 3 } } },
        new() { Id = 8, Name = "扬州队", ShortName = "扬州", Coach = "曹睿", KeyPlayers = { new() { Id = 801, Name = "王强", Position = "中卫", JerseyNumber = 5 }, new() { Id = 802, Name = "李洋", Position = "前锋", JerseyNumber = 11 }, new() { Id = 803, Name = "陈浩", Position = "中场", JerseyNumber = 14 } } },
        new() { Id = 9, Name = "镇江队", ShortName = "镇江", Coach = "刘平豫", KeyPlayers = { new() { Id = 901, Name = "周明", Position = "前腰", JerseyNumber = 10 }, new() { Id = 902, Name = "黄磊", Position = "中锋", JerseyNumber = 9 }, new() { Id = 903, Name = "陈明", Position = "右前卫", JerseyNumber = 7 } } },
        new() { Id = 10, Name = "泰州队", ShortName = "泰州", Coach = "徐冀宁", KeyPlayers = { new() { Id = 1001, Name = "吴硕涛", Position = "前锋", JerseyNumber = 19 }, new() { Id = 1002, Name = "刘俊伯", Position = "边锋", JerseyNumber = 11 }, new() { Id = 1003, Name = "白雪松", Position = "中场", JerseyNumber = 8 } } },
        new() { Id = 11, Name = "淮安队", ShortName = "淮安", Coach = "裴恩才", KeyPlayers = { new() { Id = 1101, Name = "何健", Position = "前锋", JerseyNumber = 9 }, new() { Id = 1102, Name = "杨帆", Position = "前锋", JerseyNumber = 15 }, new() { Id = 1103, Name = "林浩", Position = "后腰", JerseyNumber = 6 } } },
        new() { Id = 12, Name = "连云港队", ShortName = "连云港", Coach = "魏新", KeyPlayers = { new() { Id = 1201, Name = "刘强", Position = "门将", JerseyNumber = 1 }, new() { Id = 1202, Name = "张骋", Position = "前锋", JerseyNumber = 10 }, new() { Id = 1203, Name = "张明", Position = "中卫", JerseyNumber = 5 } } },
        new() { Id = 13, Name = "宿迁队", ShortName = "宿迁", Coach = "裴恩才", KeyPlayers = { new() { Id = 1301, Name = "高驰", Position = "前锋", JerseyNumber = 9 }, new() { Id = 1302, Name = "殷国翔", Position = "中场", JerseyNumber = 10 }, new() { Id = 1303, Name = "陈力行", Position = "前锋", JerseyNumber = 11 } } },
    ];

    private List<Match> InitializeMatches()
    {
        var matches = new List<Match>();
        var teams = _teams;
        var c = new List<(int h, int a, int hs, int as_, int r, DateTime d)>
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
        foreach (var m in c) matches.Add(new() { HomeTeam = teams.First(t=>t.Id==m.h), AwayTeam = teams.First(t=>t.Id==m.a), Date = m.d, HomeScore = m.hs, AwayScore = m.as_, Status = MatchStatus.Completed, Round = m.r });
        var u = new List<(int h,int a,int r,DateTime d)> { (4,7,10,new(2026,6,20)),(8,12,10,new(2026,6,20)),(2,5,10,new(2026,6,20)),(1,11,10,new(2026,6,20)) };
        foreach (var m in u) matches.Add(new() { HomeTeam = teams.First(t=>t.Id==m.h), AwayTeam = teams.First(t=>t.Id==m.a), Date = m.d, Status = MatchStatus.Upcoming, Round = m.r });
        return matches;
    }

    private List<TopScorer> InitializeTopScorers()
    {
        var t = _teams;
        return [
            new() { Player = t[6].KeyPlayers[0],  Team = t[6],  Goals = 5, Assists = 1 },
            new() { Player = t[12].KeyPlayers[0], Team = t[12], Goals = 4, Assists = 1 },
            new() { Player = t[9].KeyPlayers[0],  Team = t[9],  Goals = 3, Assists = 1 },
            new() { Player = t[2].KeyPlayers[1],  Team = t[2],  Goals = 3, Assists = 1 },
            new() { Player = t[1].KeyPlayers[0],  Team = t[1],  Goals = 2, Assists = 2 },
            new() { Player = t[5].KeyPlayers[0],  Team = t[5],  Goals = 2, Assists = 1 },
            new() { Player = t[12].KeyPlayers[1], Team = t[12], Goals = 2, Assists = 0 },
            new() { Player = t[3].KeyPlayers[2],  Team = t[3],  Goals = 2, Assists = 0 }
        ];
    }

    private List<TeamStats> InitializeTeamStats()
    {
        var t = _teams;
        return [
            new() { Team = t[0],  GoalsScored=4, GoalsConceded=3,  CleanSheets=2, YellowCards=5,  RedCards=0, PossessionRate=52.3 },
            new() { Team = t[1],  GoalsScored=5, GoalsConceded=4,  CleanSheets=1, YellowCards=6,  RedCards=0, PossessionRate=54.1 },
            new() { Team = t[2],  GoalsScored=7, GoalsConceded=4,  CleanSheets=2, YellowCards=7,  RedCards=0, PossessionRate=48.6 },
            new() { Team = t[3],  GoalsScored=7, GoalsConceded=4,  CleanSheets=2, YellowCards=8,  RedCards=0, PossessionRate=51.2 },
            new() { Team = t[4],  GoalsScored=2, GoalsConceded=5,  CleanSheets=2, YellowCards=6,  RedCards=0, PossessionRate=44.3 },
            new() { Team = t[5],  GoalsScored=7, GoalsConceded=4,  CleanSheets=2, YellowCards=9,  RedCards=0, PossessionRate=49.8 },
            new() { Team = t[6],  GoalsScored=7, GoalsConceded=1,  CleanSheets=4, YellowCards=3,  RedCards=0, PossessionRate=58.2 },
            new() { Team = t[7],  GoalsScored=3, GoalsConceded=6,  CleanSheets=1, YellowCards=7,  RedCards=0, PossessionRate=41.5 },
            new() { Team = t[8],  GoalsScored=2, GoalsConceded=11, CleanSheets=0, YellowCards=10, RedCards=0, PossessionRate=38.2 },
            new() { Team = t[9],  GoalsScored=6, GoalsConceded=9,  CleanSheets=1, YellowCards=8,  RedCards=1, PossessionRate=47.1 },
            new() { Team = t[10], GoalsScored=4, GoalsConceded=3,  CleanSheets=1, YellowCards=5,  RedCards=0, PossessionRate=45.6 },
            new() { Team = t[11], GoalsScored=3, GoalsConceded=7,  CleanSheets=1, YellowCards=6,  RedCards=0, PossessionRate=39.8 },
            new() { Team = t[12], GoalsScored=8, GoalsConceded=4,  CleanSheets=2, YellowCards=7,  RedCards=0, PossessionRate=50.3 },
        ];
    }
}
