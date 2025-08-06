using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace Parcels
{
    public static class Active
    {
        // Creating a shorthand for the library call for MdiActiveDocument.
        // This call `Application.DocumentManager.MdiActiveDocument` gets shortened to `Document`
        public static Document Document => Application.DocumentManager.MdiActiveDocument;

        // adding th shorthand for `Application.DocumentManager.MdiActiveDocument.Editor`
        public static Editor Editor => Document.Editor;

        // adding th shorthand for `Application.DocumentManager.MdiActiveDocument.Database`
        public static Database Database => Document.Database;

        public static void UsingTransaction(Action<Transaction> action)
        {
            using (var transaction = Active.Database.TransactionManager.StartTransaction())
            {
                //do some stuff here

                action(transaction);
                transaction.Commit();
            }
        }
    }
}
