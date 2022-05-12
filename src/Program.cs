using System;
using System.Diagnostics;
using System.Text.Json;

namespace CyberCity {
    public static class Program {
        [STAThread]
        static void Main() {
            using (var game = new Game1())
                game.Run();
        }
    }
}
