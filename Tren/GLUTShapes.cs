using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSGL12;

namespace Tren
{
    /* ADD IN para Implementacion de las figuras basicas de GLUT
     * PAra usarse con CSGL
     * Raul Trejo
     * Basado en la version 3.7 de GLUT de
     * Copyright (c) Mark J. Kilgard, 1994, 1997. */

    public class GLUTShapes
    {
        private GL gl;

        public GLUTShapes(GL gl)
        {
            this.gl = gl; 
        }

        
		public  void drawBox(float size, int type)
		{
		    float [][]n= new float[6][] 
	        {
	            new float[] {-1.0f, 0.0f, 0.0f},
	            new float[] {0.0f, 1.0f, 0.0f},
	            new float[] {1.0f, 0.0f, 0.0f},
	            new float[] {0.0f, -1.0f, 0.0f},
	            new float[] {0.0f, 0.0f, 1.0f},
	            new float[] {0.0f, 0.0f, -1.0f}
	        };
            int [][]faces =new int[6][]
            {
	            new int[] {0, 1, 2, 3},
	            new int[] {3, 2, 6, 7},
	            new int[] {7, 6, 5, 4},
	            new int[] {4, 5, 1, 0},
	            new int[] {5, 6, 2, 1},
	            new int[] {7, 4, 0, 3}
            };
            float [][]v = new float[8][];
            for (int i = 0; i < 8; i++)
                v[i] = new float[3];
           

            v[0][0] = v[1][0] = v[2][0] = v[3][0] = -size / 2;
            v[4][0] = v[5][0] = v[6][0] = v[7][0] = size / 2;
            v[0][1] = v[1][1] = v[4][1] = v[5][1] = -size / 2;
            v[2][1] = v[3][1] = v[6][1] = v[7][1] = size / 2;
            v[0][2] = v[3][2] = v[4][2] = v[7][2] = -size / 2;
            v[1][2] = v[2][2] = v[5][2] = v[6][2] = size / 2;

            for (int i = 5; i >= 0; i--) 
            {
	            gl.glBegin(GL.GL_QUADS);
	            gl.glNormal3fv(n[i]);
                gl.glVertex3fv(v[faces[i][0]]);
                gl.glVertex3fv(v[faces[i][1]]);
                gl.glVertex3fv(v[faces[i][2]]);
                gl.glVertex3fv(v[faces[i][3]]);
	            gl.glEnd();
            }
        }

        public void glutSolidCube(double size)
        {
            drawBox((float)size, GL.GL_QUADS);
        }

        public void glutSolidSphere(double radius, int slices, int stacks)
        {
            IntPtr quadObj = gl.gluNewQuadric();
            gl.gluQuadricDrawStyle(quadObj, GL.GLU_FILL);
            gl.gluQuadricNormals(quadObj, GL.GLU_SMOOTH);
            /* If we ever changed/used the texture or orientation state
            of quadObj, we'd need to change it to the defaults here
            with gluQuadricTexture and/or gluQuadricOrientation. */
            gl.gluSphere(quadObj, radius, slices, stacks);
        }


        public void doughnut(float r, float R, int nsides, int rings)
        {
          int i, j;
          float theta, phi, theta1;
          float cosTheta, sinTheta;
          float cosTheta1, sinTheta1;
          float ringDelta, sideDelta;

          ringDelta = (float)( 2.0 * Math.PI / rings);
          sideDelta = (float)(2.0 * Math.PI / nsides);

          theta = 0.0f;
          cosTheta = 1.0f;
          sinTheta = 0.0f;
          for (i = rings - 1; i >= 0; i--) {
            theta1 = theta + ringDelta;
            cosTheta1 = (float)Math.Cos(theta1);
            sinTheta1 = (float)Math.Sin(theta1);
            gl.glBegin(GL.GL_QUAD_STRIP);
            phi = 0.0f;
            for (j = nsides; j >= 0; j--) {
              float cosPhi, sinPhi, dist;

              phi += sideDelta;
              cosPhi = (float) Math.Cos(phi);
              sinPhi = (float) Math.Sin(phi);
              dist = R + r * cosPhi;

              gl.glNormal3f(cosTheta1 * cosPhi, -sinTheta1 * cosPhi, sinPhi);
              gl.glVertex3f(cosTheta1 * dist, -sinTheta1 * dist, r * sinPhi);
              gl.glNormal3f(cosTheta * cosPhi, -sinTheta * cosPhi, sinPhi);
              gl.glVertex3f(cosTheta * dist, -sinTheta * dist,  r * sinPhi);
            }
            gl.glEnd();
            theta = theta1;
            cosTheta = cosTheta1;
            sinTheta = sinTheta1;
          }
        }



        public void glutSolidTorus(double innerRadius, double outerRadius,
             int nsides, int rings)
        {
            doughnut((float)innerRadius, (float)outerRadius, nsides, rings);
        }

    }
}
