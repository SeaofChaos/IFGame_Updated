using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

//Things I need to do: finish the settings menus, comment better, work on the custom file input, update
//the github to make it look like an actual project, 

//I so far am able to open a file and put the file contents into a string, but I need to create
//a system in order to perform actions based on certain characters or commands in the file.
//For example, /d could be to indicate dialogue following. /m could indicate menu options separated
//by a '.' or '\n'. I should also implement a /help command first. I should also add a way to allow for
//custom art to be added (the art has to use printable characters.
namespace IFGame
{
    public class gameOption
    {
        public String dialogue;
        public String menu;

        public gameOption()
        {
            dialogue = String.Empty;
            menu = String.Empty;
        }

        public gameOption(String d, String m, int i){
            dialogue = d;
            menu = m;
        }
    }
    public class GameMenu
    {
        public static int ScreenWidth = 100;    //How wide the text is able to be displayed on the console
        public static int TextSpeed = 20;       //How fast the text is printed out
        public static int DialogueSpeed = 1200; //How fast the different pieces of diologue are printed out (aka, the time between chunks of sentences)
        public int index = 0;                   //Keeps track of \which item in a menu is currently selected (the one with inverted colors)
        public void startGame()
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool AllocConsole();

            AllocConsole();

            string[] titleMenu = { "Start", "Settings", "Controls", "Open File", "Credits", "Exit" };    //title screen menu options
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

            while (menuSelection != 6)  //while menu selection isn't "Exit"
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
                if (menuSelection == 4)
                {
                    String fileContent = getFile();

                    //checks for invalid file, i should do this as an else in the member function cause
                    //there is an if statement that checks if the file is valid
                    if (fileContent == String.Empty) 
                    {
                        Console.WriteLine("\nFile is invalid or empty. Returning to main menu...");
                        System.Threading.Thread.Sleep(1500);
                    }

                    runFromFile(fileContent);
                    //I need to do something with this file string /\
                }
                if (menuSelection == 5) //"About" prints off the information about the creator and maybe any assets used
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

        public int displayMenu(string[] menuOptions)    //used for main menu
        {
            List<string> menuItems = new List<string>();
            int totalChoiceLines = menuOptions.Length;
            int stringItr;
            int choiceNumber = 0;   //Holds the index of the menu option that has been chosen
            int lineBetweenChoices = 0;

            for (int i = 0; i < menuOptions.Length; i++) //removes \n character from each menu string
            {
                stringItr = 0;

                //Combines the characters into a string without the \n characters
                for (int j = 0; j < menuOptions[i].Length; j++)
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
                totalChoiceLines += menuOptions.Length + 1;

            for (int i = 0; i < menuOptions.Length; i++)
            {
                menuItems.Add(menuOptions[i]);
                totalChoiceLines += menuOptions[i].Split("\n").Length - 1;
            }

            for (int i = 0; i <= totalChoiceLines; i++)
                Console.Write("\n");

            while (choiceNumber == 0 && !(
                Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter
                && Console.ReadKey(true).Key == ConsoleKey.UpArrow
                && Console.ReadKey(true).Key == ConsoleKey.DownArrow))
            {
                choiceNumber = drawMenu(menuItems, lineBetweenChoices, totalChoiceLines);
            }
            return choiceNumber;
        }

        public int displayMenu(string[] menuOptions, int displayLines)  //used for game menu
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

            while (Console.KeyAvailable)
                Console.ReadKey(true);

            choiceNumber = drawMenu(menuItems, displayLines, totalChoiceLines);

            return choiceNumber;
        }

