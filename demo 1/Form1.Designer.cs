namespace demo_1
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddSelected = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.listViewSelectedApps = new System.Windows.Forms.ListView();
            this.imageListApps = new System.Windows.Forms.ImageList(this.components);
            this.listViewAllApps = new System.Windows.Forms.ListView();
            this.selectedAppsLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(636, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(508, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select applications to start on boot";
            // 
            // btnAddSelected
            // 
            this.btnAddSelected.Location = new System.Drawing.Point(65, 910);
            this.btnAddSelected.Name = "btnAddSelected";
            this.btnAddSelected.Size = new System.Drawing.Size(161, 78);
            this.btnAddSelected.TabIndex = 1;
            this.btnAddSelected.Text = "Add";
            this.btnAddSelected.UseVisualStyleBackColor = true;
            this.btnAddSelected.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(281, 910);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(161, 78);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRestart
            // 
            this.btnRestart.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestart.Location = new System.Drawing.Point(1628, 904);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(161, 78);
            this.btnRestart.TabIndex = 4;
            this.btnRestart.Text = "Restart";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // btnRemoveSelected
            // 
            this.btnRemoveSelected.Location = new System.Drawing.Point(667, 916);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(161, 78);
            this.btnRemoveSelected.TabIndex = 5;
            this.btnRemoveSelected.Text = "Remove";
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            // 
            // listViewSelectedApps
            // 
            this.listViewSelectedApps.CheckBoxes = true;
            this.listViewSelectedApps.HideSelection = false;
            this.listViewSelectedApps.Location = new System.Drawing.Point(1024, 278);
            this.listViewSelectedApps.Name = "listViewSelectedApps";
            this.listViewSelectedApps.Size = new System.Drawing.Size(774, 568);
            this.listViewSelectedApps.SmallImageList = this.imageListApps;
            this.listViewSelectedApps.TabIndex = 6;
            this.listViewSelectedApps.UseCompatibleStateImageBehavior = false;
            this.listViewSelectedApps.View = System.Windows.Forms.View.Details;
            // 
            // imageListApps
            // 
            this.imageListApps.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListApps.ImageSize = new System.Drawing.Size(32, 32);
            this.imageListApps.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // listViewAllApps
            // 
            this.listViewAllApps.CheckBoxes = true;
            this.listViewAllApps.HideSelection = false;
            this.listViewAllApps.Location = new System.Drawing.Point(65, 278);
            this.listViewAllApps.Name = "listViewAllApps";
            this.listViewAllApps.Size = new System.Drawing.Size(774, 568);
            this.listViewAllApps.SmallImageList = this.imageListApps;
            this.listViewAllApps.TabIndex = 7;
            this.listViewAllApps.UseCompatibleStateImageBehavior = false;
            this.listViewAllApps.View = System.Windows.Forms.View.Details;
            // 
            // selectedAppsLabel
            // 
            this.selectedAppsLabel.AutoSize = true;
            this.selectedAppsLabel.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedAppsLabel.Location = new System.Drawing.Point(1302, 170);
            this.selectedAppsLabel.Name = "selectedAppsLabel";
            this.selectedAppsLabel.Size = new System.Drawing.Size(228, 44);
            this.selectedAppsLabel.TabIndex = 8;
            this.selectedAppsLabel.Text = "Selected Apps";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(332, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(245, 44);
            this.label2.TabIndex = 8;
            this.label2.Text = "All Application";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1905, 1325);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.selectedAppsLabel);
            this.Controls.Add(this.listViewAllApps);
            this.Controls.Add(this.listViewSelectedApps);
            this.Controls.Add(this.btnRemoveSelected);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnAddSelected);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddSelected;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.ListView listViewSelectedApps;
        private System.Windows.Forms.ImageList imageListApps;
        private System.Windows.Forms.ListView listViewAllApps;
        private System.Windows.Forms.Label selectedAppsLabel;
        private System.Windows.Forms.Label label2;
    }
}

