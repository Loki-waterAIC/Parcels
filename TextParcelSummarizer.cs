using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcels
{
    public class TextParcelSummarizer : IParcelSummarizer
    {
        private readonly IParcelSummarizer _decoratedSummarizer;

        public TextParcelSummarizer(IParcelSummarizer decoratedSummarizer)
        {
            _decoratedSummarizer = decoratedSummarizer;
        }

        public string GenerateSummary(ParcelSummary summary)
        {
            var message = Active.Document.Name;
            message += _decoratedSummarizer.GenerateSummary(summary);
            message += DateTime.Now.ToString();
            return message;
        }
    }
}
