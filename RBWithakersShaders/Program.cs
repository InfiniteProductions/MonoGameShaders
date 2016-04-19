using System;

namespace RBWithakersShaders
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Transparency())
                game.Run();
        }
    }
#endif
}
