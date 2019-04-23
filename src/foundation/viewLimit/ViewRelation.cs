using System.Collections.Generic;


public class ViewRelation
{
    /// <summary>
    /// view 之间互斥关系列表
    /// </summary>
    private static List<List<string>> mutualView = new List<List<string>>();

    /// <summary>
    /// 阻止该view打开的view列表，value列表中系统若有打开，则禁止key中ui打开
    /// </summary>
    private static Dictionary<string,List<string>> preventView = new Dictionary<string, List<string>>();

    /// <summary>
    /// 添加互斥关系列表
    /// </summary>
    public static void AddMutualView(List<string> temp1)
    {
        mutualView.Add(temp1);
    }

    /// <summary>
    /// 添加阻止关系
    /// </summary>
    /// <param name="str"></param>
    /// <param name="temp"></param>
    public static void AddpreventView(string str, List<string> temp)
    {
        if(!preventView.ContainsKey(str))
        preventView.Add(str,temp);
    }

    /// <summary>
    /// 获取所有与之互斥的view
    /// </summary>
    /// <returns></returns>
    public static List<string> GetMutualPanel(string name)
    {
        List<string> mutuals = new List<string>();
        for (int i = 0; i < mutualView.Count; i++)
        {
            if (mutualView[i].Contains(name))
            {
                List<string> temp = mutualView[i];
                for (int j = 0; j < temp.Count; j++)
                {
                    if (temp[j] != name && !mutuals.Contains(temp[j]))
                    {
                        mutuals.Add(temp[j]);
                    }
                }
            }
        }
        return mutuals;
    }


    /// <summary>
    /// 获取阻止其打开的mediator列表
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static List<string> GetPreventPanel(string name)
    {
        if (preventView.ContainsKey(name))
            return preventView[name];
        return null;
    }

}

