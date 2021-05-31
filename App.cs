#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB.Architecture;

#endregion

namespace Kursach
{
    class App : IExternalApplication
    {
        public static App thisApp = null;
        private Calculating formDialog;

        public Result OnStartup(UIControlledApplication app)
        {
            formDialog = null;
            thisApp = this;

            string tabName = "���������� ���� �� ������";
            string panelName = "���������";
            app.CreateRibbonTab(tabName);
            var panel = app.CreateRibbonPanel(tabName, panelName);
            var NewButton = new PushButtonData("�������", "�������", Assembly.GetExecutingAssembly().Location, "Kursach.Command");
            var NewButon = panel.AddItem(NewButton) as PushButton;

            Image img = Properties.Resources.icon;
            ImageSource imgScr = Convert(img);
            NewButon.LargeImage = imgScr;
            NewButon.Image = imgScr;

            return Result.Succeeded;
        }


        public BitmapImage Convert(Image img)
        {
            using (var memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Jpeg);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        public Result OnShutdown(UIControlledApplication a)
        {
            if (formDialog != null && formDialog.Visible)
            {
                formDialog.Close();
            }

            return Result.Succeeded;
        }

        public void ShowForm(UIApplication uiapp, List<Room> roomList)
        {
            if (formDialog == null || formDialog.IsDisposed)
            {
                CommandHandler handler = new CommandHandler();
                ExternalEvent exEvent = ExternalEvent.Create(handler);

                formDialog = new Calculating(exEvent, handler, roomList);
                formDialog.Show();
            }
        }
    }
}
