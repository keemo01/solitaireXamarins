using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace solit
{
    public partial class MainPage : ContentPage
    {
        private GameData gameData = new GameData();


        int[,] gameBoard = new int[7, 7]{{2, 2, 1, 1, 1, 2, 2},
                                                 {2, 2, 1, 1, 1, 2, 2},
                                                 {1, 1, 1, 1, 1, 1, 1},
                                                 {1, 1, 1, 0, 1, 1, 1},
                                                 {1, 1, 1, 1, 1, 1, 1},
                                                 {2, 2, 1, 1, 1, 2, 2},
                                                 {2, 2, 1, 1, 1, 2, 2}};

       
        bool toggle_first = false;
        int[] currentSelected = { -10, -10 };
        List<(int, int)> highlightedArea = new List<(int, int)>();
        int kills = 0;
        int marbles_on_board = 0;
        
        public MainPage()
        {
            InitializeComponent();

            // Loop through the game board to create and add marble images
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    // Create and add marble images based on the game board data
                    if (gameBoard[row, col] == 1)
                    {
                        var marble = new Image { Aspect = Aspect.AspectFit };
                        marble.Source = ImageSource.FromFile("black.jpg");
                        mySolitaireGrid.Children.Add(marble, col, row);
                    }
                    else if (gameBoard[row, col] == 0)
                    {
                        var marble = new Image { Aspect = Aspect.AspectFit };
                        marble.Source = ImageSource.FromFile("hole.jpg");
                        mySolitaireGrid.Children.Add(marble, col, row);
                    }
                }
            }


            // Attach tap gesture recognizers to each marble image
            foreach (var child in mySolitaireGrid.Children)
            {
                child.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(OnTapGestureRecognizerTapped),
                    CommandParameter = child
                });
            }

            InfoLabel.Text = "Start Playing"; // Set an initial message on the info label
        }

        // Event handler for when a marble image is tapped
        private void OnTapGestureRecognizerTapped(object sender)
        {
            // Get the source, sender image, row, and column of the tapped image
            var x = (sender as Image).Source;
            var imageSender = (Image)sender;
            var r = Grid.GetRow(sender as Image);
            var c = Grid.GetColumn(sender as Image);

            if (!toggle_first) // If it's the first tap of a move
            {
                toggle_first = true; // Set the toggle to true
                imageSender.Source = "red.jpg"; // Change the image source to indicate selection
                currentSelected[0] = r; // Store the selected row
                currentSelected[1] = c; // Store the selected column
                show_possible_moves(r, c); // Show possible moves for the selected marble
            }
            else // If it's the second tap of a move
            {
                toggle_first = false; // Reset the toggle
                int x0 = currentSelected[0]; // Get the initial selected row
                int y0 = currentSelected[1]; // Get the initial selected column

                int X_2 = r; // Get the new selected row
                int Y_2 = c; // Get the new selected column

                if (highlightedArea.Contains((X_2, Y_2))) // If the second tap is in a valid move area
                {
                    executeMove(x0, y0, X_2, Y_2); // Execute the move
                    kills++; // Increase the kills count
                }
                else
                {
                    // Handle other cases if needed when second tap is not in a valid move area
                }

                currentSelected[0] = -10; // Reset the selected positions
                currentSelected[1] = -10;
                highlightedArea.Clear(); // Clear the highlighted area
                refresh_grid(); // Refresh the game board UI
            }
        }



        // Moves the selected marble to a new position
        private void executeMove(int x_s, int y_s, int x_e, int y_e)
        {
            // Set the starting position to empty
            gameBoard[x_s, y_s] = 0;

            // Set the ending position to contain the marble
            gameBoard[x_e, y_e] = 1;

            if (x_s == x_e) // If the movement is along the same row
            {
                if (y_e > y_s) // If moving to the right
                {
                    // Set the position in between to empty (marble is jumping over it)
                    gameBoard[x_e, y_s + 1] = 0;
                }
                else // If moving to the left
                {
                    // Set the position in between to empty (marble is jumping over it)
                    gameBoard[x_e, y_s - 1] = 0;
                }
            }
            else if (y_s == y_e) // If the movement is along the same column
            {
                if (x_e > x_s) // If moving downwards
                {
                    // Set the position in between to empty (marble is jumping over it)
                    gameBoard[x_s + 1, y_s] = 0;
                }
                else // If moving upwards
                {
                    // Set the position in between to empty (marble is jumping over it)
                    gameBoard[x_s - 1, y_s] = 0;
                }
            }

            // Refresh the game board UI to reflect the changes
            refresh_grid();
        }


        // Checks to see if game has finished
        // Checks whether the game is over by evaluating the state of the board
        private bool IsGameOver()
        {
            foreach (var child in mySolitaireGrid.Children)
            {
                int r = Grid.GetRow(child);
                int c = Grid.GetColumn(child);

                // Check for each marble on the board
                if (gameBoard[r, c] == 1)
                {
                    int[] rowNum = { -1, 0, 0, 1 };
                    int[] colNum = { 0, -1, 1, 0 };

                    // Check in all four directions around the marble
                    for (int i = 0; i < 4; i++)
                    {
                        int X_1 = r + rowNum[i];
                        int Y_1 = c + colNum[i];

                        int X_2 = r + 2 * rowNum[i];
                        int Y_2 = c + 2 * colNum[i];

                        // If there's a possibility for a move, the game is not over
                        if (IsInsideBoard(X_2, Y_2) && gameBoard[X_2, Y_2] == 0 && gameBoard[X_1, Y_1] == 1)
                        {
                            return false;
                        }
                    }
                }
            }

            // Calculate total marbles on the board
            int totalMarbles = CountMarbles();

            // If there's only one marble left at the center and it's in the winning position
            if (totalMarbles == 1 && gameBoard[3, 3] == 1)
            {
                InfoLabel.Text = "YOU WIN";
                GameResetButton.IsEnabled = true;
            }
            else
            {
                InfoLabel.Text = "GAME OVER";
                GameResetButton.IsEnabled = true;
            }

            // The game is over
            return true;
        }



        private int CountMarbles()
        {
            var total_1 = 0;
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    if (gameBoard[i, j] == 1)
                    {
                        total_1++;
                    }
                }
            }
            marbles_on_board = total_1;
            return total_1;

        }
        // Refreshes the visual representation of the game grid based on the current game state
        private void refresh_grid()
        {
            // Iterate through all cells in the grid
            foreach (var child in mySolitaireGrid.Children)
            {
                int r = Grid.GetRow(child);
                int c = Grid.GetColumn(child);

                //This Updates the images based on the state of each cell
                if (gameBoard[r, c] == 1)
                {
                    var img = (Image)child;
                    img.Source = "black.jpg"; // Set image for a marble
                }
                else if (gameBoard[r, c] == 0)
                {
                    var img = (Image)child;
                    img.Source = "hole.jpg"; // Set image for an empty cell
                }
            }

            // Clear save/load information
            InfoLabel_saveLoad.Text = "";

            // Update the game information labels with the current kills and remaining marbles
            InfoLabel.Text = "Kills : " + kills.ToString() + " Remaining : " + CountMarbles().ToString();

            // Check if the game is over
            IsGameOver();
        }


        // Highlights the possible move at the specified grid position (x, y)
        private void highlightPossibleMove(int x, int y)
        {
            // Loop through each child element in the grid
            foreach (var child in mySolitaireGrid.Children)
            {
                int r = Grid.GetRow(child);
                int c = Grid.GetColumn(child);

                // Check if the child's grid position matches the specified position (x, y)
                if (r == x && c == y)
                {
                    // Add the position to the highlightedArea list and change the image source
                    highlightedArea.Add((r, c));
                    var img = (Image)child;
                    img.Source = "highlight.jpg";
                }
            }
        }

        // Shows the possible moves from the given grid position (r, c)
        private void show_possible_moves(int r, int c)
        {
            int[] rowNum = { -1, 0, 0, 1 };
            int[] colNum = { 0, -1, 1, 0 };

            // Iterate through the four potential move directions
            for (int i = 0; i < 4; i++)
            {
                int X_1 = r + rowNum[i];
                int Y_1 = c + colNum[i];

                int X_2 = r + 2 * rowNum[i];
                int Y_2 = c + 2 * colNum[i];

                // Check if the move is within the board and valid (jumping over an opponent's marble)
                if (IsInsideBoard(X_2, Y_2) && gameBoard[X_2, Y_2] == 0 && gameBoard[X_1, Y_1] == 1)
                {
                    // Highlight the possible move
                    highlightPossibleMove(X_2, Y_2);
                }
            }
        }

        // Checks if a position (x, y) is within the game board and not occupied by a forbidden marble
        private bool IsInsideBoard(int x, int y)
        {
            if (x < 0 || x > 6 || y < 0 || y > 6)
            {
                return false;
            }
            else
            {
                // Check if the position (x, y) is not occupied by a forbidden marble (2)
                if (gameBoard[x, y] != 2)
                {
                    return true; // The position is inside the board and not occupied
                }
                else
                {
                    return false; // The position is occupied by a forbidden marble
                }
            }
        }

        // Resets the game board and other game-related variables
        private void GameResetButton_Clicked(object sender, EventArgs e)
        {
            // Initialize the game board with the default layout
            gameBoard = new int[7, 7]{{2, 2, 1, 1, 1, 2, 2},
                             {2, 2, 1, 1, 1, 2, 2},
                             {1, 1, 1, 1, 1, 1, 1},
                             {1, 1, 1, 0, 1, 1, 1},
                             {1, 1, 1, 1, 1, 1, 1},
                             {2, 2, 1, 1, 1, 2, 2},
                             {2, 2, 1, 1, 1, 2, 2}};

            // Reset the kills counter
            kills = 0;

            // Refresh the game grid display
            refresh_grid();
        }


        // Triggered when the "Save Game" button is clicked
        private void SaveGameButton_Clicked(object sender, EventArgs e)
        {
            // Construct the file path for saving game data in the local application data folder
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gameData.json");

            // Create a new GameData instance with the current game state
            GameData gameData = new GameData(gameBoard, kills);

            // Save the game data to the specified file path
            gameData.SaveGame(filePath);
        }

        // Triggered when the "Load Game" button is clicked
        private void LoadButton_Clicked(object sender, EventArgs e)
        {
            // Construct the file path for loading game data from the local application data folder
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gameData.json");

            // Load the saved game data from the file path
            GameData loadedGameData = GameData.LoadGame(filePath);

            // Check if loaded game data is not null (i.e., game data was successfully loaded)
            if (loadedGameData != null)
            {
                // Update the current game state with the loaded game data
                gameBoard = loadedGameData.gameBoard;
                kills = loadedGameData.kills;

                // Refresh the game grid and display a success message
                refresh_grid();
                InfoLabel_saveLoad.Text = "Game Loaded Successfully!";
            }
            else
            {
                // Display a message indicating that no saved game was found
                InfoLabel_saveLoad.Text = "No saved game found!";
            }
        }


    }
}