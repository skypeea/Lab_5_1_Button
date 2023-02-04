using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.VisualStudio.PlatformUI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelegateCommand = Prism.Commands.DelegateCommand;
using Document = Autodesk.Revit.DB.Document;

namespace Lab_5_1_Button
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand PipeCountCommand { get; }
        public DelegateCommand WallVolumeCommand { get; }
        public DelegateCommand DoorsCountCommand { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            PipeCountCommand = new DelegateCommand(OnPipeCountCommand);
            WallVolumeCommand = new DelegateCommand(OnWallVolumeCommand);
            DoorsCountCommand = new DelegateCommand(OnDoorsCountCommand);
        }

        public event EventHandler HideRequest;
        private void RaiseCloseRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShowRequest;
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnPipeCountCommand()
        {
            RaiseCloseRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var pipeList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .Cast<Pipe>()
                .ToList();

            TaskDialog.Show("Сообщение", $"Труб в модели: {pipeList.Count}");
            RaiseShowRequest();
        }

        private void OnWallVolumeCommand()
        {
            RaiseCloseRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var wallList = new FilteredElementCollector(doc) //получение всех стен в модели
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .Cast<Wall>()
                .ToList();

            double wallsVolume = 0; //переменная для хранения объема стен
            foreach (Wall wall in wallList)
            {
                Parameter volume = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                wallsVolume += volume.AsDouble();
            }
           
            TaskDialog.Show("Сообщение", $"Объем всех стен в модели: {UnitUtils.ConvertFromInternalUnits(wallsVolume, UnitTypeId.CubicMeters)} м3");

            RaiseShowRequest();
        }

        private void OnDoorsCountCommand()
        {
            RaiseCloseRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var doorsList = new FilteredElementCollector(doc) 
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            TaskDialog.Show("Сообщение", $"Количество дверей в модели: {doorsList.Count}");

            RaiseShowRequest();
        }

    }
}
