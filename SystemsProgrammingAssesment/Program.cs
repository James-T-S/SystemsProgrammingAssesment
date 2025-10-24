using System;
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
            new char[] {'#','.','.','.','.','.','.','.','.', '\u2588', '.','.','@','.','.','.','.','.','.','#'},
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
            //login();

            //Console.WriteLine($"Welcome {currentUser.username}, your high score is {currentUser.highScore}");

            char lastInput = 'd';

            //get input in another thread so the while loop can still run
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
                Console.WriteLine(movement(lastInput));

                //Write Game Map
                for (int i = 0; i < gameMap.Count(); i++)
                {
                    foreach (char c in gameMap[i]) Console.Write(c);
                    Console.Write('\n');
                }

                //wait half a second
                Thread.Sleep(500);
            }
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



        static void login()
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



        static int movement(char lastInput)
        {
            char player = '\u2588';
            
            HashSet<(int x, int y)> lastPositions = new HashSet<(int x, int y)> { (playerX, playerY) };
            List<(int x, int y)> lastPositionsList = lastPositions.ToList();

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

            if (gameMap[playerY + movementKey[lastInput].y][playerX + movementKey[lastInput].x] == '@') playerLength++;
            if (gameMap[playerY + movementKey[lastInput].y][playerX + movementKey[lastInput].x] == '#') return playerLength;


            foreach ((int x, int y) pos in lastPositions) gameMap[pos.y][pos.x] = '.';

            playerY += movementKey[lastInput].y;
            playerX += movementKey[lastInput].x;

            lastPositions.Add((playerX, playerY));

            for (int i = 0; i < playerLength && i < lastPositions.Count(); i++) gameMap[lastPositionsList[i].y][lastPositionsList[i].x] = player;

            return playerLength;
        }
    }
}
