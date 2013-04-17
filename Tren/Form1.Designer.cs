namespace Tren
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.csgL12Control1 = new CSGL12.CSGL12Control();
            this.SuspendLayout();
            // 
            // csgL12Control1
            // 
            this.csgL12Control1.Location = new System.Drawing.Point(2, 2);
            this.csgL12Control1.Name = "csgL12Control1";
            this.csgL12Control1.Size = new System.Drawing.Size(688, 490);
            this.csgL12Control1.TabIndex = 0;
            this.csgL12Control1.Text = "csgL12Control1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 494);
            this.Controls.Add(this.csgL12Control1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private CSGL12.CSGL12Control csgL12Control1;
    }
}

