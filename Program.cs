using System;

namespace Match3
{
    class Program
    {
        // Match 3 Program!

        static void Main(string[] args)
        {
            Console.WriteLine("Hello! I'm a Match 3 game!");

            Board theBoard = new Board();

            // this won't produce ANY result - it's just a dummy console program, the board is never setup/populated!

            Board.Move bestMove = theBoard.CalculateBestMoveForBoard();

            Console.WriteLine("Best Move found at: X, ", bestMove.x,
                                                 " Y, ", bestMove.y,
                                           " Direction", bestMove.direction);
        }

        public class Board
        {
            // board width and height
            int bWidth = 8;   // just assumed for this example
            int bHeight = 12; // just assumed for this example

            // Other functions will set up the contents (with no match3's initially) and
            // also create the working copy of the board for us to play with
            // the CalculateBestMoveForBoard() doesn't do anything but check for the best score possible on the current board
            // other functions will perform removal of the matched tiles and re-populating again

            private int[,] theBoard;  // our 2 dimensional board array (initialised/populated somewhere etc)

            enum JewelKind
            {
                Empty = 0,
                Red = 1,
                Orange = 2,
                Yellow = 3,
                Green = 4,
                Blue = 5,
                Indigo = 6,
                Violet = 7
            };

            public enum MoveDirection
            {
                None, // none added as using this to track movement in our for loops
                Up,
                Down,
                Left,
                Right
            };

            public struct Move
            {
                public Move(int xPos, int yPos, MoveDirection moveDir)
                {
                    x = xPos; // x position
                    y = yPos; // y position
                    direction = moveDir;  // move direction
                }

                public int x { get; set; }
                public int y { get; set; }
                public MoveDirection direction { get; set; }
            };

            int GetWidth()
            {
                return bWidth;
            }

            int GetHeight()
            {
                return bHeight;
            }

            JewelKind GetJewel(int x, int y)
            {
                return (JewelKind)theBoard[x, y];
            }

            void SetJewel(int x, int y, JewelKind kind)
            {
                theBoard[x, y] = (int)kind;
            }

           
            //
            // Please note, haven't had time to functionalise and make some checkDown(), checkRight(), checkLeft(), checkUp() functions - so looks a little messy!
            //

