using System;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices; // adds types: Document
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput; // adds types: PromptSelectionOptions
using Autodesk.AutoCAD.Runtime; // adds attributes: CommandMethod
using Autodesk.AutoCAD.Windows.Data; // adds types: LayerTable and LayerTableRecord

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

        // [CommandMethod("PS_CountParcels")]
        // public void CountParcels()
        // {
        //     var cmd = new ParcelCounter();
        //     var summary = cmd.Count();
        //     Active.Editor.WriteMessage($"\nFound {summary} parcels.");
        // }
        [CommandMethod("PS_CountParcels")]
        public void CountParcels()
        {
            var cmd = new ParcelCounter();
            var summary = cmd.Count();
            var summarizer = new ParcelSummarizer();
            summary.Write(summarizer, new AutocadMessageWriter());
            var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            summary.Write(
                new TextParcelSummarizer(summarizer),
                new TextMessageWriter(Path.Combine(myDocuments, "ParcelSummary.txt"), true)
            );
        }
    }

    internal class ParcelCounter
    {
        public ParcelCounter() { }

        // PromptSelectionResult returns the value and a status if the user finished the prompt screne or canceled.
        private PromptSelectionResult SelectParcels()
        {
            var options = new PromptSelectionOptions();
            options.MessageForAdding = "Add parcels";
            options.MessageForRemoval = "Remove parcels";
            var filter = new SelectionFilter(
                new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Start, "LWPOLYLINE"),
                    new TypedValue((int)DxfCode.LayerName, "Parcels"),
                    /*
                        Filter for objects with a DotCode of Start that has the value of "LWPOLYLINE"
                            Dotcode for "Start" is 0.
                        Filter for objects with a DotCode of Layer that has the value of "Parcels"
                            Dotcode for "layer" is 8

                        NOTE: you can find the dotcodes for the values by going into AutoCAD and
                            entering the command `(entget (car (entsel)))`,
                            this will return all of the objects in the drawing
                            along with their entity id's and dotcode values.
                            DotCode values are consistent throughout AutoCAD.

                            Meaning you would use TypedValue(0, "LWPOLYLINE") or TypedValue(8, "Parcels")
                            and it would work the same as the above.
                            DxfCode is just an ENUM holding all of the human friendly names of all the dotcodes
                            to make reading code more human friendly.

                        ```sh
                            Command: (entget (car (entsel)))
                            select object: ((-1 . <Entity name: 2423a77bcd0>) (0 . "LWPOLYLINE")
                                (330 . <Entity name: 2423a7789f0>) (5 . "20D") (100 . "AcDbEntity") (67 . 0)
                                (410 . "Model") (8 . "Parcels") (100 . "AcDbP01y1ine") (90 . 4) (70 . 0) (43 . 0.0)
                                (38 . 0.0) (39 . 0.0) (10 5.16359 0.180022) (40 . 0.0) (41 . 0.0) (42 . 0.0)
                                (91 . 0) (l0 8.34169 3.31995) (40 . 0.0) (41 . 0.0) (42 . 0.0) (91 . 0)
                                (10 11.6257 0.0741823) (40 . 0.0) (41 . 0.0) (42 . 0.0) (91 . 0) (10 15.4748 3.28467)
                                (40 . 0.0) (41 . 0.0) (42 . 0.0) (91 . e) (210 0.0 0.0 1.0)
                            )
                        ```
                    */
                }
            );
            return Active.Editor.GetSelection(options, filter);
        }

        // public int Count()
        // {
        //     var count = 0;
        //     // var options = new PromptSelectionOptions();
        //     // options.MessageForAdding = "Add parcels";
        //     // options.MessageForRemoval = "Remove parcels";
        //     // var filter = new SelectionFilter(
        //     //     new TypedValue[]
        //     //     {
        //     //         new TypedValue((int)DxfCode.Start, "LWPOLYLINE"),
        //     //         new TypedValue((int)DxfCode.LayerName, "Parcels"),
        //     //     }
        //     // );
        //     // var result = Active.Editor.GetSelection(options, filter);
        //     var result = SelectParcels();

        //     // result.Status is PromptStatus.OK if the user selected objects and pressed Enter.
        //     if (result.Status == PromptStatus.OK)
        //     {
        //         Active.UsingTransaction(tr =>
        //         {
        //             // => is a lambda operator, so this is a subfunction.
        //             foreach (var objectId in result.Value.GetObjectIds())
        //             {
        //                 var polyline = (Polyline)tr.GetObject(objectId, OpenMode.ForRead);
        //                 if (polyline.Closed)
        //                 {
        //                     count++;
        //                 }
        //             }
        //         });
        //     }
        //     return count;
        // }
        public ParcelSummary Count()
        {
            var summary = new ParcelSummary();
            var result = SelectParcels();
            if (result.Status == PromptStatus.OK)
            {
                Active.UsingTransaction(tr =>
                {
                    foreach (var objectId in result.Value.GetObjectIds())
                    {
                        var polyline = (Polyline)tr.GetObject(objectId, OpenMode.ForRead);
                        if (polyline.Closed)
                        {
                            summary.Count++;
                            summary.Area += polyline.Area;
                        }
                    }
                });
            }
            return summary;
        }
    }

    internal class ParcelLayer
    {
        public ParcelLayer() { }

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
                        Color = Color.FromColorIndex(ColorMethod.ByAci, 161),
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
        }
    }
}
