# Solitaire Game 
This README provides information about the Solitaire game developed using C# and Xamarin.Forms. The game is a classic peg solitaire where the objective is to eliminate as many marbles as possible by jumping over them with other marbles until there is only one marble left on the board.

###  Table of Contents
Game Overview
Getting Started
How to Play
Game Rules
Features
Screenshots
License
Game Overview
The Solitaire game is a mobile application developed using C# and Xamarin.Forms. It follows the rules of the classic peg solitaire game. The game board consists of a 7x7 grid, and the initial setup includes marbles arranged in a specific pattern. The objective is to eliminate as many marbles as possible through strategic moves until only one marble remains on the board. The game provides features for saving and loading game progress.

### Getting Started
To play the Solitaire game, follow these steps:

Install Xamarin.Forms on your development environment if you haven't already.

Clone or download the game's source code.

Open the project in your preferred C# IDE or development environment.

Build and run the application on your Android or iOS device/emulator.

### How to Play
When you start the game, you'll see a 7x7 grid representing the game board, with marbles and holes.

To make a move, tap on a marble. It will turn red, indicating that it's selected.

Tap on an adjacent empty spot to move the selected marble. If there's another marble in the middle of the selected marble and the destination, it will be eliminated, and the selected marble will move to the empty spot.

Keep making moves to eliminate marbles strategically. The goal is to eliminate as many marbles as possible until only one marble remains on the board.

If you can't make any more valid moves, the game is over.

You win if only one marble is left in the center of the board.

### Game Rules
You can move a marble to an empty spot horizontally or vertically by jumping over another marble.
The marble that is jumped over is eliminated.
The game ends when you can't make any more valid moves.
Features
Interactive user interface with tap gestures for selecting and making moves.
Visual feedback with highlighted valid move positions.
Save and load game progress.
Game over detection and win condition.
Resetting the game board.
Screenshots
Game Screenshot 1
Game Screenshot 2

### License
This Solitaire game is open-source and available under the MIT License. You are free to use, modify, and distribute the code for personal or commercial purposes, as long as you include the original license and attribution.
