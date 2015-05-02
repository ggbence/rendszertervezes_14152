namespace unoClientForm
{
    partial class form_main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.text_console = new System.Windows.Forms.TextBox();
            this.text_command = new System.Windows.Forms.TextBox();
            this.but_send = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // text_console
            // 
            this.text_console.Location = new System.Drawing.Point(12, 12);
            this.text_console.Multiline = true;
            this.text_console.Name = "text_console";
            this.text_console.ReadOnly = true;
            this.text_console.Size = new System.Drawing.Size(592, 450);
            this.text_console.TabIndex = 0;
            // 
            // text_command
            // 
            this.text_command.Location = new System.Drawing.Point(13, 469);
            this.text_command.Name = "text_command";
            this.text_command.Size = new System.Drawing.Size(487, 20);
            this.text_command.TabIndex = 1;
            this.text_command.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_command_KeyDown);
            // 
            // but_send
            // 
            this.but_send.Location = new System.Drawing.Point(507, 469);
            this.but_send.Name = "but_send";
            this.but_send.Size = new System.Drawing.Size(97, 23);
            this.but_send.TabIndex = 2;
            this.but_send.Text = "send";
            this.but_send.UseVisualStyleBackColor = true;
            this.but_send.Click += new System.EventHandler(this.but_send_Click);
            // 
            // form_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 517);
            this.Controls.Add(this.but_send);
            this.Controls.Add(this.text_command);
            this.Controls.Add(this.text_console);
            this.Name = "form_main";
            this.Text = "UnoClient";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox text_console;
        private System.Windows.Forms.TextBox text_command;
        private System.Windows.Forms.Button but_send;
    }
}

