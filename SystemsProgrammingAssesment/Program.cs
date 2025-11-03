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
        static UserClass currentUser = new UserClass();
        static List<char[]> gameMap = new List<char[]>
        {
            new char[] {'#','#','#','#','#','#','#','#','#','#','#','#','#','#','#','#','#','#','#','#'},
            new char[] {'#','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','#'},
            new char[] {'#','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','#'},
            new char[] {'#','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','#'},
            new char[] {'#','.','.','.','.','.','.','.','.', '\u2588', '.','.','@','.','.','@','.','@','.','#'},
            new char[] {'#','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','#'},
            new char[] {'#','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','#'},
            new char[] {'#','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','#'},
            new char[] {'#','#','#','#','#','#','#','#','#','#','#','#','#','#','#','#','#','#','#','#'}
        };

        static int playerX = 9;
        static int playerY = 4;
        static int playerLength = 0;


        static void Main(string[] args)
        {
            reciveArgs(args);

            bool running = true;

            while (running)
            {
                Console.WriteLine($"Welcome {currentUser.username}, your high score is {currentUser.highScore}");
                Console.WriteLine("Enter: \"play\" to play snake");
                Console.WriteLine("Enter: \"join\" to join Group");
                Console.WriteLine("Enter: \"exit\" to exit");

                string input = Console.ReadLine().Trim().ToLower();

                if (input == "play")
                {
                    playGame();
                }
                else if (input == "join")
                {
                    // do sstuff
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

            writeUserFile(45);
            Environment.Exit(0);
        }



        static List<UserClass> readUsersFile()
        {
            if (!File.Exists("users.txt"))
            {
                File.Create("users.txt").Close();
            }

            List<UserClass> usersList = new List<UserClass>();

            string[] userFileLines = File.ReadAllLines("users.txt");

            foreach (string line in userFileLines)
            {
                line.Trim();
                string[] lineSplit = line.Split(' ');

                UserClass user = new UserClass();
                user.username = lineSplit[0];
                user.password = lineSplit[1];
                user.highScore = Convert.ToInt32(lineSplit[2]);
                //user.isInGroup = Convert.ToBoolean(lineSplit[3]);
                //user.groupName = lineSplit[4];

                usersList.Add(user);
            }

            return usersList;
        }
        static void writeUserFile(int newHighScore)
        {
            bool found = false;
            int userIndex = 0;
            List<UserClass> usersList = readUsersFile();

            for (int i = 0; i < usersList.Count(); i++)
            {
                if (usersList[i].username == currentUser.username && usersList[i].password == currentUser.password)
                {
                    found = true;
                    userIndex = i;
                }
            }

            if (found)
            {
                usersList[userIndex].highScore = newHighScore;

                List<string> newLines = new List<string>();

                foreach (UserClass user in usersList)
                {
                    string line = $"{user.username} {user.password} {user.highScore}";
                    newLines.Add(line);
                }

                File.WriteAllLines("users.txt", newLines);
            }
            else
            {
                File.AppendAllText("users.txt", $"{currentUser.username} {currentUser.password}" +
                    $" {newHighScore} {currentUser.isInGroup} {currentUser.groupName}\n");
            }
        }



        static void reciveArgs(string[] args)
        {
            //string formattedInput = args[0].ToLower().Trim().Replace(" ", "");

            if (args[0].ToLower().Trim() == "login")
            {
                login(args);
            }
            else if (args[0].ToLower().Trim() == "signup")
            {
                signUp(args);
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


        static void signUp(string[] args)
        {
            bool alreadyExists = false;

            string username = args[1];
            string password = args[2];

            List<UserClass> users = readUsersFile();

            for (int i = 0; i < users.Count(); i++)
            {
                if (users[i].username == username && users[i].password == password)
                {
                    currentUser = users[i];
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
            {
                currentUser.username = username;
                currentUser.password = password;
                currentUser.highScore = 0;
                currentUser.isInGroup = false;
            }
        }
        static void login(string[] args)
        {
            bool found = false;

            Console.Clear();

            string username = args[1];
            string password = args[2];

            List<UserClass> users = readUsersFile();

            for (int i = 0; i < users.Count(); i++)
            {
                if (users[i].username == username && users[i].password == password)
                {
                    currentUser = users[i];
                    found = true;
                    break;
                }
            }
        }



        static void playGame()
        {
            char lastInput = 'd';
            bool dead = false;

            Task.Run(() =>
            {
                while (!dead)
                {
                    lastInput = Console.ReadKey(true).KeyChar;
                }
            });

            while (!dead)
            {
                //clear console
                Console.Clear();

                //move player
                dead = movement(lastInput);

                //Write Game Map
                for (int i = 0; i < gameMap.Count(); i++)
                {
                    foreach (char c in gameMap[i]) Console.Write(c);
                    Console.Write('\n');
                }

                //wait half a second
                Thread.Sleep(250);
            }

            Console.Clear();
            return;
        }

        static List<(int x, int y)> lastPositions = new List<(int x, int y)> { (playerX, playerY) };
        static bool movement(char lastInput)
        {

            Dictionary<char, (int x, int y)> movementKey = new Dictionary<char, (int X, int y)>()
            {
                {'w', (0,-1) },
                {'a', (-1,0) },
                {'s', (0,1) },
                {'d', (1,0) }
            };
            Dictionary<char, int> otherKeys = new Dictionary<char, int>()
            {
                {'e', 0 }
            };
            int newX = 0;
            int newY = 0;



            newX = playerX + movementKey[lastInput].x;
            newY = playerY + movementKey[lastInput].y;



            if (gameMap[newY][newX] == '#') return true;
            if (gameMap[newY][newX] == '\u2588') return true;

            if (gameMap[newY][newX] == '@')
            {
                lastPositions.Insert(0, (newX, newY));

                gameMap[newY][newX] = '\u2588';

                playerY = newY;
                playerX = newX;

                spawnNewApple();

                return false;
            }
            else
            {
                (int x, int y) tail = lastPositions[lastPositions.Count() - 1];

                lastPositions.RemoveAt(lastPositions.Count() - 1);
                lastPositions.Insert(0, (newX, newY));

                gameMap[tail.y][tail.x] = '.';
                gameMap[newY][newX] = '\u2588';

                playerY = newY;
                playerX = newX;

                return false;
            }

        }
        static void spawnNewApple()
        {
            (int x, int y) appleCoords;
            bool invalidPosition = true;
            Random rng = new Random();

            while (invalidPosition)
            {
                appleCoords.x = rng.Next(1, gameMap[0].Count() - 2);
                appleCoords.y = rng.Next(1, gameMap.Count() - 2);

                if (gameMap[appleCoords.y][appleCoords.x] == '.')
                {
                    gameMap[appleCoords.y][appleCoords.x] = '@';
                    invalidPosition = false;
                }
            }
        }


        /// <summary>
        /// 
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
