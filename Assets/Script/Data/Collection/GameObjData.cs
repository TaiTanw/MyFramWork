using System.Collections.Generic;
public class GameObjData:BaseCollection
{
     public Dictionary< int,TowerInfo> TowerInfoDic =new Dictionary<int,TowerInfo>();
     public Dictionary< int,TestInfo> TestInfoDic =new Dictionary<int,TestInfo>();
}