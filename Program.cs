/*
 * ${res:XML.StandardHeader.CreatedByJohnnyCasual}
 *${res:XML.StandardHeader.©Johnny Casual}{YEAR}
 * Date: 27.3.2014
 * Time: 16:06
 */
using System;
using System.IO;
using System.Linq;
using System.Reflection;
namespace lotterySimulator
{
	class Program
	{
		public static void Main(string[] args)
		{
			Assembly assem = Assembly.GetExecutingAssembly();
			AssemblyName assemName = assem.GetName();
			Version ver = assemName.Version;
			Console.Write("//////////(L)(O)(T)(T)(E)(R)(Y)(S)(I)(M)(U)(L)(A)(T)(O)(R)//////////\n//\t\t\t\t\t\t\t\t //\n//\tLets find out your theoretical odds in Lottery \t\t//\n//\t\t\t\t\t\t\t       //\n//\tCreated by: Johnny Casual (C) 2014\t\t      //\n//\tVersion: {0}", ver.ToString() +"\t\t\t\t     //\n//\tContact: johnniecasual@gmail.com\t\t    //\n//\t\t\t\t\t\t\t   //\n////////////////////////////////////////////////////////////\n\n");
			
			Console.WriteLine("With this simulator you can test your theoretical chances to win \nthe lottery jackpot or any smaller wins using the same row all the time.\n\nThe program runs until you have either won the jackpot by having all your \nnumbers in generated lotteryrow or your moneybalance (which starts from 1e) \nis more 'below freezing' than you could possibly win to get it \nback to the good.\n\nAfter the program is finished you can take a look at 2 log-files called \n'log.txt' and 'winningRaffles.txt' where you can find different recorded data \nabout the lottery raffles.\n\n");
			Console.Write("Pres any key to continue...");
			Console.ReadKey(true);
			Console.Clear();
			Console.Write("Do you want to choose your numbers by yourself[1] or let the computer draw you one[2]\n\nChoose 1 or 2: ");
			
			int[] userLottery = new int[7];
			int loop = 0;
			
			while(loop < 1 || loop > 2)
			{
				string userAnswer = Console.ReadLine();
				if(!int.TryParse(userAnswer, out loop))
				{
					Console.Write("Invalid answer! Please input 1 or 2: ");
				}
				
				else
				{
					if(loop == 1)
					{
						Console.Clear();
						int i = 0;
						while (i < 7) //Mr Thorsten Dittmar corrected/altered my code from my request http://stackoverflow.com/a/22805098/3488301
						{
							Console.Write("Your choice #{0}: ", i+1);// The part ""... #{0}: ", i+1" showes the current number to choose by increasing the current integer i by one since the i starts from 0 and increases only if the user input is valid
							string userRow = Console.ReadLine();
							int userNumber; //this variable is used to store valid input that comes after TryParsing!

							if (!Int32.TryParse(userRow, out userNumber) || userNumber < 1 || userNumber > 39)//if user input is not integer number or the number is out of the wanted range (1-39) the program gives error message
							{
								Console.WriteLine("Invalid number! Please try again!");
							}
							else if(userLottery.Contains(userNumber)) //I added this to find if the number already existed in array and it don't let the variable i increase since the while-loop must be ran until user has 7 unique numbers
							{
								Console.WriteLine("You had already chosen the number {0} ", userNumber);
							}
							else
							{
								userLottery[i++] = userNumber; //if the number is valid the userLottery-array gets the current value for that index
							}
						}
					}
					else if(loop == 2)
					{
						var drawLottery = Enumerable.Range(1, 39).OrderBy( x => Guid.NewGuid());
						userLottery = drawLottery.Distinct().Take(7).ToArray();
						Console.Clear();
					}
					else
						Console.Write("Invalid answer! Please input 1 or 2: ");
				}
			}
			Array.Sort(userLottery);
			Console.WriteLine("Your lucky row: " + "\n" + string.Join(", ", userLottery) + "\n");
			
			int runCounter = 0; //variable to keep track of the  total lottery raffles
			TextWriter logger = new StreamWriter("log.txt"); // 2 log files are created
			TextWriter winLogger = new StreamWriter("winningRaffles.txt");
			logger.WriteLine("You chose: " + string.Join(",", userLottery )+ "\n"); //user's row is saved in log-file
			var matchCount = 0; //variable to hold the matches in individual lottery raffle
			var matchCountPlus2 = 0; //and matches from plus-numbers
			double matchValue = 0;
			double matchAverage;
			double average = 0; //to calculate average matches
			decimal rowPrice = -1; //one lottery row/raffle costs 1e and it's decremented from moneybalance
			decimal winning = 0; //wins are based on matches in 7+2 computer lottery numbers
			decimal moneyBalance = 1; //  moneybalance starts from 1e
			int maxMatch = 0; // variables to store the maximum matches
			int maxMatchPlus = 0;
			int [] maxMatches = new int[2]; // array to hold the maximum match from 7 main numbers and 2 plus numbers
			string winningRows = ""; //string variable to save all winning information to winningRaffles.txt -log-file
			decimal winTotal = 0; // to count total wins in euros
			decimal winningAverage = 0; //to count average of all wins
			decimal winCounter = 0; //to count the times user win something
			while (moneyBalance >= -1000000) //loop runs until it's so low that 1 000 000e jackpot won't get it back to the good
			{

				var computerLottery = Enumerable.Range(1, 39).OrderBy( x => Guid.NewGuid()); //example found from http://www.wiktorzychla.com/2008/07/generating-random-sequences-with-linq.html //
				int[] winningRow = computerLottery.Distinct().Take(7).ToArray(); //duplicates are removed and 7 numbers are stored to winningRow array
				var computerLotteryPlus2 = Enumerable.Range(1,39).Where(y=> !winningRow.Contains(y)).OrderBy(x => Guid.NewGuid()); //I created this to generate plus numbers and check that they are not already existing in winninRow array
				int[] winningRowPlus2 = computerLotteryPlus2.Distinct().Take(2).ToArray();
				//sort both rows
				Array.Sort(winningRow);
				Array.Sort(winningRowPlus2);
				bool equal = userLottery.SequenceEqual(winningRow); //if all 7 main numbers match with userRow array
				if(equal) //the while loop quits
				{
					maxMatch = 7;
					winning = 1000000;
					moneyBalance = moneyBalance + 1000000; // maximum matches is now 7 and 1 000 000e jackpot is added to bank
					Console.WriteLine("Congrazulations!! You won the jackpot!\n");
				}
				else
				{
					runCounter++; //if not the runCounter increases by one
					matchCount = userLottery.Count(winningRow.Contains); //2 x two arrays are compared and the total matches is counted
					matchCountPlus2 = userLottery.Count(winningRowPlus2.Contains);
					matchValue = matchValue + matchCount;
					maxMatchPlus = matchCountPlus2;
					//maxMatches array is formed and the if-statement checks wheter in new raffle user got more main numbers or same as the previous maximum BUT more plus-numbers than on the previous times
					if(matchCount > maxMatch || matchCount == maxMatch && matchCountPlus2 > maxMatches[1])
					{
						maxMatch = matchCount;
						maxMatches[0] = maxMatch;
						maxMatches[1] = maxMatchPlus;
					}
					//next winning data  is formed on every raffle by following rules:
					if(matchCount == 3 && matchCountPlus2 == 1)
					{
						winning = 1M;
					}
					else if(matchCount == 3 && matchCountPlus2 == 2)
					{
						winning = 5M;
					}
					else if(matchCount == 4 && matchCountPlus2 < 1)
					{
						winning = 9.40M;
					}
					else if(matchCount == 4 && matchCountPlus2 == 1)
					{
						winning = 19.90M;
					}
					else if(matchCount == 4 && matchCountPlus2 == 2)
					{
						winning = 107.60M;
					}
					else if(matchCount == 5 && matchCountPlus2 < 1)
					{
						winning = 48.90M;
					}
					else if(matchCount == 5 && matchCountPlus2 == 1)
					{
						winning = 135.40M;
					}
					else if(matchCount == 5 && matchCountPlus2 == 2)
					{
						winning = 3384.40M;
					}
					else if(matchCount == 6 && matchCountPlus2 < 1)
					{
						winning = 1661.90M;
					}
					else if(matchCount == 6 && matchCountPlus2 == 1)
					{
						winning = 20475.80M;
					}

					else
					{
						winning = 0M;
					}
					
					if(winning != 0)
					{
						winCounter++; // if user has won at least the minimum 1e the winCounter -variable is increased by one
						winTotal = Math.Round(winTotal + winning); //total winnings in euros is calculated and rounded
					}
					moneyBalance = moneyBalance + rowPrice + winning; //money balance is now previous moneybalance minus rowprice + possible win
					decimal.Round(moneyBalance, 2); //money balance is rounded to 2 decimals
					if(matchCount >= 3 && matchCountPlus2 >= 1 || matchCount >= 4) //if user won something these data is added to winningRows-string:
						winningRows = winningRows + "#" + runCounter + " (" + matchCount + "+" + matchCountPlus2 + "), ";
					Console.Write("\rLottery raffle: " + runCounter + ". Max matches: " + string.Join("+", maxMatches) + ". Moneybalance: " + moneyBalance); //in console this information is updated
					logger.Write("Raffle nro: " + runCounter + ". Lucky row:  " + string.Join(", ", winningRow) +", Plus numbers: " + string.Join(", ", winningRowPlus2) + ". You had: " + matchCount + "+" + matchCountPlus2 + ". You won: " + winning + "e. Your money balance is: " + moneyBalance + "e");
					logger.WriteLine(); //and this information is stored to log.txt
				}

			}
			matchAverage = Math.Round(matchValue / runCounter); //match-average is counted
			average = matchAverage;
			winningAverage = Math.Round(winTotal / winCounter); // average win is counted
			winLogger.WriteLine("All the winning raffles: " + "\n"); // all the winning information is stored in winningRaffles.txt log-file
			winLogger.Write(winningRows);
			winLogger.Close();
			logger.Close(); //log-files are closed
			Console.WriteLine("\n\n" + "Total raffles: " + runCounter + ", average matches " + average + ", highest matches " + string.Join("+", maxMatches) + ", average win " + winningAverage + "e" + ", total wins " + winTotal +"e" + ", total winning times " + winCounter);
			//some data is printed in console
			Console.WriteLine("\n\nPress any key to exit...");
			Console.ReadKey(true);
			/////// (c)Joni Kämppä 2014
		}
	}
}