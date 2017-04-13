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

        //Rank Class allows leveling system to be implemented in to the users
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
        
        //Rewards that can be redeemed many times
        public class Rewards
        {
            private int levelRequirement;
            private string description;



        }

        //Rewards that can only be used once
        public class UniqueRewards : Rewards
        {
            Redemtion re = new Redemtion();


        }

        //Tracking when a unique reward is claimed
        public class Redemtion
        {
            Rewards r = new Rewards();

            private string redemptionDate;

        }

        //Starting user class
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

        //After a certian level users are upgraded
        public class UpgradedUser : ListedUsers
        {
            private bool extrasPermissions;

            public UpgradedUser()
            {
                extrasPermissions = true;
            }
        }

        //Way of seperating admins from users
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
            //Creates Bot
            _client = new DiscordClient(x =>
            {
                x.AppName = "255 Project";
                x.AppUrl = "http://Hello.me";
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            //Defines command Prefix
            _client.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Public;
            });

            //Creates an echo chamber
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

            //Increases users score when speaking and adds new users to a list
            _client.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    int c = searchArrayList(e.User.ToString());

                    //Adds new user
                    if (c == userList.Count)
                    {
                        ListedUsers z = new ListedUsers();

                        await e.Channel.SendMessage("You have been added to the system.");
                        z.setUser(e.User.ToString());

                        userList.Add(z);
                    }

                    //Adds points to profile
                    if (c < userList.Count)
                    {
                        ListedUsers z = (ListedUsers)userList[c];

                        z.increaseScore();

                        userList.RemoveAt(c);

                        userList.Add(z);
                    }
                }
            };

            //Token for connection to bot
            var token = "Mjg2MjczMzYxODkyMzQzODE5.C5emvA.Lc-gsSz5ivsaUTvfQe-aw-lqhRM";

            //Allows for commeand creation 
            CreateCommands();

            //Connects bot to server
            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(token, TokenType.Bot);
            });
        }

        //User created commands
        public void CreateCommands()
        {
            var cService = _client.GetService<CommandService>();
            
            //Ping 
            cService.CreateCommand("Ping")
                .Description("Returns Pong")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Pong");
                });

            //Hello
            cService.CreateCommand("Hello")
                .Description("Greets user")
                .Do(async (e) =>
                {
                    var toReturn = $"Hello {e.User.Mention}";
                    await e.Channel.SendMessage(toReturn);
                });

            //Cat
            cService.CreateCommand("Cat")
                .Description("Sends a cat to a Channel")
                .Do(async (e) =>
                {
                    await e.Channel.SendFile("Cat.jpg");
                    await e.Channel.SendMessage("Your cat has arrived!");
                });

            //Roll
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

            //Score
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

        //Methoed torandomly seect a number from 1 to a defines value
        //Used in the Roll Methoed
        private int Roll(int x)
        {
            int result;
            Random rnd = new Random();

            result = rnd.Next(1, x++);
            return result;
        }

        //Searches for users in a list
        //Used in the process of add user not listed and define whoes to add points to
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
        
        //Methoed to add score to a users profle
        private void addScore(string x)
        {
            int z = searchArrayList(x);

            ListedUsers Temp = (ListedUsers)userList[z];

            Temp.increaseScore();
        }
    }

    //Interface to add extra points to a users profile so they can be boosted to ugraded user
    interface Bonus
    {
        void addBonus(int x);
    }
}