            public Move CalculateBestMoveForBoard()
            {
                // Calculates the best move for the current board, the board is assumed to have been populated elsewhere
                // and will not contain any initial match 3's
                //
                // The best score in any match3 move, can NEVER exceed 7, from a maximum of 3 "match 3s" created by the move (minus any repeated shared Jewel position)

                int boardWidth = GetWidth();  // width of board
                int boardHeight = GetHeight(); // height of board
                int bestPointScore = new int();   // best score found
                int currentPointScore = new int();   // current score during search
                Move currentMove = new Move();  // current move
                Move bestMove = new Move();  // best possible scoring move - another function will remove the "best scoring move" match3's & repopulate

                JewelKind currentJewel; // current jewel being examined
                JewelKind nextJewel;    // next jewel to be examined

                // make sure initialised (it should be anyway)
                bestPointScore = currentPointScore = 0;

                // METHOD OF CHECKING:
                //
                // Starting from top left, check the next Jewel RIGHT for Swap possibility, if we can swap it RIGHT (due to it being different to the current one),
                // then we need to check all of the following cases:
                //    1. has the swap resulted in a match3 going right
                //    2. did the swap result in any match3 going up (and down too) where the swap point may be either at the top, bottom, or middle of a match3 streak
                //
                // Add the match3 points up for the current move (a possible maximum of three match 3's can be formed, resulting in 7 points maximum)
                //
                // Note: if we score 7 points there is NO need for ANY further checks as that is the MAXIMUM that can be achieved, so return this move 
                //
                // If we CAN'T do a swap RIGHT, then we need to check for a swap DOWN, and if a swap DOWN is possible, check for any match3's in LEFT, RIGHT and DOWN directions only
                // from the new position.
                //
                // The move with the greatest score after iterating over the table will be returned as the best move.

                // holds initial direction we moved the current Jewel (only ever RIGHT, or DOWN) before doing any further checks
                MoveDirection currentMoveDirection = new MoveDirection();
                currentMoveDirection = MoveDirection.None; // haven't moved yet

                bool fullMatchFound = false; // set if we find a match3

                // Traverse down and across the board row by row, note: entries in the board array start at element zero,zero.
                for (int down = 0; down < boardHeight; down++)
                {
                    // traverse across the board from row 0
                    for (int across = 0; across < boardWidth; across++)
                    {
                        currentMove.x = across;
                        currentMove.y = down;

                        if (across <= boardWidth - 2)
                        {
                            // Only try to swap right if not in last column (of an array starting at position 0) of this row
                            currentJewel = GetJewel(across, down);
                            nextJewel = GetJewel(across + 1, down);

                            if (currentJewel.CompareTo(JewelKind.Empty) != 0)
                            {
                                // we never swap "empty" Jewels
                                if (currentJewel.CompareTo(nextJewel) != 0)
                                {
                                    // swap the Jewels as different
                                    JewelKind tempJewel = nextJewel;
                                    nextJewel = currentJewel;
                                    currentJewel = tempJewel;

                                    currentMoveDirection = MoveDirection.Right; // we got here with a RIGHT swap

                                    // CHECK if there are two columns AFTER the position we are swapping into
                                    if (across <= boardWidth - 4)
                                    {
                                        // ok to check for a possible RIGHT facing match3
                                        JewelKind jewel2 = new JewelKind();
                                        JewelKind jewel3 = new JewelKind();

                                        jewel2 = GetJewel(across + 2, down);
                                        jewel3 = GetJewel(across + 3, down);

                                        if (nextJewel.CompareTo(jewel2) == 0)
                                        {
                                            if (jewel2.CompareTo(jewel3) == 0)
                                            {
                                                // match 3 found going RIGHT
                                                currentPointScore = 3; // give full 3 points for an inital match3
                                                fullMatchFound = true;
                                            }
                                        }
                                    }

                                    // we did a swap earlier, so check for a Match3 from the swap position that could be the following
                                    // a) the bottom of an UP match3, b) top of a DOWN match3, c) the middle point of one
                                    // d) and could also have both UP and DOWN matches

                                    if (down <= boardHeight - 4)
                                    {
                                        // can check for a FULL match3 downwards
                                        // "down" goes from 0 to "boardHeight -1" - looks odd but correct! boardHeight starts at 1.

                                        JewelKind jewelDown1, jewelDown2;
                                        jewelDown1 = GetJewel(across + 1, down + 1);
                                        jewelDown2 = GetJewel(across + 1, down + 2);

                                        if (nextJewel.CompareTo(jewelDown1) == 0 && jewelDown1.CompareTo(jewelDown2) == 0)
                                        {
                                            // downwards match found
                                            if (!fullMatchFound)
                                            {
                                                // first match found as previous RIGHT check didnt find one
                                                fullMatchFound = true;
                                                currentPointScore = 3;
                                            }
                                            else currentPointScore += 2; // don't score the jewel in common
                                        }
                                    }

                                    // check for a FULL Upwards match3, but only if we are at/below the 3rd column down in the board
                                    if (down >= 2)
                                    {
                                        // check for a match created upwards by the swap
                                        JewelKind jewelUp1, jewelUp2;
                                        jewelUp1 = GetJewel(across + 1, down - 1);
                                        jewelUp2 = GetJewel(across + 1, down - 2);

                                        if (nextJewel.CompareTo(jewelUp1) == 0 && jewelUp1.CompareTo(jewelUp2) == 0)
                                        {
                                            if (!fullMatchFound)
                                            {
                                                // first match found as previous right check & down check didnt find any
                                                fullMatchFound = true;
                                                currentPointScore = 3;
                                            }
                                            else currentPointScore += 2; // don't score the jewel in common
                                        }
                                    }

                                    // last case, check if the swap position is just the middle point of a match3
                                    // but only need to do if no FULL up/down matches were already found
                                    if (down >= 1 && !fullMatchFound)
                                    {
                                        // check for a match3 where the swap point is the MIDDLE (check 1 cell up & 1 cell down) point
                                        JewelKind jewelUp1, jewelDown1;
                                        jewelUp1 = GetJewel(across + 1, down - 1);
                                        jewelDown1 = GetJewel(across + 1, down + 1);

                                        if (nextJewel.CompareTo(jewelUp1) == 0 && jewelUp1.CompareTo(jewelDown1) == 0)
                                        {
                                            // match3 found, and was first one found so far
                                            currentPointScore = 3;
                                        }
                                        else currentPointScore += 2; // don't score common jewel
                                    }

                                    // now swap the "SWAPPED RIGHT" Jewel back to its original position for next pass through
                                    JewelKind right = GetJewel(across + 1, down);
                                    JewelKind temp = GetJewel(across, down);

                                    SetJewel(across, down, right);
                                    SetJewel(across + 1, down, temp);

                                    fullMatchFound = false;
                                }
                                else
                                {
                                    // We didn't SWAP RIGHT, so try a swap DOWN if not on bottom row,
                                    // then check for left, right and (possible) down matches from there
                                    if (down <= boardHeight - 2)
                                    {
                                        // not on bottom row
                                        currentJewel = GetJewel(across, down);
                                        nextJewel = GetJewel(across, down + 1);

                                        // swap the Jewel DOWN
                                        JewelKind tempJewel = nextJewel;
                                        nextJewel = currentJewel;
                                        currentJewel = tempJewel;
                                        currentMoveDirection = MoveDirection.Down; // we got here with a Down swap

                                        // now check right (if we can)
                                        if (across <= boardWidth - 3)
                                        {
                                            // check for RIGHT facing match3
                                            JewelKind jewel2 = new JewelKind();
                                            JewelKind jewel3 = new JewelKind();

                                            jewel2 = GetJewel(across + 1, down + 1);
                                            jewel3 = GetJewel(across + 2, down + 1);

                                            if (nextJewel.CompareTo(jewel2) == 0)
                                            {
                                                if (jewel2.CompareTo(jewel3) == 0)
                                                {
                                                    // match 3 found going RIGHT
                                                    currentPointScore = 3; // full 3 points for an inital match3
                                                    fullMatchFound = true;
                                                }
                                            }
                                        }

                                        // now check for a DOWN facing match3
                                        if (down <= boardHeight - 4)
                                        {
                                            // ok to check down from swap position as there are 2 elements below it
                                            JewelKind jewelDown1, jewelDown2;
                                            jewelDown1 = GetJewel(across, down + 2);
                                            jewelDown2 = GetJewel(across, down + 3);

                                            if (nextJewel.CompareTo(jewelDown1) == 0 && jewelDown1.CompareTo(jewelDown2) == 0)
                                            {
                                                // downwards match found
                                                if (!fullMatchFound)
                                                {
                                                    // first time match found after the swap down
                                                    fullMatchFound = true;
                                                    currentPointScore = 3;
                                                }
                                                else currentPointScore += 2; // don't score jewel in common
                                            }
                                        }

                                        // finally check for a LEFT match3 
                                        if (across >= 2)
                                        {
                                            // check for a possible LEFT facing match3
                                            JewelKind jewel2 = new JewelKind();
                                            JewelKind jewel3 = new JewelKind();

                                            jewel2 = GetJewel(across - 1, down + 1);
                                            jewel3 = GetJewel(across - 2, down + 1);

                                            if (nextJewel.CompareTo(jewel2) == 0)
                                            {
                                                if (jewel2.CompareTo(jewel3) == 0)
                                                {
                                                    // match 3 found going LEFT
                                                    if (!fullMatchFound)
                                                    {
                                                        fullMatchFound = true;
                                                        currentPointScore = 3;
                                                    }
                                                    else currentPointScore += 2;
                                                }
                                            }
                                        }

                                        // now swap the current "SWAPPED DOWN" Jewel back to its original position for next pass through
                                        JewelKind current = GetJewel(across, down);
                                        JewelKind temp = GetJewel(across, down + 1);

                                        SetJewel(across, down + 1, current);
                                        SetJewel(across, down, temp);
                                    }
                                }
                            }

                            if (currentPointScore > bestPointScore)
                            {
                                // we have a new best score
                                bestPointScore = currentPointScore;
                                bestMove.x = across;
                                bestMove.y = down;
                                bestMove.direction = currentMoveDirection;
                            }

                            // finish if we have found the "best score£ possible, which is ALWAYS 7 in a match3 game
                            if (currentPointScore == 7)
                                return bestMove;

                            // reset for next time through
                            currentMoveDirection = MoveDirection.None;
                            currentPointScore = 0;
                            fullMatchFound = false;
                        }
                    }

                    // finished board search, return best move found
                    return bestMove;
                }

                return bestMove;
            }
        }
    }
}
