#region Namespaces
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
#endregion

namespace Sections
{


    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class App : IExternalApplication
    {
        private Form1 MyForm;

        public Result OnStartup(UIControlledApplication a)
        {
            // Add the UI buttons on start up
            RibbonPanel panel = a.CreateRibbonPanel(Tab.AddIns, "Section Naming");

            string path = GetType().Assembly.Location;
            string sDirectory = System.IO.Path.GetDirectoryName(path);

            // create toggle buttons for radio button group 
            ToggleButtonData toggleButtonData1 = new ToggleButtonData("SecOff", "Section Off", path, "Sections.UIDynamicModelUpdateOff");
            ToggleButtonData toggleButtonData2 = new ToggleButtonData("SecOn", "Section On", path, "Sections.UIDynamicModelUpdateOn");

            RadioButtonGroupData radioBtnGroupData = new RadioButtonGroupData("SectionTool");

            RadioButtonGroup radioBtnGroup = panel.AddItem(radioBtnGroupData) as RadioButtonGroup;
            radioBtnGroup.AddItem(toggleButtonData1);
            radioBtnGroup.AddItem(toggleButtonData2);


            SectionUpdater updater = new SectionUpdater(a.ActiveAddInId);

            // Register the updater in the singleton 
            // UpdateRegistry class
            UpdaterRegistry.RegisterUpdater(updater);

            // Set the filter
            ElementCategoryFilter filter
              = new ElementCategoryFilter(
                BuiltInCategory.OST_Views);

            // Add trigger 
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeElementAddition());

            return Result.Succeeded;


        }

        public Result OnShutdown(UIControlledApplication a)
        {
            SectionUpdater updater = new SectionUpdater(a.ActiveAddInId);

            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());

            return Result.Succeeded;
        }

    }

    public class SectionUpdater : IUpdater
    {
        public static bool m_updateActive = false;
        AddInId addinID = null;
        UpdaterId updaterID = null;

        public Form1 MyForm { get; private set; }

        public SectionUpdater(AddInId id)
        {
            addinID = id;
            // UpdaterId that is used to register and 
            // unregister updaters and triggers
            updaterID = new UpdaterId(addinID, new Guid("3053d5de-ae95-4f92-ba65-04d4a6b675a7"));
        }

        public void Execute(UpdaterData data)
        {
            if (m_updateActive == false) { return; }

            // Get access to document object
            Document doc = data.GetDocument();
            ElementId id = data.GetAddedElementIds().First();
            View v = doc.GetElement(id) as View;
            if (v.ViewType != ViewType.Section) { return; }
            Appdata.theview = v;

            Transactionevent handler = new Transactionevent();

            ExternalEvent exEvent = ExternalEvent.Create(handler);

            FilteredElementCollector fec = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet));
            MyForm = new Form1(doc, fec, exEvent, handler);
            MyForm.ShowDialog();

        }

        public string GetAdditionalInformation()
        {
            return "Automatically create sections";
        }

        public string GetUpdaterName()
        {
            return "View Updater";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Views;
        }

        public UpdaterId GetUpdaterId()
        {
            return updaterID;
        }
    }
    public class Appdata
    {
        public static View theview { get; set; }
        public static ViewSheet thesheet { get; set; }
        public static string viewname { get; set; }
        public static string detailnum { get; set; }
    }

    public class Transactionevent : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            Document doc = app.ActiveUIDocument.Document;

            ViewSheet vs = Appdata.thesheet;

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Section Tool");
                View v = Appdata.theview;
                if (Appdata.viewname != "")
                {
                    v.Name = Appdata.viewname;
                }

                if (vs != null)
                {
                    XYZ point = new XYZ();
                    Viewport vp = Viewport.Create(doc, vs.Id, v.Id, point);

                    if (Appdata.detailnum != "")
                    {
                        vp.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER).Set(Appdata.detailnum);
                    }
                }

                tx.Commit();
            }
        }

        public string GetName()
        {
            return "Transaction Event";
        }
    }
}
