using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using ComboBox = Autodesk.Revit.UI.ComboBox;
using Form = System.Windows.Forms.Form;


namespace Kursach
{
    public partial class Calculating : Form
    {
        private ExternalEvent f_ExEvent;
        private CommandHandler f_Handler;
        private List<Room> f_roomList;
        private string[] f_typesArray = new string[] {"Стены", "Пол", "Потолок"};


        public Calculating(ExternalEvent exEvent, CommandHandler handler, List<Room> roomList)
        {
            InitializeComponent();
            f_ExEvent = exEvent;
            f_Handler = handler;
            f_roomList = roomList;
            comboBox1.Items.AddRange(Utils.GetRoomNames(f_roomList).ToArray());
            comboBox2.Items.AddRange(f_typesArray);
            textBox1.ReadOnly = true;
            
        }

        


        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            f_ExEvent.Dispose();
            f_ExEvent = null;
            f_Handler = null;

            base.OnFormClosed(e);
        }
        


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            try
            {
                switch (comboBox2.SelectedItem.ToString().Trim())
                {
                    case "Стены":

                        try
                        {
                            Room room = Utils.GetRoomByName(f_roomList, comboBox1.Text);
                            List<Wall> walls = Utils.GetWallsInRoom(Utils.GetRoomByName(f_roomList, comboBox1.Text));
                            Random rand = new Random();
                            int item = rand.Next(0,walls.Count - 1);
                            List<Material> materials = new List<Material>();
                            foreach (ElementId mat_id in walls[item].GetMaterialIds(false))
                            {
                                materials.Add(room.Document.GetElement(mat_id) as Material);
                            }

                            foreach (var material in materials)
                            {
                                comboBox3.Items.Add(material.Name);
                            }
                            
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show($"{exception.Message}");
                        }
                        
                        break;

                    case "Пол":
                        try
                        {
                            Room room = Utils.GetRoomByName(f_roomList, comboBox1.Text);
                            MessageBox.Show("Выберите пол в комнате");
                            Floor floor = room.Document.GetElement(Utils.GetPickedElement(room)) as Floor;
                            if (floor == null)
                            {
                                MessageBox.Show("Пожалуйста, выберите пол");
                                break;
                            }
                            List<Material> materials = new List<Material>();
                            foreach (ElementId mat_id in floor.GetMaterialIds(false))
                            {
                                materials.Add(room.Document.GetElement(mat_id) as Material);
                            }

                            foreach (var material in materials)
                            {
                                comboBox3.Items.Add(material.Name);
                            }

                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show($"{exception.Message}");
                        }
                        break;

                    case "Потолок":
                        try
                        {
                            Room room = Utils.GetRoomByName(f_roomList, comboBox1.Text);
                            MessageBox.Show("Выберите потолок в комнате");
                            Floor floor = room.Document.GetElement(Utils.GetPickedElement(room)) as Floor;
                            if (floor == null)
                            {
                                MessageBox.Show("Пожалуйста, выберите потолок");

                                break;
                            }
                            List<Material> materials = new List<Material>();
                            foreach (ElementId mat_id in floor.GetMaterialIds(false))
                            {
                                materials.Add(room.Document.GetElement(mat_id) as Material);
                            }

                            foreach (var material in materials)
                            {
                                comboBox3.Items.Add(material.Name);
                            }

                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show($"{exception.Message}");
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            
                
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private void okbtn_Click(object sender, EventArgs e)
        {
            


      

            try
            {
                switch (comboBox2.SelectedItem.ToString().Trim())
                {
                    case "Стены":

                        try
                        {
                            Room room = Utils.GetRoomByName(f_roomList, comboBox1.Text);
                            double materialCount;
                            if (double.TryParse(textBox2.Text,out materialCount))
                            {
                                double result =
                                    Math.Ceiling(room.Perimeter * room.UnboundedHeight / (materialCount * 32));
                                textBox1.Text = result.ToString();
                            }
                            else
                            {
                                MessageBox.Show("Введите корректное значение количества материалов");
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show($"{exception.Message}");
                        }

                        break;

                    case "Пол":
                        try
                        {
                            Room room = Utils.GetRoomByName(f_roomList, comboBox1.Text);
                            double materialCount;
                            if (double.TryParse(textBox2.Text, out materialCount))
                            {
                                double result =
                                    Math.Ceiling(room.Area/ (materialCount * 32));
                                textBox1.Text = result.ToString();
                            }
                            else
                            {
                                MessageBox.Show("Введите корректное значение количества материалов");
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show($"{exception.Message}");
                        }
                        break;

                    case "Потолок":
                        try
                        {
                            Room room = Utils.GetRoomByName(f_roomList, comboBox1.Text);
                            double materialCount;
                            if (double.TryParse(textBox2.Text, out materialCount))
                            {
                                double result =
                                    Math.Ceiling(room.Area / (materialCount * 32));
                                textBox1.Text = result.ToString();
                            }
                            else
                            {
                                MessageBox.Show("Введите корректное значение количества материалов");
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show($"{exception.Message}");
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

        }

        private void cnlbtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
