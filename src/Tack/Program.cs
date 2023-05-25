using System.Threading.Tasks;

namespace Tack
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Tack blueTack = new Tack();
            return await blueTack.Run(args);
        }
    }
}
