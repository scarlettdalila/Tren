using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using CSGL12;

namespace Tren
{
    public class Handler
    {
        public double poscamara, acamara; // posicion de la camara (radio, angulo)
        public double acamarasave, dcamara; // angulo de camara antes de mouse down, incremento del angulo de la camara
        public int mouseY; //posición del mouse al hacer mouse down

        public float tren1x;
        public float tren2x;
        public float tren3x;

        public Handler()
        {
            poscamara = 20;
            acamara = -0 * Math.PI/180; //para definirlo en grados
            dcamara = 0;
            tren1x = 0.0f;
            tren2x = 0.0f;
            tren3x = 0.0f;
        }

        public void OpenGLStarted(CSGL12Control csgl12Control)
        {
            GL gl = csgl12Control.GetGL();

            if (null == gl) { return; }

            // Basic drawing conditions

           
            gl.glEnable(GL.GL_CULL_FACE);
            gl.glCullFace(GL.GL_BACK);
            gl.glFrontFace(GL.GL_CCW);
           
            //Activar iluminación
            gl.glEnable(GL.GL_LIGHTING); 
            gl.glEnable(GL.GL_LIGHT0); //Luz del tren
            gl.glEnable(GL.GL_NORMALIZE); //garantizar vectores de luz apropiados
            //Modelo de iluminación
            float[] lmKa = { 0.2f, 0.2f, 0.0f, 0.1f }; //intensidad luz ambiental (0.2, 0.2, 0.2, 1) default
            gl.glLightModelfv(GL.GL_LIGHT_MODEL_AMBIENT, lmKa); //definir luz ambiental
            gl.glLightModeli(GL.GL_LIGHT_MODEL_LOCAL_VIEWER, GL.GL_TRUE); //luz local o infinita
            gl.glLightModeli(GL.GL_LIGHT_MODEL_TWO_SIDE, GL.GL_FALSE); //iluminar una o dos caras
            //Luz del tren
            float[] light_pos = { 0.0f, 0.0f, 0.0f, 0.0f };//direccional
            float[] light_amb = { 1.0f, 1.0f, 1.0f, 1.0f };//white light
            float[] light_dif = { 1.0f, 1.0f, 1.0f, 1.0f };//white light
            float[] light_spe = { 1.0f, 1.0f, 1.0f, 1.0f };//white light
            float[] light_dir = { 1.0f, 0.0f, 0.0f};
            gl.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, light_pos); //posición de la luz
            gl.glLightfv(GL.GL_LIGHT0, GL.GL_AMBIENT, light_amb); //color luz ambiental
            gl.glLightfv(GL.GL_LIGHT0, GL.GL_DIFFUSE, light_dif); //color de luz difusa
            gl.glLightfv(GL.GL_LIGHT0, GL.GL_SPECULAR, light_spe); //color de brillo
            gl.glLightf(GL.GL_LIGHT0, GL.GL_SPOT_CUTOFF, 15.0f); //apertura de la luz
            gl.glLightfv(GL.GL_LIGHT0, GL.GL_SPOT_DIRECTION, light_dir);//direccion de la luz
            gl.glLightf(GL.GL_LIGHT0, GL.GL_SPOT_EXPONENT, 50.0f); //intensidad de la luz
            gl.glLightf(GL.GL_LIGHT0, GL.GL_LINEAR_ATTENUATION, 5.0f); //atenuacion de la luz
            

            gl.glShadeModel(GL.GL_SMOOTH);								// enable smooth shading
            gl.glClearColor(0.0f, 0.0f, 0.0f, 0.5f);					// black background
            gl.glClearDepth(1.0f);										// depth buffer setup
            gl.glEnable(GL.GL_DEPTH_TEST);								// enables depth testing
            gl.glDepthFunc(GL.GL_LEQUAL);								// type of depth testing
            gl.glHint(GL.GL_PERSPECTIVE_CORRECTION_HINT, GL.GL_NICEST);	// nice perspective calculations

            // evitar parpadeos
            if (true == gl.bwglSwapIntervalEXT)
            {
                gl.wglSwapIntervalEXT(1);
            }
        }


        public void Vagon(GL gl)
        {
            float[] sombra = { 0.0f, 0.0f, 0.2f, 1.0f };
            float[] luz = { 0.0f, 0.0f, 1.0f, 1.0f };
            float[] reflejo = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] brillo = { 0.0f, 0.0f, 0.0f, 1.0f };
            float intensidadbrillo = 40.0f;//0 - 128
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_AMBIENT, sombra); //color a la sombra
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_DIFFUSE, luz); //color a la luz
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, reflejo); //color del reflejo
            gl.glMaterialf(GL.GL_FRONT, GL.GL_SHININESS, intensidadbrillo); //intensidad del reflejo
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_EMISSION, brillo); //color de luz emitida
            GLUTShapes obj = new GLUTShapes(gl);
            gl.glScalef(2.0f, 0.0f, 0.0f);
            obj.glutSolidCube(1.0);
        }

        public void Paint(object sender, PaintEventArgs e)
        {
            if (null == sender) { return; }
            if (false == (sender is CSGL12Control)) { return; }

            //Sacar el control de GL y sus dimensiones
            CSGL12Control csgl12Control = (sender as CSGL12Control);
            GL gl = csgl12Control.GetGL();

            int clientWidth = csgl12Control.ClientRectangle.Width;
            int clientHeight = csgl12Control.ClientRectangle.Height;

            if (clientWidth <= 0)
            {
                clientWidth = 1;
            }

            if (clientHeight <= 0)
            {
                clientHeight = 1;
            }

            //Asignar un viewport  del tamaño del control
            gl.glViewport(0, 0, clientWidth, clientHeight);

            //Limpiar la pantalla con un color de fondo
            gl.glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);

          
            //Asignar la vista del modelo y la proyeccion
            gl.glMatrixMode(GL.GL_PROJECTION);
            gl.glLoadIdentity();
            
            double aspectRatio = 1.0;

            if (0 != clientHeight)
            {
                aspectRatio = ((double)(clientWidth) / (double)(clientHeight));
            }

            double verticalFieldOfViewAngle = 45.0;

            gl.gluPerspective
            (
                verticalFieldOfViewAngle, // Field of view angle (Y angle; degrees)
                aspectRatio, // width/height
                0.1, // distance to near clipping plane
                64000.0 // distance to far clipping plane
            );

            gl.gluLookAt(0, poscamara*Math.Sin(acamara), poscamara*Math.Cos(acamara), // pos camara
                         0, 0, 0, // pos objetivo
                         0, Math.Cos(acamara), -Math.Sin(acamara)); // orientación UP

            gl.glMatrixMode(GL.GL_MODELVIEW);
            gl.glLoadIdentity();

                
            //Aquí va tu dibujito chido
            //Espacio para guardar las mallas de las esferas
            gl.glTranslatef(0.0f, 0.0f, -10.0f);
            GLUTShapes obj = new GLUTShapes(gl);
            gl.glScalef(2.0f, 0.0f, 0.0f);
            gl.glColor3f(1.0f, 0.0f, 0.0f);
            obj.glutSolidCube(5.0);
            Vagon(gl);

            // Forzar el dibujado de todo y cambiar el buffer de ser necesario
            gl.wglSwapBuffers(csgl12Control.GetHDC());
        }
        
    }
}

