using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSGL12;

namespace Tren
{
    public partial class Form1 : Form
    {
        public Handler handler;
        private System.Windows.Forms.Timer timer;

        public Form1()
        {
            InitializeComponent();
            //Registrar los manejadores de eventos: GL y otros eventos de perifericos
            handler = new Handler();
            csgL12Control1.OpenGLStarted += new CSGL12Control.DelegateOpenGLStarted(handler.OpenGLStarted);
            csgL12Control1.Paint += new PaintEventHandler(handler.Paint);
            csgL12Control1.MouseDown += new MouseEventHandler(handler.MouseDown);
            csgL12Control1.MouseMove += new MouseEventHandler(handler.MouseMove);
            csgL12Control1.MouseWheel += new MouseEventHandler(handler.MouseWheel);
            /* Mas eventos que puedo manejar , solo agregarlos al Handler */
            
            //Timer para animación
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 10; // 10 ms... suficiente para lograr hasta 70fps
            timer.Tick += new EventHandler(timerHandler);
            timer.Start();
            
        }

        void timerHandler(object sender, EventArgs e)
        {
            if (false == DesignMode)
            {
                csgL12Control1.Invalidate(); //obligar a redibujar!!!!
            }
        }
    }
}
