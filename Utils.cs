using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace Kursach
{
    public static class Utils
    {
        public static Room GetRoomByName(List<Room> roomList, string roomName)
        {
            List<string> roomNames = GetRoomNames(roomList);
            Room[] roomArray = roomList.ToArray();
            string[] roomNamesArray = roomNames.ToArray();
            foreach (var room in roomList)
            {
                if (room.Name == roomName)
                {
                    return room;
                }
            }

            throw new Exception();
        }

        public static List<string> GetRoomNames<T>(List<T> roomList) where T : Room
        {
            return (from Room room in roomList select room.Name).ToList();
        }

        public static void createWallsFromLines(Document doc)
        {

           // FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(new ElementClassFilter(typeof(CurveElement)));

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("CreateWall");
                ICollection<Element> levels = getLevels(doc);

                foreach (Element el in levels)
                {
                    FilteredElementCollector collector = new FilteredElementCollector(doc);
                    FilteredElementCollector lines = collector.OfClass(typeof(CurveElement));
                    foreach (Element line in lines)
                    {
                        CurveElement curveElem = line as CurveElement;
                        if (line == null || curveElem.SketchPlane.Name != el.Name) continue;
                        createWall(line, doc, el.Id);
                    }
                }
                tx.Commit(); 
           
            }
        }



        public static void createWall(Element line  , Document doc ,ElementId levelId)
        {
            ModelLine ml = line as ModelLine;
            Line geomLine = Line.CreateBound(ml.GeometryCurve.GetEndPoint(0), ml.GeometryCurve.GetEndPoint(1));
            Wall wall = Wall.Create(doc, geomLine, levelId, false);
        }

        public static ICollection<Element> getLevels( Document doc)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(Level)).ToElements();
        }


        public static IList<IList<BoundarySegment>> GetFurniture(Room room)
        {
            BoundingBoxXYZ bb = room.get_BoundingBox(null);
            Outline outline = new Outline(bb.Min, bb.Max);
            bb.Transform.ScaleBasis(1.2);
            BoundingBoxIntersectsFilter filter
                = new BoundingBoxIntersectsFilter(outline);

            Document doc = room.Document;

        

            FilteredElementCollector collector
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilyInstance))
                    .WhereElementIsElementType()

                    .WherePasses(filter);

            string roomname = room.Name;


            var result = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
            return result;
        }



        public static List<Wall> GetWallsInRoom(Room room)
        {
            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            options.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish;
            List<Wall> result = new List<Wall>();
            foreach (IList<BoundarySegment> boundSegList in room.GetBoundarySegments(options))
            {
                foreach (BoundarySegment boundSeg in boundSegList)
                {
                    ElementId elem = boundSeg.ElementId;
                    Document doc = room.Document;
                    Element e = doc.GetElement(elem);
                    Wall wall = e as Wall;
                    try
                    {
                        LocationCurve locationCurve = wall?.Location as LocationCurve;
                        Curve curve = locationCurve?.Curve;
                        result.Add((Wall) e);
                    }
                    catch (Exception exception)
                    {
                        TaskDialog.Show("Revit Exception", "Неверно создана комната или стена");
                        throw;
                    }
                }
            }

            return result;

        }

        public static ElementId GetPickedElement(Room room)
        {
            try
            {
                ElementId elementId = null;
                Document doc = room.Document;
                UIDocument uidoc = new UIDocument(doc);
                elementId = uidoc.Selection
                    .PickObject(ObjectType.Element, "Выберите необходимый " +
                                                    "вам элемент или нажмите ESC").ElementId;
                Element elem = doc.GetElement(elementId);
                return elementId;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Revit", $"{e.Message}");
                return null;
            }
        }
        
    }
}
