using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcels
{
    public class HtmlParcelSummarizer : IParcelSummarizer
    {
        public string GenerateSummary(ParcelSummary summary)
        {
            // csharpier-ignore
            return
$@"<!DOCTYPE html>
<html>
  <head>
    <title>Parcel Summary</title>
  </head>
  <body>
    <h1>Parcel Summary for {Active.Document.Name}</h1>
    <ul>
      <li>Found {summary.Count} parcels.</li>
      <li>Combined area: {summary.Area:N2}</li>
    </ul>
  </body>
</html>";
        }
    }
}
