using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snes9x.Common
{
    interface IRomFile
    {
        string Name { get; }
        string FileName { get; }
        string Path { get; }

        Task<byte[]> GetBytesAsync();
    }
}
