using System;
using System.Collections.Generic;
using System.Text;


namespace IFGame
{
    class GamePlay
    {
        string outputText;  //put diologue and text to display here and send it to displayText()
        string playerName;  //holds the name of the player
        string[] listChoices;   //holds the custom menu choices in an array of strings
        int playerChoice;   //holds the number of the menu item they chose
        GameMenu menuObject = new GameMenu();
        int pictureSpeed = GameMenu.DialogueSpeed * 7 / 5;

        public void runGame()
        {
            bool runGame = true;

            while (runGame)     //loop to run until the player exits the game
            {

                //code for game goes here


                //Display text like diologue \/
                outputText = "This is how you print out code in the game";
                menuObject.slowTextOutput(outputText);


                //How to create choices and display them \/
                listChoices = new string[] {"Ok cool. Now take me back to the main menu",
                                                "Output this text again." };
                playerChoice = menuObject.displayMenu(listChoices, 0);


                //Do something depending on menu choice \/
                if (playerChoice == 1)
                    runGame = false;    //stop loop to end game
                else if (playerChoice == 2)
                {
                    Console.Clear();
                    continue;   //skip the rest of the code in the loop and iterate again
                }
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            GameMenu game = new GameMenu();
            game.startGame();
        }
    }
}