        //Helper of displayMenu to keep track of the keypresses and which menu item is selected. Returns selected item's index            
        private int drawMenu(List<string> menuItems, int lineBetweenChoices, int totalChoiceLines)
        {
            while (true)
            {
                clearMenu(totalChoiceLines);
                //Makes sure the index currently selected exists on the menu
                for (int listIndex = 0; listIndex < menuItems.Count; listIndex++)
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

                System.ConsoleKey keyPress = Console.ReadKey(true).Key;

                //determines what action to take on a menu depending on what key is pressed
                if (!(keyPress == ConsoleKey.Enter
                    || keyPress == ConsoleKey.UpArrow
                    || keyPress == ConsoleKey.DownArrow))
                    continue;

                if (keyPress == ConsoleKey.DownArrow)
                {
                    if (!(menuItems.Count - 1 == index))
                    {
                        index++;
                    }
                }

                else if (keyPress == ConsoleKey.UpArrow)
                {
                    if (!(index <= 0))
                    {
                        index--;
                    }
                }

                else if (keyPress == ConsoleKey.Enter)
                {
                    return index + 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        static void titleMessage()
        {
            //put title here
        }

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
        public void slowTextOutput(string outputText)
        {
            int originalTextSpeed = GameMenu.TextSpeed;
            int letterCount = 0, i = 0;    //keeps track of how many characters are in a single line to prevent going past the defined ScreenWidth
            System.Threading.Thread.Sleep(GameMenu.DialogueSpeed);

            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter) && i < outputText.Length)
            {

                System.Threading.Thread.Sleep(GameMenu.TextSpeed);

                Console.Write(outputText[i]);

                if (outputText[i] == '\n')
                {
                    letterCount = 0;
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
                ++i;
            }

            if (i < outputText.Length)
            {
                string temp = outputText.Substring(i);
                Console.Write(temp);
            }

            Console.Write("\n");
            System.Threading.Thread.Sleep(GameMenu.DialogueSpeed);
        }

        //goes really fast for sudden stuff or system messages
        public void instantTextOutput(string outputText)
        {
            int originalTextSpeed = GameMenu.TextSpeed;
            int letterCount = 0, i = 0;    //keeps track of how many characters are in a single line to prevent going past the defined ScreenWidth

            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter) && i < outputText.Length)
            {
                Console.Write(outputText[i]);

                if (outputText[i] == '\n')
                {
                    letterCount = 0;
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
                ++i;
            }

            if (i < outputText.Length)
            {
                string temp = outputText.Substring(i);
                Console.Write(temp);
            }

            Console.Write("\n");
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
        
        public String getFile()
        {
            String filePath = String.Empty;
            String fileContent = String.Empty;
            OpenFileDialog ofd = new OpenFileDialog(); //create a file explorer instance to browse for a file
            ofd.InitialDirectory = "c:\\Users\\Desktop";
            ofd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            ofd.FilterIndex = 2;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK) //run once a good file is selected
            {
                filePath = ofd.FileName;

                var fileStream = ofd.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
            }
            return fileContent;
        }

        public void runFromFile(String text)
        {
            if (text.Length == 0) return;

            int lineNum = 0;
            Dictionary<int, gameOption> inputOptions = new Dictionary<int, gameOption>();

            for (int i=0; i<text.Length; ++i)
            {
                if (text[i] == '\n') { ++lineNum; }

                if (text[i] == '/')
                {
                    if (text[i + 1] == '/')
                        continue;

                    if (text[i + 1] == 'd') //diologue
                    {
                        string tempID = "";
                        int ID = 0;
                        string dialogue = String.Empty;
                        string menu = String.Empty;

                        try
                        {
                            for (int j = i + 3; text[j] != ' ' && text[j] != '\n' && text[j] != '\t'; ++j)
                            {
                                tempID += text[j];
                            }
                            ID = int.Parse(tempID);
                        }
                        catch (Exception e)
                        {
                            instantTextOutput("\nThere was an error with the dialogue number on line "
                                + lineNum + ". Please check it and try again. Make sure it starts exactly 1 space behind"
                                + " the command and that it is a whole number.");
                            Console.ReadKey();
                            break;
                        }

                        dialogue = customDialogue(ref text, ref i, ref lineNum);
                        
                        if (dialogue == String.Empty) //if dialogue is blank or if there is no '|' character
                            break;

                        if (inputOptions.ContainsKey(ID)) //if the key already exists
                        {
                            instantTextOutput("\nThe inputted ID " + ID + " on line " + lineNum +
                                " is already assigned with the dialogue: \"" + inputOptions[ID].dialogue + "\"\n\nPlease change the ID.");
                            instantTextOutput("\nPress any key to continue.");
                            Console.ReadKey();
                            break;
                        }
                        inputOptions[ID] = new gameOption(dialogue, menu, ID);
                    }
                    else if (text[i + 1] == 'm')  //menu options
                    {
                        string menu = String.Empty;
                        string tempID = "";
                        int ID = 0;

                        try
                        {
                            for (int j = i + 3; text[j] != ' ' && text[j] != '\n' && text[j] != '\t'; ++j)
                            {
                                tempID += text[j];
                            }
                            ID = int.Parse(tempID);
                        }
                        catch (Exception e)
                        {
                            instantTextOutput("\nThere was an error with the dialogue number on line "
                                + lineNum + ". Please check it and try again. Make sure it starts exactly 1 space behind"
                                + " the command and that it is a whole number.");
                            Console.ReadKey();
                            break;
                        }

                        menu = customMenu(ref text, ref i, ref lineNum);

                        instantTextOutput(menu);
                        Console.ReadKey();

                        if (menu == String.Empty) //if dialogue is blank or if there is no '|' character
                            break;

                        try
                        {
                            inputOptions[ID].menu = menu;
                        }
                        catch (Exception e)
                        {
                            instantTextOutput("\nThe ID provided is currently empty. Make sure there is a previous piece of dialogue assigned to it"
                                + "\nPress enter to continue");

                            while (Console.KeyAvailable)
                            {
                                Console.ReadKey(true);
                            }

                            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
                            break;
                        }
                    }
                    else if (text[i + 1] == 't')  //title picture
                    {

                    }
                    else if (text[i + 1] == 's')    //text speed
                    {

                    }
                }
            }
        }
        private String customDialogue(ref string text, ref int i, ref int lineNum)
        {
            String dialogue = String.Empty;

            while (text[i] != '\r' && text[i+1] != '\n')
                ++i;
            ++i;
            ++lineNum;

            try
            {
                if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    for(; text[i] != '|'; ++i)
                    {
                        if (text[i] == '\r' && text[i + 1] == '\n')
                        {
                            ++lineNum;
                            if (text[i + 2] == '\r' && text[i+3] == '\n')
                            {
                               ++i;
                                ++lineNum;
                              continue;
                            }
                            ++i;
                            dialogue += ' ';
                            continue;
                        }

                        dialogue += text[i];

                        if (text[i] == '/')
                        {
                            if (text[i+1] == 'd' || text[i+1] == 'm' || text[i + 1] == 't' 
                                || text[i + 1] == 's')
                            {
                                throw new FormatException();
                            }
                        }
                    }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    for (; text[i] != '|'; ++i)
                    {
                        if (text[i] == '\n')
                        {
                            ++lineNum;
                            if (text[i + 1] == '\n')
                            {
                                ++lineNum;
                                ++i;
                                continue;
                            }
                            ++i;
                            dialogue += ' ';
                            continue;
                        }

                        dialogue += text[i];

                        if (text[i] == '/')
                        {
                            if (text[i + 1] == 'd' || text[i + 1] == 'm' || text[i + 1] == 't'
                                || text[i + 1] == 's')
                            {
                                throw new FormatException();
                            }
                        }
                    }
            }
            catch (Exception e)
            {
                instantTextOutput("\nNo end character '|' found (sentinal value). Check around line " + lineNum + " for a missing '|'.");
                instantTextOutput("\nClick any key to continue.");
                Console.ReadKey();
                return String.Empty;
            }
            return dialogue;
        }

        private String customMenu(ref String text, ref int i, ref int lineNum)
        {
            String menu = String.Empty;


            while (text[i] != '\r' && text[i + 1] != '\n')
                ++i;
            ++i;
            ++lineNum;

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    for (; text[i] != '|'; ++i)
                    {
                        if (text[i] == '\r' && text[i + 1] == '\n')
                        {
                            if (text[i + 2] == '\r' && text[i + 3] == '\n')
                            {
                                ++i;
                                ++lineNum;
                                continue;
                            }
                        }

                        menu += text[i];

                        if (text[i] == '/')
                        {
                            if (text[i + 1] == 'd' || text[i + 1] == 'm' || text[i + 1] == 't'
                                || text[i + 1] == 's')
                            {
                                throw new FormatException();
                            }
                        }
                    }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    for (; text[i] != '|'; ++i)
                    {
                        if (text[i] == '\n')
                        {
                            if (text[i + 1] == '\n')
                            {
                                ++lineNum;
                                ++i;
                                continue;
                            }
                        }

                        menu += text[i];

                        if (text[i] == '/')
                        {
                            if (text[i + 1] == 'd' || text[i + 1] == 'm' || text[i + 1] == 't'
                                || text[i + 1] == 's')
                            {
                                throw new FormatException();
                            }
                        }
                    }
            }
            catch (Exception e)
            {
                instantTextOutput("\nNo end character '|' found (sentinal value). Check around line " + lineNum + " for a missing '|'.");
                instantTextOutput("\nClick any key to continue.");
                Console.ReadKey();
                return String.Empty;
            }
            return menu;
        }
    }
}
