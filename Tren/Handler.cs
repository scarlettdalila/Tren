using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using CSGL12; //Manejador de Open GL

namespace Tren
{
    public class Handler
    {
        public double poscamara, acamara; //pocision de la camara (radio, angulo)
        public double acamarasave, dcamara; //Guarda el ultimo angulo de la camara ** incremento de angulo que he movido
        public int mouseY;
        public double trayectoria;
        public float TrenX, TrenY, TrenA, AnguloTan;
        public double PendienteTan;
        public int  Elipsea, Elipseb;

        public Handler()
        {
            poscamara = 20;
            acamara = 0 * Math.PI / 180; //Lo convierte a radianes
            dcamara = 0;
            trayectoria = 0 * Math.PI / 180;
            TrenX = TrenY = TrenA = AnguloTan = 0.0f;
            PendienteTan = 0;
            Elipsea = 8;
            Elipseb = 6;
        }

        public void dibuja_arbol(GL gl)
        {
            IntPtr qobj = gl.gluNewQuadric();
            gl.gluQuadricDrawStyle(qobj, GL.GLU_FILL);
            //Material tronco
            float[] sombra_tronco = { 0.588f, 0.2275f, 0.004f, 1.0f };
            float[] luz_tronco = { 0.643f, 0.353f, 0.016f, 1.0f };
            float[] reflejo = { 1.0f, 1.0f, 1.0f, 1.0f };//Blaco
            float[] brillo = { 0.0f, 0.0f, 0.0f, 1.0f };//Negro
            float intensidadbrillo = 40.0f;//0 - 128
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_AMBIENT, sombra_tronco); //color a la sombra
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_DIFFUSE, luz_tronco); //color a la luz
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, reflejo); //color del reflejo
            gl.glMaterialf(GL.GL_FRONT, GL.GL_SHININESS, intensidadbrillo); //intensidad del reflejo
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_EMISSION, brillo);
            //Tronco
            gl.gluCylinder(qobj, 0.5, 0.5, 0.25, 10, 10);
            //Capa uno del arbol
            float[] sombra_arbol = { 0.118f, 0.353f, 0.003f };
            float[] luz_arbol = { 0.169f, 0.518f, 0.008f };
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_AMBIENT, sombra_arbol); //color a la sombra
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_DIFFUSE, luz_arbol); //color a la luz
            gl.glTranslatef(0.0f, 0.0f, 0.25f);
            gl.gluCylinder(qobj, 1.2 , 0.9, 0.6, 10, 10);
            gl.glTranslatef(0.0f, 0.0f, 0.6f);
            gl.gluCylinder(qobj, 1.2, 0.6, 0.6, 10, 10);
            gl.glTranslatef(0.0f, 0.0f, 0.6f);
            gl.gluCylinder(qobj, 0.9, 0.3, 0.6, 10, 10);
            gl.glTranslatef(0.0f, 0.0f, 0.6f);
            gl.gluCylinder(qobj, 0.6, 0.0, 0.5, 10, 10);
        }

        public void dibuja_cuadrado(GL gl, float sizecuadro)
        {
            gl.glBegin(GL.GL_QUADS);
            gl.glVertex3f(0.0f, 0.0f, 0.0f);
            gl.glVertex3f(0.0f, -sizecuadro, 0.0f);
            gl.glVertex3f(sizecuadro, sizecuadro, 0.0f);
            gl.glVertex3f(sizecuadro, 0.0f, 0.0f);
            gl.glEnd();
        }

        public void dibuja_plano(GL gl, float sizeplano, float sizecuadro)
        {
            gl.glTranslatef(-sizeplano, -sizeplano, 0.0f);
            for (float j = 0; j < sizeplano / sizecuadro; j = j+sizecuadro)
            {
                gl.glTranslatef(0.0f, j, 0.0f);
                gl.glPushMatrix();
                for (float i = 0; i < (sizeplano / sizecuadro); i = i+sizecuadro)
                {
                    gl.glTranslatef(i, 0.0f, 0.0f);
                    dibuja_cuadrado(gl, 0.1f);
                    gl.glPopMatrix();
                }
            }
        }

        public void OpenGLStarted(CSGL12Control csgl12Control) //Metodo OpenGLStartes
        {//Se ejecuta cada vez que se inicia Open GL.

            //Extraer la clase donde esta OpenGL, yo la hago para manejar OpenGL
            GL gl = csgl12Control.GetGL();

            if (null == gl) { return; }

            gl.glEnable(GL.GL_CULL_FACE);
            gl.glCullFace(GL.GL_BACK);
            gl.glFrontFace(GL.GL_CCW);

            //Iluminación
            gl.glEnable(GL.GL_LIGHTING);
            gl.glEnable(GL.GL_LIGHT1);//Luz tren
            gl.glEnable(GL.GL_NORMALIZE);
            //Modelo de iluminación
            float[] lmKa = { 0.8f, 0.8f, 0.8f, 0.1f }; //intensidad luz ambiental (0.2, 0.2, 0.2, 1) default
            gl.glLightModelfv(GL.GL_LIGHT_MODEL_AMBIENT, lmKa); //definir luz ambiental
            gl.glLightModeli(GL.GL_LIGHT_MODEL_LOCAL_VIEWER, GL.GL_TRUE); //luz local o infinita
            gl.glLightModeli(GL.GL_LIGHT_MODEL_TWO_SIDE, GL.GL_TRUE); //iluminar una o dos caras

            gl.glShadeModel(GL.GL_SMOOTH);								// enable smooth shading
            gl.glClearColor(0.0f, 0.0f, 0.0f, 0.5f);					// black background
            gl.glClearDepth(1.0f);										// depth buffer setup
            gl.glEnable(GL.GL_DEPTH_TEST);								// enables depth testing
            gl.glDepthFunc(GL.GL_LEQUAL);								// type of depth testing
            gl.glHint(GL.GL_PERSPECTIVE_CORRECTION_HINT, GL.GL_NICEST);	// nice perspective calculations

            if (true == gl.bwglSwapIntervalEXT)
            {
                gl.wglSwapIntervalEXT(1);
            }
       }


        public void Paint(object sender, PaintEventArgs e) //Metodo Paint
        {
            if (null == sender) { return; }
            if (false == (sender is CSGL12Control)) { return; }

            //Sacar el control de GL y sus dimensiones
            CSGL12Control csgl12Control = (sender as CSGL12Control);
            GL gl = csgl12Control.GetGL();//Pedimos el control de OpenGl
                        
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

            //Asignar un viewport (el unico que maneja la vista fisica)
            gl.glViewport(0, 0, clientWidth, clientHeight); //Nos dice de que tamaño es la pantalla fisica, yo le digo la vista fisica tiene este tamaño
            
            //Limpiar la pantalla con un color de fondo
            gl.glClearColor(0.0f, 0.0f, 0.0f, 1.0f); //Limpia la pantalla blanco, trasparencia 10% opaco
            gl.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT); //Limpia el Buffer de color y de profundidad (diferente del buffer de profundidad -> que sabe que esta atras de que)

            //Asignar la vista del modelo
            gl.glMatrixMode(GL.GL_PROJECTION); //Lo que voy a hacer ahorita modifica a la proyeccion unicamente
            gl.glLoadIdentity();
            double aspectRatio = 1.0;

            if (0 != clientHeight)
            {
                aspectRatio = ((double)(clientWidth) / (double)(clientHeight));
            }

            double verticalFieldOfViewAngle = 45.0;

            gl.gluPerspective //maneja la perspectivz
            (
                verticalFieldOfViewAngle, // Field of view angle (Y angle; degrees)
                aspectRatio, // width/height
                0.1, // distance to near clipping plane
                64000.0 // distance to far clipping plane
            );

            //Movimiento circular de la camara
            gl.gluLookAt(0, poscamara * Math.Sin(acamara), poscamara * Math.Cos(acamara),
                0, 0, 0,
                0, Math.Cos(acamara), -Math.Sin(acamara));

            gl.glMatrixMode(GL.GL_MODELVIEW); //VIsta de modelo
            gl.glLoadIdentity();
                    

            //Aquí va tu dibujito chido
            gl.glPushMatrix();
            float[] sombratren = { 0.0f, 0.2f, 0.0f, 1.0f };
            float[] luztren = { 0.0f, 1.0f, 0.0f, 1.0f };
            float[] reflejotren = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] brillotren = { 0.0f, 0.0f, 0.0f, 1.0f };
            float intensidadbrillotren = 40.0f;//0 - 128
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_AMBIENT, sombratren); //color a la sombra
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_DIFFUSE, luztren); //color a la luz
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, reflejotren); //color del reflejo
            gl.glMaterialf(GL.GL_FRONT, GL.GL_SHININESS, intensidadbrillotren); //intensidad del reflejo
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_EMISSION, brillotren); //color de luz emitida
            TrenX = (float)(Elipsea*Math.Cos(trayectoria));
            TrenY = (float)(Elipseb*Math.Sin(trayectoria));
            PendienteTan = ((Elipseb*TrenX)/(Elipsea*(Math.Sqrt(Math.Pow(Elipsea,2)-Math.Pow(TrenX,2)))));
            AnguloTan = (float)(Math.Abs((Math.Atan(PendienteTan)))*(180/Math.PI));
            gl.glTranslatef(TrenX, TrenY, 0.0f);
            if (TrenX > 0 && TrenY > 0)
                TrenA = (180 - AnguloTan);
            else if (TrenX < 0 && TrenY > 0)
                TrenA = (180 + AnguloTan);
            else if (TrenX < 0 && TrenY < 0)
                TrenA = (AnguloTan * (-1));
            else if (TrenX > 0 && TrenY < 0)
                TrenA = AnguloTan;
            gl.glRotatef(TrenA, 0.0f, 0.0f, 1.0f);
            gl.glScalef(2.0f, 1.0f, 1.0f);
            GLUTShapes shape = new GLUTShapes(gl);
            shape.glutSolidCube(1);
            gl.glTranslatef(0.5f, 0.0f, 0.0f);
            float[] light1_pos = { 0.0f, 0.0f, 0.0f, 1.0f }; //x,y,z, w  w=0 luz direccional, else posicional
            float[] light_Ka = { 1.0f, 1.0f, 1.0f, 1.0f };//white light
            float[] light_Kd = { 1.0f, 1.0f, 1.0f, 1.0f };//white light
            float[] light_Ks = { 1.0f, 1.0f, 1.0f, 1.0f };//white light
            float[] light_dir = {1.0f, 0.0f, 0.0f};
            gl.glLightfv(GL.GL_LIGHT1, GL.GL_POSITION, light1_pos); //posición de la luz
            gl.glLightfv(GL.GL_LIGHT1, GL.GL_AMBIENT, light_Ka); //color luz ambiental
            gl.glLightfv(GL.GL_LIGHT1, GL.GL_DIFFUSE, light_Kd); //color de luz difusa
            gl.glLightfv(GL.GL_LIGHT1, GL.GL_SPECULAR, light_Ks); //color del brillo
            gl.glLightf(GL.GL_LIGHT1, GL.GL_SPOT_CUTOFF, 20.0f);
            gl.glLightfv(GL.GL_LIGHT1, GL.GL_SPOT_DIRECTION, light_dir);
            gl.glLightf(GL.GL_LIGHT1, GL.GL_SPOT_EXPONENT, 0.0f); //0 - 128, 0 es pareja
            gl.glPopMatrix();

            gl.glPushMatrix();
            gl.glTranslatef(0.0f, 0.0f, 0.5f);
            float[] sombratierra = { 0.545f, 0.255f, 0.005f, 1.0f };
            float[] luztierra = { 0.91f, 0.42f, 0.005f, 1.0f };
            float[] reflejotierra = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] brillotierra = { 0.0f, 0.0f, 0.0f, 1.0f };
            float intensidadbrillotierra = 40.0f;//0 - 128
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_AMBIENT, sombratierra); //color a la sombra
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_DIFFUSE, luztierra); //color a la luz
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, reflejotierra); //color del reflejo
            gl.glMaterialf(GL.GL_FRONT, GL.GL_SHININESS, intensidadbrillotierra); //intensidad del reflejo
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_EMISSION, brillotierra); //color de luz emitida
            //shape.glutSolidSphere(2, 10, 10);
            //gl.glTranslatef(0.0f, 0.0f, -0.5f);
            //dibuja_plano(gl, 15.0f, 0.1f);
            gl.glPopMatrix();

            gl.glMaterialfv(GL.GL_FRONT, GL.GL_AMBIENT, sombratren); //color a la sombra
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_DIFFUSE, luztren); //color a la luz
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, reflejotren); //color del reflejo
            gl.glMaterialf(GL.GL_FRONT, GL.GL_SHININESS, intensidadbrillotren); //intensidad del reflejo
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_EMISSION, brillotren); //color de luz emitida
            gl.glTranslatef(8.6f, 0.0f, 0.0f);
            dibuja_arbol(gl);

            trayectoria += 0.02f;
            // Forzar el dibujado de todo y cambiar el buffer de ser necesario
            gl.wglSwapBuffers(csgl12Control.GetHDC()); //Pasa del buffer a la pantalla (plancha cuando ya acabe esto) no cuando tu quieras
            //Es estoy listo cuando hagas el re-fresco para planchar en la pantalla
        }

        public void MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                mouseY = e.Y;
                acamarasave = acamara;
            }
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dcamara = (e.Y - mouseY) * (30.0 / 200.0) * Math.PI / 180; //cada 200px son 30 grados
                acamara = acamarasave + dcamara;
            }
        }

        public void MouseWheel(object sender, MouseEventArgs e)
        {
            double d = e.Delta * 0.1;
            poscamara += d;
        }

    }
}
