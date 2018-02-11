using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
namespace SeniorDBServer
{
    [ServiceContract]
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    public interface IDBFunctions
    {
        #region GameFrame
        [OperationContract]
        int AddGameFrame(GameFrameModel gf);
        [OperationContract]
        int DeleteGameFrame(string T);
        [OperationContract]
        GameFrameModel RetreiveGameFrame(string T);
        [OperationContract]
        int UpdateGameFrameMinPlayers(string title , int min);
        [OperationContract]
        int UpdateGameFrameMaxPlayers(string title , int max);
        [OperationContract]
        int UpdateGameFrameMinStrategies(string title , int min );
        [OperationContract]
        int UpdateGameFrameMaxStrategies(string title , int max);
        [OperationContract]
        List<GameFrameModel> RetreiveAllGameFrames();
        #endregion
        #region User
        [OperationContract]
        int AddUser(UserModel user);
        //[OperationContract]
        //int DeleteUser(string username);
        [OperationContract]
        UserModel RetreiveUser(string username , string password);
        int UpdateUser(int userid);
        #endregion
        #region GamePlayer
        [OperationContract]
        int AddGamePlayer(GamePlayerModel player);
        //[OperationContract]
        //int DeleteGamePlayer(string username);
        [OperationContract]
        List<GamePlayerModel> RetreiveGamePlayers(int gid);
      //  [OperationContract]
        //GameFrameModel RetreiveGamePlayer(string username);
        [OperationContract]
        int UpdateGamePlayer(string username);

        #endregion
        #region Game
        [OperationContract]
        int AddGame(GameModel game);
        [OperationContract]
        int DeleteGame(int gameid);
        [OperationContract]
        GameModel RetreiveGame(int gameid);
        [OperationContract]
        List<GameModel> RetreiveAllGames();
        [OperationContract]
        int UpdateGameNumofPlayers(int gid, int nop);
        #endregion
        #region GFStrategy
        [OperationContract]
        int AddGFStrategy(GFStrategyModel gfstrategy);
        //[OperationContract]
        //int DeleteGFStrategy(int gstrategy);
        [OperationContract]
        GFStrategyModel RetreiveGFStrategy(int gfstrategyid);
        [OperationContract]
        int UpdateGFStrategy(int gfstrategyid);
        #endregion
        #region NPStrategy
        [OperationContract]
        int AddNPStrategy(NPStrategyModel npstrategy);
        //[OperationContract]
        //int DeleteNPStrategy(int nptrategy,string username);
        [OperationContract]
        NPStrategyModel RetreiveNPStrategy(int npstrategyid,string username);
        [OperationContract]
        int UpdateNPStrategy(int npstrategyid,string username);
        #endregion
        #region NashPointProfile
        [OperationContract]
        int AddNashPointProfile(NashPointProfileModel NP);
        //[OperationContract]
        //int DeleteNashPointProfile(int NPID);
        [OperationContract]
        NashPointProfileModel RetreiveNashPointProfile(int NPID);
        [OperationContract]
        int UpdateNashPointProfile(int NPID);
        #endregion
        #region NPPayoff
        [OperationContract]
        int AddNPPayoff(NPPayoffModel payoff);
        //[OperationContract]
        //int DeleteNPPayoff(int payoffid , string username);
        [OperationContract]
        NPPayoffModel RetreiveNPPayoff(int payoffid , string username);
        [OperationContract]
        int UpdateNPPayoff(int payoffid , string username);
        #endregion
    }
    
    [DataContract]
    public class GameFrameModel
    {
        [DataMember]
        public string Title { set; get; }
        [DataMember]
        public int MinPlayers { set; get; }
        [DataMember]
        public int MaxPlayers { set; get; }
        [DataMember]
        public int MinStrategies { set; get; }
        [DataMember]
        public int MaxStrategies { set; get; }
    }
    [DataContract]
    public class UserModel
    {
        [DataMember]
        public string Username { set; get; }
        [DataMember]
        public string Password { set; get; }
        [DataMember]
        public char AdminFlag { set; get; }
    }
    [DataContract]
    public class GamePlayerModel
    {
        [DataMember]
        public int GID { set; get ; }
        [DataMember]
        public string UserName { set; get; }
    }

    [DataContract]
    public class GameModel
    {
        [DataMember]
        public int GID { set; get; }
        [DataMember]
        public string Title { set; get; }
        [DataMember]
        public int NPlayers { set; get; }
    }
    
    [DataContract]
    public class GFStrategyModel
    {
        [DataMember]
        public int StrategyId { get; set; }
        [DataMember]
        public String Title { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
    
    [DataContract]
    public class NPStrategyModel
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public int StrategyId { get; set; }
        [DataMember]
        public int NPID { get; set; }
    }

    [DataContract]
    public class NashPointProfileModel
    {
        [DataMember]
        public int NPID { get; set; }
        [DataMember]
        public int GID { get; set; }
    }

    [DataContract]
    public class NPPayoffModel
    {
        [DataMember]
        public int PayoffId { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public int NPID { get; set; }
        [DataMember]
        public int Payoff { get; set; }
    }
}
