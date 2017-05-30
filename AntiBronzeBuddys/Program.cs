using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace AntiBronzeBuddys
{
    class Program
    {
        static List<string> palavarBloqueadas = new List<string> {
                "cu", "noob", "retardado", "mongol", "fuck", "fuckoff", "lixo", "retardado", "doente",
        "script", "scripter", "scripting"};

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            Chat.Say("/mute all");
            Chat.OnInput += Chat_OnInput;
        }

        private static void Chat_OnInput(ChatInputEventArgs args)
        {
            args.Process = true;
            if (palavarBloqueadas.Any(str => args.Input.StartsWith(str)))
                args.Process = false;
        }
    }
}
