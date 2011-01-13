using System;

namespace Precipitate
{
    public interface IProjectParser
    {
        Project Parse(string filename, string name);
    }
}