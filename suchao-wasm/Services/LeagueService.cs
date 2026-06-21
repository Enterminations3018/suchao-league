using SuchaoLeagueWasm.Models;

namespace SuchaoLeagueWasm.Services;

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

    public Team? GetTeamById(int id) => _teams.FirstOrDefault(t => t.Id == id);

    public List<Standing> GetStandings()
    {
        var standings = new List<Standing>();
        
        foreach (var team in _teams)
        {
            var teamMatches = _matches.Where(m => 
                (m.HomeTeam.Id == team.Id || m.AwayTeam.Id == team.Id) && 
                m.Status == MatchStatus.Completed).ToList();

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
                Team = team,
                Played = teamMatches.Count,
                Won = won,
                Drawn = drawn,
                Lost = lost,
                GoalsFor = goalsFor,
                GoalsAgainst = goalsAgainst
            });
        }

        return standings.OrderByDescending(s => s.Points)
                       .ThenByDescending(s => s.GoalDifference)
                       .ThenByDescending(s => s.GoalsFor)
                       .Select((s, i) => { s.Rank = i + 1; return s; })
                       .ToList();
    }

    public List<Match> GetAllMatches() => _matches.OrderBy(m => m.Date).ToList();

    public List<Match> GetUpcomingMatches() => _matches
        .Where(m => m.Status == MatchStatus.Upcoming)
        .OrderBy(m => m.Date)
        .ToList();

    public List<Match> GetCompletedMatches() => _matches
        .Where(m => m.Status == MatchStatus.Completed)
        .OrderByDescending(m => m.Date)
        .ToList();

    public List<Match> GetMatchesByTeam(int teamId) => _matches
        .Where(m => m.HomeTeam.Id == teamId || m.AwayTeam.Id == teamId)
        .OrderBy(m => m.Date)
        .ToList();

    public List<Match> GetWeeklyMatches(DateTime date)
    {
        var startOfWeek = date.Date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
        var endOfWeek = startOfWeek.AddDays(7);
        return _matches
            .Where(m => m.Date >= startOfWeek && m.Date < endOfWeek)
            .OrderBy(m => m.Date)
            .ToList();
    }

    public List<News> GetNews() => _news.OrderByDescending(n => n.PublishDate).ToList();

    public List<News> GetNewsByCategory(string category) => _news
        .Where(n => n.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
        .OrderByDescending(n => n.PublishDate)
        .ToList();

    public List<TopScorer> GetTopScorers() => _topScorers.OrderByDescending(t => t.Goals).ToList();

    public List<TeamStats> GetTeamStats() => _teamStats;

    public List<TeamStats> GetTeamTeamStats() => _teamStats;

    private List<Team> InitializeTeams() =>
    [
        new Team
        {
            Id = 1,
            Name = "南京队",
            ShortName = "南京",
            City = "南京",
            Coach = "何塞（西班牙）",
            Tactics = "4231控球传控体系，后场出球、中场层层递进",
            Formation = "4-2-3-1",
            ColorPrimary = "#C8102E",
            ColorSecondary = "#FFFFFF",
            FoundedYear = 2025,
            HomeStadium = "南京奥体中心",
            Capacity = 61443,
            Description = "传统劲旅，前4场仅入2球且全部为点球，运动战进球荒亟待打破",
            KeyPlayers =
            [
                new Player { Id = 101, Name = "滕帅", Position = "前锋", JerseyNumber = 9, Age = 26, Description = "点球主罚手，本赛季2粒进球均来自点球" },
                new Player { Id = 102, Name = "司俊远", Position = "边锋", JerseyNumber = 11, Age = 23, Description = "边路快马，突破犀利但转化率待提升" },
                new Player { Id = 103, Name = "曹翰晨", Position = "中场", JerseyNumber = 8, Age = 25, Description = "中场组织者，负责攻防转换" }
            ]
        },
        new Team
        {
            Id = 2,
            Name = "苏州队",
            ShortName = "苏州",
            City = "苏州",
            Coach = "陈婉婷",
            Tactics = "433高位逼抢+边路两翼齐飞，全员前场压迫",
            Formation = "4-3-3",
            ColorPrimary = "#005BA9",
            ColorSecondary = "#FFFFFF",
            FoundedYear = 2025,
            HomeStadium = "苏州奥体中心",
            Capacity = 45000,
            Description = "年轻化的经济强队，冲击力十足但稳定性欠缺",
            KeyPlayers =
            [
                new Player { Id = 201, Name = "吴宇帆", Position = "中锋", JerseyNumber = 9, Age = 25, Description = "支点前锋，门前抢点能力突出" },
                new Player { Id = 202, Name = "周鑫", Position = "左边锋", JerseyNumber = 11, Age = 23, Description = "昆山青训出品，内切远射能力突出" },
                new Player { Id = 203, Name = "沈佳豪", Position = "后腰", JerseyNumber = 6, Age = 24, Description = "中场绞肉机，抢断后快速攻防转换" }
            ]
        },
        new Team
        {
            Id = 3,
            Name = "无锡队",
            ShortName = "无锡",
            City = "无锡",
            Coach = "唐京",
            Tactics = "451稳守反击，5中场密集布防压缩中路空间",
            Formation = "4-5-1",
            ColorPrimary = "#2D3436",
            ColorSecondary = "#FDCB6E",
            FoundedYear = 2025,
            HomeStadium = "无锡体育中心",
            Capacity = 28000,
            Description = "目前唯一不败球队，防守铁军，积分榜稳居次席",
            KeyPlayers =
            [
                new Player { Id = 301, Name = "谢志伟", Position = "中卫", JerseyNumber = 4, Age = 26, Description = "后场定海神针，角球进攻利器" },
                new Player { Id = 302, Name = "卢则灵", Position = "前锋", JerseyNumber = 7, Age = 21, Description = "第9轮客场绝杀徐州，反击单刀专家" },
                new Player { Id = 303, Name = "蒋孟泽", Position = "中场", JerseyNumber = 10, Age = 24, Description = "中场发动机，直塞球精准" }
            ]
        },
        new Team
        {
            Id = 4,
            Name = "常州队",
            ShortName = "常州",
            City = "常州",
            Coach = "殷铁生（顾问）、周斌",
            Tactics = "4141全攻阵型，单后腰兜底，中前场五人联动短传渗透",
            Formation = "4-1-4-1",
            ColorPrimary = "#1E8449",
            ColorSecondary = "#FFFFFF",
            FoundedYear = 2025,
            HomeStadium = "常州奥体中心",
            Capacity = 36000,
            Description = "揭幕战3-0横扫南通，黑马姿态拉满，但状态起伏较大",
            KeyPlayers =
            [
                new Player { Id = 401, Name = "吉翔", Position = "右后卫", JerseyNumber = 24, Age = 34, Description = "前山东泰山、江苏队功勋，边路全能手" },
                new Player { Id = 402, Name = "苗润东", Position = "中前卫", JerseyNumber = 14, Age = 25, Description = "常晋中场核心，攻防全能" },
                new Player { Id = 403, Name = "叶文杰", Position = "前锋", JerseyNumber = 18, Age = 20, Description = "青年射手，门前终结效率联赛前列" }
            ]
        },
        new Team
        {
            Id = 5,
            Name = "南通队",
            ShortName = "南通",
            City = "南通",
            Coach = "葛勇",
            Tactics = "352三中卫+双边翼位，边翼位全攻全守",
            Formation = "3-5-2",
            ColorPrimary = "#2C3E50",
            ColorSecondary = "#E74C3C",
            FoundedYear = 2025,
            HomeStadium = "南通奥体中心",
            Capacity = 30000,
            Description = "上赛季亚军，本赛季进攻端严重乏力，目前仅积5分排名第10",
            KeyPlayers =
            [
                new Player { Id = 501, Name = "李贤成", Position = "中场", JerseyNumber = 8, Age = 24, Description = "核心中场，停赛缺阵对球队影响巨大" },
                new Player { Id = 502, Name = "潘万权", Position = "前锋", JerseyNumber = 9, Age = 25, Description = "锋线主力，射门曾击中立柱" },
                new Player { Id = 503, Name = "赵东旭", Position = "门将", JerseyNumber = 1, Age = 27, Description = "门线技术出色，多次救险" }
            ]
        },
        new Team
        {
            Id = 6,
            Name = "徐州队",
            ShortName = "徐州",
            City = "徐州",
            Coach = "胡云峰",
            Tactics = "442菱形中场，双后腰保护+两翼齐飞",
            Formation = "4-4-2",
            ColorPrimary = "#E67E22",
            ColorSecondary = "#FFFFFF",
            FoundedYear = 2025,
            HomeStadium = "徐州奥体中心",
            Capacity = 35000,
            Description = "苏北劲旅，第9轮主场被无锡绝杀，从积分榜次席滑落",
            KeyPlayers =
            [
                new Player { Id = 601, Name = "郑雪健", Position = "后腰", JerseyNumber = 6, Age = 26, Description = "徐州队长，任意球曾击中横梁险些扳平" },
                new Player { Id = 602, Name = "苗润东", Position = "前锋", JerseyNumber = 9, Age = 22, Description = "超新星，因国青队赛事曾缺阵" },
                new Player { Id = 603, Name = "刘洋", Position = "右前卫", JerseyNumber = 7, Age = 24, Description = "边路快马，传中精准" }
            ]
        },
        new Team
        {
            Id = 7,
            Name = "盐城队",
            ShortName = "盐城",
            City = "盐城",
            Coach = "德拉甘（塞尔维亚）",
            Tactics = "4231攻守平衡，强调快速攻防转换",
            Formation = "4-2-3-1",
            ColorPrimary = "#00A8FF",
            ColorSecondary = "#FFFFFF",
            FoundedYear = 2025,
            HomeStadium = "盐城体育中心",
            Capacity = 30000,
            Description = "五战全胜断层领跑，进7球仅失1球，苏超实力天花板",
            KeyPlayers =
            [
                new Player { Id = 701, Name = "李明", Position = "中锋", JerseyNumber = 10, Age = 25, Description = "赛季射手王，门前嗅觉敏锐" },
                new Player { Id = 702, Name = "赵鹏", Position = "后腰", JerseyNumber = 6, Age = 29, Description = "中场核心，调度能力强" },
                new Player { Id = 703, Name = "孙浩", Position = "左后卫", JerseyNumber = 3, Age = 23, Description = "助攻型边卫，插上能力强" }
            ]
        },
        new Team
        {
            Id = 8,
            Name = "扬州队",
            ShortName = "扬州",
            City = "扬州",
            Coach = "曹睿",
            Tactics = "532防守反击，稳固后防线",
            Formation = "5-3-2",
            ColorPrimary = "#8B4513",
            ColorSecondary = "#FFFFFF",
            FoundedYear = 2025,
            HomeStadium = "扬州体育公园",
            Capacity = 25000,
            Description = "积4分排名第11，保级形势严峻",
            KeyPlayers =
            [
                new Player { Id = 801, Name = "王强", Position = "中卫", JerseyNumber = 5, Age = 30, Description = "后防中坚，经验丰富" },
                new Player { Id = 802, Name = "李洋", Position = "前锋", JerseyNumber = 11, Age = 24, Description = "反击尖刀，速度快" },
                new Player { Id = 803, Name = "陈浩", Position = "中场", JerseyNumber = 14, Age = 26, Description = "攻防转换枢纽" }
            ]
        },
        new Team
        {
            Id = 9,
            Name = "镇江队",
            ShortName = "镇江",
            City = "镇江",
            Coach = "刘平豫",
            Tactics = "442平行站位，注重边路配合",
            Formation = "4-4-2",
            ColorPrimary = "#9B59B6",
            ColorSecondary = "#FFFFFF",
            FoundedYear = 2025,
            HomeStadium = "镇江体育会展中心",
            Capacity = 28000,
            Description = "5战全败积0分垫底，跨赛季16连败，急需止血",
            KeyPlayers =
            [
                new Player { Id = 901, Name = "周明", Position = "前腰", JerseyNumber = 10, Age = 25, Description = "中场大脑，传球精准" },
                new Player { Id = 902, Name = "黄磊", Position = "中锋", JerseyNumber = 9, Age = 27, Description = "禁区终结者" },
                new Player { Id = 903, Name = "陈明", Position = "右前卫", JerseyNumber = 7, Age = 23, Description = "边路突破手" }
            ]
        },
        new Team
        {
            Id = 10,
            Name = "泰州队",
            ShortName = "泰州",
            City = "泰州",
            Coach = "徐冀宁",
            Tactics = "343压迫式打法，前场逼抢凶狠",
            Formation = "3-4-3",
            ColorPrimary = "#1ABC9C",
            ColorSecondary = "#FFFFFF",
            FoundedYear = 2025,
            HomeStadium = "泰州体育中心",
            Capacity = 30000,
            Description = "卫冕冠军，第9轮绝杀南通终结连败，积6分升至第8",
            KeyPlayers =
            [
                new Player { Id = 1001, Name = "吴硕涛", Position = "前锋", JerseyNumber = 19, Age = 24, Description = "第9轮替补登场绝杀南通，终结连败功臣" },
                new Player { Id = 1002, Name = "刘俊伯", Position = "边锋", JerseyNumber = 11, Age = 23, Description = "送出精准传中助攻绝杀" },
                new Player { Id = 1003, Name = "白雪松", Position = "中场", JerseyNumber = 8, Age = 25, Description = "任意球高手，威胁球能力强" }
            ]
        },
        new Team
        {
            Id = 11,
            Name = "淮安队",
            ShortName = "淮安",
            City = "淮安",
            Coach = "裴恩才",
            Tactics = "442防守反击，作风硬朗",
            Formation = "4-4-2",
            ColorPrimary = "#F39C12",
            ColorSecondary = "#FFFFFF",
            FoundedYear = 2025,
            HomeStadium = "淮安奥体中心",
            Capacity = 35000,
            Description = "作风硬朗、防守坚韧，2胜1平1负积7分排名中游",
            KeyPlayers =
            [
                new Player { Id = 1101, Name = "何健", Position = "前锋", JerseyNumber = 9, Age = 28, Description = "头球能力出色，上赛季对阵南京扳平比分" },
                new Player { Id = 1102, Name = "杨帆", Position = "前锋", JerseyNumber = 15, Age = 25, Description = "机会主义者" },
                new Player { Id = 1103, Name = "林浩", Position = "后腰", JerseyNumber = 6, Age = 26, Description = "中场拦截器" }
            ]
        },
        new Team
        {
            Id = 12,
            Name = "连云港队",
            ShortName = "连云港",
            City = "连云港",
            Coach = "魏新",
            Tactics = "541密集防守，伺机反击",
            Formation = "5-4-1",
            ColorPrimary = "#27AE60",
            ColorSecondary = "#FFFFFF",
            FoundedYear = 2025,
            HomeStadium = "连云港体育中心",
            Capacity = 28000,
            Description = "赛季未尝胜绩仅积2分，排名第12",
            KeyPlayers =
            [
                new Player { Id = 1201, Name = "刘强", Position = "门将", JerseyNumber = 1, Age = 27, Description = "门线技术出色" },
                new Player { Id = 1202, Name = "张骋", Position = "前锋", JerseyNumber = 10, Age = 23, Description = "高中锋，上赛季曾头球破门绝杀扬州" },
                new Player { Id = 1203, Name = "张明", Position = "中卫", JerseyNumber = 5, Age = 29, Description = "后防经验丰富" }
            ]
        },
        new Team
        {
            Id = 13,
            Name = "宿迁队",
            ShortName = "宿迁",
            City = "宿迁",
            Coach = "裴恩才",
            Tactics = "442快速反击，前场压迫凶狠",
            Formation = "4-4-2",
            ColorPrimary = "#E74C3C",
            ColorSecondary = "#FFFFFF",
            FoundedYear = 2025,
            HomeStadium = "宿迁奥体中心",
            Capacity = 32000,
            Description = "3胜2负积9分排名第3，第9轮3-0大胜镇江重返三甲",
            KeyPlayers =
            [
                new Player { Id = 1301, Name = "高驰", Position = "前锋", JerseyNumber = 9, Age = 26, Description = "第9轮首开记录，嗅觉敏锐" },
                new Player { Id = 1302, Name = "殷国翔", Position = "中场", JerseyNumber = 10, Age = 24, Description = "第9轮替补登场扩大比分" },
                new Player { Id = 1303, Name = "陈力行", Position = "前锋", JerseyNumber = 11, Age = 22, Description = "第9轮补时阶段捅射破门，终结悬念" }
            ]
        }
    ];

    private List<Match> InitializeMatches()
    {
        var matches = new List<Match>();
        var teams = _teams;
        int matchId = 1;

        var completedMatches = new List<(int home, int away, int homeScore, int awayScore, int round, DateTime date)>
        {
            (4, 5, 3, 0, 1, new DateTime(2026, 4, 11)),
            (8, 2, 0, 1, 1, new DateTime(2026, 4, 11)),
            (3, 9, 3, 1, 1, new DateTime(2026, 4, 11)),
            (12, 7, 0, 2, 1, new DateTime(2026, 4, 11)),
            (13, 1, 2, 0, 2, new DateTime(2026, 4, 18)),
            (11, 8, 1, 1, 2, new DateTime(2026, 4, 18)),
            (6, 10, 3, 0, 2, new DateTime(2026, 4, 18)),
            (12, 3, 0, 0, 3, new DateTime(2026, 4, 25)),
            (5, 6, 0, 0, 3, new DateTime(2026, 4, 25)),
            (7, 13, 1, 0, 3, new DateTime(2026, 4, 25)),
            (1, 4, 2, 1, 4, new DateTime(2026, 5, 2)),
            (10, 8, 3, 1, 4, new DateTime(2026, 5, 2)),
            (2, 11, 0, 1, 4, new DateTime(2026, 5, 2)),
            (9, 7, 1, 2, 4, new DateTime(2026, 5, 2)),
            (3, 10, 3, 1, 5, new DateTime(2026, 5, 9)),
            (5, 1, 0, 0, 5, new DateTime(2026, 5, 9)),
            (6, 13, 1, 2, 5, new DateTime(2026, 5, 9)),
            (4, 11, 1, 2, 6, new DateTime(2026, 5, 16)),
            (2, 12, 2, 2, 6, new DateTime(2026, 5, 16)),
            (8, 9, 1, 0, 6, new DateTime(2026, 5, 16)),
            (11, 7, 0, 1, 7, new DateTime(2026, 5, 23)),
            (12, 6, 1, 3, 7, new DateTime(2026, 5, 23)),
            (13, 5, 1, 2, 7, new DateTime(2026, 5, 23)),
            (10, 2, 1, 2, 8, new DateTime(2026, 5, 30)),
            (7, 8, 1, 0, 8, new DateTime(2026, 5, 30)),
            (9, 4, 0, 2, 8, new DateTime(2026, 5, 30)),
            (3, 1, 0, 2, 8, new DateTime(2026, 5, 30)),
            (6, 3, 0, 1, 9, new DateTime(2026, 6, 13)),
            (13, 9, 3, 0, 9, new DateTime(2026, 6, 13)),
            (5, 10, 0, 1, 9, new DateTime(2026, 6, 13))
        };

        foreach (var cm in completedMatches)
        {
            matches.Add(new Match
            {
                Id = matchId++,
                HomeTeam = teams.First(t => t.Id == cm.home),
                AwayTeam = teams.First(t => t.Id == cm.away),
                Date = cm.date,
                Time = "19:40",
                Stadium = teams.First(t => t.Id == cm.home).HomeStadium,
                HomeScore = cm.homeScore,
                AwayScore = cm.awayScore,
                Status = MatchStatus.Completed,
                Round = cm.round
            });
        }

        var upcomingMatches = new List<(int home, int away, int round, DateTime date)>
        {
            (4, 7, 10, new DateTime(2026, 6, 20)),
            (8, 12, 10, new DateTime(2026, 6, 20)),
            (2, 5, 10, new DateTime(2026, 6, 20)),
            (1, 11, 10, new DateTime(2026, 6, 20))
        };

        foreach (var um in upcomingMatches)
        {
            matches.Add(new Match
            {
                Id = matchId++,
                HomeTeam = teams.First(t => t.Id == um.home),
                AwayTeam = teams.First(t => t.Id == um.away),
                Date = um.date,
                Time = "19:40",
                Stadium = teams.First(t => t.Id == um.home).HomeStadium,
                Status = MatchStatus.Upcoming,
                Round = um.round
            });
        }

        return matches;
    }

    private List<News> InitializeNews() =>
    [
        new News
        {
            Id = 1,
            Title = "苏超第十周前瞻：半程雨夜激战，各有各的破局命题",
            Summary = "端午佳节，苏超常规赛迎来半程关口。盐城能否延续不败金身？南京能否打破运动战进球荒？",
            Content = "当端午的粽叶香漫过江苏的街巷，2026江苏省城市足球联赛常规赛也走到了半程关口。第十周的四场对决将于6月20日同步打响...",
            PublishDate = new DateTime(2026, 6, 19),
            Category = "赛事前瞻",
            ImageUrl = ""
        },
        new News
        {
            Id = 2,
            Title = "盐城五连胜断层领跑，攻防体系成联赛标杆",
            Summary = "盐城队赛季至今五战全胜，进7球仅失1球，统治力尽显",
            Content = "盐城队在本赛季展现出的统治力无需多言。赛季至今五战全胜豪取15分，稳居榜首且身后优势巨大...",
            PublishDate = new DateTime(2026, 6, 18),
            Category = "球队动态",
            ImageUrl = ""
        },
        new News
        {
            Id = 3,
            Title = "南京队遭遇运动战进球荒，前4场2球均为点球",
            Summary = "滕帅点球独苗，主帅何塞面临严峻考验",
            Content = "南京队的进球荒让球迷揪心。赛季前四场比赛仅打入2球，且全部来自滕帅的点球破门，运动战进球至今颗粒无收...",
            PublishDate = new DateTime(2026, 6, 17),
            Category = "球队动态",
            ImageUrl = ""
        },
        new News
        {
            Id = 4,
            Title = "1-0、3-0！苏超疯狂一夜！泰州绝杀南通",
            Summary = "第九周三场比赛全部分出胜负，盐城未赛已稳居榜首",
            Content = "2026年6月13日晚，苏超第九周三场比赛同时在徐州、宿迁、南通三地开踢。徐州0-1无锡、宿迁3-0镇江、南通0-1泰州...",
            PublishDate = new DateTime(2026, 6, 14),
            Category = "赛事战报",
            ImageUrl = ""
        },
        new News
        {
            Id = 5,
            Title = "苏超购票新规：预约+摇号制，公平公正",
            Summary = "彻底取消分散抢票，由公证机构全程公证",
            Content = "2026苏超购票沿用统一规则——彻底取消分散抢票，改为「预约+摇号」制...",
            PublishDate = new DateTime(2026, 6, 15),
            Category = "票务信息",
            ImageUrl = ""
        }
    ];

    private List<TopScorer> InitializeTopScorers()
    {
        var teams = _teams;
        return
        [
            new TopScorer { Player = teams[6].KeyPlayers[0], Team = teams[6], Goals = 5, Assists = 1 },
            new TopScorer { Player = teams[12].KeyPlayers[0], Team = teams[12], Goals = 4, Assists = 1 },
            new TopScorer { Player = teams[9].KeyPlayers[0], Team = teams[9], Goals = 3, Assists = 1 },
            new TopScorer { Player = teams[2].KeyPlayers[1], Team = teams[2], Goals = 3, Assists = 1 },
            new TopScorer { Player = teams[1].KeyPlayers[0], Team = teams[1], Goals = 2, Assists = 2 },
            new TopScorer { Player = teams[5].KeyPlayers[0], Team = teams[5], Goals = 2, Assists = 1 },
            new TopScorer { Player = teams[12].KeyPlayers[1], Team = teams[12], Goals = 2, Assists = 0 },
            new TopScorer { Player = teams[3].KeyPlayers[2], Team = teams[3], Goals = 2, Assists = 0 }
        ];
    }

    private List<TeamStats> InitializeTeamStats()
    {
        var teams = _teams;
        return
        [
            new TeamStats { Team = teams[0], GoalsScored = 4, GoalsConceded = 3, CleanSheets = 2, YellowCards = 5, RedCards = 0, PossessionRate = 52.3, ShotsOnTarget = 18 },
            new TeamStats { Team = teams[1], GoalsScored = 5, GoalsConceded = 4, CleanSheets = 1, YellowCards = 6, RedCards = 0, PossessionRate = 54.1, ShotsOnTarget = 22 },
            new TeamStats { Team = teams[2], GoalsScored = 7, GoalsConceded = 4, CleanSheets = 2, YellowCards = 7, RedCards = 0, PossessionRate = 48.6, ShotsOnTarget = 24 },
            new TeamStats { Team = teams[3], GoalsScored = 7, GoalsConceded = 4, CleanSheets = 1, YellowCards = 8, RedCards = 0, PossessionRate = 51.2, ShotsOnTarget = 26 },
            new TeamStats { Team = teams[4], GoalsScored = 2, GoalsConceded = 5, CleanSheets = 2, YellowCards = 6, RedCards = 0, PossessionRate = 44.3, ShotsOnTarget = 14 },
            new TeamStats { Team = teams[5], GoalsScored = 7, GoalsConceded = 4, CleanSheets = 1, YellowCards = 9, RedCards = 0, PossessionRate = 49.8, ShotsOnTarget = 28 },
            new TeamStats { Team = teams[6], GoalsScored = 7, GoalsConceded = 1, CleanSheets = 4, YellowCards = 3, RedCards = 0, PossessionRate = 58.2, ShotsOnTarget = 32 },
            new TeamStats { Team = teams[7], GoalsScored = 3, GoalsConceded = 6, CleanSheets = 1, YellowCards = 7, RedCards = 0, PossessionRate = 41.5, ShotsOnTarget = 16 },
            new TeamStats { Team = teams[8], GoalsScored = 2, GoalsConceded = 11, CleanSheets = 0, YellowCards = 10, RedCards = 0, PossessionRate = 38.2, ShotsOnTarget = 12 },
            new TeamStats { Team = teams[9], GoalsScored = 6, GoalsConceded = 9, CleanSheets = 1, YellowCards = 8, RedCards = 1, PossessionRate = 47.1, ShotsOnTarget = 24 },
            new TeamStats { Team = teams[10], GoalsScored = 4, GoalsConceded = 3, CleanSheets = 2, YellowCards = 5, RedCards = 0, PossessionRate = 45.6, ShotsOnTarget = 20 },
            new TeamStats { Team = teams[11], GoalsScored = 3, GoalsConceded = 7, CleanSheets = 0, YellowCards = 6, RedCards = 0, PossessionRate = 39.8, ShotsOnTarget = 14 },
            new TeamStats { Team = teams[12], GoalsScored = 8, GoalsConceded = 4, CleanSheets = 2, YellowCards = 7, RedCards = 0, PossessionRate = 50.3, ShotsOnTarget = 26 }
        ];
    }
}
