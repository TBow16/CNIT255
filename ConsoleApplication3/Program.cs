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
        ArrayList pizzaList = new ArrayList();

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
            public Rewards()
            {

            }

            public Rewards(string x, int y)
            {
                name = x;
                levelRequirement = y;
            }

            private int levelRequirement;
            private string name;

            public string getName()
            {
                return this.name;
            }

            public int getLevel()
            {
                return this.levelRequirement;
            }

            public void setName(string x)
            {
                name = x;
            }

            public void setlevel(int x)
            {
                levelRequirement = x;
            }
        }

        //Rewards that can only be used once
        public class UniqueRewards : Rewards
        {
            Redemtion re = new Redemtion();

            private int Limit;
            private int amountUsed;

            public void setLimit(int x)
            {
                Limit = x;
            }

            public void increaceUsed()
            {
                amountUsed++;
            }

            public int getLimit()
            {
                return this.Limit;
            }

            public void setRemptionDate(string x)
            {
                re.setRemptionDate(x);
            }

            public void setUser(string x)
            {
                re.setUser(x);
            }

            public string getDate()
            {
                return re.getDate();
            }

            public string getUser()
            {
                return re.getUser();
            }

        }

        //Tracking when a unique reward is claimed
        public class Redemtion
        {
            private string redemptionDate;
            private string lastUser;

            public void setRemptionDate(string x)
            {
                redemptionDate = x;
            }

            public void setUser(string x)
            {
                lastUser = x;
            }

            public string getDate()
            {
                return this.redemptionDate;
            }

            public string getUser()
            {
                return this.lastUser;
            }

        }

        //Starting user class
        public class ListedUsers
        {
            public ListedUsers()
            {

            }

            public ListedUsers(string x)
            {
                user = x;
            }

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
            private bool extrasPermissions = true;

            public bool getPermission()
            {
                return this.extrasPermissions;
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

            public bool getControl()
            {
                return this.controlval;
            }
        }

        //Names of nearby pizza places
        public class Pizza
        {
            public Pizza()
            {

            }

            public Pizza(string x, string y, string z)
            {
                Name = x;
                Address = y;
                PhoneNumber = z;
            }
            
            private string Name;
            private string Address;
            private string PhoneNumber;

            public void setName(string x)
            {
                Name = x;
            }

            public void setAddress(string x)
            {
                Address = x;
            }

            public void setPhone(string x)
            {
                PhoneNumber = x;
            }

            public string getName()
            {
                return this.Name;
            }

            public string getAddress()
            {
                return this.Address;
            }

            public string getPhone()
            {
                return this.PhoneNumber;
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
                    int c = searchUserArrayList(e.User.ToString());

                    //Adds new user
                    if (c == userList.Count)
                    {
                        ListedUsers z = new ListedUsers();

                        await e.Channel.SendMessage("You have been added to the system.");
                        z.setUser(e.User.ToString());

                        userList.Add(z);
                    }

                    //Adds points to profile dependign on class
                    if (c < userList.Count)
                    {
                        if (classTest(c) == 1)
                        {
                            ListedUsers z = (ListedUsers)userList[c];

                            z.increaseScore();

                            //Adds points to user who is not at upgrade point
                            if (z.getScore() < 1000)
                            {
                                userList.RemoveAt(c);

                                userList.Add(z);
                            }

                            //Upgrades a users status
                            if (z.getScore() >= 1000)
                            {
                                userList.RemoveAt(c);

                                UpgradedUser x = new UpgradedUser();

                                x.setUser(e.User.ToString());

                                userList.Add(x);

                                await e.Channel.SendMessage("You have been promoted to upgraded user, but your score has been reset");
                            }
                        }

                        else if (classTest(c) == 2)
                        {
                            UpgradedUser z = (UpgradedUser)userList[c];

                            z.increaseScore();

                            userList.RemoveAt(c);

                            userList.Add(z);
                        }

                        else
                        {
                            MasterUser z = (MasterUser)userList[c];

                            z.increaseScore();

                            userList.RemoveAt(c);

                            userList.Add(z);
                        }
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
                    int x = searchUserArrayList(e.User.ToString());

                    ListedUsers temp = (ListedUsers)userList[x];

                    var output = $"Your Score is {temp.getScore()}";

                    await e.Channel.SendMessage(output);
                });

            //add Reward
            cService.CreateCommand("NewReward")
                .Description("Creates a new reward with the perameters of a name and level requirement")
                .Parameter("Name", ParameterType.Required)
                .Parameter("Level", ParameterType.Required)
                .Do(async (e) =>
                {
                    string Name = e.GetArg("Name");
                    int Level;

                    if (int.TryParse(e.GetArg("Level"), out Level) == false)
                    {
                        await e.Channel.SendMessage("Invaild Level");
                    }

                    Rewards x = new Rewards(Name, Level);

                    rewardList.Add(x);

                    await e.Channel.SendMessage("Reward has been added");
                });

            //List Rewards
            cService.CreateCommand("listrewards")
                .Description("Lists available rewards")
                .Do(async (e) =>
                {
                    int x = rewardList.Count;

                    for (int y = 0; y < x; y++)
                    {
                        Rewards c = (Rewards)rewardList[y];

                        string v = c.getName();

                        await e.Channel.SendMessage(v);
                    }
                });

            //Command to claim the reward
            cService.CreateCommand("claimreward")
                .Description("Notifies the server that you will be claiming said reward.")
                .Parameter("RewardName", ParameterType.Required)
                .Do(async (e) =>
                {
                    string x = e.GetArg("RewardName");
                    int y = searchRewardsArrayList(x);

                    Rewards Temp = (Rewards)rewardList[y];

                    var results = $"{e.User.Mention} has claimed the reward of {Temp.getName()}";

                    await e.Channel.SendMessage(results);

                });

            //add Unique Reward
            cService.CreateCommand("NewUniqueReward")
                .Description("Creates a new reward with the perameters of a name and level requirement and a limit of uses")
                .Parameter("Name", ParameterType.Required)
                .Parameter("Level", ParameterType.Required)
                .Parameter("Limit", ParameterType.Required)
                .Do(async (e) =>
                {
                    string Name = e.GetArg("Name");
                    int Level;
                    int limit;

                    if (int.TryParse(e.GetArg("Level"), out Level) == false)
                    {
                        await e.Channel.SendMessage("Invaild Level");
                    }

                    if (int.TryParse(e.GetArg("Limit"), out limit) == false)
                    {
                        await e.Channel.SendMessage("Invaild Limit");
                    }

                    UniqueRewards x = new UniqueRewards();

                    x.setName(Name);
                    x.setlevel(Level);
                    x.setLimit(limit);

                    rewardList.Add(x);

                    await e.Channel.SendMessage("Reward has been added");
                });

            //Promotion command increases the users score by a defined amount
            cService.CreateCommand("Promote")
                .Description("increases your score")
                .Parameter("Amount")
                .Do(async (e) =>
                {
                    int amount;

                    if (int.TryParse(e.GetArg("Amount"), out amount) == false)
                    {
                        await e.Channel.SendMessage("The amount to be added must be a valid whole number");
                    }
                    else
                    {
                        int x = searchUserArrayList(e.User.ToString());
                        ListedUsers y = (ListedUsers)userList[x];

                        y.addBonus(amount);

                        var result = $"Congratulations {e.User.Mention}! You have gained {amount} score.";
                        await e.Channel.SendMessage(result);
                    }
                });
            
            //adds pizza place
            cService.CreateCommand("addpizza")
                .Description("Add a pizza place to the list")
                .Parameter("Name", ParameterType.Required)
                .Parameter("Address", ParameterType.Required)
                .Parameter("Phone", ParameterType.Required)
                .Do(async (e) =>
                {
                    string name = e.GetArg("Name");
                    string address = e.GetArg("Address");
                    string phone = e.GetArg("Phone");

                    Pizza x = new Pizza(name, address, phone);

                    pizzaList.Add(x);

                    await e.Channel.SendMessage("Your pizza location has been added to the last.");
                });

            //lists pizza places
            cService.CreateCommand("listpizza")
                .Description("Lists the pizza places on the list")
                .Do(async (e) =>
                {
                    int y = pizzaList.Count;

                    for(int x = 0; x < y; x++)
                    {
                        Pizza temp = (Pizza)pizzaList[x];

                        string a = temp.getName();

                        await e.Channel.SendMessage(a);
                    }
                });

            //gives pizza place info
            cService.CreateCommand("Selectpizza")
                .Description("Gives the information for the selected pizza place.")
                .Parameter("Name", ParameterType.Required)
                .Do(async (e) =>
                {
                    string x = e.GetArg("Name");
                    int y = searchPizzaArrayList(x);
                    if (y == pizzaList.Count)
                    {
                        await e.Channel.SendMessage("Invaid pizza location");
                    }
                    else
                    {
                        Pizza temp = (Pizza)pizzaList[y];

                        var Result = $"Name: {temp.getName()} Phone: {temp.getPhone()} Address: {temp.getAddress()}";

                        await e.Channel.SendMessage(Result);
                    }
                });

        }

        public void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Severity}] [{e.Severity}] {e.Message}");
        }

        //Method to randomly select a number from 1 to a value defined by the user
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
        private int searchUserArrayList(string x)
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

        //Searches the Rewards arraylist
        private int searchRewardsArrayList(string x)
        {
            int z;

            for (z = 0; z < rewardList.Count; z++)
            {
                Rewards Temp = (Rewards)rewardList[z];

                if (Temp.getName().Equals(x))
                {
                    break;
                }
            }

            return z;
        }

        //Searches the Pizza arraylist
        private int searchPizzaArrayList(string x)
        {
            int z;

            for (z = 0; z < pizzaList.Count; z++)
            {
                Pizza Temp = (Pizza)pizzaList[z];

                if (Temp.getName().Equals(x))
                {
                    break;
                }
            }

            return z;
        }

        //Method to add score to a users profle
        private void addScore(string x)
        {
            int z = searchUserArrayList(x);

            ListedUsers Temp = (ListedUsers)userList[z];

            Temp.increaseScore();
        }

        //test class for type
        private int classTest(int x)
        {
            if (userList[x].GetType() == typeof(ListedUsers))
            {
                return 1;
            }
            else if (userList[x].GetType() == typeof(UpgradedUser))
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
    }

    //Interface to add extra points to a users profile so they can be boosted to ugraded user
    interface Bonus
    {
        void addBonus(int x);
    }
}
