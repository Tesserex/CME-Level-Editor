using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CodeProject
{
    public partial class GraphicalOverlay : Component
    {
        public event EventHandler<PaintEventArgs> Paint;
        private Form form;
        private List<Control> controlList = null;

        public GraphicalOverlay()
        {
            InitializeComponent();
        }

        public GraphicalOverlay(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private List<Control> ControlList
        {
            get
            {
                if (controlList == null)
                {
                    controlList = new List<Control>();

                    Control control = form.GetNextControl(form, true);

                    while (control != null)
                    {
                        controlList.Add(control);
                        control = form.GetNextControl(control, true);
                    }

                    controlList.Add(form);
                }

                return controlList;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Form Owner
        {
            get { return form; }
            set 
            {
                if (form != null)
                    form.Resize -= new EventHandler(Form_Resize);

                form = value;
                form.Resize += new EventHandler(Form_Resize);

                form.ControlAdded += new ControlEventHandler(form_ControlAdded);

                foreach (Control control in ControlList)
                    control.Paint += new PaintEventHandler(Control_Paint);
            }
        }

        void form_ControlAdded(object sender, ControlEventArgs e)
        {
            Add(e.Control);
        }

        public void Invalidate()
        {
            foreach (Control control in ControlList)
                control.Invalidate();
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        public void Add(Control control)
        {
            controlList.Add(control);
            control.Paint += new PaintEventHandler(Control_Paint);
        }

        private void Control_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(sender, e);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (Paint != null)
                Paint(sender, e);
        }
    }
}

//namespace System.Windows.Forms
//{
//    public static class Extensions
//    {
//        public static Rectangle Coordinates(this Control control)
//        {
//            Rectangle coordinates;
//            Form form = (Form)control.TopLevelControl;

//            if (control == form)
//                coordinates = form.ClientRectangle;
//            else
//                coordinates = form.RectangleToClient(control.Parent.RectangleToScreen(control.Bounds));

//            return coordinates;
//        }

//        public static IEnumerable<Control> ControlList(this Form form)
//        {
//            Control control = form.GetNextControl(form, true);

//            while(control != null)
//            {
//                yield return control;
//                control = form.GetNextControl(control, true);
//            }

//            yield return form;
//        }
//    }
//}
