// Example based off the guide found here: https://static.au-uw2-prd.autodesk.com/Class_Handout_SD467609_Ben_Rand.pdf

using Autodesk.AutoCAD.Runtime; // CommandMethod attribute comes from this namespace
using Autodesk.AutoCAD.ApplicationServices; // missing from example, added for Document
using System;
using System.Linq;
namespace Parcels
{
    public class Commands
    {
        /**
        When the user types PS_Hello in the AutoCAD command line,
        the following method `Hello()` will be executed.

        This attribute comes from the Autodesk.AutoCAD.Runtime
        */
        [CommandMethod("PS_Hello")]
        public void Hello()
        {
            // var document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            //      Making the document variable explicit in the following uncommented line.

            Document document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            // Document type comes from the Autodesk.AutoCAD.ApplicationServices namespace
            //      MdiActiveDocument is the currently active document in AutoCAD.

            var editor = document.Editor;
            // Editor is a property of the Document class that provides access to the command line and other editor features.
            //      The Editor can do things like select objects in drawings, write messages, etc.

            editor.WriteMessage("\nHello World!");
            // Writes "Hello World!" to the command line in AutoCAD.

        }
    }
}
