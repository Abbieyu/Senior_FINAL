using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SeniorDBServer;
namespace SeniorServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface INsashServices
    {

        #region NPlayer
        [OperationContract]
        List<String> NPlayerWrapper();
        #endregion

        #region Common Region Between 2P and 3P
        void Swap(List<List<string>> list1, List<List<string>> list2);
        List<List<string>> Cartisian_Product(List<List<string>> Players_Strategies, List<NE_Profile> p);
        void PayOffs_generater(List<string> players_preferences, List<NE_Profile> P);

        void Utility_Function(List<List<int>> pref, List<NE_Profile> P, int player);
        List<NE_Profile> P1search_Max_Col(int colIndex, int rows, List<NE_Profile> p);

        List<NE_Profile> P2search_Max_Row(int StartCol, int rowIndex, int Numcols, List<NE_Profile> p);
        #endregion
        #region 2Players
        [OperationContract]
        List<int> Two_PlayerWrapper();//List<int> dummy, List<int> dummy2, int strat1, int strat2);
        #endregion
        #region 3Players
        List<NE_Profile> P3search_Max_Cell(List<NE_Profile> p, List<NE_Profile> maxPayoff, int p2NumStrategies, int p3NumStrategies);
        [OperationContract]
        List<int> Three_PlayerWrapper();
        #endregion

        #region Admin Functions

        #endregion

        #region User Functions
        // SignIn();
        [OperationContract]
        UserModel SignIn(UserModel s);
        [OperationContract]
        string Register(UserModel usermodel);
        #endregion
    }



    public class GF
    {
        public NE_Profile[,] GameRep;
        public int Player1StrategyNum;
        public int Player2StrategyNum;
        public GF(int p1strats, int p2strats)
        {
            GameRep = new NE_Profile[p1strats, p2strats];
            Player1StrategyNum = p1strats;
            Player2StrategyNum = p2strats;
        }
        public void FillGameRep(List<NE_Profile> GFRep)//the list contains the payoffs listed as p[0,0],p[0,1],...
        {
            int K = GFRep.Count() - 1;
            int ksmall = 0;
            for (int i = 0; i < Player1StrategyNum; i++)
                for (int j = 0; j < Player2StrategyNum; j++)
                {
                    GameRep[i, j] = GFRep[ksmall++];
                }
        }
    }

    [DataContract]
    public class NE_Profile
    {
        public int payoff1;
        public int col;
        public int row;
        public int payoff2;
        public int payoff3;
        public NE_Profile(int off1, int off2, int r, int c)//constructor
        {
            this.payoff1 = off1;
            this.col = c;
            this.row = r;
            this.payoff2 = off2;
        }
        public NE_Profile()//constructor
        { }

        public NE_Profile(int r, int c)//constructor
        {
            this.col = c;
            this.row = r;
        }
        public NE_Profile(int off1, int off2, int off3, int r, int c)//constructor
        {
            this.payoff1 = off1;
            this.payoff2 = off2;
            this.payoff3 = off3;
            this.col = c;
            this.row = r;
        }
    }

    #region NPlayer
    [DataContract]
    public class GameFrame
    {
        #region Attributes
        int I;//Number of players
        List<List<string>> Players_Strategies;// Each player has a sublist of her strategy.
        //Set of cartesian product 
        List<List<string>> S;
        List<List<int>> O; //List of profiles of players' Payoffs
        List<int> Nash_Indices;//Hold the indices of outcomes in O with 'n' true best-payoff in Outcome_Checker.
        List<List<bool>> Outcome_Checker;//Changes an element to true if was a best Outcome for a certain profile.
                                         //Constructor
        #endregion
        public GameFrame()
        {
            Players_Strategies = new List<List<string>>();
            S = new List<List<string>>();
            O = new List<List<int>>();
            Nash_Indices = new List<int>();
            Outcome_Checker = new List<List<bool>>();//initally false
        }

        #region setters/Getters
        public int get_NumOfPlayer //I
        {
            get
            {
                return I;
            }
        }
        public List<List<string>> get_SetOfPlayersStrategies //Players_Strategies
        {
            get
            {
                return Players_Strategies;
            }
        }
        public List<List<string>> get_SetOfCStategy //S
        {
            get
            {
                return S;
            }
        }
        public List<List<int>> get_SetOfOutcomes //O
        {
            get
            {
                return O;
            }
        }
        public List<List<bool>> get_Outcome_Checker //Outcomechecker
        {
            get { return Outcome_Checker; }
        }
        public List<int> get_Nash_Indices //Nash_Indices
        {
            get
            {
                return
                Nash_Indices;
            }
        }
        #endregion
        public void Game_Initializr(int players, List<List<string>> PS_Strategies)//Sets Players,Strategies, and C.P.
        {
            this.I = players;
            get_SetOfPlayersStrategies.AddRange(PS_Strategies);
            Cartisian_Product();
        }
        public void Swap(List<List<string>> list1, List<List<string>> list2)
        {//Swap for lists: swaps the elements of a list with with the elements of the other, and vise versa.
            List<List<string>> temp = new List<List<string>>(list1);
            list1.Clear();
            list1.AddRange(list2);
            list2.Clear();
            list2.AddRange(temp);
        }
        //Calculate cartisian product of player strategies.
        public void Cartisian_Product()
        {// Players_Strategies is a list of lists. Each element is u-Player's list of strategies.
            S.Add(new List<string>() { }); //Added a new 'empty' list to S to add a strategy from each player in it.
            foreach (var u in Players_Strategies) //u: u-player set of lists , V the list of lists of all players' strategies.
            {
                List<List<string>> r = new List<List<string>>();//Temporary holder
                foreach (List<string> x in S)
                {//x is the empty/(partially filled each loop with a strategy from u-player's s) list in x.
                    foreach (string y in u)
                    {//Foreach strategy 'y' in u-player's list of strategies.
                        r.Add(new List<string>(x)); //Create a new list in r and add a copy x's elements.
                        r[r.Count - 1].Add(y); //Add strategy 'y' into the last new list created above.
                    }
                }
                Swap(r, S);//Add the content of r to S and vise versa.
            }
            for (int i = 0; i < S.Count; i++)
            {
                O.Add(new List<int>() { }); //Initialization O's lists after knowing the number of products produced.
            }

        }

        //Give all possible outcomes. 
        public void PayOffs_generater(List<string> pp)// Input players preferences.
        {
            //pp is a list of list of a player preferences.
            foreach (var x in pp)// foreach player's preferences
            {
                List<List<int>> utilityinput = new List<List<int>>();//Splited preferences parsed into numbers.
                List<string> SplitedString = x.Split(' ').ToList<string>();//List spliter 
                utilityinput.Add(new List<int>());
                for (int j = 0; j < SplitedString.Count; j++)//for each substring of x in pp
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
                Utility_Function(utilityinput);//Call utility function for the current player's preferences
            }//end foreach
            //Fill Outcomechecker
            foreach (var L in O)//Outcomechecker count = O.count.
            {
                Outcome_Checker.Add(new List<bool>());
                foreach (var l in L)//Each list in OutcomeChecker contains false elements == L.count
                {
                    Outcome_Checker[Outcome_Checker.Count - 1].Add(false);
                }
            }
        }

        //apply preferences on outcomes and returns payoffs. 
        public void Utility_Function(List<List<int>> pref)
        {//input list of lists of prefs of a player sorted from most desired to least des.
            int utilitynum = pref.Count; //Use the number of sublists to determine the utlility number
            foreach (var x in pref)
            {//x is a sublist of prefs that are equally prefered. 
                if (x.Count > 1)
                {//two or more prefs have the same utility
                    foreach (var d in x)
                    {///outcomes presented to the player are ranked from 1 to number/ While in GameFrame are indexed from 0 to number-1
                        O[d - 1].Add(utilitynum * 2);// Add the utility number to the index of the outcome
                    }
                    utilitynum--; //Reduce the utility lvl
                }
                else//size of x=1
                {
                    O[x[0] - 1].Add(utilitynum * 2); //Add the outcome to it's index /e.g: If o is 1 then it is indexed at 0
                    utilitynum--;
                }
            }
        }

        //public void Best_Payoff_Finder()//Ente gut, alles gut.
        //{
        //    for (int i = 0; i < get_SetOfPlayersStrategies.Count; i++)// loop on players lists of strategies.
        //    {
        //        foreach (string s in get_SetOfPlayersStrategies[i])//loop strategies of player i.
        //        {
        //            //Finds all lists in S that contains string 's' at index i.
        //            List<List<string>> curr_Strategy = S.FindAll(li => (li[i] == s));
        //            List<int> curr_Stra_indices = new List<int>();//indices of curr_Strategy in S.
        //            List<List<int>> curr_Outcomes = new List<List<int>>();// Outcomes at curr_Stra_indices.
        //            foreach (var l in curr_Strategy)//find indices of each permu in curr_strategy.
        //            {
        //                curr_Stra_indices.Add(S.FindIndex(a => (a == l)));
        //            }
        //            foreach (var index in curr_Stra_indices)//collecting Outcomes of the same indices.
        //            {
        //                curr_Outcomes.Add(O.ElementAt(index));
        //            }
        //            //Find the best payoff for a player other than i when player'i' play strategy 's'.
        //            for (int j = 0; j < I; j++)
        //            {
        //                if (j == i)//Do not search the current player's max (p=i)
        //                    continue;
        //                List<int> Index_max = new List<int>();//Holds the indices of the best outcomes in curr
        //                int max = int.MinValue;
        //                foreach (var profile in curr_Outcomes)//outcomes for a strategy of player i
        //                {
        //                    if (profile[j] > max)
        //                    {
        //                        Index_max.Clear();
        //                        max = profile[j];
        //                        Index_max.Add(curr_Outcomes.IndexOf(profile));//Compare with index in curr_Stra_indices
        //                    }
        //                    else
        //                    if (profile[j] == max)
        //                    {
        //                        Index_max.Add(curr_Outcomes.IndexOf(profile));
        //                    }
        //                }//each player j best outcome for s of player i
        //                foreach (var m in Index_max)
        //                {//outcome_checker at index([][]) is a max so we changed the value to true
        //                    Outcome_Checker[curr_Stra_indices[m]][j] = true;
        //                }
        //            }//end for j
        //        }//end for s
        //    }//end for i
        //}
        #region Best_Payoff_Finder
        public void Best_Payoff_Finder()//Ente gut, alles gut.
        {
            for (int i = 0; i < get_SetOfPlayersStrategies.Count; i++)// loop on players lists of strategies.
            {
                List<int> C_P_Players_Indicies = new List<int>();//Store all players indicies except player i
                //
                List<List<string>> F_C_P = Filtered_Cartesian_Product(this.Players_Strategies, C_P_Players_Indicies, i);
                foreach (var sub_fcp in F_C_P)//for each list in F_C_P
                {
                    List<int> matched_index_holder = BPF_Index_Finder(sub_fcp, C_P_Players_Indicies);//call function

                    Outcome_Checker_editor(matched_index_holder, i);
                }
            }//end for i
        }
        public void Outcome_Checker_editor(List<int> matched_index_holder, int player)
        {
            List<int> Index_max = new List<int>();//Holds the indices of the best outcomes in curr
            int max = int.MinValue;
            for (int j = 0; j < matched_index_holder.Count; j++)
            {
                if (O[matched_index_holder[j]].ElementAt(player) > max)
                {
                    Index_max.Clear();
                    max = O[matched_index_holder[j]].ElementAt(player);
                    Index_max.Add(matched_index_holder[j]);
                }
                else
                if (O[matched_index_holder[j]].ElementAt(player) == max)
                {
                    Index_max.Add(matched_index_holder[j]);
                }
            }//each player j best outcome for s of player i
            foreach (var m in Index_max)
            {
                Outcome_Checker[m][player] = true;
            }
        }
        public List<int> BPF_Index_Finder(List<string> fcp, List<int> C_P_Players_Indicies)//find all lists in S, that contains fcp sublist
        {
            //find all lists in S where the string at C_P_Players_Indicies[0] equals the string at 0 in fcp
            List<List<string>> curr_CP_lists = S.FindAll(n => (n.ElementAt(C_P_Players_Indicies[0]) == fcp.ElementAt(0)));
            for (int i = 1; i < fcp.Count; i++)//search in S for each strategy in fcp/ Matching the index of it 
            {//find all lists in S where the string at C_P_Players_Indicies[i] equals the string at i in fcp
                curr_CP_lists = curr_CP_lists.FindAll(n => (n.ElementAt(C_P_Players_Indicies[i]) == fcp.ElementAt(i)));

            }
            //return list matching indicies 
            List<int> results = new List<int>();

            for (int i = 0; i < curr_CP_lists.Count; i++)
            {
                //each iteration adds the founded index in S to res
                results.Add(S.IndexOf(S.Find(n => (n == curr_CP_lists[i]))));
            }
            return results;
        }
        List<List<string>> Filtered_Cartesian_Product(List<List<string>> Players_Strategies, List<int> C_P_Players_Indicies, int player)//player:execluded player
        {// Players_Strategies is a list of lists. Each element is u-Player's list of strategies.
            List<List<string>> Results = new List<List<string>>
            {
                new List<string>() { } //Added a new 'empty' list to S to add a strategy from each player in it.
            };
            foreach (var u in Players_Strategies) //u: u-player set of lists , Players_Strategies the list of lists of all players' strategies.
            {
                if (Players_Strategies.IndexOf(u) == player)//skip the player sent from finder function
                    continue;
                C_P_Players_Indicies.Add(Players_Strategies.IndexOf(u));//Saved the indicies of each added player.
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
            return Results;
        }//end of I.C.p.
        #endregion



        public void Fill_Nash_Equilibrium_Set()//Fills Nash_Indices based on Outcomes_Checker's values.
        {
            Best_Payoff_Finder();//Find best Payoffs then precede to find the Nash.
            foreach (var Olist in Outcome_Checker)
            {
                if (Olist.FindAll(n => (n.Equals(true))).Count == I)//if numb of trues in a profile equals the number of players.
                {//NASH FOUND
                    Nash_Indices.Add(Outcome_Checker.IndexOf(Olist));//Add the index of the profile to N_I list of lists.
                }
            }
        }
    }
    #endregion
}
