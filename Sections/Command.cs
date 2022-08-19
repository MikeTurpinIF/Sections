#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace Sections
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Determine view family type to use

            ViewFamilyType vft
              = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault<ViewFamilyType>(x =>
                 ViewFamily.Section == x.ViewFamily);

            //XYZ b = uidoc.Selection.PickPoint("Pick point 2");

            //BoundingBoxXYZ BB = new BoundingBoxXYZ();
            //BB.Min = b;
            //BB.Max = b;


            // Modify document within a transaction

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Transaction Name");

                //Create section
                //ViewSection Sec = ViewSection.CreateSection(doc, vft.Id, BB);

                //RevitCommandId id= RevitCommandId.LookupPostableCommandId(PostableCommand.Section);
                //uiapp.PostCommand(id);
                
               // uidoc.PromptToPlaceViewOnSheet()





                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
