using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemsProgrammingAssesment
{
    abstract class User
    {
        public int ID { get; set; } = 0;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public int HighScore { get; set; } = 0;
        private string Rank
        {
            get { return Rank; }
            set
            {
                if (Rank.Equals("gold", StringComparison.OrdinalIgnoreCase) ||
                    Rank.Equals("silver", StringComparison.OrdinalIgnoreCase) ||
                    Rank.Equals("bronze", StringComparison.OrdinalIgnoreCase)) Rank = Rank;

                else Rank = "None";
            }
        }


        //Game Variables
        public int playerX = 9;
        public int playerY = 4;
        public int playerLength = 0;

        public List<char[]> gameMap = new List<char[]>
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
        public List<(int x, int y)> lastPositions;


        public User()
        {
            Username = "";
            Password = "";
            HighScore = 0;
            Rank = "None";
            lastPositions = new List<(int x, int y)> { (playerX, playerY) };
        }
        public User(int ID, string username, string password, int highScore, string rank)
        {
            this.ID = ID;
            this.Username = username;
            this.Password = password;
            this.HighScore = highScore;
            SetRank(rank);
            lastPositions = new List<(int x, int y)> { (playerX, playerY) };
        }


        public void SetRank(string rank)
        {
            if (this.Rank.ToLower() != "gold" || this.Rank.ToLower() != "silver" || this.Rank.ToLower() != "bronze")
                this.Rank = rank;
            else this.Rank = "None";
        }
        public string GetRank()
        {
            return Rank;
        }


        public void ViewLeaderboard()
        {

        }



        public virtual void playGame()
        {

        }
        public virtual bool movement(char lastInput)
        {
            return false;
        }
        public virtual void spawnNewApple()
        {

        }


    }



    class BronzeUser : User
    {
        public BronzeUser(int ID, string username, string password, int highScore)
        {
            this.ID = ID;
            this.Username = username;
            this.Password = password;
            this.HighScore = highScore;
            SetRank("Bronze");
        }

        public override void playGame()
        {
            Console.WriteLine("Bronze user does not have permissions to play this game.");
        }
    }



    class SilverUser : User
    {
        public SilverUser(int ID, string username, string password, int highScore)
        {
            this.ID = ID;
            this.Username = username;
            this.Password = password;
            this.HighScore = highScore;
            SetRank("Silver");
        }

        //////////////////////////////////////////////
        //////////All the snake game logic////////////
        /////////////////////////////////////////////
        public override void playGame()
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
                Console.Clear();

                dead = movement(lastInput);

                for (int i = 0; i < gameMap.Count(); i++)
                {
                    foreach (char c in gameMap[i]) Console.Write(c);
                    Console.Write('\n');
                }

                Thread.Sleep(250);
            }

            if (playerLength > HighScore) HighScore = playerLength;

            Console.Clear();
            return;
        }
        public override bool movement(char lastInput)
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

                playerLength = lastPositions.Count();

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
        public override void spawnNewApple()
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
    }



    class GoldUser : User
    {
        public GoldUser(int ID, string username, string password, int highScore)
        {
            this.ID = ID;
            this.Username = username;
            this.Password = password;
            this.HighScore = highScore;
            SetRank("Gold");
        }

        //////////////////////////////////////////////
        //////////All the snake game logic////////////
        /////////////////////////////////////////////
        public override void playGame()
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
                Console.Clear();

                dead = movement(lastInput);

                for (int i = 0; i < gameMap.Count(); i++)
                {
                    foreach (char c in gameMap[i]) Console.Write(c);
                    Console.Write('\n');
                }

                Thread.Sleep(250);
            }

            if (playerLength > HighScore) HighScore = playerLength;

            Console.Clear();
            return;
        }
        public override bool movement(char lastInput)
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

                playerLength = lastPositions.Count();

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
        public override void spawnNewApple()
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
    }
}
