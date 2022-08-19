using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sections
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private FilteredElementCollector ffec;
        private ExternalEvent ExEvent;
        private Transactionevent Handler;

        public Form1(Document doc, FilteredElementCollector fec, ExternalEvent exEvent, Transactionevent handler)
        {
            InitializeComponent();
            ExEvent = exEvent;
            Handler = handler;
            ffec = fec;
            getsheets(doc);
        }

            private void button1_Click(object sender, EventArgs e)
        {
            Appdata.thesheet = Getview();
            Appdata.viewname = textBox3.Text;
            Appdata.detailnum = textBox1.Text;
            ExEvent.Raise();
            this.Close();
        }

        private void getsheets(Document doc)
        {
            foreach (ViewSheet vs in ffec)
            {
                listBox1.Items.Add(vs.SheetNumber);
            }
        }

        public ViewSheet Getview()
        {
        foreach (ViewSheet vs in ffec)
            {
                if (vs.SheetNumber == listBox1.SelectedItem.ToString())
                {
                    return vs;
                }
            }
            return null;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked == true)
            {
                textBox1.Enabled = true;
                listBox1.Enabled = true;
                textBox1.ReadOnly = false;
            }
            else
            {
                textBox1.Enabled = false;
                listBox1.Enabled = false;
                textBox1.ReadOnly = true;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
