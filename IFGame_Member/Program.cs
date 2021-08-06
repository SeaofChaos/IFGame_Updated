using System;
using System.Collections.Generic;
using System.Windows.Input;

//Things I need to do: disable input when text is outputting, make it so enter can skip text output,
//finish the settings menus, comment better, 
namespace IFGame
{
    public class GameMenu
    {
        public static int ScreenWidth = 100;    //How wide the text is able to be displayed on the console
        public static int TextSpeed = 20;       //How fast the text is printed out
        public static int DialogueSpeed = 1200; //How fast the different pieces of diologue are printed out (aka, the time between chunks of sentences)
        public int index = 0;                   //Keeps track of \which item in a menu is currently selected (the one with inverted colors)
        int lineBetweenChoices = 0;     //do you want lines between the menu options?
        public void startGame()
        {
            string[] titleMenu = { "Start", "Settings", "Controls", "Credits", "Exit" };    //title screen menu options
            string loadingDots = "...";                 //It doesn't need to load, but I like the way it looks haha
            GamePlay accessGamePlay = new GamePlay();   //GamePlay object instance
            int menuSelection = 0;                      //Keeps track of which item on a menu is selected
            int linestoErase = 0;                       //keeps track of how many lines of text need to be erased to allow the menu screens to be updated
            GameMenu menuObject = new GameMenu();       //GameMenu objet instance
            bool keepAbout = false;                     //Makes it so the "About" information above the title screen disappears after it prints out the first time
            ConsoleKeyInfo keyPress;                    //Keeps track of which key is pressed

            Console.CursorVisible = false;

            openingMessage();

            Console.Write("\nLoading");  //Adds a 1 second delay between the '.' after the loading message
            for (int i = 0; i < loadingDots.Length; i++)
            {
                System.Threading.Thread.Sleep(300);
                Console.Write(loadingDots[i]);
            }
            System.Threading.Thread.Sleep(800);
            Console.Clear();

            while (menuSelection != 5)  //while menu selection isn't "Exit"
            {
                if (keepAbout)
                {
                    Console.Clear();
                }
                Console.CursorVisible = false;
                titleMessage();

                //Console.Write("\n");

                menuSelection = menuObject.displayMenu(titleMenu);

                if (menuSelection == 1)  //"Start"
                {
                    Console.Clear();

                    accessGamePlay.runGame();
                }
                if (menuSelection == 2) //"Settings"
                {
                    menuObject.gameSettings();
                }
                if (menuSelection == 3) //"Controls" not yet implemented
                {

                }
                if (menuSelection == 4) //"About" prints off the information about the creator and maybe any assets used
                {
                    openingMessage();
                    linestoErase += 7;

                    Console.Write("\n\nPress Enter to continue back to main menu\n");
                    linestoErase += 3;

                    keyPress = Console.ReadKey();

                    while (keyPress.Key != ConsoleKey.Enter)
                    {
                        keyPress = Console.ReadKey();
                    }
                    linestoErase = 0;
                }
                keepAbout = true;
            }
            Environment.Exit(0);
        }

        static void openingMessage()
        {
            //put opening message here if you want one
        }

        /*
         * C# Menu Code idea from: https://www.youtube.com/watch?v=1ydSw4afA1o
         * Used in methods displayMenu() and drawMenu()
         */
        public int displayMenu(string[] menuOptions)
        {
            List<string> menuItems = new List<string>();
            int totalChoiceLines = menuOptions.Length;
            int stringItr;
            int choiceNumber = 0;   //Holds the index of the menu option that has been chosen
            int lineBetweenChoices = 0;

            for (int i = 0; i < menuOptions.Length; i++)
            {
                stringItr = 0;

                for (int j = 0; j < menuOptions[i].Length; j++)     //Combines the strings that contain a \n character to keep the number of characaters in a line accurate
                {
                    if ((stringItr > ScreenWidth) && (menuOptions[i][j] == ' '))
                    {
                        string temp = menuOptions[i].Substring(0, j) + "\n" + menuOptions[i].Substring(j);
                        menuOptions[i] = temp;
                        stringItr = 0;
                    }
                    stringItr++;
                }
            }
            stringItr = 0;

            //Keeps track of how many lines to erase depending on number of menu options
            if (lineBetweenChoices == 1)
            {
                totalChoiceLines += menuOptions.Length + 1;
            }

            for (int i = 0; i < menuOptions.Length; i++)
            {
                menuItems.Add(menuOptions[i]);
                totalChoiceLines += menuOptions[i].Split("\n").Length - 1;
            }

            for (int i = 0; i <= totalChoiceLines; i++)
            {
                Console.Write("\n");
            }

            while (choiceNumber == 0)
            {
                clearMenu(totalChoiceLines);
                choiceNumber = drawMenu(menuItems, lineBetweenChoices);
            }

            return choiceNumber;
        }

