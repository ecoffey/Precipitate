using System.Text;

namespace Precipitate
{
    public static class StringPrinting
    {
        public static string Depth(this int depth)
        {
            var depthBuilder = new StringBuilder();

            for (int i = 0; i < depth; i++)
            {
                depthBuilder.Append(' ');
            }

            return depthBuilder.ToString();
        }
    }
}