using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcels
{
    public class AutocadMessageWriter : IMessageWriter
    {
        public void WriteMessage(string message)
        {
            Active.Editor.WriteMessage($"\n{message}");
        }
    }
}