        public int displayMenu(string[] menuOptions, int displayLines)
        {
            List<string> menuItems = new List<string>();
            int totalChoiceLines = menuOptions.Length;
            int stringItr;
            int choiceNumber = 0;   //Holds the index of the menu option that has been chosen

            for (int i = 0; i < menuOptions.Length; i++)
            {
                stringItr = 0;

                for (int j = 0; j < menuOptions[i].Length; j++)     //Combines the strings that contain a \n character to keep the number of characaters in a line accurate
                {
                    if ((stringItr > ScreenWidth) && (menuOptions[i][j] == ' '))
                    {
                        string temp = menuOptions[i].Substring(0, j) + "\n" + menuOptions[i].Substring(j);
                        menuOptions[i] = temp;
                        stringItr = 0;
                    }
                    stringItr++;
                }
            }
            stringItr = 0;

            //Keeps track of how many lines to erase depending on number of menu options
            if (displayLines == 1)
            {
                totalChoiceLines += menuOptions.Length + 1;
            }

            for (int i = 0; i < menuOptions.Length; i++)
            {
                menuItems.Add(menuOptions[i]);
                totalChoiceLines += menuOptions[i].Split("\n").Length - 1;
            }

            for (int i = 0; i <= totalChoiceLines; i++)
            {
                Console.Write("\n");
            }

            while (choiceNumber == 0)
            {
                clearMenu(totalChoiceLines);
                choiceNumber = drawMenu(menuItems, displayLines);
            }

            return choiceNumber;
        }

        //Helper of displayMenu to keep track of the keypresses and which menu item is selected. Returns selected item's index            
        private int drawMenu(List<string> menuItems, int lineBetweenChoices)
        {
            for (int listIndex = 0; listIndex < menuItems.Count; listIndex++)  //Makes sure the index currently selected exists on the menu
            {
                if (listIndex == index)
                {
                    if (lineBetweenChoices == 1) //inverts the colors of the selected menu item
                    {
                        Console.Write("- - - - - - - - - - - - - - - - - - - - - - - - -\n");
                    }

                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;

                    Console.WriteLine(menuItems[listIndex]);
                }
                else
                {
                    if (lineBetweenChoices == 1)
                    {
                        Console.Write("- - - - - - - - - - - - - - - - - - - - - - - - -\n");
                    }

                    Console.WriteLine(menuItems[listIndex]);
                }


                Console.ResetColor();
            }

            if (lineBetweenChoices == 1)
            {
                Console.Write("- - - - - - - - - - - - - - - - - - - - - - - - -\n");
            }

            //determines what action to take on a menu depending on what key is pressed
            ConsoleKeyInfo keyPress = Console.ReadKey();

            if (keyPress.Key == ConsoleKey.DownArrow)
            {
                if (!(menuItems.Count - 1 == index))
                {
                    index++;
                }
            }

            else if (keyPress.Key == ConsoleKey.UpArrow)
            {
                if (!(index <= 0))
                {
                    index--;
                }
            }

            else if (keyPress.Key == ConsoleKey.Enter)
            {
                return index + 1;
            }
            else
            {
                return 0;
            }

            return 0;
        }

        static void titleMessage()
        {
            //put title here
        }

        //Implemented from https://stackoverflow.com/questions/8946808/can-console-clear-be-used-to-only-clear-a-line-instead-of-whole-console
        private static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        static private void clearMenu(int lines)
        {
            //clears however many lines the menu has printed out
            for (int i = 0; i < lines; i++)
            {
                ClearCurrentConsoleLine();
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }
        }

