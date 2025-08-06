using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcels
{
    public class TextMessageWriter : IMessageWriter
    {
        private readonly string _filePath;
        private readonly bool _shouldAppend = false;

        public TextMessageWriter(string filePath, bool shouldAppend = false)
        {
            _filePath = filePath;
            _shouldAppend = shouldAppend;
        }

        public void WriteMessage(string message)
        {
            using (var streamWriter = new StreamWriter(_filePath, _shouldAppend))
            {
                streamWriter.WriteLine(message);
            }
        }
    }
}
