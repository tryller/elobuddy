using System;
using EloBuddy;
using EloBuddy.SDK.Events;

namespace WuAIO
{
    class Program
    {
        static void Main(string[] args) { Loading.OnLoadingComplete += OnLoadingComplete; }

        static void OnLoadingComplete(EventArgs args)
        {
            try
            {
                Activator.CreateInstance(null, "WuAIO." + Player.Instance.ChampionName);
                Chat.Print("Wu{0} Loaded, [By WujuSan]", Player.Instance.ChampionName == "MasterYi" ? "Yi" : Player.Instance.ChampionName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}