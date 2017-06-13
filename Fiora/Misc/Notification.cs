using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JokerFioraBuddy.Properties;
using EloBuddy;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace JokerFioraBuddy.Misc
{
    static class Notification
    {
        static List<NotificationModel> _notifications = new List<NotificationModel>();
        public static readonly TextureLoader TextureLoader = new TextureLoader();
        private static Sprite MainBar { get; set; }
        private static readonly Text Text = new Text("", new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular)) { Color = Color.White };

        static Notification()
        {
            TextureLoader.Load("notification", Resources.notification);
        }

        public static void DrawNotification(NotificationModel notification)
        {
            _notifications.Add(notification);

            MainBar = new Sprite(() => TextureLoader["notification"]);

            Init();
        }

        private static void Init()
        {
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += OnDomainUnload;
        }

        public static bool IsDivisble(int x, int n)
        {
            return (x % n) == 0;
        }

        private static void OnDomainUnload(object sender, EventArgs e)
        {
            TextureLoader.Dispose();
        }

    }
}
