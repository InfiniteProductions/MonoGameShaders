using System;

namespace _2DEffect_test
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lights()) // LightsExtended())
                game.Run();
        }
    }
#endif
}
