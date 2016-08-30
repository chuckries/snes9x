using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snes9xWRC;
using Windows.Storage;
using Windows.Foundation;
using Windows.Storage.Streams;
using System.IO.Compression;
using System.IO;

namespace Snes9x.Common
{
    class StorageFileRom : IRom
    {
        private StorageFile _file;

        public StorageFileRom(StorageFile file)
        {
            _file = file;
        }

        public string Name { get { return _file.DisplayName; } }
        public string Path { get { return _file.Path; } }

        public IAsyncOperation<object> GetBytesAsync()
        {
            return Task.Run(async () =>
            {
                if (_file.FileType == ".zip")
                {
                    using (ZipArchive archive = new ZipArchive(await _file.OpenStreamForReadAsync()))
                    {
                        ZipArchiveEntry entry = archive.Entries.FirstOrDefault(e => System.IO.Path.GetFileNameWithoutExtension(e.Name) == System.IO.Path.GetFileNameWithoutExtension(_file.Name));
                        using (Stream entryStream = entry.Open())
                        {
                            MemoryStream memoryStream = new MemoryStream();
                            await entryStream.CopyToAsync(memoryStream);
                            return memoryStream.ToArray();
                        }
                    }
                }
                else
                {
                    IBuffer buffer = await FileIO.ReadBufferAsync(_file);
                    DataReader reader = DataReader.FromBuffer(buffer);
                    byte[] bytes = new byte[buffer.Length];
                    reader.ReadBytes(bytes);
                    return (object)bytes;
                }
            }).AsAsyncOperation();
        }
    }
}
