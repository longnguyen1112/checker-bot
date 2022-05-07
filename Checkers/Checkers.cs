using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Checkers
{
    enum CheckerType { Empty,Red,RedKing,Black,BlackKing};
    public partial class Checkers : Form
    {
        //the game board
        private CheckerBox[,] chkbxBoard;

        //the three numbers storing the current board state
        private uint Red;
        private uint Black;
        private uint Kings;

        //Images to use representing the board and pieces
        private Image lightSquare;
        private Image darkSquare;
        private Image redChecker;
        private Image redCheckerSelected;
        private Image redKing;
        private Image redKingSelected;
        private Image blackChecker;
        private Image blackCheckerSelected;
        private Image blackKing;
        private Image blackKingSelected;

        //null if no checker is selected. otherwise references the CheckerBox of the selected checker
        private CheckerBox chkSelectedChecker;
        
        //If setup mode is true, clicking on a board square will cycle through the 
        //available pieces that can be placed on that square.
        private bool setupMode;

        //If midJump is true, the human player is in the middle of a multi-part jump. 
        //The selected piece cannot be changed.
        private bool midJump;

        public Checkers()
        {
            InitializeComponent();

            lightSquare = Image.FromFile("images\\lightSquare.png");
            darkSquare = Image.FromFile("images\\darkSquare.png");
            redChecker = Image.FromFile("images\\redChecker.png");
            redCheckerSelected = Image.FromFile("images\\redCheckerSelected.png");
            redKing = Image.FromFile("images\\redKing.png");
            redKingSelected = Image.FromFile("images\\redKingSelected.png");
            blackChecker = Image.FromFile("images\\blackChecker.png");
            blackCheckerSelected = Image.FromFile("images\\blackCheckerSelected.png");
            blackKing = Image.FromFile("images\\blackKing.png");
            blackKingSelected = Image.FromFile("images\\blackKingSelected.png");

            chkbxBoard = new CheckerBox[8, 8];
            chkSelectedChecker = null;
            Red = Black = Kings = 0;

            setupMode = false;
            midJump = false;

            tlpBoard.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            int panelColor = 0;
            for (int row=0;row<8;row++)
            {
                for (int col=0;col<8;col++)
                {
                    chkbxBoard[row,col] = new CheckerBox();
                    chkbxBoard[row, col].BorderStyle = BorderStyle.FixedSingle;
                    chkbxBoard[row, col].Padding = new Padding(0);
                    chkbxBoard[row, col].Margin = new Padding(0);
                    chkbxBoard[row, col].Dock = DockStyle.Fill;
                    chkbxBoard[row, col].SizeMode = PictureBoxSizeMode.StretchImage;

                    chkbxBoard[row, col].Row = row;
                    chkbxBoard[row, col].Col = col;

                    if (panelColor % 2 == 0)
                    {
                        chkbxBoard[row, col].Image = darkSquare;
                        chkbxBoard[row, col].Checker = CheckerType.Empty;
                        //We never click a dark square
                    }
                    else
                    {
                        chkbxBoard[row, col].Image = lightSquare;
                        chkbxBoard[row, col].Checker = CheckerType.Empty;
                        chkbxBoard[row, col].Click += CheckerBox_Click;

                    }



                    this.tlpBoard.Controls.Add(chkbxBoard[row, col], col, row);
                    panelColor += 1;

                }
                panelColor += 1;
            }
        }

        //Exit is chosen from the menu
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Force the computer to take a turn
        private void forceComputerTurnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComputerMove();
        }

        //switch into or out of setup mode
        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (setupMode)
            {
                newToolStripMenuItem.Enabled = true;
                loadToolStripMenuItem.Enabled = true;
                forceComputerTurnToolStripMenuItem.Enabled = true;
                setupMode = false;
            }
            else
            {
                newToolStripMenuItem.Enabled = false;
                loadToolStripMenuItem.Enabled = false;
                forceComputerTurnToolStripMenuItem.Enabled = false;
                setupMode = true;
            }

        }

        //start a new game with the initial setup values
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Red = 4095;
            Black = 4293918720;
            Kings = 0;
            midJump = false;

            drawBoard(Red, Black, Kings);
        }

        //load a specific set of numbers for red, black, king values into the game
        //calls a custom dialog to do this
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadValues dlgLV = new LoadValues();
            dlgLV.Red = Red;
            dlgLV.Black = Black;
            dlgLV.Kings = Kings;

            if (dlgLV.ShowDialog() == DialogResult.OK)
            {
                Red = dlgLV.Red;
                Black = dlgLV.Black;
                Kings = dlgLV.Kings;

                drawBoard(Red, Black, Kings);
            }
        }

        //what happens when one of the checkerboxes that can hold a piece is clicked.
        private void CheckerBox_Click(object sender, EventArgs e)
        {
            CheckerBox curSquare = sender as CheckerBox;

            //Spit out to the console what we are clicking on
            switch (curSquare.Checker)
            {
                case CheckerType.Empty:
                    Console.WriteLine("Row: " + curSquare.Row.ToString() + " Col: " + curSquare.Col.ToString() + " Empty");
                    break;
                case CheckerType.Red:
                    Console.WriteLine("Row: " + curSquare.Row.ToString() + " Col: " + curSquare.Col.ToString() + " Red");
                    break;
                case CheckerType.RedKing:
                    Console.WriteLine("Row: " + curSquare.Row.ToString() + " Col: " + curSquare.Col.ToString() + " Red King");
                    break;
                case CheckerType.Black:
                    Console.WriteLine("Row: " + curSquare.Row.ToString() + " Col: " + curSquare.Col.ToString() + " Black");
                    break;
                case CheckerType.BlackKing:
                    Console.WriteLine("Row: " + curSquare.Row.ToString() + " Col: " + curSquare.Col.ToString() + " Black King");
                    break;
            }

            //Setup mode allows you to put checkers whereever you want on the board
            //Use this to construct specific situations.
            if (setupMode) {
                //clear the selected checker.
                //there's a good chance running setup will mess that up.
                chkSelectedChecker = null;

                //clear mid jump
                //there's a good chance running setup will mess this up too.
                midJump = false;

                switch (curSquare.Checker)
                {
                    case CheckerType.Empty:
                        curSquare.Checker = CheckerType.Red;
                        break;
                    case CheckerType.Red:
                        curSquare.Checker = CheckerType.RedKing;
                        break;
                    case CheckerType.RedKing:
                        curSquare.Checker = CheckerType.Black;
                        break;
                    case CheckerType.Black:
                        curSquare.Checker = CheckerType.BlackKing;
                        break;
                    case CheckerType.BlackKing:
                        curSquare.Checker = CheckerType.Empty;
                        break;
                }

                updateRBK();
                drawBoard(Red, Black, Kings);
                return;
            }

            if (chkSelectedChecker == curSquare && !midJump)
            {
                chkSelectedChecker = null;
                drawBoard(Red, Black, Kings);
            }
                
            else if (chkSelectedChecker == null)
            {
                //No Checker is selected. Make sure this is our checker.
                if (curSquare.Checker==CheckerType.Black || curSquare.Checker==CheckerType.BlackKing)
                {
                    chkSelectedChecker = curSquare;
                    drawBoard(Red, Black, Kings);
                }
                    
                //Else No Checker is selected and this isn't our checker. Ignore.
            }                
            else
            {
                //Selected Checker is set and we've clicked on a different square.
                //This is an attempt at a move
                if (MakeMove(curSquare))
                {
                    //The move succeeded. It's now the computer's turn
                    //Set the selected checker to null and draw the new board first.
                     ComputerMove();
                }
                //Else this wasn't a valid move (or is an unfinished multi-part move). Don't do anything.
            }
        }



        private void Checkers_ResizeEnd(object sender, EventArgs e)
        {
            int diff = this.Width - this.Height;

            if (diff < 77)
                this.Height = this.Width - 77;
            else if (diff > 77)
                this.Width = this.Height + 77;

            tlpBoard.ResumeLayout();
            this.Refresh();

        }

        private void drawBoard(uint redCheckers,uint blackCheckers, uint kings)
        {
            int shiftBits = 0;
            int curPos = 0;
            for (int row=0;row<8;row++)
            {
                for (int col=0;col<8;col++)
                {
                    if (curPos%2 == 1)
                    {                       
                        bool isRed = Convert.ToBoolean(redCheckers >> shiftBits & 1);
                        bool isBlack = Convert.ToBoolean(blackCheckers >> shiftBits & 1);

                        if (isRed)
                        {
                            if ((kings>>shiftBits & 1) == 1)
                            {
                                if (chkSelectedChecker==chkbxBoard[row,col])
                                    chkbxBoard[row, col].Image = redKingSelected;
                                else
                                    chkbxBoard[row, col].Image = redKing;
                                chkbxBoard[row, col].Checker = CheckerType.RedKing;
                            }                                
                            else {
                                if (chkSelectedChecker == chkbxBoard[row, col])
                                    chkbxBoard[row, col].Image = redCheckerSelected;
                                else
                                    chkbxBoard[row, col].Image = redChecker;
                                chkbxBoard[row, col].Checker = CheckerType.Red;
                            }
                        }
                        else if (isBlack)
                        {
                            if ((kings >> shiftBits & 1) == 1)
                            {
                                if (chkSelectedChecker == chkbxBoard[row, col])
                                    chkbxBoard[row, col].Image = blackKingSelected;
                                else
                                    chkbxBoard[row, col].Image = blackKing;
                                chkbxBoard[row, col].Checker = CheckerType.BlackKing;
                            }
                            else
                            {
                                if (chkSelectedChecker == chkbxBoard[row, col])
                                    chkbxBoard[row, col].Image = blackCheckerSelected;
                                else
                                    chkbxBoard[row, col].Image = blackChecker;
                                chkbxBoard[row, col].Checker = CheckerType.Black;
                            }
                        }
                        else
                        { 
                            chkbxBoard[row, col].Image = lightSquare;
                            chkbxBoard[row, col].Checker = CheckerType.Empty;
                        }
                        shiftBits++;
                    }
                    curPos++;
                }
                curPos++;
               
            }
        }

        private void Checkers_ResizeBegin(object sender, EventArgs e)
        {
            tlpBoard.SuspendLayout();
        }

        private bool MakeMove(CheckerBox curSquare)
        {
            //curSquare is the second clicked square. Should be empty
            //chkSelectedChecker is the first clicked square. Should be a black checker
            
            //Make sure we've clicked a checker and an empty space
            if (curSquare.Checker == CheckerType.Empty && (chkSelectedChecker.Checker == CheckerType.Black || chkSelectedChecker.Checker == CheckerType.BlackKing))
            {

                //Moves for a Black Checker               
                if (curSquare.Row + 1 == chkSelectedChecker.Row && (curSquare.Col == chkSelectedChecker.Col - 1 || curSquare.Col == chkSelectedChecker.Col + 1) && !hasJumps())
                {
                    curSquare.Image = chkSelectedChecker.Image;

                    //Make the piece a king if moveing into the 0th row
                    if (curSquare.Row == 0)
                        curSquare.Checker = CheckerType.BlackKing;
                    else
                        curSquare.Checker = chkSelectedChecker.Checker;

                    chkSelectedChecker.Image = lightSquare;
                    chkSelectedChecker.Checker = CheckerType.Empty;
                    chkSelectedChecker = null;
                    updateRBK();
                    drawBoard(Red, Black, Kings);
                    return true;
               }

                //Additional moves for a Black King
                else if (curSquare.Row - 1 == chkSelectedChecker.Row && (curSquare.Col == chkSelectedChecker.Col - 1 || curSquare.Col == chkSelectedChecker.Col + 1) && chkSelectedChecker.Checker == CheckerType.BlackKing && !hasJumps())
                {
                    curSquare.Image = chkSelectedChecker.Image;
                    curSquare.Checker = chkSelectedChecker.Checker;
                    chkSelectedChecker.Image = lightSquare;
                    chkSelectedChecker.Checker = CheckerType.Empty;
                    chkSelectedChecker = null;
                    updateRBK();
                    drawBoard(Red, Black, Kings);
                    return true;
                }
                //Jumps for a Black Checker
                else if (curSquare.Row + 2 == chkSelectedChecker.Row && curSquare.Col + 2 == chkSelectedChecker.Col && (chkbxBoard[curSquare.Row+1,curSquare.Col+1].Checker == CheckerType.Red || chkbxBoard[curSquare.Row + 1, curSquare.Col + 1].Checker == CheckerType.RedKing))
                {
                    chkbxBoard[curSquare.Row + 1, curSquare.Col + 1].Checker = CheckerType.Empty;
                    bool newKing = false;

                    //Make the piece a king if jumping into the 0th row.
                    if (curSquare.Row == 0 && chkSelectedChecker.Checker == CheckerType.Black)
                    {
                        chkbxBoard[curSquare.Row, curSquare.Col].Checker = CheckerType.BlackKing;
                        newKing = true;
                    }                        
                    else
                        chkbxBoard[curSquare.Row, curSquare.Col].Checker = chkbxBoard[curSquare.Row + 2, curSquare.Col + 2].Checker;

                    chkbxBoard[curSquare.Row + 2, curSquare.Col + 2].Checker = CheckerType.Empty;
                    chkSelectedChecker = chkbxBoard[curSquare.Row, curSquare.Col];

                    //If there are no more jumps to make (or there are, but the piece is a new king)
                    //clear the selected checker
                    //redraw the board
                    //let the computer take a turn (return true)
                    if (!hasJumps(chkSelectedChecker) || newKing)
                    {
                        midJump = false;
                        chkSelectedChecker = null;
                        updateRBK();
                        drawBoard(Red, Black, Kings);
                        return true;
                    }
                    //there are jumps. redraw the board
                    //don't let the computer go.
                    else
                    {
                        midJump = true;
                        updateRBK();
                        drawBoard(Red, Black, Kings);
                        return false;
                    }
                }
                else if (curSquare.Row + 2 == chkSelectedChecker.Row && curSquare.Col - 2 == chkSelectedChecker.Col && (chkbxBoard[curSquare.Row + 1, curSquare.Col - 1].Checker == CheckerType.Red || chkbxBoard[curSquare.Row + 1, curSquare.Col - 1].Checker == CheckerType.RedKing))
                {
                    chkbxBoard[curSquare.Row + 1, curSquare.Col - 1].Checker = CheckerType.Empty;
                    bool newKing = false;

                    //Make the piece a king if jumping into the 0th row.
                    if (curSquare.Row == 0 && chkSelectedChecker.Checker == CheckerType.Black)
                    {
                        chkbxBoard[curSquare.Row, curSquare.Col].Checker = CheckerType.BlackKing;
                        newKing = true;
                    }                        
                    else
                        chkbxBoard[curSquare.Row, curSquare.Col].Checker = chkbxBoard[curSquare.Row + 2, curSquare.Col - 2].Checker;

                    chkbxBoard[curSquare.Row + 2, curSquare.Col - 2].Checker = CheckerType.Empty;
                    chkSelectedChecker = chkbxBoard[curSquare.Row, curSquare.Col];

                    //If there are no more jumps to make (or there are, but the piece is a new king)
                    //clear the selected checker
                    //redraw the board
                    //let the computer take a turn (return true)
                    if (!hasJumps(chkSelectedChecker) || newKing)
                    {
                        midJump = false;
                        chkSelectedChecker = null;
                        updateRBK();
                        drawBoard(Red, Black, Kings);
                        return true;
                    }
                    //there are jumps. redraw the board
                    //don't let the computer go.
                    else
                    {
                        midJump = true;
                        updateRBK();
                        drawBoard(Red, Black, Kings);
                        return false;
                    }
                }
                //Additional Jumps for a Black King
                else if (curSquare.Row - 2 == chkSelectedChecker.Row && curSquare.Col - 2 == chkSelectedChecker.Col && (chkbxBoard[curSquare.Row - 1, curSquare.Col - 1].Checker == CheckerType.Red || chkbxBoard[curSquare.Row - 1, curSquare.Col - 1].Checker == CheckerType.RedKing) && chkSelectedChecker.Checker == CheckerType.BlackKing)
                {
                    chkbxBoard[curSquare.Row - 1, curSquare.Col - 1].Checker = CheckerType.Empty;
                    chkbxBoard[curSquare.Row, curSquare.Col].Checker = chkbxBoard[curSquare.Row - 2, curSquare.Col - 2].Checker;
                    chkbxBoard[curSquare.Row - 2, curSquare.Col - 2].Checker = CheckerType.Empty;
                    chkSelectedChecker = chkbxBoard[curSquare.Row, curSquare.Col];

                    //If there are no more jumps to make
                    //clear the selected checker
                    //redraw the board
                    //let the computer take a turn (return true)
                    if (!hasJumps(chkSelectedChecker))
                    {
                        midJump = false;
                        chkSelectedChecker = null;
                        updateRBK();
                        drawBoard(Red, Black, Kings);
                        return true;
                    }
                    //there are jumps. redraw the board
                    //don't let the computer go.
                    else
                    {
                        midJump = true;
                        updateRBK();
                        drawBoard(Red, Black, Kings);
                        return false;
                    }
                }
                else if (curSquare.Row - 2 == chkSelectedChecker.Row && curSquare.Col + 2 == chkSelectedChecker.Col && (chkbxBoard[curSquare.Row - 1, curSquare.Col + 1].Checker == CheckerType.Red || chkbxBoard[curSquare.Row - 1, curSquare.Col + 1].Checker == CheckerType.RedKing) && chkSelectedChecker.Checker == CheckerType.BlackKing)
                {
                    chkbxBoard[curSquare.Row - 1, curSquare.Col + 1].Checker = CheckerType.Empty;
                    chkbxBoard[curSquare.Row, curSquare.Col].Checker = chkbxBoard[curSquare.Row - 2, curSquare.Col + 2].Checker;
                    chkbxBoard[curSquare.Row - 2, curSquare.Col + 2].Checker = CheckerType.Empty;
                    chkSelectedChecker = chkbxBoard[curSquare.Row, curSquare.Col];

                    //If there are no more jumps to make
                    //clear the selected checker
                    //redraw the board
                    //let the computer take a turn (return true)
                    if (!hasJumps(chkSelectedChecker))
                    {
                        midJump = false;
                        chkSelectedChecker = null;
                        updateRBK();
                        drawBoard(Red, Black, Kings);
                        return true;
                    }
                    //there are jumps. redraw the board
                    //don't let the computer go.
                    else
                    {
                        midJump = true;
                        updateRBK();
                        drawBoard(Red, Black, Kings);
                        return false;
                    }
                        
                }
            }  
            //no valid move was found
            return false;
        }

        //hasJumps has two parts.
        //part1: if there is a checker selected, report true if only that checker has jumps. Otherwise false.
        //part2: if no checker is selected, report true if any black checker has a jump. Otherwise false.
        private  bool hasJumps(CheckerBox curChecker = null)
        {
            //part 1
            if (curChecker != null)
            {
                //Could jump up.
                if (curChecker.Row>1)
                {
                    //Could jump up and left
                    if (curChecker.Col > 1 && (chkbxBoard[curChecker.Row - 1, curChecker.Col - 1].Checker == CheckerType.Red || chkbxBoard[curChecker.Row - 1, curChecker.Col - 1].Checker == CheckerType.RedKing) && chkbxBoard[curChecker.Row-2, curChecker.Col-2].Checker == CheckerType.Empty)
                    {
                        return true;
                    }

                    //Could jump up and right
                    if (curChecker.Col < 6 && (chkbxBoard[curChecker.Row - 1, curChecker.Col + 1].Checker == CheckerType.Red || chkbxBoard[curChecker.Row - 1, curChecker.Col + 1].Checker == CheckerType.RedKing) && chkbxBoard[curChecker.Row - 2, curChecker.Col + 2].Checker == CheckerType.Empty)
                    {
                        return true;
                    }
                }

                //Could jump down
                if (curChecker.Row < 6 && curChecker.Checker == CheckerType.BlackKing)
                {

                    //Could jump down and left
                    if (curChecker.Col > 1 && (chkbxBoard[curChecker.Row + 1, curChecker.Col - 1].Checker == CheckerType.Red || chkbxBoard[curChecker.Row + 1, curChecker.Col - 1].Checker == CheckerType.RedKing) && chkbxBoard[curChecker.Row + 2, curChecker.Col - 2].Checker == CheckerType.Empty)
                    {
                        return true;
                    }

                    //Could jump down and right
                    if (curChecker.Col < 6 && (chkbxBoard[curChecker.Row + 1, curChecker.Col + 1].Checker == CheckerType.Red || chkbxBoard[curChecker.Row + 1, curChecker.Col + 1].Checker == CheckerType.RedKing) && chkbxBoard[curChecker.Row + 2, curChecker.Col + 2].Checker == CheckerType.Empty)
                    {
                        return true;
                    }
                }
            }
            //part 2
            else
            {
                for (int row=0;row<8;row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        if ((chkbxBoard[row, col].Checker == CheckerType.Black || chkbxBoard[row, col].Checker == CheckerType.BlackKing) && hasJumps(chkbxBoard[row, col]))
                            return true;
                    }
                }
            }

            return false;
        }

        //Updates the Red, Black and King values based on the current state of the pictureboxes on the board
        private void updateRBK()
        {
            Red = Black = Kings = 0;
            int curPos = 0;
            int bit = 0;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (curPos % 2 == 1)
                    {
                        switch (chkbxBoard[row, col].Checker)
                        {
                            case CheckerType.Black:
                                Black = Black | (uint)(1 << bit);
                                break;
                            case CheckerType.BlackKing:
                                Black = Black | (uint)(1 << bit);
                                Kings = Kings | (uint)(1 << bit);
                                break;
                            case CheckerType.Red:
                                Red = Red | (uint)(1 << bit);
                                break;
                            case CheckerType.RedKing:
                                Red = Red | (uint)(1 << bit);
                                Kings = Kings | (uint)(1  << bit);
                                break;
                        }
                        bit++;
                    }

                    curPos++;
                }
                curPos++;
            }
        }
        
        int idx = 0;
        private bool checkJump(CheckerType yourColor, uint oppositeColor, int pos, int boardType, ref string returnDir, ref int des, ref int res,ref string[] savePath, bool kingorNot) //boardType is 1st half and 2nd half
        {
            bool makeJump = false;
            int topLeft, topRight, botLeft, botRight;
            int topLeftDes, topRightDes, botLeftDes, botRightDes;
            if (boardType == 1) { topLeft = pos - 4; topRight = pos - 3; botLeft = pos + 4; botRight = pos + 5; topLeftDes = pos - 9; topRightDes = pos - 7; botLeftDes = pos + 7; botRightDes = pos + 9; }
            else { topLeft = pos - 5; topRight = pos - 4; botLeft = pos + 3; botRight = pos + 4; topLeftDes = pos - 9; topRightDes = pos - 7; botLeftDes = pos + 7; botRightDes = pos + 9; }
            //if king, if not king
            if (yourColor == CheckerType.Red || kingorNot)
            {

                if (((oppositeColor & (1U << botLeft)) != 0) && botLeftDes >= 0 && botLeftDes <= 32 && Convert.ToBoolean(~(Red | Black) & (1 << botLeftDes)))
                {
                    makeJump = true;
                    des = botLeftDes;
                    returnDir += " jump-botleft";
                    //savePath = savePath.Concat(new string[] { returnDir }).ToArray();
                    
                    if (checkJump(yourColor, oppositeColor, des, boardType, ref returnDir, ref des, ref res,ref savePath,kingorNot))
                    {
                        res--;
                    }
                    else
                    {
                        savePath[idx] = returnDir;
                        idx++;
                        
                    }
                    returnDir = returnDir.Substring(0, returnDir.Length - 13);
                    res++;
                    
                }
                
                if (((oppositeColor & (1U << botRight)) != 0) && botRightDes >= 0 && botRightDes <= 32 && Convert.ToBoolean(~(Red | Black) & (1 << botRightDes)))
                {
                    
                    makeJump = true;
                    des = botRightDes;
                    returnDir += " jump-botrigh";
                    //savePath = savePath.Concat(new string[] { returnDir }).ToArray();
                    
                    if (checkJump(yourColor, oppositeColor, des, boardType, ref returnDir, ref des, ref res, ref savePath, kingorNot))
                    {
                        res--;
                    }
                    else
                    {
                        savePath[idx] = returnDir;
                        idx++;
                        
                    }
                    returnDir = returnDir.Substring(0, returnDir.Length - 13);
                    res++;
                   
                }
                
            }
            if (yourColor == CheckerType.Black || kingorNot)
            {
                if (((oppositeColor & (1U << topLeft)) != 0) && topLeftDes >= 0 && topLeftDes <= 32 && Convert.ToBoolean(~(Red | Black) & (1 << topLeftDes)))
                {
                    makeJump = true;
                    returnDir += " jump-topleft";
                    des = topLeftDes;
                    //savePath = savePath.Concat(new string[] { returnDir }).ToArray();
                    
                    if (checkJump(yourColor, oppositeColor, des, boardType, ref returnDir, ref des, ref res, ref savePath, kingorNot))
                    {
                        res--;
                    }
                    else
                    {
                        savePath[idx] = returnDir;
                        idx++;
                        
                    }
                    res++;
                    returnDir = returnDir.Substring(0, returnDir.Length - 13);
                }
                
                if (((oppositeColor & (1U << topRight)) != 0) && topRightDes >= 0 && topRightDes <= 32 && Convert.ToBoolean(~(Red | Black) & (1 << topRightDes)))
                {
                    makeJump = true;
                    returnDir += " jump-toprigh";
                    des = topRightDes;
                    //savePath = savePath.Concat(new string[] { returnDir }).ToArray();
                    
                    if (checkJump(yourColor, oppositeColor, des, boardType, ref returnDir, ref des, ref res, ref savePath, kingorNot))
                    {
                        res--;
                    }
                    else
                    {
                        savePath[idx] = returnDir;
                        idx++;
                        
                    }
                    res++;
                    returnDir = returnDir.Substring(0, returnDir.Length - 13);
                }
                
            }
            if (makeJump==true)
            {
                return true;
            }
            else
            {
                
                return false;
            }
        }
        //Put your game tree here.
        //Use the values Red, Black and King to determine
        //Where the pieces are on the board currently.

        //Make sure to  assign your chosen values back to these numbers
        //and call drawBoard() when finished.
        private void GetMoves(CheckMove[] move, ref int moveCount, CheckerType color, uint black, uint red, uint king)
        {
            moveCount = 0;
            //this is Reg Move case; //move, jump , red (redking),black(blackking)
            if (color == CheckerType.Red || color == CheckerType.RedKing)
            {
                for (int i = 0; i <= 24; i += 8) //1st half of the board
                {
                    for (int j = 0; j <= 3; j++)
                    {
                        int pos = i + j;
                        bool kingCheck = false;
                        if(((king & (1U << pos)) != 0)) { kingCheck = true; }
                        if (Convert.ToBoolean(Red & (1 << pos))) //there is a red
                        {
                            
                            int topLeft, topRight, botLeft, botRight;
                            topLeft = pos - 4; topRight = pos - 3; botLeft = pos + 4; botRight = pos + 5;
                            string jumpDir = "";
                            int des=-1000000000;
                            int totalMove = 0;
                            string[] path = new string[1000];
                            //if jumps else move
                            if (checkJump(color, black, pos, 1, ref jumpDir,ref des, ref totalMove, ref path, kingCheck))//jumps
                            {
                                idx = 0;
                                for(int it=0;it<totalMove;it++)
                                {
                                    List<int> tempCaptured = new List<int>();
                                    int tempDes=pos;
                                    
                                    while (path[it]!="")
                                    {
                                        topLeft = tempDes - 4; topRight = tempDes - 3; botLeft = tempDes + 4; botRight = tempDes + 5;
                                        if (path[it].Substring(0,13)==" jump-botleft")
                                        {
                                            tempCaptured.Add(botLeft);
                                            tempDes += 7;
                                        }
                                        else if(path[it].Substring(0, 13) == " jump-botrigh")
                                        {
                                            tempCaptured.Add(botRight);
                                            tempDes += 9;
                                        }
                                        else if(path[it].Substring(0, 13) == " jump-topleft")
                                        {
                                            tempCaptured.Add(topLeft);
                                            tempDes -= 9;
                                        }
                                        else
                                        {
                                            tempCaptured.Add(topRight);
                                            tempDes -= 7;
                                        }
                                        path[it] = path[it].Remove(0, 13);
                                    }
                                    move[moveCount].Black = black;
                                    move[moveCount].King = king;
                                    move[moveCount].Red = red;
                                    for (int x=0;x<tempCaptured.Count;x++)
                                    {
                                        if((king&(1U<<tempCaptured[x]))!=0)
                                        {
                                            move[moveCount].King = move[moveCount].King ^ (1U << tempCaptured[x]); //remove king
                                        }
                                        move[moveCount].Black = move[moveCount].Black ^ (1U << tempCaptured[x]);
                                    }
                                  
                                    if ((king & (1U << pos)) != 0)
                                    { 
                                        move[moveCount].King = (move[moveCount].King | 1U << tempDes) ^ (1U << pos); // move king
                                    }
                                    move[moveCount].Red = (move[moveCount].Red | 1U << tempDes) ^ (1U << pos);                                    
                                    moveCount++;
                                }
                            }
                            else //moves
                            {
                                if (Convert.ToBoolean(~(Red | Black) & (1 << botLeft))) // available to move
                                {
                                    move[moveCount].Red = red;
                                    move[moveCount].Red = (move[moveCount].Red | 1U << botLeft) ^ (1U << pos);
                                    
                                    //become king
                                    move[moveCount].King = king;
                                    if ((king & (1U << pos)) != 0) { move[moveCount].King = (move[moveCount].King | 1U << botLeft) ^ (1U << pos); }
                                    if (botLeft>=24&&botLeft<=27) { move[moveCount].King = (move[moveCount].King | 1U << botLeft); }
                                    move[moveCount].Black = black;
                                    moveCount++;
                                }
                                if (Convert.ToBoolean(~(Red | Black) & (1 << botRight)) && (j != 3))
                                {
                                    move[moveCount].Red = (red | 1U << botRight) ^ (1U << pos);
                                    move[moveCount].King = king;
                                    if ((king & (1U << pos)) != 0) { move[moveCount].King = (move[moveCount].King | 1U << botRight) ^ (1U << pos); }
                                    if (botRight >= 24 && botRight <= 27) { move[moveCount].King = (move[moveCount].King | 1U << botLeft); }
                                    move[moveCount].Black = black;
                                    moveCount++;
                                }
                                if ((king&(1U<<pos))!=0) //extra moves for king
                                {
                                    if (Convert.ToBoolean(~(Red | Black) & (1 << topLeft)) && (i >= 8))
                                    {
                                        move[moveCount].Red = (red | 1U << topLeft) ^ (1U << pos);
                                        move[moveCount].King = (king | 1U << topLeft) ^ (1U << pos);
                                        move[moveCount].Black = black;
                                        moveCount++;
                                    }
                                    if (Convert.ToBoolean(~(Red | Black) & (1 << topRight)) && (i >= 8) && (j != 3))
                                    {
                                        move[moveCount].Red = (red | 1U << topRight) ^ (1U << pos);
                                        move[moveCount].King = (king | 1U << topRight) ^ (1U << pos);
                                        move[moveCount].Black = black;
                                        moveCount++;
                                    }
                                }
                            }
                        }
                    }
                }

                for (int i = 4; i <= 28; i += 8) //2nd half of the board
                {
                    for (int j = 0; j <= 3; j++)
                    {
                        int pos = i + j;
                        bool kingCheck = false;
                        if (((king & (1U << pos)) != 0)) { kingCheck = true; }
                        if (Convert.ToBoolean(Red & (1 << pos))) //there is a red
                        {
                            
                            int topLeft, topRight, botLeft, botRight;
                            topLeft = pos - 5; topRight = pos - 4; botLeft = pos + 3; botRight = pos + 4;
                            string jumpDir = "";
                            int des = -1000000000;
                            int totalMove = 0;
                            string[] path = new string[1000];
                            bool becomeKing = false;
                            //if jumps else move
                            if (checkJump(color, black, pos, 2, ref jumpDir, ref des, ref totalMove, ref path, kingCheck))//jumps
                            {
                                idx = 0;
                                for (int it = 0; it < totalMove; it++)
                                {
                                    List<int> tempCaptured = new List<int>();
                                    int tempDes = pos;

                                    while (path[it] != "")
                                    {
                                        topLeft = tempDes - 5; topRight = tempDes - 4; botLeft = tempDes + 3; botRight = tempDes + 4;
                                        if (path[it].Substring(0, 13) == " jump-botleft")
                                        {
                                            tempCaptured.Add(botLeft);
                                            tempDes += 7;
                                        }
                                        else if (path[it].Substring(0, 13) == " jump-botrigh")
                                        {
                                            tempCaptured.Add(botRight);
                                            tempDes += 9;
                                        }
                                        else if (path[it].Substring(0, 13) == " jump-topleft")
                                        {
                                            tempCaptured.Add(topLeft);
                                            tempDes -= 9;
                                        }
                                        else
                                        {
                                            tempCaptured.Add(topRight);
                                            tempDes -= 7;
                                        }
                                        path[it] = path[it].Remove(0, 13);
                                    }
                                    move[moveCount].Red = red;
                                    move[moveCount].King = king;
                                    move[moveCount].Black = black;
                                    for (int x = 0; x < tempCaptured.Count; x++)
                                    {
                                        if ((king & (1U << tempCaptured[x])) != 0)
                                        {
                                            move[moveCount].King = move[moveCount].King ^ (1U << tempCaptured[x]); //remove king
                                        }
                                        move[moveCount].Black = move[moveCount].Black ^ (1U << tempCaptured[x]);
                                    }
                                    if (tempDes >= 28 && tempDes <= 31)
                                    {
                                        move[moveCount].King = move[moveCount].King | 1U << tempDes; //become king
                                    }
                                    if ((king & (1U << pos)) != 0)
                                    {
                                        move[moveCount].King = (move[moveCount].King | 1U << tempDes) ^ (1U << pos); //move king
                                    }
                                    move[moveCount].Red = (move[moveCount].Red | 1U << tempDes) ^ (1U << pos);
                                    moveCount++;
                                }
                            }
                            else //moves
                            {
                                if (Convert.ToBoolean(~(Red | Black) & (1 << botLeft)) && (j != 0)) // available to move
                                {
                                    move[moveCount].Red = (red | 1U << botLeft) ^ (1U << pos);
                                    move[moveCount].King = king;
                                    if ((king & (1U << pos)) != 0) { move[moveCount].King = (move[moveCount].King | 1U << botLeft) ^ (1U << pos); }
                                    move[moveCount].Black = black;
                                    moveCount++;
                                }
                                if (Convert.ToBoolean(~(Red | Black) & (1 << botRight)) )
                                {
                                    move[moveCount].Red = (red | 1U << botRight) ^ (1U << pos);
                                    move[moveCount].King = king;
                                    if ((king & (1U << pos)) != 0) { move[moveCount].King = (move[moveCount].King | 1U << botRight) ^ (1U << pos); }
                                    move[moveCount].Black = black;
                                    moveCount++;
                                }
                                if ((king & (1U << pos)) != 0) //extra moves for king
                                {
                                    if (Convert.ToBoolean(~(Red | Black) & (1 << topLeft)) && (i >= 8))
                                    {
                                        move[moveCount].Red = (red | 1U << topLeft) ^ (1U << pos);
                                        move[moveCount].King = (king | 1U << topLeft) ^ (1U << pos);
                                        move[moveCount].Black = black;
                                        moveCount++;
                                    }
                                    if (Convert.ToBoolean(~(Red | Black) & (1 << topRight)) && (i >= 8) && (j != 3))
                                    {
                                        move[moveCount].Red = (red | 1U << topRight) ^ (1U << pos);
                                        move[moveCount].King = (king | 1U << topRight) ^ (1U << pos);
                                        move[moveCount].Black = black;
                                        moveCount++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (color == CheckerType.Black || color == CheckerType.BlackKing)  //black
            {
                for (int i = 0; i <= 24; i += 8) //1st half of the board
                {
                    for (int j = 0; j <= 3; j++)
                    {
                        int pos = i + j;
                        bool kingCheck = false;
                        if (((king & (1U << pos)) != 0)) { kingCheck = true; }
                        if (Convert.ToBoolean(black & (1 << pos))) //there is a black
                        {
                            
                            int topLeft, topRight, botLeft, botRight;
                            topLeft = pos - 4; topRight = pos - 3; botLeft = pos + 4; botRight = pos + 5;
                            string jumpDir = "";
                            int des = -10000000;
                            int totalMove = 0;
                            string[] path = new string[1000];
                           
                            //if jumps else move
                            if (checkJump(color, red, pos, 1, ref jumpDir, ref des, ref totalMove, ref path, kingCheck))//jumps
                            {
                                idx = 0;
                                for (int it = 0; it < totalMove; it++)
                                {
                                    List<int> tempCaptured = new List<int>();
                                    int tempDes = pos;

                                    while (path[it] != "")
                                    {
                                        topLeft = tempDes - 4; topRight = tempDes - 3; botLeft = tempDes + 4; botRight = tempDes + 5;
                                        if (path[it].Substring(0, 13) == " jump-botleft")
                                        {
                                            tempCaptured.Add(botLeft);
                                            tempDes += 7;
                                        }
                                        else if (path[it].Substring(0, 13) == " jump-botrigh")
                                        {
                                            tempCaptured.Add(botRight);
                                            tempDes += 9;
                                        }
                                        else if (path[it].Substring(0, 13) == " jump-topleft")
                                        {
                                            tempCaptured.Add(topLeft);
                                            tempDes -= 9;
                                        }
                                        else
                                        {
                                            tempCaptured.Add(topRight);
                                            tempDes -= 7;
                                        }
                                        path[it] = path[it].Remove(0, 13);
                                    }
                                    move[moveCount].Red = red;
                                    move[moveCount].King = king;
                                    move[moveCount].Black = black;
                                    for (int x = 0; x < tempCaptured.Count; x++)
                                    {
                                        if ((king & (1U << tempCaptured[x])) != 0)
                                        {
                                            move[moveCount].King = move[moveCount].King ^ (1U << tempCaptured[x]); //remove king
                                        }
                                        move[moveCount].Red = move[moveCount].Red ^ (1U << tempCaptured[x]);
                                    }
                                    if(tempDes>=0 && tempDes<=3)
                                    {
                                        move[moveCount].King = move[moveCount].King | 1U << tempDes; //become king
                                    }
                                    if((king&(1U<<pos))!=0)
                                    {
                                        move[moveCount].King = (move[moveCount].King | 1U <<tempDes) ^ (1U << pos); //move king
                                    }
                                    move[moveCount].Black = (move[moveCount].Black | 1U << tempDes) ^ (1U << pos);
                                    moveCount++;
                                }
                            }
                            else //moves
                            {
                                if (Convert.ToBoolean(~(Red | Black) & (1 << topLeft))) // available to move
                                {
                                    move[moveCount].Red = red;
                                    move[moveCount].King = king;
                                    if ((king & (1U << pos)) != 0) { move[moveCount].King = (move[moveCount].King | 1U << topLeft) ^ (1U << pos); }
                                    move[moveCount].Black = (black | 1U << topLeft) ^ (1U << pos);
                                    moveCount++;
                                }
                                if (Convert.ToBoolean(~(Red | Black) & (1 << topRight)) && (j != 3))
                                {
                                    move[moveCount].Red = red;
                                    move[moveCount].King = king;
                                    if ((king & (1U << pos)) != 0) { move[moveCount].King = (move[moveCount].King | 1U << topRight) ^ (1U << pos); }
                                    move[moveCount].Black = (black | 1U << topRight) ^ (1U << pos);
                                    moveCount++;
                                }
                                if ((king & (1U << pos)) != 0) //extra moves for king
                                {
                                    if (Convert.ToBoolean(~(Red | Black) & (1 << botLeft)) && (i >= 8))
                                    {
                                        move[moveCount].Red = red;
                                        move[moveCount].King = (king | 1U << botLeft) ^ (1U << pos);
                                        move[moveCount].Black = (black | 1U << botLeft) ^ (1U << pos);
                                        moveCount++;
                                    }
                                    if (Convert.ToBoolean(~(Red | Black) & (1 << botRight)) && (i >= 8) && (j != 3))
                                    {
                                        move[moveCount].Red =red;
                                        move[moveCount].King = (king | 1U << botRight) ^ (1U << pos);
                                        move[moveCount].Black = (black | 1U << botRight) ^ (1U << pos);
                                        moveCount++;
                                    }
                                }
                            }
                        }
                    }
                }

                for (int i = 4; i <= 28; i += 8) //2nd half of the board
                {
                    for (int j = 0; j <= 3; j++)
                    {
                        int pos = i + j;
                        bool kingCheck = false;
                        if (((king & (1U << pos)) != 0)) { kingCheck = true; }
                        if (Convert.ToBoolean(Black & (1 << pos))) //there is a red
                        {
                            
                            int topLeft, topRight, botLeft, botRight;
                            topLeft = pos - 5; topRight = pos - 4; botLeft = pos + 3; botRight = pos + 4;
                            string jumpDir = "";
                            int des = -100000000;
                            int totalMove = 0;
                            string[] path = new string[1000];
                            //if jumps else move
                            if (checkJump(color, red, pos, 2, ref jumpDir, ref des, ref totalMove, ref path, kingCheck))//jumps
                            {
                                idx = 0;
                                for (int it = 0; it < totalMove; it++)
                                {
                                    List<int> tempCaptured = new List<int>();
                                    int tempDes = pos;

                                    while (path[it] != "")
                                    {
                                        topLeft = tempDes - 5; topRight = tempDes - 4; botLeft = tempDes + 3; botRight = tempDes + 4;
                                        if (path[it].Substring(0, 13) == " jump-botleft")
                                        {
                                            tempCaptured.Add(botLeft);
                                            tempDes += 7;
                                        }
                                        else if (path[it].Substring(0, 13) == " jump-botrigh")
                                        {
                                            tempCaptured.Add(botRight);
                                            tempDes += 9;
                                        }
                                        else if (path[it].Substring(0, 13) == " jump-topleft")
                                        {
                                            tempCaptured.Add(topLeft);
                                            tempDes -= 9;
                                        }
                                        else
                                        {
                                            tempCaptured.Add(topRight);
                                            tempDes -= 7;
                                        }
                                        path[it] = path[it].Remove(0, 13);
                                    }
                                    move[moveCount].Red = red;
                                    move[moveCount].King = king;
                                    move[moveCount].Black = black;
                                    for (int x = 0; x < tempCaptured.Count; x++)
                                    {
                                        if ((king & (1U << tempCaptured[x])) != 0)
                                        {
                                            move[moveCount].King = move[moveCount].King ^ (1U << tempCaptured[x]); //remove king
                                        }
                                        move[moveCount].Red = move[moveCount].Red ^ (1U << tempCaptured[x]);
                                    }
                                    if ((king & (1U << pos)) != 0)
                                    {
                                        move[moveCount].King = (move[moveCount].King | 1U << tempDes) ^ (1U << pos); //move king
                                    }
                                    move[moveCount].Black = (move[moveCount].Black | 1U << tempDes) ^ (1U << pos);
                                    moveCount++;
                                }
                            }
                            else //moves
                            {
                                if (Convert.ToBoolean(~(Red | Black) & (1 << topLeft)) && (j != 0)) // available to move
                                {
                                    move[moveCount].Red =red;
                                    move[moveCount].King = king;
                                    if (topLeft >= 24 && topLeft <= 27) { move[moveCount].King = (move[moveCount].King | 1U << topLeft); }
                                    if ((king & (1U << pos)) != 0) { move[moveCount].King = (move[moveCount].King | 1U << topLeft) ^ (1U << pos); }
                                    move[moveCount].Black = (black | 1U << topLeft) ^ (1U << pos);
                                    moveCount++;
                                }
                                if (Convert.ToBoolean(~(Red | Black) & (1 << topRight)))
                                {
                                    move[moveCount].Red = red;
                                    move[moveCount].King = king;
                                    if (topRight >= 24 && topRight <= 27) { move[moveCount].King = (move[moveCount].King | 1U << topRight); }
                                    if ((king & (1U << pos)) != 0) { move[moveCount].King = (move[moveCount].King | 1U << topRight) ^ (1U << pos); }
                                    move[moveCount].Black = (black | 1U << topRight) ^ (1U << pos);
                                    moveCount++;
                                }
                                if ((king & (1U << pos)) != 0) //extra moves for king
                                {
                                    if (Convert.ToBoolean(~(Red | Black) & (1 << botLeft)) && (i >= 8))
                                    {
                                        move[moveCount].Red = red;
                                        move[moveCount].King = (king | 1U << botLeft) ^ (1U << pos);
                                        move[moveCount].Black = (black | 1U << botLeft) ^ (1U << pos);
                                        moveCount++;
                                    }
                                    if (Convert.ToBoolean(~(Red | Black) & (1 << botRight)) && (i >= 8) && (j != 3))
                                    {
                                        move[moveCount].Red = red;
                                        move[moveCount].King = (king | 1U << botRight) ^ (1U << pos);
                                        move[moveCount].Black = (black | 1U << botRight) ^ (1U << pos);
                                        moveCount++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        bool winCondition(uint red,uint black)
        {
            if(black==0)
            {
                //red win
                return true;
            }
            
            else
            {
                return false;
            }
        }

        int getHeuristicVal(uint red, uint black, uint king)
        {
            int redCount=0, blackCount=0, redKingCount=0, blackKingCount=0;
            for (int i = 0; i < 32; i++)
            {
                if ((black & (1U << i)) != 0 && (king & (1U << i)) != 0)
                {
                    blackKingCount=blackKingCount+1;
                }
                if ((red & (1U << i)) != 0 && (king & (1U << i)) != 0)
                {
                    redKingCount= redKingCount + 1;
                }
                if ((black & (1U << i)) != 0 && (king & (1U << i)) == 0)
                {
                    blackCount= blackCount + 1;
                }
                if ((red & (1U << i)) != 0 && (king & (1U << i)) == 0)
                {
                    redCount= redCount + 1;
                }
            }
            return redCount*2 - blackCount*2 + (redKingCount  - blackKingCount );
        }

        

        private List<object> ComputerRec(uint redParam, uint blackParam, uint kingParam, int ply,int alpha, int beta, bool Maxplayer)
        {
            uint red = 0;
            uint black = 0;
            uint king = 0;
            int evaluation=0;
            List<object> position = new List<object>() { red, black, king, evaluation };

            if(ply==0)
            {
                position[3] = (uint)getHeuristicVal(redParam, blackParam, kingParam);
                position[0] = redParam; position[1] = blackParam; position[2] = kingParam;
            }

            CheckMove[] listofMoves = new CheckMove[48];
            for (int i = 0; i < 48; i++)
            {
                listofMoves[i] = new CheckMove();
            }
            int totalValidMoves = 0;
            
            if (Maxplayer)
            {
                int maxVal = int.MinValue;
                GetMoves(listofMoves, ref totalValidMoves, CheckerType.Red, blackParam, redParam, kingParam);
                //Thread[] ThreadArr = new Thread[totalValidMoves];
                for (int i =0;i<totalValidMoves;i++)
                {
                    //ThreadArr[i] = new Thread(new ParameterizedThreadStart(ComputerRec(listofMoves[i].Red, listofMoves[i].Black, listofMoves[i].King, ply - 1, alpha, beta, false)))
                    position[3] = ComputerRec(listofMoves[i].Red, listofMoves[i].Black, listofMoves[i].King,ply-1,alpha,beta,false)[3];
                    maxVal = Math.Max(maxVal, (int)evaluation);
                    alpha = Math.Max(alpha, (int)evaluation);
                    if(beta<=alpha) { break; }
                    if(maxVal==evaluation) { position[0] = listofMoves[i].Red; position[1] = listofMoves[i].Black; position[2] = listofMoves[i].King; }
                }
                return position;
            }
            else
            {
                int minVal = int.MaxValue;
                GetMoves(listofMoves, ref totalValidMoves, CheckerType.Black, blackParam, redParam, kingParam);
                for (int i = 0; i < totalValidMoves; i++)
                {
                    position[3] = ComputerRec(listofMoves[i].Red, listofMoves[i].Black, listofMoves[i].King, ply - 1, alpha, beta, true)[3];
                    minVal = Math.Max(minVal, (int)evaluation);
                    beta = Math.Min(beta, (int)evaluation);
                    if (beta <= alpha) { break; }
                    if (minVal == evaluation) { position[0] = listofMoves[i].Red; position[1] = listofMoves[i].Black; position[2] = listofMoves[i].King; }
                }
                return position;
            }
        }

      
        private void ComputerMove()
        {
            MessageBox.Show("Computer's Turn");
            List<object> newPos = ComputerRec(Red, Black, Kings, 0, int.MinValue, int.MaxValue, true);
            drawBoard((uint)newPos[0], (uint)newPos[1], (uint)newPos[2]);
            /*Thread[] ThreadArr = new Thread[48];

            for (int curMove = 0; curMove < 48; curMove++)
            {
                ThreadArr[curMove] = new Thread(new ParameterizedThreadStart(ThreadMove));
                ThreadArr[curMove].Start(InitialMoves[curMove]);
            }
*/
        }

        private void ThreadMove(uint redParam, uint blackParam, uint kingParam, int ply, int alpha, int beta, bool Maxplayer)
        {
           
            
        }


    }
}
