namespace ColorResearchAnalysis
{
    partial class FormViewResults
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
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.checkBoxShowRadius = new System.Windows.Forms.CheckBox();
            this.checkBoxTarget = new System.Windows.Forms.CheckBox();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.checkBoxNumbers = new System.Windows.Forms.CheckBox();
            this.comboBoxFilter = new System.Windows.Forms.ComboBox();
            this.comboBoxColorSpace = new System.Windows.Forms.ComboBox();
            this.numericUpDownMinimumColorDist = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownSearchDiameter = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinimumColorDist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSearchDiameter)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonNext
            // 
            this.buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonNext.Location = new System.Drawing.Point(106, 386);
            this.buttonNext.Margin = new System.Windows.Forms.Padding(2);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(91, 30);
            this.buttonNext.TabIndex = 1;
            this.buttonNext.Text = "Next picture";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonPrevious
            // 
            this.buttonPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPrevious.Location = new System.Drawing.Point(11, 386);
            this.buttonPrevious.Margin = new System.Windows.Forms.Padding(2);
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.Size = new System.Drawing.Size(91, 30);
            this.buttonPrevious.TabIndex = 2;
            this.buttonPrevious.Text = "Previous picture";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.buttonPrevious_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.AllowDrop = true;
            this.pictureBox.BackColor = System.Drawing.Color.White;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(425, 241);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.Click += this.pictureBox_Click;
            // 
            // checkBoxShowRadius
            // 
            this.checkBoxShowRadius.AutoSize = true;
            this.checkBoxShowRadius.Location = new System.Drawing.Point(18, 255);
            this.checkBoxShowRadius.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxShowRadius.Name = "checkBoxShowRadius";
            this.checkBoxShowRadius.Size = new System.Drawing.Size(84, 17);
            this.checkBoxShowRadius.TabIndex = 6;
            this.checkBoxShowRadius.Text = "Show radius";
            this.checkBoxShowRadius.UseVisualStyleBackColor = true;
            this.checkBoxShowRadius.CheckedChanged += new System.EventHandler(this.checkBoxShowRadius_CheckedChanged);
            // 
            // checkBoxTarget
            // 
            this.checkBoxTarget.AutoSize = true;
            this.checkBoxTarget.Location = new System.Drawing.Point(106, 255);
            this.checkBoxTarget.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxTarget.Name = "checkBoxTarget";
            this.checkBoxTarget.Size = new System.Drawing.Size(83, 17);
            this.checkBoxTarget.TabIndex = 7;
            this.checkBoxTarget.Text = "Show target";
            this.checkBoxTarget.UseVisualStyleBackColor = true;
            this.checkBoxTarget.CheckedChanged += new System.EventHandler(this.checkBoxTarget_CheckedChanged);
            // 
            // buttonCopy
            // 
            this.buttonCopy.Location = new System.Drawing.Point(298, 249);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(73, 26);
            this.buttonCopy.TabIndex = 14;
            this.buttonCopy.Text = "Copy";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // checkBoxNumbers
            // 
            this.checkBoxNumbers.AutoSize = true;
            this.checkBoxNumbers.Location = new System.Drawing.Point(195, 255);
            this.checkBoxNumbers.Name = "checkBoxNumbers";
            this.checkBoxNumbers.Size = new System.Drawing.Size(97, 17);
            this.checkBoxNumbers.TabIndex = 18;
            this.checkBoxNumbers.Text = "Order of circles";
            this.checkBoxNumbers.UseVisualStyleBackColor = true;
            this.checkBoxNumbers.CheckedChanged += new System.EventHandler(this.checkBoxNumbers_CheckedChanged);
            // 
            // comboBoxFilter
            // 
            this.comboBoxFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFilter.FormattingEnabled = true;
            this.comboBoxFilter.Items.AddRange(new object[] {
            "Everything",
            "1st circle",
            "2nd circle",
            "3rd circle",
            "4th circle",
            "5th circle"});
            this.comboBoxFilter.Location = new System.Drawing.Point(11, 277);
            this.comboBoxFilter.Name = "comboBoxFilter";
            this.comboBoxFilter.Size = new System.Drawing.Size(121, 21);
            this.comboBoxFilter.TabIndex = 19;
            this.comboBoxFilter.SelectedIndexChanged += new System.EventHandler(this.comboBoxFilter_SelectedIndexChanged);
            // 
            // comboBoxColorSpace
            // 
            this.comboBoxColorSpace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxColorSpace.FormattingEnabled = true;
            this.comboBoxColorSpace.Items.AddRange(new object[] {
            "RGB",
            "Lab"});
            this.comboBoxColorSpace.Location = new System.Drawing.Point(139, 276);
            this.comboBoxColorSpace.Name = "comboBoxColorSpace";
            this.comboBoxColorSpace.Size = new System.Drawing.Size(121, 21);
            this.comboBoxColorSpace.TabIndex = 20;
            this.comboBoxColorSpace.SelectedIndexChanged += new System.EventHandler(this.comboBoxColorSpace_SelectedIndexChanged);
            // 
            // numericUpDownMinimumColorDist
            // 
            this.numericUpDownMinimumColorDist.Location = new System.Drawing.Point(89, 309);
            this.numericUpDownMinimumColorDist.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownMinimumColorDist.Name = "numericUpDownMinimumColorDist";
            this.numericUpDownMinimumColorDist.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownMinimumColorDist.TabIndex = 21;
            this.numericUpDownMinimumColorDist.ValueChanged += new System.EventHandler(this.numericUpDownMinimumColorDist_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 311);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Min Color Dist:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(162, 311);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Search Diameter:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // numericUpDownSearchDiameter
            // 
            this.numericUpDownSearchDiameter.Location = new System.Drawing.Point(258, 308);
            this.numericUpDownSearchDiameter.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.numericUpDownSearchDiameter.Name = "numericUpDownSearchDiameter";
            this.numericUpDownSearchDiameter.Size = new System.Drawing.Size(63, 20);
            this.numericUpDownSearchDiameter.TabIndex = 24;
            this.numericUpDownSearchDiameter.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.numericUpDownSearchDiameter.ValueChanged += new System.EventHandler(this.numericUpDownSearchDiameter_ValueChanged);
            // 
            // FormViewResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(426, 427);
            this.Controls.Add(this.numericUpDownSearchDiameter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownMinimumColorDist);
            this.Controls.Add(this.comboBoxColorSpace);
            this.Controls.Add(this.comboBoxFilter);
            this.Controls.Add(this.checkBoxNumbers);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.checkBoxTarget);
            this.Controls.Add(this.checkBoxShowRadius);
            this.Controls.Add(this.buttonPrevious);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.pictureBox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormViewResults";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinimumColorDist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSearchDiameter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonPrevious;
        private System.Windows.Forms.CheckBox checkBoxShowRadius;
        private System.Windows.Forms.CheckBox checkBoxTarget;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.CheckBox checkBoxNumbers;
        private System.Windows.Forms.ComboBox comboBoxFilter;
        private System.Windows.Forms.ComboBox comboBoxColorSpace;
        private System.Windows.Forms.NumericUpDown numericUpDownMinimumColorDist;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownSearchDiameter;
    }
}

