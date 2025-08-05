using System;
using System.Linq;

using Autodesk.AutoCAD.Runtime; // adds attributes: CommandMethod
using Autodesk.AutoCAD.ApplicationServices; // adds types: Document
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Windows.Data; // adds types: LayerTable, and LayerTableRecord

namespace Parcels
{
    public class Commands
    {
        /**
        NOTE: When the user types PS_Hello in the AutoCAD command line,
        the following method `Hello()` will be executed.

        This attribute comes from the Autodesk.AutoCAD.Runtime
        */
        [CommandMethod("PS_Hello")]
        public void Hello()
        {
            var document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            // Alternative: Document document = Autodesk.AutoCAD
            //                                          .ApplicationServices
            //                                          .Application
            //                                          .DocumentManager
            //                                          .MdiActiveDocument;
            // NOTE: Above is also an example of a line breaking in C#
            // Document type comes from the Autodesk.AutoCAD.ApplicationServices namespace
            //      NOTE: MdiActiveDocument is the currently active document in AutoCAD.

            var editor = document.Editor;
            // NOTE: Editor is a property of the Document class that provides access
            //      to the command line and other editor features.
            //      The Editor can do things like select objects in drawings, write messages, etc.

            editor.WriteMessage("\nHello World!");
            // Writes "Hello World!" to the command line in AutoCAD.

        }

        [CommandMethod("PS_CreateParcelLayer")]
        public void CreateLayer()
        {
            var creator = new ParcelLayer();
            creator.Create();
        }
    }

    internal class ParcelLayer
    {
        public ParcelLayer()
        {
        }

        internal void Create()
        {
            var layerName = "Parcels";
            var document = Application.DocumentManager.MdiActiveDocument;
            var database = document.Database;
            //  NOTE: Any time we want to draw, or access layers, styles, etc., we’ll be working with the Database class

            /**
            NOTE: Note the using in this case. It is similar to  `with open()` in Python.
                it is used to note that the using only applies to this block `{...}` of code.

                "
                Many objects in C# use an _interface_ called `IDisposable` which ensures
                that objects release important resources.
                "

                "
                A transaction is a critical mechanism Autocad uses to safeguard access to the database.
                It ensures that things get saved properly if everything goes well,
                in which case we can call  transaction.Commit() at the very end of the using block.
                If something goes wrong, a  transaction.Abort() statement is issued,
                and nothing that happened within the using block is persisted to the database.
                "

            */
            using (var transaction = database.TransactionManager.StartTransaction())
            {
                /**
                Casting a variable to a specific type can be done one many ways.
                The two main ways are by using a front cast (left over from C/C++) (Type)variable;
                Or by using back casting, variable as Type.
                */
                var layerTable = (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);
                // Alternative: var layerTable = transaction.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;

                LayerTableRecord layer;
                if (layerTable.Has(layerName) == false)
                {
                    // NOTE: This is an object initializer block.
                    layer = new LayerTableRecord
                    {
                        Name = layerName,
                        Color = Color.FromColorIndex(ColorMethod.ByAci, 161)
                        // NOTE: sets the color of the layer to a specific AutoCAD Color Index (ACI). Colors range from 0 to 256
                    };
                    /**
                    Code Block Alternative, assigning values after init

                    layer = new LayerTableRecord();
                    layer.Name = layerName;
                    layer.Color = Color.FromColorIndex(ColorMethod.ByAci, 161);
                    */

                    // layerTable in read only mode in memory, need to set it to write in order to write to the database.
                    //      We do that by using the UpgradeOpen() method.
                    layerTable.UpgradeOpen();
                    layerTable.Add(layer);
                    transaction.AddNewlyCreatedDBObject(layer, true);
                }
                database.Clayer = layerTable[layerName]; // Clayer -> "Current Layer"
                transaction.Commit(); // commit data to the datebase
            }
            ;

        }
    }
}
