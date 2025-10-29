using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;

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
            if (args[0].ToLower().Trim() == "login")
            {
                
            }
            login(args);

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
                    Console.WriteLine("Feature not implemented yet.");
                }
                else if (input == "exit")
                {
                    running = false;
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }
            }

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

                usersList.Add(user);
            }

            return usersList;
        }
        static void writeUserFile(int newHighScore)
        {
            List<UserClass> usersList = readUsersFile();

            for (int i = 0; i < usersList.Count(); i++)
            {
                if (usersList[i].username == currentUser.username && usersList[i].password == currentUser.password)
                {
                    usersList[i].highScore = newHighScore;

                    List<string> newLines = new List<string>();

                    foreach (UserClass user in usersList)
                    {
                        string line = $"{user.username} {user.password} {user.highScore}";
                        newLines.Add(line);
                    }

                    File.WriteAllLines("users.txt", newLines);
                }
            }
        }



        static void reciveArgs(string[] args)
        {
            if (args.Count() == 0)
            {
                Console.WriteLine("No arguments provided.");
                writeColour("Argument 1: Login or SignUp\n", ConsoleColor.Red);
                writeColour("Argument 2: Username\n", ConsoleColor.Red);
                writeColour("Argument 3: Password\n", ConsoleColor.Red);
            }
            else if (args[0].Trim().ToLower() == "help")
            {
                Console.WriteLine("Argument 1: Username");
                Console.WriteLine("Argument 2: Password");
            }
        }


        static void signUp()
        {

        }
        static void login(string[] args)
        {
            bool found = false;

            for (int tries = 0; tries < 3 && !found; tries++)
            {
                Console.Clear();

                Console.WriteLine("LOGIN");
                Console.Write("Username: ");
                string username = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();

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
        }



        static void playGame()
        {
            char lastInput = 'd';

            Task.Run(() =>
            {
                while (true)
                {
                    lastInput = Console.ReadKey(true).KeyChar;
                }
            });

            while (true)
            {
                //clear console
                Console.Clear();

                //move player
                movement(lastInput);

                //Write Game Map
                for (int i = 0; i < gameMap.Count(); i++)
                {
                    foreach (char c in gameMap[i]) Console.Write(c);
                    Console.Write('\n');
                }

                //wait half a second
                Thread.Sleep(250);
            }
        }
        static List<(int x, int y)> lastPositions = new List<(int x, int y)> { (playerX, playerY) };
        static void movement(char lastInput)
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



            if (gameMap[newY][newX] == '#') return;
            if (gameMap[newY][newX] == '\u2588') return;

            if (gameMap[newY][newX] == '@')
            {
                lastPositions.Insert(0, (newX, newY));

                gameMap[newY][newX] = '\u2588';

                playerY = newY;
                playerX = newX;

                spawnNewApple();

                return;
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

                return;
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



        static void writeColour(string text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.Write(text);
            Console.ResetColor();
            return;
        }
    }
}
