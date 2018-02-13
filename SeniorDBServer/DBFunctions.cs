using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
namespace SeniorDBServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class DBFunctions : IDBFunctions
    {
        //checked
        #region Game
        public int AddGame(GameModel gamesample)// returns the GID if the insertion was successful , -1 otherwise
        {
            var cont = new SeniorLinqDataContext();
            var gfcheck = (from gf in cont.GameFrames where gf.Title == gamesample.Title && gamesample.NPlayers >= gf.MinPlayers && gamesample.NPlayers <=  gf.MaxPlayers  select gf).SingleOrDefault();
            if (gfcheck != null)
            {
                Game temp = new Game();
                temp.NPlayers = gamesample.NPlayers;
                temp.Title = gamesample.Title;
                cont.Games.InsertOnSubmit(temp);
                cont.SubmitChanges();
                List<Game> games = cont.Games.ToList();
                return games[games.Count - 1].GID;
            }
            return -1;
        } // checked

        public int DeleteGame(int gameid)//returns -1 if the game was not found // -2 if it was linked to other objects//-1 success//checked
        {
            var cont = new SeniorLinqDataContext();
            var query = (from game in cont.Games where game.GID == gameid select game).SingleOrDefault();
            if (query != null)
            {
                var gamplayerchecker = (from gameplayer in cont.Gameplayers where gameplayer.GID == query.GID select gameplayer).SingleOrDefault();
                var nashpointchecker = (from np in cont.NashPointProfiles where np.GID == query.GID select np).SingleOrDefault();
                if (gamplayerchecker == null && nashpointchecker == null)
                {
                    cont.Games.DeleteOnSubmit(query);
                    cont.SubmitChanges();
                    var q = (from game in cont.Games where game.GID == gameid select game).SingleOrDefault();
                    if (q == null)
                        return 1;
                }
                else return -2;
            }
            return -1;
        }
        public GameModel RetreiveGame(int gid)// returns the game if found // null otherwise//checked
        {
            var cont = new SeniorLinqDataContext();
            var query = (from game in cont.Games where game.GID == gid select game).SingleOrDefault();
            if (query != null)
            {
                GameModel gm = new GameModel();
                gm.GID = query.GID;
                gm.Title = query.Title;
                gm.NPlayers = query.NPlayers;
                return gm;
            }
            return null;
        }
        public int UpdateGameNumofPlayers(int gid, int nop)
        {
            var cont = new SeniorLinqDataContext();
            var query = (from game in cont.Games where game.GID == gid select game).SingleOrDefault();
           
            if (query != null)
            {
                var res = (from game in cont.Games where game.GID == gid select game).SingleOrDefault();
                var gamesample = (from gf in cont.GameFrames where res.Title == gf.Title select gf).SingleOrDefault();
                var gfcheck = (from gf in cont.GameFrames where res.Title==gf.Title select gf).SingleOrDefault();//res.NPlayers >= gf.MinPlayers && res.NPlayers <= gf.MaxPlayers select gf).SingleOrDefault();
                if (gfcheck != null && nop >= gfcheck.MinPlayers && nop <= gfcheck.MaxPlayers)
                {
                    query.NPlayers = nop;
                    cont.SubmitChanges();

                    if (res.Title == query.Title)
                        return 1;
                }
            }
            return -1;
        }//checked
        public List<GameModel> RetreiveAllGames()// returns null if no games were found
        {
            var cont = new SeniorLinqDataContext();
            var query = (from games in cont.Games select games).ToList();
            if (query == null)
                return null;
                List<GameModel> gamemodels = new List<GameModel>();
            for (int i = 0; i < query.Count; i++)
            {
                GameModel Model1 = new GameModel();
                Model1.GID = query[i].GID;
                Model1.NPlayers = query[i].NPlayers;
                Model1.Title = query[i].Title;
                gamemodels.Add(Model1);
            }
            
            return gamemodels;
       
        }
        #endregion
        //checked
        #region GameFrame 
        public int AddGameFrame(GameFrameModel gf) // returns 1 if the insertion was successful //-1 if parameters are incoherent//-2 if the GF already exists// checked//
        {
            var cont = new SeniorLinqDataContext();
            GameFrame temp = new GameFrame();
            if (gf.MinPlayers <= gf.MaxPlayers && gf.MinStrategies <= gf.MaxStrategies)
            {
                var query1 = (from gfs in cont.GameFrames where gfs.Title == gf.Title select gfs).SingleOrDefault();
                if (query1 == null)
                {
                    temp.Title = gf.Title;
                    temp.MinPlayers = gf.MinPlayers;
                    temp.MaxPlayers = gf.MaxPlayers;
                    temp.MinStrategies = gf.MinStrategies;
                    temp.MaxStrategies = gf.MaxStrategies;
                    cont.GameFrames.InsertOnSubmit(temp);
                    cont.SubmitChanges();
                    var query = (from gff in cont.GameFrames where gff.Title == temp.Title select gff.Title).SingleOrDefault();
                    if (query != null)
                    { return 1; }
                }
                return -2;
            }
            return -1;

        }
        public int DeleteGameFrame(string T)// returns -1 if the GF was not found // -2 if it is linked to games // 1 success//checked
        {
            var cont = new SeniorLinqDataContext();
            var query = (from gf in cont.GameFrames where gf.Title == T select gf).SingleOrDefault();
            if (query != null)
                {
                var gamechecker = (from game in cont.Games where game.Title == query.Title select game).SingleOrDefault();
                if (gamechecker != null)
                    return -2;
                cont.GameFrames.DeleteOnSubmit(query);
                    cont.SubmitChanges();
                    var q = (from gff in cont.GameFrames where gff.Title == T select gff).SingleOrDefault();
                    if (q == null)
                        return 1;
                }
                return -1;
        }

        public GameFrameModel RetreiveGameFrame(string title)// returns a gameframe if found , otherwise null ///checked
        {
            var cont = new SeniorLinqDataContext();
            var query = (from gff in cont.GameFrames where gff.Title == title select gff).SingleOrDefault();
            if (query != null)
            {
                GameFrameModel gf = new GameFrameModel();
                gf.Title = query.Title;
                gf.MinPlayers = query.MinPlayers;
                gf.MaxPlayers = query.MaxPlayers;
                gf.MinStrategies = query.MinStrategies;
                gf.MaxStrategies = query.MaxStrategies;
                return gf;
            }
            return null;
        }

        public List<GameFrameModel> RetreiveAllGameFrames()// returns a list of all gameframes // null if the list is empty//checked
        {
            var cont = new SeniorLinqDataContext();
            var query = (from gfs in cont.GameFrames select gfs).ToList();
            if (query == null)
                return null;
            List<GameFrameModel> gfmodels = new List<GameFrameModel>();
            for(int i = 0; i < query.Count; i++)
            {
                gfmodels.Add( new GameFrameModel());
                gfmodels[gfmodels.Count-1].Title = query[i].Title;
                gfmodels[gfmodels.Count - 1].MinPlayers = query[i].MinPlayers;
                gfmodels[gfmodels.Count - 1].MaxPlayers = query[i].MaxPlayers;
                gfmodels[gfmodels.Count - 1].MaxStrategies = query[i].MaxStrategies;
                gfmodels[gfmodels.Count - 1].MinStrategies = query[i].MinStrategies;
            }
            
            return gfmodels;
            
        }
        public List<GameFrameModel> GameByNumPlayer(int nop)
        {
            var cont = new SeniorLinqDataContext();
            List<GameFrameModel> gameframemodels = new List<GameFrameModel>();
            if (nop < 4)
            {
                var query = (from gf in cont.GameFrames where gf.MinPlayers == gf.MaxPlayers && gf.MinPlayers == nop select gf).ToList();// all gameframes with nop players
                List<GameFrameModel> gamemodels = new List<GameFrameModel>();
                for (int i = 0; i < query.Count; i++)
                {
                    GameFrameModel Model1 = new GameFrameModel();
                    Model1.Title = query[i].Title;
                    Model1.MaxPlayers = query[i].MaxPlayers;
                    Model1.MaxStrategies = query[i].MaxStrategies;
                    Model1.MinPlayers = query[i].MinPlayers;
                    Model1.MinStrategies = query[i].MinStrategies;
                    gameframemodels.Add(Model1);
                }
            }
            else
            {
                var query = (from gf in cont.GameFrames where gf.MaxPlayers > 3 select gf).ToList();//all N gameframes
                for (int i = 0; i < query.Count; i++)
                {
                    GameFrameModel Model1 = new GameFrameModel();
                    Model1.Title = query[i].Title;
                    Model1.MaxPlayers = query[i].MaxPlayers;
                    Model1.MaxStrategies = query[i].MaxStrategies;
                    Model1.MinPlayers = query[i].MinPlayers;
                    Model1.MinStrategies = query[i].MinStrategies;
                    gameframemodels.Add(Model1);
                }
            }
            return gameframemodels;

        }

        public int RetreiveMinPlayers(string title)//return -1 if no GF was found // number of min players if success
        {
            var cont = new SeniorLinqDataContext();
            var query = (from gfs in cont.GameFrames where gfs.Title == title select gfs).SingleOrDefault();
            if (query == null)
                return -1;
            return (from gfs in cont.GameFrames where gfs.Title == title select gfs.MinPlayers).SingleOrDefault();
        }

        public int RetreiveMaxPlayers(string title)
        {
            var cont = new SeniorLinqDataContext();
            var query = (from gfs in cont.GameFrames where gfs.Title == title select gfs).SingleOrDefault();
            if (query == null)
                return -1;
            return (from gfs in cont.GameFrames where gfs.Title == title select gfs.MaxPlayers).SingleOrDefault();
        }

        public List<GameFrameModel> RetreiveFreeGameFrame()
        {
            var cont = new SeniorLinqDataContext();
            var allGFs = (from gfs in cont.GameFrames select gfs.Title).ToList();
            var allGames = (from games in cont.Games select games.Title).ToList();
            var result = allGFs.Except(allGames);
            List<GameFrameModel> result2 = new List<GameFrameModel>();
            foreach(string title in result)
            {
                GameFrameModel temp = new GameFrameModel();
                var x = (from gfs in cont.GameFrames where gfs.Title == title select gfs).SingleOrDefault();
                temp.Title = x.Title;
                temp.MinStrategies = x.MinStrategies;
                temp.MinPlayers = x.MinPlayers;
                temp.MaxPlayers = x.MaxPlayers;
                temp.MaxStrategies = x.MaxStrategies;
                result2.Add(temp);
            }
            return result2;
        }
        public int UpdateGameFrame(GameFrameModel edited)
        {
            if (edited.MinPlayers > edited.MaxPlayers && edited.MinStrategies > edited.MaxStrategies)
                return -1;
            var cont = new SeniorLinqDataContext();
            var query = (from gfs in cont.GameFrames where gfs.Title == edited.Title select gfs).SingleOrDefault();
            if(query!=null)
            {
                GameFrameModel temp = new GameFrameModel();
                query.MaxPlayers = edited.MaxPlayers;
                query.MaxStrategies = edited.MaxStrategies;
                query.MinPlayers = edited.MinPlayers;
                query.MinStrategies = edited.MinStrategies;
                cont.SubmitChanges();
                return 1;
            }
            return -1;
        }

        #endregion
        #region GamePlayer
        public int AddGamePlayer(GamePlayerModel player) // returns -1 if the username already exists//-2 if there is no game // 1 success
        {
            var cont = new SeniorLinqDataContext();
            var query1 = (from gameplayers in cont.Gameplayers where gameplayers.Username == player.UserName select gameplayers).SingleOrDefault();
            if (query1 == null)
            {
                var gamechecker = (from game in cont.Games where game.GID == player.GID select game).SingleOrDefault();
                if (gamechecker == null)
                    return -2;
                Gameplayer temp = new Gameplayer();
                temp.GID = player.GID;
                temp.Username = player.UserName;
                cont.Gameplayers.InsertOnSubmit(temp);
                cont.SubmitChanges();
                    return 1;
            }
            return -1;
        }

        //public int DeleteGamePlayer(String uname)
        //{
        //    var cont = new SeniorLinqDataContext();
        //    var query = (from gp in cont.Gameplayers where gp.Username == uname select gp).SingleOrDefault();
        //    cont.Gameplayers.DeleteOnSubmit(query);
        //    cont.SubmitChanges();
        //    var q = (from palyer in cont.Gameplayers where palyer.Username == uname select palyer).SingleOrDefault();
        //    if (q == null)
        //        return 1;
        //    else return -1;
        //}
        public List<GamePlayerModel> RetreiveGamePlayers(int gid)//retruns all found GPs if found // null List otherwise//checked
        {
            var cont = new SeniorLinqDataContext();
            var query = (from gp in cont.Gameplayers where gp.GID == gid select gp).ToList();
            if (query != null)//found associated players
            {
                List<GamePlayerModel> gpm = new List<GamePlayerModel>();
                for(int i=0;i<query.Count;i++)
                {
                    GamePlayerModel g = new GamePlayerModel();
                    g.GID = query[i].GID;
                    g.UserName = query[i].Username;
                    gpm.Add(g);
                    cont.SubmitChanges();
                }
                return gpm;
            }
            return null;
        }
        public List<int> RetreivePlayerGames(string username)//returns null if no games/players were found//a list of all Gids associated to this player when successful//checked
        {
            var cont = new SeniorLinqDataContext();
            var usercheck = (from user in cont.Users where user.Username == username select user).SingleOrDefault();
            if(usercheck==null)
                return null;
            var query = (from games in cont.Gameplayers where games.Username == username select games).ToList();//returns all games with associated username
            if (query == null)
                return null;
            List<int> Gids = new List<int>();
            for(int i=0;i<query.Count();i++)
            {
                Gids.Add(query[i].GID);
            }
            return Gids;
            
        }
        public List<GamePlayerModel> RetreiveAllGamePlayers()
        {
            var cont = new SeniorLinqDataContext();
            List<GamePlayerModel> gameplayers = new List<GamePlayerModel>();
            var query = (from gp in cont.Gameplayers select gp).ToList();
            for(int i=0;i<query.Count;i++)
            {
                GamePlayerModel gpm = new GamePlayerModel();
                gpm.GID = query[i].GID;
                gpm.UserName = query[i].Username;
                gameplayers.Add(gpm);
            }
            return gameplayers;
        }
        public List<GamePlayerModel> RetreiveAllGamePlayersByPlayer(string usrname)
        {
            var cont = new SeniorLinqDataContext();
            List<GamePlayerModel> gameplayers = new List<GamePlayerModel>();
            var query = (from gp in cont.Gameplayers where gp.Username==usrname select gp).ToList();
            for (int i = 0; i < query.Count; i++)
            {
                GamePlayerModel gpm = new GamePlayerModel();
                gpm.GID = query[i].GID;
                gpm.UserName = query[i].Username;
                gameplayers.Add(gpm);
            }
            return gameplayers;
        }
        #endregion
        #region GFStrategy

        public int AddGFStrategy(GFStrategyModel gfstrategy)
        {
            var cont = new SeniorLinqDataContext();
            GFStrategy temp = new GFStrategy();
            temp.Title = gfstrategy.Title;
            temp.StrategyId = gfstrategy.StrategyId;
            cont.GFStrategies.InsertOnSubmit(temp);
            cont.SubmitChanges();
            var query = (from gs in cont.GFStrategies where gs.StrategyId == temp.StrategyId && gs.Title == temp.Title select gs.Title).SingleOrDefault();
            if (query != null)
                return 1;
            return -1;
        }
        //public int DeleteGFStrategy(int gstrategy)
        //{
        //    var cont = new SeniorLinqDataContext();
        //    var query = (from strat in cont.GFStrategies where strat.StrategyId == gstrategy select strat).SingleOrDefault();
        //    cont.GFStrategies.DeleteOnSubmit(query);
        //    cont.SubmitChanges();
        //    var q = (from strat in cont.GFStrategies where strat.StrategyId == gstrategy select strat).SingleOrDefault();
        //    if (q == null)
        //        return 1;
        //    else return -1;
        //}
        public GFStrategyModel RetreiveGFStrategy(int gfstrategyid)
        {
            var cont = new SeniorLinqDataContext();
            var query = (from fgstr in cont.GFStrategies where fgstr.StrategyId == gfstrategyid select fgstr).SingleOrDefault();
            GFStrategyModel gfstratm = new GFStrategyModel();
            gfstratm.StrategyId = query.StrategyId;
            gfstratm.Title = query.Title;
            gfstratm.Description = query.Description;
            return gfstratm;
        }
        #endregion
        #region NashPointProfile
        public int AddNashPointProfile(NashPointProfileModel NP)
        {

            var cont = new SeniorLinqDataContext();
            NashPointProfile temp = new NashPointProfile();
            temp.NPID = NP.NPID;
            temp.GID = NP.GID;
            cont.NashPointProfiles.InsertOnSubmit(temp);
            cont.SubmitChanges();
            var query = (from np in cont.NashPointProfiles where np.NPID == temp.NPID && np.NPID == temp.NPID select np.NPID).SingleOrDefault();
            if (query != null)
                return query;
            return -1;
        }
        //public int DeleteNashPointProfile(int NPID)
        //{
        //    var cont = new SeniorLinqDataContext();
        //    var query = (from npp in cont.NashPointProfiles where npp.NPID == NPID select npp).SingleOrDefault();
        //    cont.NashPointProfiles.DeleteOnSubmit(query);
        //    cont.SubmitChanges();
        //    var q = (from npp in cont.NashPointProfiles where npp.NPID == NPID select npp).SingleOrDefault();
        //    if (q == null)
        //        return 1;
        //    else return -1;
        //}
        public NashPointProfileModel RetreiveNashPointProfile(int NPID)
        {
            var cont = new SeniorLinqDataContext();
            var query = (from npp in cont.NashPointProfiles where npp.NPID == NPID select npp).SingleOrDefault();
            NashPointProfileModel nppm = new NashPointProfileModel();
            nppm.NPID = query.NPID;
            nppm.GID = query.GID;
            return nppm;
        }

        #endregion
        #region NPPayoff
        public int AddNPPayoff(NPPayoffModel payoff)
        {
            var cont = new SeniorLinqDataContext();
            NPPayoff temp = new NPPayoff();
            temp.NPID = payoff.NPID;
            temp.PayoffId = payoff.PayoffId;
            temp.Username = payoff.Username;
            temp.Payoff = payoff.Payoff;
            cont.NPPayoffs.InsertOnSubmit(temp);
            cont.SubmitChanges();
            var query = (from pof in cont.NPPayoffs where temp.NPID == pof.NPID && temp.PayoffId == pof.PayoffId && temp.Username == payoff.Username select pof.Payoff).SingleOrDefault();
            if (query != null)
                return -1;
            return 1;
        }
        //public int DeleteNPPayoff(int payoffid,string username)
        //{
        //    var cont = new SeniorLinqDataContext();
        //    var query = (from pof in cont.NPPayoffs where pof.PayoffId == payoffid && pof.Username ==username select pof).SingleOrDefault();
        //    cont.NPPayoffs.DeleteOnSubmit(query);
        //    cont.SubmitChanges();
        //    var q = (from pof in cont.NPPayoffs where pof.PayoffId == payoffid && pof.Username == username select pof).SingleOrDefault();
        //    if (q == null)
        //        return 1;
        //    else return -1;
        //}
        public NPPayoffModel RetreiveNPPayoff(int payoffid, string username)
        {
            var cont = new SeniorLinqDataContext();
            var query = (from npof in cont.NPPayoffs where npof.Payoff == payoffid && npof.Username == username select npof).SingleOrDefault();
            NPPayoffModel pm = new NPPayoffModel();
            pm.NPID = query.NPID;
            pm.Payoff = query.Payoff;
            pm.PayoffId = query.PayoffId;
            pm.Username = query.Username;
            return pm;
        }

        #endregion
        #region NPStrategy
        public int AddNPStrategy(NPStrategyModel npstrategy)
        {
            var cont = new SeniorLinqDataContext();
            NPStrategy temp = new NPStrategy();
            temp.StrategyId = npstrategy.StrategyId;
            temp.NPID = npstrategy.NPID;
            temp.Username = npstrategy.Username;
            cont.NPStrategies.InsertOnSubmit(temp);
            cont.SubmitChanges();
            var query = (from strat in cont.NPStrategies where temp.NPID == strat.NPID && temp.Username == strat.Username && temp.StrategyId == strat.StrategyId select strat.NPID).SingleOrDefault();
            if (query != null)
                return -1;
            return 1;
        }

        //public int DeleteNPStrategy(int nptrategy)
        //{
        //    var cont = new SeniorLinqDataContext();
        //    var query = (from npstrr in cont.NPStrategies where npstrr.StrategyId == nptrategy select npstrr).SingleOrDefault();
        //    cont.NPStrategies.DeleteOnSubmit(query);
        //    cont.SubmitChanges();
        //    var q = (from npstrr in cont.NPStrategies where npstrr.StrategyId == nptrategy select npstrr).SingleOrDefault();
        //    if (q == null)
        //        return 1;
        //    else return -1;
        //}


        #endregion
        #region User
        public int AddUser(UserModel user)
        {
            var cont = new SeniorLinqDataContext();
            User temp = new User();
            temp.Username = user.Username;
            //string tp = user.Password;
           // temp.Password = tp;
            temp.Password = user.Password;// % 10).ToString();
            temp.AdminFlag = user.AdminFlag;
            cont.Users.InsertOnSubmit(temp);
            cont.SubmitChanges();
            var query = (from u in cont.Users where temp.Username == u.Username select u.Username).SingleOrDefault();
          //  if (query != null)
                return 1;
           // return -1;
        }
        //public int DeleteUser(string username)
        //{
        //    var cont = new SeniorLinqDataContext();
        //    var query = (from user in cont.Users where user.Username == username select user).SingleOrDefault();
        //    cont.Users.DeleteOnSubmit(query);
        //    cont.SubmitChanges();
        //    var q = (from user in cont.Users where user.Username == username select user).SingleOrDefault();
        //    if (q == null)
        //        return 1;
        //    else return -1;
        //}
        public UserModel RetreiveUser(string username , string password)
        {
            var cont = new SeniorLinqDataContext();
            var query = (from us in cont.Users where us.Username == username && us.Password==password select us).SingleOrDefault();
            UserModel user = new UserModel();
            user.Username = query.Username;
            user.Password = query.Password;
            user.AdminFlag = (char)query.AdminFlag;
            return user;
        }
        #endregion


        public NPStrategyModel RetreiveNPStrategy(int npstrategyid , string username)
        {
            var cont = new SeniorLinqDataContext();
            var query = (from nps in cont.NPStrategies where nps.Username == username && nps.StrategyId==npstrategyid select nps).SingleOrDefault();
            NPStrategyModel npsm = new NPStrategyModel();
            npsm.StrategyId = query.StrategyId;
            npsm.Username = query.Username;
            npsm.NPID = query.NPID;
            return npsm;
    }

  
        

        //public int UpdateGamePlayer(int gameplayerid)
        //{
        //    throw new NotImplementedException();
        //}

        //public int UpdateGFStrategy(int gfstrategyid)
        //{
        //    throw new NotImplementedException();
        //}

        //public int UpdateNashPointProfile(int NPID)
        //{
        //    throw new NotImplementedException();
        //}

        //public int UpdateNPPayoff(int payoffid)
        //{
        //    throw new NotImplementedException();
        //}

        //public int UpdateNPStrategy(int npstrategyid)
        //{
        //    throw new NotImplementedException();
        //}

        public int UpdateUser(string username , string password)//returns 1 on success // -2 if the user was not found // -1 on failure//checked
        {
            var cont = new SeniorLinqDataContext();
            var query = (from user in cont.Users where user.Username == username select user).SingleOrDefault();
            var temp = new UserModel();
            if (query != null)
            {
                query.Password = password;
                cont.SubmitChanges();
            }
            else return -2;
            var query2 = (from user in cont.Users where user.Username == username select user).SingleOrDefault();
            if (query2.Password == password)
                return 1;
            return -1;
        }

        //public int UpdateGamePlayer(string username)
        //{
        //    throw new NotImplementedException();
        //}

        public int DeleteNPStrategy(int nptrategy, string username)
        {   
            throw new NotImplementedException();
        }

       


        //public int UpdateNPStrategy(int npstrategyid, string username)
        //{
        //    throw new NotImplementedException();
        //}

        //public int UpdateNPPayoff(int payoffid, string username)
        //{
        //    throw new NotImplementedException();
        //}



    }
    }

