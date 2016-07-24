using System;

namespace XNAseries3
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (var game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}