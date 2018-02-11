using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SeniorServer.DBPr;
using SeniorDBServer;
using SeniorServer;
namespace SeniorServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class NashServices : INsashServices
    {
        #region 2Players
        public List<int> Two_PlayerWrapper()//List<int> dummy , List<int> dummy2,int strat1 , int strat2)//dummy is the value of payoffs
        {
            #region OLD CODE //To be deleted at last
            //List<int> dummy = new List<int>{ 2, 0, 3, 0 };
            //List<int> dummy2 = new List<int>{ 2, 3, 0, 0 };
            //int strat1 = 2;
            //int strat2 = 2;
            //List<NE_Profile> NEDummy = new List<NE_Profile>();
            //int rowCounter = 0;
            //int colCounter = 0;
            //for (int j = 0; j < dummy.Count; j++)
            //{
            //    NEDummy.Add(new NE_Profile(dummy[j], rowCounter, colCounter, dummy2[j]));
            //    colCounter++;
            //    if (colCounter % strat2==0 && colCounter!= 0)
            //    {
            //        rowCounter++;
            //        colCounter = 0;
            //    }
            //}
            //            List<NE_Profile> resulT = CallNash2PNS(strat1, strat2, NEDummy);
            //for (int i = 0; i < resulT.Count;i++)
            //{
            //
            #endregion
            List<List<string>> strategiesp = new List<List<string>>();
            strategiesp.Add(new List<string> { "split1", "steal1" });
            strategiesp.Add(new List<string> { "split2", "steal2" });
            //page67,exc1.15
            int p1_numStrategy = 2;//rows
            int p2_numStrategy = 2;//cols
            int p3_numStrategy = 1;//matrix
            List<NE_Profile> p = new List<NE_Profile>();

            List<List<string>> CP = Cartisian_Product(strategiesp, p);

            List<string> players_preferences = new List<string>();
            //2p
            players_preferences.Add("o3 > o1 > o2 = o4");//p1
            players_preferences.Add("o2 > o1 > o3 = o4");//p2

            PayOffs_generater(players_preferences, p);

            List<NE_Profile> Max_Payoff = new List<NE_Profile>();//Max payyoffs

            for (int i = 0; i < p1_numStrategy; i++)//loop: rows/p2
            {
                for (int j = 0; j < p2_numStrategy * p3_numStrategy; j = j + p2_numStrategy)
                {
                    List<NE_Profile> tempRes = P2search_Max_Row(j, i, p2_numStrategy, p); // calls function and get MaxPayoff_row
                    Max_Payoff.AddRange(tempRes); // merging the results of the called function with Max_Payoff matrix
                }
            }
            for (int j = 0; j < p2_numStrategy * p3_numStrategy; j++)//loop col/p1
            {
                List<NE_Profile> tempRes = P1search_Max_Col(j, p1_numStrategy, p);// calls function and get MaxPayoff_col
                Max_Payoff.AddRange(tempRes);// merging the results of the called function with Max_Payoff matrix
            }

            int k = 0;// k is the index of the first element in the list and it's unchangable //NO TASTE
            bool flag_pair = false; // indicates if there is at least one Nash
            List<NE_Profile> PairedMax;//for p1_p12 results
            NE_Profile element_maxPayoff;//temp for the first element of maxpayoff
            List<NE_Profile> Paired_p1_p2 = new List<NE_Profile>();//to be used to find the max for p3
                                                                   //List<NE_Profile> Finale = new List<NE_Profile>();//NASH EQUILIBRIUM PRIFLES ARE STORED HERE
            while (Max_Payoff.Count > 0)//pairing the results
            {
                element_maxPayoff = new NE_Profile(Max_Payoff[k].payoff1, Max_Payoff[k].payoff2, /*Max_Payoff[k].payoff3,*/ Max_Payoff[k].row, Max_Payoff[k].col);//first node in list
                PairedMax = Max_Payoff.FindAll(n => (n.col == element_maxPayoff.col && n.row == element_maxPayoff.row)).ToList<NE_Profile>();//Finding all matching profiles
                Max_Payoff.RemoveAll(n => (n.col == element_maxPayoff.col && n.row == element_maxPayoff.row));//removed paired nodes
                if (PairedMax.Count > 1)
                {
                    Paired_p1_p2.Add(PairedMax[0]);//NOOO Duplication
                    flag_pair = true;//at least one Nash was found
                }
            }
            List<int> temp = new List<int>();
            if (!flag_pair)// no NE
                Console.WriteLine("There is no Nash Equilibrium for the game.");
            else //
            {
                //Finale = P3search_Max_Cell(GameProfiles, Paired_p1_p2, p2_numStrategy, p3_numStrategy);
                if (Paired_p1_p2.Count > 0)
                    for (int i = 0; i < Paired_p1_p2.Count; i++)
                    {
                        temp.Add(Paired_p1_p2[i].payoff1);
                        temp.Add(Paired_p1_p2[i].payoff2);
                    }
            }
            return temp;
        }
       
        #endregion
        #region 3Players
        public List<NE_Profile> P3search_Max_Cell(List<NE_Profile> p, List<NE_Profile> maxPayoff, int p2NumStrategies, int p3NumStrategies)
        {
            //List<NE_Profile> temp_maxPayoff = new List<NE_Profile>();//so we can delete without affecting the data
            //temp_maxPayoff.AddRange(maxPayoff);//copy by value
            List<NE_Profile> results = new List<NE_Profile>();//returned list
            List<NE_Profile> temp_results = new List<NE_Profile>();//loop temp list
            List<NE_Profile> temp = new List<NE_Profile>();//find all profiles in row
            while (maxPayoff.Count > 0)
            {
                NE_Profile tempMax = new NE_Profile(maxPayoff[0].payoff1, maxPayoff[0].payoff2, maxPayoff[0].payoff3, maxPayoff[0].row, maxPayoff[0].col);//first node in list
                temp = p.FindAll(n => (Math.Abs(n.col - tempMax.col) == p2NumStrategies && n.row == tempMax.row)).ToList<NE_Profile>();//Finding all same row profiles
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].payoff3 > tempMax.payoff3)
                    {
                        temp_results.Clear();
                        maxPayoff.RemoveAll(n => (n.col == tempMax.col && n.row == tempMax.row));//removed becuase it's not the max in the row
                        if (maxPayoff.Find(n => (n.payoff1 == temp[i].payoff1 && n.payoff2 == temp[i].payoff2 && n.payoff3 == temp[i].payoff3 && n.col == temp[i].col && n.row == temp[i].row)) != null)
                        {//if temp[i] is found in the Maxes list then add it to res and delete it
                            temp_results.Add(temp[i]);
                            tempMax = new NE_Profile(temp[i].payoff1, temp[i].payoff2, temp[i].payoff3, temp[i].row, temp[i].col);
                            maxPayoff.RemoveAll(n => (n.col == temp[i].col && n.row == temp[i].row)); ;
                        }
                        continue;
                    }
                    if (tempMax.payoff3 > temp[i].payoff3)
                    {
                        temp_results.Clear();
                        temp_results.Add(tempMax);
                        maxPayoff.RemoveAll(n => (n.col == tempMax.col && n.row == tempMax.row)); ;//removed
                        maxPayoff.RemoveAll(n => (n.col == temp[i].col && n.row == temp[i].row)); ;//removed if found
                        continue;
                    }
                    if (temp[i].payoff3 == tempMax.payoff3)//(REVIEW 2players)
                    {
                        List<NE_Profile> container = temp_results.FindAll(n => (n.payoff3 == tempMax.payoff3 && n.row == tempMax.row && n.col != tempMax.col)).ToList<NE_Profile>();
                        if (container.Count > 0)
                        {
                            temp_results.Add(tempMax);
                            if (maxPayoff.Find(n => (n.payoff1 == temp[i].payoff1 && n.payoff2 == temp[i].payoff2 && n.payoff3 == temp[i].payoff3 && n.col == temp[i].col && n.row == temp[i].row)) != null)
                            {//if it was in the Maxes list then added the temp along side the tempMax
                                temp_results.Add(temp[i]);
                                maxPayoff.RemoveAll(n => (n.col == temp[i].col && n.row == temp[i].row));//if found
                            }
                            maxPayoff.RemoveAll(n => (n.col == tempMax.col && n.row == tempMax.row)); ;
                        }
                        else
                        {
                            temp_results.Clear();
                            temp_results.Add(tempMax);
                            maxPayoff.RemoveAll(n => (n.col == tempMax.col && n.row == tempMax.row));
                            if (maxPayoff.Find(n => (n.payoff1 == temp[i].payoff1 && n.payoff2 == temp[i].payoff2 && n.payoff3 == temp[i].payoff3 && n.col == temp[i].col && n.row == temp[i].row)) != null)
                            {//if it was in the Maxes list then added the temp along side the tempMax
                                temp_results.Add(temp[i]);
                                maxPayoff.RemoveAll(n => (n.col == temp[i].col && n.row == temp[i].row));
                            }
                        }
                    }//if ==
                }//end for
                results.AddRange(temp_results);//addig sub-results of a row to the returned list
                temp_results.Clear();
            }//end while

            return results;
        }
        
        public List<int> Three_PlayerWrapper()
        {
            #region Input Exercises
            //------------------------
            //page67,exc1.15
            int p1_numStrategy = 2;//rows
            int p2_numStrategy = 2;//cols
            int p3_numStrategy = 2;//matrix
            List<NE_Profile> GameProfiles = new List<NE_Profile>();
            List<List<string>> playersstrategies = new List<List<string>>();
            playersstrategies.Add(new List<string> { "p1", "np1" });
            playersstrategies.Add(new List<string> { "p2", "np2" });
            playersstrategies.Add(new List<string> { "p3", "np3" });
            List<List<string>> CP = Cartisian_Product(playersstrategies, GameProfiles);
            List<string> players_preferences = new List<string>();
            //players_preferences.Add("o1 = o4 = o6 = o7 > o2 = o3 = o5 = o8");//p1
            //players_preferences.Add("o2 = o3 = o5 > o1 = o4 = o6 = o7 = o8");//p2
            //players_preferences.Add("o8 > o1 = o2 = o3 = o4 = o5 = o6 = o7");//p3
            players_preferences.Add("o1 > o4 = o6 = o7 > o2 = o3 = o5 = o8");//p1
            players_preferences.Add("o2 = o3 > o5 > o1 = o4 = o6 = o7 = o8");//p2
            players_preferences.Add("o8 > o1 = o2 = o3 = o4 > o5 = o6 = o7");//p3
            PayOffs_generater(players_preferences, GameProfiles);

            //NE_Profile[,] p = new NE_Profile[p1_numStrategy, p2_numStrategy * p3_numStrategy];
            //p[0, 0] = new NE_Profile(1, 0, 0, 0, 0);
            //p[0, 1] = new NE_Profile(0, 1, 0, 0, 1);
            //p[0, 2] = new NE_Profile(0, 1, 0, 0, 2);
            //p[0, 3] = new NE_Profile(1, 0, 0, 0, 3);
            //p[1, 0] = new NE_Profile(0, 1, 0, 1, 0);
            //p[1, 1] = new NE_Profile(1, 0, 0, 1, 1);
            //p[1, 2] = new NE_Profile(1, 0, 0, 1, 2);
            //p[1, 3] = new NE_Profile(0, 0, 1, 1, 3);
            //page104,exc2.6
            //int p1_numStrategy = 2;//rows
            //int p2_numStrategy = 4;//cols
            //int p3_numStrategy = 2;//matrix
            //NE_Profile[,] p = new NE_Profile[p1_numStrategy, p2_numStrategy * p3_numStrategy];
            //p[0, 0] = new NE_Profile(2, 1, 0, 0, 0);
            //p[0, 1] = new NE_Profile(2, 1, 0, 0, 1);
            //p[0, 2] = new NE_Profile(0, 0, 2, 0, 2);
            //p[0, 3] = new NE_Profile(0, 0, 2, 0, 3);
            //p[0, 4] = new NE_Profile(2, 1, 0, 0, 5);
            //p[0, 5] = new NE_Profile(2, 1, 0, 0, 6);
            //p[0, 6] = new NE_Profile(0, 0, 2, 0, 7);
            //p[0, 7] = new NE_Profile(0, 0, 2, 0, 8);
            //p[1, 0] = new NE_Profile(3, 1, 0, 1, 0);
            //p[1, 1] = new NE_Profile(1, 2, 1, 1, 1);
            //p[1, 2] = new NE_Profile(3, 1, 0, 1, 2);
            //p[1, 3] = new NE_Profile(1, 2, 1, 1, 3);
            //p[1, 4] = new NE_Profile(3, 1, 0, 1, 4);
            //p[1, 5] = new NE_Profile(0, 0, 1, 1, 5);
            //p[1, 6] = new NE_Profile(3, 1, 0, 1, 6);
            //p[1, 7] = new NE_Profile(0, 0, 1, 1, 7);
            //------------------------Printing-----------
            #endregion
            List<NE_Profile> Max_Payoff = new List<NE_Profile>();//Max payyoffs

            for (int i = 0; i < p1_numStrategy; i++)//loop: rows/p2
            {
                for (int j = 0; j < p2_numStrategy * p3_numStrategy; j = j + p2_numStrategy)
                {
                    List<NE_Profile> tempRes = P2search_Max_Row(j, i, p2_numStrategy, GameProfiles); // calls function and get MaxPayoff_row
                    Max_Payoff.AddRange(tempRes); // merging the results of the called function with Max_Payoff matrix
                }
            }
            for (int j = 0; j < p2_numStrategy * p3_numStrategy; j++)//loop col/p1
            {
                List<NE_Profile> tempRes = P1search_Max_Col(j, p1_numStrategy, GameProfiles);// calls function and get MaxPayoff_col
                Max_Payoff.AddRange(tempRes);// merging the results of the called function with Max_Payoff matrix
            }
            int k = 0;// k is the index of the first element in the list and it's unchangable //NO TASTE
            bool flag_pair = false; // indicates if there is at least one Nash
            List<NE_Profile> PairedMax;//for p1_p12 results
            NE_Profile element_maxPayoff;//temp for the first element of maxpayoff
            List<NE_Profile> Paired_p1_p2 = new List<NE_Profile>();//to be used to find the max for p3
            List<NE_Profile> Finale = new List<NE_Profile>();//NASH EQUILIBRIUM PRIFLES ARE STORED HERE
             while (Max_Payoff.Count > 0)//pairing the results
            {
                element_maxPayoff = new NE_Profile(Max_Payoff[k].payoff1, Max_Payoff[k].payoff2, Max_Payoff[k].payoff3, Max_Payoff[k].row, Max_Payoff[k].col);//first node in list
                PairedMax = Max_Payoff.FindAll(n => (n.col == element_maxPayoff.col && n.row == element_maxPayoff.row)).ToList<NE_Profile>();//Finding all matching profiles
                Max_Payoff.RemoveAll(n => (n.col == element_maxPayoff.col && n.row == element_maxPayoff.row));//removed paired nodes
                if (PairedMax.Count > 1)
                {
                    Paired_p1_p2.Add(PairedMax[0]);//NOOO Duplication
                    flag_pair = true;//at least one Nash was found
                }
            }
            List<int> Temp = new List<int>();
            if (!flag_pair)
            {
                List<int> ErrorList = new List<int>();
                ErrorList.Add(0);
                return ErrorList;
            } 
            else
            {

                Finale = P3search_Max_Cell(GameProfiles, Paired_p1_p2, p2_numStrategy, p3_numStrategy);
                if (Finale.Count > 0)
                    for (int i = 0; i < Finale.Count; i++)
                    {
                        Temp.Add(Finale[i].payoff1);
                        Temp.Add(Finale[i].payoff2);
                        Temp.Add(Finale[i].payoff3);
                    }
            }
            return Temp;
        }
        #endregion                                                              
        #region Common
        public void Swap(List<List<string>> list1, List<List<string>> list2)
        {
            //Swap for lists: swaps the elements of a list with with the elements of the other, and vise versa.
            List<List<string>> temp = new List<List<string>>(list1);
            list1.Clear();
            list1.AddRange(list2);
            list2.Clear();
            list2.AddRange(temp);
        }

        public void Utility_Function(List<List<int>> pref, List<NE_Profile> P, int player)
        {
            //input list of lists of prefs of a player sorted from most desired to least des.
            int utilitynum = pref.Count; //Use the number of sublists to determine the utlility number
            foreach (var x in pref)
            {//x is a sublist of prefs that are equally prefered. 
                if (x.Count > 1)
                {//two or more prefs have the same utility
                    foreach (var d in x)
                    {///outcomes presented to the player are ranked from 1 to number/ While in GameFrame are indexed from 0 to number-1
                        switch (player)
                        {
                            case 1: P[d - 1].payoff1 = utilitynum * 2; break;// Add the utility number to the index of the outcome
                            case 2: P[d - 1].payoff2 = utilitynum * 2; break;// Add the utility number to the index of the outcome
                            case 3: P[d - 1].payoff3 = utilitynum * 2; break;// Add the utility number to the index of the outcome
                        }
                    }
                    utilitynum--; //Reduce the utility lvl
                }
                else//size of x=1
                {
                    switch (player)
                    {
                        case 1: P[x[0] - 1].payoff1 = utilitynum * 2; break;//Add the outcome to it's index /e.g: If o is 1 then it is indexed at 0
                        case 2: P[x[0] - 1].payoff2 = utilitynum * 2; break;// Add the utility number to the index of the outcome
                        case 3: P[x[0] - 1].payoff3 = utilitynum * 2; break;// Add the utility number to the index of the outcome
                    }
                    utilitynum--;
                }
            }
        }

        public List<List<string>> Cartisian_Product(List<List<string>> Players_Strategies, List<NE_Profile> p)
        {
            // Players_Strategies is a list of lists. Each element is u-Player's list of strategies.
            List<List<string>> Results = new List<List<string>>();
            Results.Add(new List<string>() { }); //Added a new 'empty' list to S to add a strategy from each player in it.
            foreach (var u in Players_Strategies) //u: u-player set of lists , Players_Strategies the list of lists of all players' strategies.
            {
                List<List<string>> r = new List<List<string>>();//Temporary holder
                foreach (List<string> x in Results)
                {//x is the empty/(partially filled each loop with a strategy from u-player's s) list in x.
                    foreach (string y in u)
                    {//Foreach strategy 'y' in u-player's list of strategies.
                        r.Add(new List<string>(x)); //Create a new list in r and add a copy x's elements.
                        r[r.Count - 1].Add(y); //Add strategy 'y' into the last new list created above.
                    }
                }
                Swap(r, Results);//Add the content of r to S and vise versa.
            }
            //Init p the payoff matrix
            int rowcount = 0, colcount = 0;
            for (int i = 0; i < Results.Count; i++)
            {
                p.Add(new NE_Profile(rowcount, colcount)); //Initialization O's lists after knowing the number of products produced.
                colcount++;
                if (Players_Strategies.Count == 3)
                {
                    if (colcount % (Players_Strategies[1].Count * Players_Strategies[2].Count) == 0 && colcount != 0)
                    {
                        rowcount++;
                        colcount = 0;
                    }
                }
                else
                {
                    if (colcount % Players_Strategies[1].Count == 0 && colcount != 0)
                    {
                        rowcount++;
                        colcount = 0;
                    }
                }
            }

            return Results;
        }

        public void PayOffs_generater(List<string> players_preferences, List<NE_Profile> P)
        {
            int players_count = 1;
            //players_preferences is a list of list of a player preferences.
            foreach (var x in players_preferences)// foreach player's preferences
            {
                List<List<int>> utilityinput = new List<List<int>>();//Splited preferences parsed into numbers.
                List<string> SplitedString = x.Split(' ').ToList<string>();//List spliter 
                utilityinput.Add(new List<int>());
                for (int j = 0; j < SplitedString.Count; j++)//for each substring of x in players_preferences
                {
                    if (SplitedString[j] != ">" && SplitedString[j] != "=")// if the substring contains an outcome
                    {//Add the element at index 1 -> because -> e.g "o1" so the outcome number index is 1
                     // utilityinput.Add(new List<int>() { int.Parse(SplitedString[j].ElementAt(1)+"")});

                        if (j + 1 < SplitedString.Count)//if the next substring is > then skip it and continue to j+2
                        {
                            if (SplitedString[j + 1] == ">")
                            {
                                utilityinput[utilityinput.Count - 1].Add(int.Parse(SplitedString[j].Substring(1) + ""));
                                utilityinput.Add(new List<int>());
                                j++;
                                continue;
                            }
                            if (SplitedString[j + 1] == "=")
                            {
                                utilityinput[utilityinput.Count - 1].Add(int.Parse(SplitedString[j].Substring(1) + ""));
                                j++;
                                continue;
                            }
                        }
                        else
                        {
                            utilityinput[utilityinput.Count - 1].Add(int.Parse(SplitedString[j].Substring(1) + ""));
                        }
                    }//endif not (> or =)
                }//end for
                Utility_Function(utilityinput, P, players_count);//Call utility function for the current player's preferences
                players_count++;
            }//end foreach
        }

        public List<NE_Profile> P1search_Max_Col(int colIndex, int rows, List<NE_Profile> p)
        {

            List<NE_Profile> results = new List<NE_Profile>();//returned list
                                                              //tempMax: used for comparison with the next cell
            NE_Profile TempNash = new NE_Profile(p.Find(n => (n.row == 0 && n.col == colIndex)).payoff1, p.Find(n => (n.row == 0 && n.col == colIndex)).payoff2, p.Find(n => (n.row == 0 && n.col == colIndex)).payoff3, 0, colIndex);

            for (int i = 1; i < rows; i++)//loop on rows of the selected col
            {
                if (TempNash.payoff1 > p.Find(n => (n.row == i && n.col == colIndex)).payoff1)
                {
                    List<NE_Profile> container = results.FindAll(n => (n.payoff1 == TempNash.payoff1)).ToList<NE_Profile>();//if TempNash or/and an equal payoff was added before
                    if (container.Count == 0)//if not then TempNash is already in the list and no need to add it a sec time
                    {
                        results.Clear();
                        results.Add(TempNash);
                    }
                    continue;
                }
                else if (TempNash.payoff1 < p.Find(n => (n.row == i && n.col == colIndex)).payoff1)
                {
                    results.Clear();
                    TempNash.payoff1 = p.Find(n => (n.row == i && n.col == colIndex)).payoff1;
                    TempNash.payoff2 = p.Find(n => (n.row == i && n.col == colIndex)).payoff2;
                    TempNash.payoff3 = p.Find(n => (n.row == i && n.col == colIndex)).payoff3;
                    TempNash.row = i;// same col but different row
                    results.Add(TempNash);
                    continue;
                }
                else if (TempNash.payoff1 == p.Find(n => (n.row == i && n.col == colIndex)).payoff1)//Add both profiles
                {
                    List<NE_Profile> container = results.FindAll(n => (n.payoff1 == TempNash.payoff1)).ToList<NE_Profile>();//Detects prevsly added payoffs, whether of the same val of not
                    if (container.Count > 0)//TempNash is already in the list
                    {
                        results.Add(new NE_Profile(p.Find(n => (n.row == i && n.col == colIndex)).payoff1, p.Find(n => (n.row == i && n.col == colIndex)).payoff2, p.Find(n => (n.row == i && n.col == colIndex)).payoff3, i, colIndex));
                    }
                    else
                    {//no values were found or only TempNash was found
                        results.Clear();
                        results.Add(TempNash);
                        results.Add(new NE_Profile(p.Find(n => (n.row == i && n.col == colIndex)).payoff1, p.Find(n => (n.row == i && n.col == colIndex)).payoff2, p.Find(n => (n.row == i && n.col == colIndex)).payoff3, i, colIndex));
                    }
                    continue;
                }
            }
            return results;
        }

        public List<NE_Profile> P2search_Max_Row(int StartCol, int rowIndex, int Numcols, List<NE_Profile> p)
        {
            List<NE_Profile> results = new List<NE_Profile>();//returned list
                                                              //temp for comparison
            NE_Profile TempNash = new NE_Profile(p.Find(n => (n.row == rowIndex && n.col == StartCol)).payoff1, p.Find(n => (n.row == rowIndex && n.col == StartCol)).payoff2, p.Find(n => (n.row == rowIndex && n.col == StartCol)).payoff3, rowIndex, StartCol);
            for (int i = StartCol + 1; i < Numcols + StartCol; i++)
            {
                if (TempNash.payoff2 > p.Find(n => (n.row == rowIndex && n.col == i)).payoff2)//Add TempNash
                {
                    List<NE_Profile> container = results.FindAll(n => (n.payoff2 == TempNash.payoff2)).ToList<NE_Profile>();
                    if (container.Count == 0)//if TempNash wasn't added before
                    {
                        results.Clear();
                        results.Add(TempNash);
                    }
                    continue;
                }
                else
                    if (TempNash.payoff2 < p.Find(n => (n.row == rowIndex && n.col == i)).payoff2)//Add p[row,i]
                {
                    results.Clear();
                    TempNash.payoff2 = p.Find(n => (n.row == rowIndex && n.col == i)).payoff2;
                    TempNash.payoff1 = p.Find(n => (n.row == rowIndex && n.col == i)).payoff1;
                    TempNash.payoff3 = p.Find(n => (n.row == rowIndex && n.col == i)).payoff3;
                    TempNash.col = i;
                    results.Add(TempNash);
                    continue;
                }
                else if (p.Find(n => (n.row == rowIndex && n.col == i)).payoff2 == TempNash.payoff2)//Add both
                {
                    List<NE_Profile> container = results.FindAll(n => (n.payoff2 == TempNash.payoff2)).ToList<NE_Profile>();
                    if (container.Count > 0)
                    {//TempNash already exists
                        results.Add(new NE_Profile(p.Find(n => (n.row == rowIndex && n.col == i)).payoff1, p.Find(n => (n.row == rowIndex && n.col == i)).payoff2, p.Find(n => (n.row == rowIndex && n.col == i)).payoff3, rowIndex, i));
                    }
                    else
                    {//no values were found or only TempNash was found
                        results.Clear();
                        results.Add(TempNash);
                        results.Add(new NE_Profile(p.Find(n => (n.row == rowIndex && n.col == i)).payoff1, p.Find(n => (n.row == rowIndex && n.col == i)).payoff2, p.Find(n => (n.row == rowIndex && n.col == i)).payoff3, rowIndex, i));
                    }
                }
            }
            return results;
        }

        #endregion
        #region NPlayer

        //probably wont need it now
        public void Print_list_of_lists<T>(List<List<T>> X)
        {
            throw new NotImplementedException();
        }

        public List<String> NPlayerWrapper()
        {
            GameFrame GF = new GameFrame();//initialize a GameFrame
            //int PI =2;//number of players
            int PI = 4;
            //strategies of each player
            List<List<string>> playersstrategies = new List<List<string>>();
            //p2
            // playersstrategies.Add(new List<string> { "split1", "steal1" });
            //playersstrategies.Add(new List<string> { "split2", "steal2" });
            //p3




            //4p
            playersstrategies.Add(new List<string> { "p1", "np1" });
            playersstrategies.Add(new List<string> { "p2", "np2" });
            playersstrategies.Add(new List<string> { "p3", "np3" });
            playersstrategies.Add(new List<string> { "p4", "np4" });


            GF.Game_Initializr(PI, playersstrategies);//Initialize Game

            //preferences of each player
            List<string> players_preferences = new List<string>();

            players_preferences.Add("o1 = o4 = o6 = o7 = o9 = o12 = o14 = o16 > o2 = o3 = o5 = o8 = o10 = o11 = o13 = o15");//p1
            players_preferences.Add("o2 = o3 = o5 = o9 = o11 = o16 > o1 = o4 = o6 = o7 = o8 = o10 = o12 = o13 = o14 = o15");//p2
            players_preferences.Add("o8 > o1 = o2 = o3 = o4 = o5 = o6 = o7 = o9 = o10 = o11 = o12 = o13 = o14 = o15 = o16");//p3
            players_preferences.Add("o8 = o1 > o2 > o3 > o4 = o5 = o6 > o7 > o9 > o10 = o11 > o12 > o13 = o14 > o15 > o16");//p4

            //2p
            //players_preferences.Add("o3 > o1 > o2 = o4");//p1
            //players_preferences.Add("o2 > o1 > o3 = o4");//p2
            GF.PayOffs_generater(players_preferences);// Enter preferences to generate Payoffs // only needed in a console application

            GF.Fill_Nash_Equilibrium_Set();//One call for both Best_outcomes and Fill_NE_set

            List<String> Result = new List<String>();
            for (int i = 0; i < GF.get_Nash_Indices.Count; i++)
            {
                for (int j = 0; j < GF.get_NumOfPlayer; j++)
                {
                    Result.Add(GF.get_SetOfCStategy[GF.get_Nash_Indices[i]][j]);//returns a list of string of the strategies of the players at the nash points
                }
            }
            return Result;
        }








        #endregion

        #region Admin Functions
        #endregion

        #region User Functions
        
        public UserModel SignIn(string username, string password)
        {
            var prox = new DBPr.DBFunctionsClient();
            UserModel user = prox.RetreiveUser(username, password);
            return user;
        }

        public string Register(UserModel us)
        {
            var prox = new DBPr.DBFunctionsClient();
            prox.AddUser(us);
            return "done!";
        }
        #endregion
    }
}