        //Outputs strings in a nice typing like format.
        public void slowtextOutput(string outputText)
        {
            int originalTextSpeed = GameMenu.TextSpeed;
            int letterCount = 0;    //keeps track of how many characters are in a single line to prevent going past the defined ScreenWidth
            System.Threading.Thread.Sleep(GameMenu.DialogueSpeed);

            for (int i = 0; i < outputText.Length; i++)  //prints out string with a small delay between each character
            {
                if (System.Windows.Input.Keyboard.IsKeyDown())  //i need to determine if key is being held down to speed up character printing
                {
                    GameMenu.TextSpeed *= 2;
                }
                else
                {
                    GameMenu.TextSpeed = originalTextSpeed;
                }

                System.Threading.Thread.Sleep(GameMenu.TextSpeed);
                Console.Write(outputText[i]);

                if (outputText[i] == '\\')
                {
                    if (outputText[i + 1] == 'n')
                    {
                        letterCount = 0;
                    }
                }

                if (letterCount != GameMenu.ScreenWidth)
                {
                    letterCount++;
                }

                if ((letterCount == GameMenu.ScreenWidth) && (outputText[i] == ' '))
                {
                    Console.Write("\n");
                    letterCount = 0;
                }
            }

            Console.Write("\n");
            System.Threading.Thread.Sleep(GameMenu.DialogueSpeed);
        }

        private void gameSettings()
        {
            //strings to hold the number before converting to 32 bit int
            string inputTextString;
            string inputLoadingString;
            string inputWidthString;
            int selection = 0;      //which menu item is selected

            //numbers of new speeds to change with the previous speed
            int inputWidth = ScreenWidth;
            int inputText = TextSpeed;
            int inputLoading;
            int selectedExit = 0;
            int linestoClear = 0;
            bool isNewValue = false;

            while (selectedExit == 0)
            {
                string[] settings = { $"Screen Width : {ScreenWidth}", $"Text Speed : {TextSpeed}", $"Loading Speed : {DialogueSpeed}", "Back to Main Menu" };
                selection = displayMenu(settings);
                linestoClear += 5;

                if (selection == 1)     //"Screen Width"
                {
                    while (!(isNewValue))
                    {
                        Console.Write($"\nWhat do you want to change the Screen Width to? (Default is 80) \nType in a number between 20 and 200: ");
                        inputWidthString = Console.ReadLine();
                        Int32.TryParse(inputWidthString, out inputWidth);
                        linestoClear += 2;

                        if ((inputWidth) >= 20 && (inputWidth <= 200))
                        {
                            isNewValue = true;
                            ScreenWidth = inputWidth;
                            Console.Write($"\nSuccessfully Changed Width to {ScreenWidth}");
                            linestoClear += 2;
                        }
                        else
                        {
                            Console.Write("\nPlease try entering the number again. Make sure it is between 20 and 200.\n");
                            linestoClear += 2;
                        }
                    }
                    System.Threading.Thread.Sleep(1500);
                    clearMenu(linestoClear);
                    linestoClear = 0;
                    isNewValue = false;
                }
                if (selection == 2)     //Text Speed
                {
                    while (!(isNewValue))
                    {
                        Console.Write($"\nWhat do you want to change the Text Speed to? (Default is 10) (The smaller the faster) \nType in a number between 1 and 30: ");
                        inputTextString = Console.ReadLine();
                        Int32.TryParse(inputTextString, out inputText);
                        linestoClear += 2;

                        if ((inputText >= 1) && (inputText <= 30))
                        {
                            isNewValue = true;
                            TextSpeed = inputText;
                            Console.Write($"\nSuccessfully Changed Text Speed to {TextSpeed}");
                            linestoClear += 2;
                        }
                        else
                        {
                            Console.Write("\nPlease try entering the number again. Make sure it is between 1 and 30.\n");
                            linestoClear += 2;
                        }
                    }
                    System.Threading.Thread.Sleep(1500);
                    clearMenu(linestoClear);
                    linestoClear = 0;
                    isNewValue = false;
                }
                if (selection == 3)     //Loading Speed
                {
                    while (!(isNewValue))
                    {
                        Console.Write($"\nWhat do you want to change the Loading Speed to? (Default is 800) (The smaller the faster) \nType in a number between 100 and 1500: ");
                        inputLoadingString = Console.ReadLine();
                        Int32.TryParse(inputLoadingString, out inputLoading);
                        linestoClear += 2;

                        if ((inputLoading >= 100) && (inputLoading <= 1500))
                        {
                            isNewValue = true;
                            DialogueSpeed = inputLoading;
                            Console.Write($"\nSuccessfully Changed Loading Text Speed to {DialogueSpeed}");
                            linestoClear += 2;
                        }
                        else
                        {
                            Console.Write("\nPlease try entering the number again. Make sure it is between 100 and 1500.\n");
                            linestoClear += 2;
                        }
                    }
                    System.Threading.Thread.Sleep(1500);
                    clearMenu(linestoClear);
                    linestoClear = 0;
                    isNewValue = false;
                }
                if (selection == 4)     //Back to Main Menu
                {
                    selectedExit = 1;
                }
            }
        }
    }
}
