using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;
using static System.Net.Mime.MediaTypeNames;

namespace SystemsProgrammingAssesment
{
    internal class Program
    {
        static User? currentUser;

        static void Main(string[] args)
        {
            ReciveArgs(args);

            bool running = true;

            while (running)
            {
                Console.WriteLine($"Welcome {currentUser.Username}, your high score is {currentUser.HighScore}");
                Console.WriteLine("Enter: \"play\" to play snake");
                Console.WriteLine("Enter: \"join\" to join Group");
                Console.WriteLine("Enter: \"exit\" to exit");

                string input = Console.ReadLine().Trim().ToLower();

                if (input == "play")
                {
                    currentUser.playGame();
                }
                else if (input == "join")
                {
                    Console.WriteLine("Enter a group name to join:");
                }
                else if (input == "exit")
                {
                    running = false;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input.\n\n");
                }
            }
            Environment.Exit(0);
        }


        //////////////////////////////////////////////
        ///////////Read and Write txt File///////////
        //////////////////////////////////////////////
        static List<Clan> ReadClanFiles()
        {
            string[] FileLines = Array.Empty<string>();
            List<string> Users = new List<string>();
            List<Clan> ClansList = new List<Clan>();

            try
            {
                FileLines = File.ReadAllLines("users.txt");
            }
            catch (FileNotFoundException exception)
            {
                File.Create("users.txt").Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex.Message}");
            }

            Parallel.ForEach(FileLines, line =>
            {
                if (line.Trim() == "  ")
                {

                }
                line.Trim();
                string[] lineSplit = line.Split(' ');

                Clan clan = new Clan(Convert.ToInt32(lineSplit[0]), lineSplit[1],
                    lineSplit[2], Convert.ToInt32(lineSplit[3]));

                ClansList.Add(clan);
            });
            return ClansList;
        }
        static List<User> ReadUsersFile()
        {
            string[] FileLines = Array.Empty<string>();
            List<User> usersList = new List<User>();

            try
            {
                FileLines = File.ReadAllLines("users.txt");
            }
            catch (FileNotFoundException exception)
            {
                File.Create("users.txt").Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex.Message}");
            }

            Parallel.ForEach(FileLines, line =>
            {
                line.Trim();
                string[] lineSplit = line.Split(' ');

                switch (lineSplit[0])
                {
                    case "bronze":
                        currentUser = new BronzeUser(Convert.ToInt32(lineSplit[0]), lineSplit[1], lineSplit[2], Convert.ToInt32(lineSplit[3]));
                        usersList.Add(currentUser);
                        break;

                    case "silver":
                        currentUser = new SilverUser(Convert.ToInt32(lineSplit[0]), lineSplit[1], lineSplit[2], Convert.ToInt32(lineSplit[3]));
                        usersList.Add(currentUser);
                        break;

                    case "gold":
                        currentUser = new GoldUser(Convert.ToInt32(lineSplit[0]), lineSplit[1], lineSplit[2], Convert.ToInt32(lineSplit[3]));
                        usersList.Add(currentUser);
                        break;
                }
            });
            return usersList;
        }
        static void WriteUserFile(int newHighScore)
        {
            bool found = false;
            int userIndex = 0;
            List<User> usersList = ReadUsersFile();

            for (int i = 0; i < usersList.Count(); i++)
            {
                if (usersList[i].Username == currentUser.Username && usersList[i].Password == currentUser.Password)
                {
                    found = true;
                    userIndex = i;
                }
            }

            if (found)
            {
                usersList[userIndex].HighScore = newHighScore;

                List<string> newLines = new List<string>();

                foreach (User user in usersList)
                {
                    string line = $"{user.Username} {user.Password} {user.HighScore}";
                    newLines.Add(line);
                }

                File.WriteAllLines("users.txt", newLines);
            }
            else
            {
                File.AppendAllText("users.txt", $"{currentUser.Username} {currentUser.Password}" +
                    $" {newHighScore}\n");
            }
        }



        //////////////////////////////////////////////
        ///////////Sign in Login In System////////////
        //////////////////////////////////////////////
        static void ReciveArgs(string[] args)
        {
            //string formattedInput = args[0].ToLower().Trim().Replace(" ", "");

            if (args[0].ToLower().Trim() == "login")
            {
                Login(args);
            }
            else if (args[0].ToLower().Trim() == "signup")
            {
                SignUp(args);
            }
            else if (args.Count() == 0)
            {
                Console.WriteLine("No arguments provided.");
                writeColour("Argument 1: Login or SignUp\n", ConsoleColor.Red);
                writeColour("Argument 2: Username\n", ConsoleColor.Red);
                writeColour("Argument 3: Password\n", ConsoleColor.Red);
                Environment.Exit(0);
            }
            else if (args[0].Trim().ToLower() == "help")
            {
                Console.WriteLine("Argument 1: Login or SignUp\n");
                Console.WriteLine("Argument 2: Username\n");
                Console.WriteLine("Argument 3: Password\n");
                Environment.Exit(0);
            }
        }
        static void SignUp(string[] args)
        {
            bool alreadyExists = false;

            string username = args[1];
            string password = args[2];

            List<User> users = ReadUsersFile();

            for (int i = 0; i < users.Count(); i++)
            {
                if (users[i].Username == username && users[i].Password == password)
                {
                    currentUser = users[i];
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
            {
                currentUser.Username = username;
                currentUser.Password = password;
                currentUser.HighScore = 0;
                currentUser.SetRank("gold");
            }
        }
        static void Login(string[] args)
        {
            Console.Clear();

            string username = args[1];
            string password = args[2];

            List<User> users = ReadUsersFile();

            for (int i = 0; i < users.Count(); i++)
            {
                if (users[i].Username == username && users[i].Password == password)
                {
                    currentUser = users[i];
                    break;
                }
            }
        }







        /// <summary>
        /// Function to write coloured text to the console
        /// </summary>
        /// <param name="text">Text that will change colour</param>
        /// <param name="colour">the ConsoleColor to change it to</param>
        static void writeColour(string text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.Write(text);
            Console.ResetColor();
            return;
        }
    }
}
