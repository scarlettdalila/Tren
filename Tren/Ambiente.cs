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
    public class Ambiente
    {

        GL gl;
        GLUTShapes shapes1;
       
        public Ambiente(GL gl)
        {
            this.gl = gl; 
            shapes1 = new GLUTShapes(gl);
        }

        public void DibujaTodo()
        {
            DibujaSuelo();
        }

        public void DibujaSuelo()
        {

            float[] Sombra = { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] Luz = { 0.3f, 0.3f, 0.2f, 1.0f };
            float[] Reflejo = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] Brillo = { 0.0f, 0.0f, 0.0f, 1.0f };

            float IntensidadBrillo = 5.0f;//0 - 128

            gl.glMaterialfv(GL.GL_FRONT, GL.GL_AMBIENT, Sombra); //color a la sombra
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_DIFFUSE, Luz); //color a la luz
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, Reflejo); //color del reflejo
            gl.glMaterialf(GL.GL_FRONT, GL.GL_SHININESS, IntensidadBrillo); //intensidad del reflejo
            gl.glMaterialfv(GL.GL_FRONT, GL.GL_EMISSION, Brillo); //color de luz emitida


            gl.glBegin(GL.GL_QUADS);
            gl.glVertex3f(-100.0f, 0.0f, -100.0f);
            gl.glVertex3f(-100.0f, 0.0f, 100.0f);
            gl.glVertex3f(100.0f, 0.0f, 100.0f);
            gl.glVertex3f(100.0f, 0.0f, -100.0f);            
           
            gl.glEnd();
        }

        public void DibujaArboles()
        {
 
        }

    }
}
