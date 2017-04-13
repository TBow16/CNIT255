using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Collections;





namespace ConsoleApplication2
{
    class Program
    {
        ArrayList userList = new ArrayList();
        ArrayList rewardList = new ArrayList();

        public class Rank : Bonus
        {
            public Rank()
            {
                level = 0;
                score = 0;
            }

            private int level;
            private int score;

            public void increaseScore()
            {
                score = score + 5;
            }

            public int getScore()
            {
                return this.score;
            }

            public void increaseLevel()
            {
                level++;
            }

            private int getLevel()
            {
                return this.level;
            }

            public void addBonus(int x)
            {
                score = score + x;
            }
        }
        
        public class Rewards
        {
            private int levelRequirement;
            private string description;



        }

        public class UniqueRewards : Rewards
        {
            Redemtion re = new Redemtion();


        }

        public class Redemtion
        {
            Rewards r = new Rewards();

            private string redemptionDate;

        }

        public class ListedUsers
        {
            Rank ra = new Rank();

            private string user;

            public void setUser(string x)
            {
                user = x;
            }

            public string getUser()
            {
                return this.user;
            }

            public void increaseScore()
            {
                ra.increaseScore();
            }

            public int getScore()
            {
                return ra.getScore();
            }

            public void addBonus(int x)
            {
                ra.addBonus(x);
            }
        }

        public class UpgradedUser : ListedUsers
        {
            private bool extrasPermissions;

            public UpgradedUser()
            {
                extrasPermissions = true;
            }
        }

        public class MasterUser : UpgradedUser
        {
            private bool controlval;

            public MasterUser()
            {
                controlval = true;                
            }
        }

        static void Main(string[] args)
        {
            new Program().Start();
        }

        private DiscordClient _client;

        public void Start()
        {
            _client = new DiscordClient(x =>
            {
                x.AppName = "255 Project";
                x.AppUrl = "http://Hello.me";
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            _client.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Public;
            });

            _client.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    if (e.Channel.Name == "troll")
                    {
                        await e.Channel.SendMessage(e.Message.Text);
                    }
                }
            };

            _client.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    int c = searchArrayList(e.User.ToString());

                    if (c == userList.Count)
                    {
                        ListedUsers z = new ListedUsers();

                        await e.Channel.SendMessage("You have been added to the system.");
                        z.setUser(e.User.ToString());

                        userList.Add(z);
                    }

                    if (c < userList.Count)
                    {
                        ListedUsers z = (ListedUsers)userList[c];

                        z.increaseScore();

                        userList.RemoveAt(c);

                        userList.Add(z);
                    }
                }
            };

            var token = "Mjg2MjczMzYxODkyMzQzODE5.C5emvA.Lc-gsSz5ivsaUTvfQe-aw-lqhRM";

            CreateCommands();

            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(token, TokenType.Bot);
            });
        }

        public void CreateCommands()
        {
            var cService = _client.GetService<CommandService>();
            cService.CreateCommand("Ping")
                .Description("Returns Pong")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Pong");
                });

            cService.CreateCommand("Hello")
                .Description("Greets user")
                .Do(async (e) =>
                {
                    var toReturn = $"Hello {e.User.Mention}";
                    await e.Channel.SendMessage(toReturn);
                });

            cService.CreateCommand("Cat")
                .Description("Sends a cat to a Channel")
                .Do(async (e) =>
                {
                    await e.Channel.SendFile("Cat.jpg");
                    await e.Channel.SendMessage("Your cat has arrived!");
                });

            cService.CreateCommand("Roll")
                .Description("Rolls a die of your choice.")
                .Parameter("sides", ParameterType.Required)
                .Do(async (e) =>
                {
                    string sides = e.GetArg("sides");
                    int die;
                    if (int.TryParse(sides, out die) == false)
                    {
                        return;
                    }
                    int roll = Roll(die);
                    var Result = $"You rolled a {roll.ToString()} {e.User.Mention}";
                    await e.Channel.SendMessage(Result);
                    if (roll == die)
                    {
                        await e.Channel.SendMessage("Well Done!");
                    }
                    if (roll == 1)
                    {
                        await e.Channel.SendMessage("Better luck next time.");
                    }
                });

            cService.CreateCommand("Score")
                .Description("Gets your chat score")
                .Do(async (e) =>
                {
                    int x = searchArrayList(e.User.ToString());

                    ListedUsers temp = (ListedUsers)userList[x];

                    var output = $"Your Score is {temp.getScore()}";

                    await e.Channel.SendMessage(output);
                });
        }

        public void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Severity}] [{e.Severity}] {e.Message}");
        }

        private int Roll(int x)
        {
            int result;
            Random rnd = new Random();

            result = rnd.Next(1, x++);
            return result;
        }

        private int searchArrayList(string x)
        {
            int z;

            for (z = 0; z < userList.Count; z++)
            {
                ListedUsers Temp = (ListedUsers)userList[z];

                if (Temp.getUser().Equals(x))
                {
                    break;
                }
            }

            return z;
        }

        private void addScore(string x)
        {
            int z = searchArrayList(x);

            ListedUsers Temp = (ListedUsers)userList[z];

            Temp.increaseScore();
        }
    }

    interface Bonus
    {
        void addBonus(int x);
    }
}
