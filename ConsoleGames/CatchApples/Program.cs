using System;
using System.Collections.Generic;
using System.Threading;

class Object
{
    public int x;
    public int y;
    public char symbol;
    public ConsoleColor color;

    public Object(int x, int y, char symbol, ConsoleColor color)
    {
        this.x = x;
        this.y = y;
        this.symbol = symbol;
        this.color = color;
    }
}

class Program
{
    static int padSize = 10;
    static int playerPos = 0; // x position
    static int playerScore = 0;
    static int playerLives = 3;

    static void SetInitialPos()
    {
        Console.BufferHeight = Console.WindowHeight;
        Console.BufferWidth = Console.WindowWidth;
        playerPos = Console.WindowWidth / 2 - padSize / 2;
    }
    static void PrintObjAtPos(int x, int y, char symbol, ConsoleColor color = ConsoleColor.Yellow)
    {
        Console.ForegroundColor = color;
        Console.SetCursorPosition(x, y);
        Console.WriteLine(symbol);
    }
    static void PrintPlayer()
    {
        for (int x = playerPos; x < playerPos + padSize; x++)
        {
            PrintObjAtPos(x, Console.WindowHeight - 3, '-');
        }
    }
    static void GenerateSkyObject(Random randGenerator, List<Object> skyObjects)
    {
        int genChance = randGenerator.Next(0, 101);
        int objChance = randGenerator.Next(0, 101);
        if (genChance <= 10) // 10% chance of dropping items
        {
            if (objChance <= 70) // 70% apple
            {
                Object newApple = new Object(randGenerator.Next(Console.WindowLeft, Console.WindowLeft + Console.WindowWidth),
                    Console.WindowTop, 'O', ConsoleColor.Red);
                skyObjects.Add(newApple);
            }
            else // 30% bomb
            {
                Object newBomb = new Object(randGenerator.Next(Console.WindowLeft, Console.WindowLeft + Console.WindowWidth),
                    Console.WindowTop, '@', ConsoleColor.Blue);
                skyObjects.Add(newBomb);
            }
        }
    }
    static void UpdateObjects(List<Object> skyObjects)
    {
        // moving, checking for collisions,...
        List<Object> objectsToRemove = new List<Object>();

        foreach (Object obj in skyObjects)
        {
            // object hit player
            if (obj.y >= Console.WindowHeight - 3 && playerPos <= obj.x && playerPos+padSize >= obj.x)
            {
                if (obj.symbol == 'O')
                {
                    playerScore++;
                }
                else if (obj.symbol == '@')
                {
                    playerLives--;
                }
                objectsToRemove.Add(obj);
                continue;
            }
            // object reaches end
            if (obj.y == Console.WindowHeight)
            {
                objectsToRemove.Add(obj);
                continue;
            }

            PrintObjAtPos(obj.x, obj.y, obj.symbol, obj.color);
            obj.y++;
        }

        foreach (Object objToRemove in objectsToRemove)
        {
            skyObjects.Remove(objToRemove);
        }
    }
    static void PrintStatus()
    {
        Console.ForegroundColor = ConsoleColor.White;
        string scoreText = $"SCORE: | {playerScore} |";
        string livesText = $"LIVES: | {playerLives} |";
        Console.SetCursorPosition(Console.WindowLeft + 5, 1);
        Console.Write(scoreText);
        Console.SetCursorPosition(Console.WindowWidth - livesText.Length - 5, 1);
        Console.Write(livesText);
    }
    static bool CheckEnd()
    {
        if (playerLives == 0)
        {
            return true;
        }
        return false;
    }
    static void PrintEndScore()
    {
        string endScoreText = $"You scored {playerScore} points. Press any key to restart";
        Console.Clear();
        Console.SetCursorPosition(Console.WindowWidth / 2 - endScoreText.Length / 2, Console.WindowHeight / 2);
        Console.ForegroundColor= ConsoleColor.White;
        Console.Write(endScoreText);
        Thread.Sleep(5000);
        Console.ReadKey();
        // reseting lives and scores
        playerLives = 3;
        playerScore = 0;
    }
    static void MovePadRight()
    {
        if (playerPos < Console.WindowWidth - padSize)
        {
            playerPos++;
        }
    }
    static void MovePadLeft()
    {
        if (playerPos > 0)
        {
            playerPos--;
        }
    }
    static void Main()
    {        
        SetInitialPos();
        Random randGenerator = new Random();
        List<Object> skyObjects = new List<Object>();
        
        while (true)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    MovePadRight();
                }
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    MovePadLeft();
                }
            }
            Console.Clear();
            GenerateSkyObject(randGenerator, skyObjects);
            UpdateObjects(skyObjects);
            PrintPlayer();
            PrintStatus();
            if (CheckEnd())
            {
                PrintEndScore();
                skyObjects.Clear();
                SetInitialPos();
            }
            Thread.Sleep(200);
        }
    }
}
